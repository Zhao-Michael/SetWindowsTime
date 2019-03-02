using System;
using System.Net;
using System.Runtime.InteropServices;

namespace UpdateWindowsTime
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start to get time stamp from internet");

            WebClient web = new WebClient();

            var result = web.DownloadString("http://quan.suning.com/getSysTime.do");

            if (!string.IsNullOrWhiteSpace(result) && result.Contains("sysTime1\":\""))
            {
                try
                {
                    var tick = result.Split(new[] { "sysTime1\":\"", "\"" }, StringSplitOptions.None)[6];

                    var date = DateTime.ParseExact(tick, "yyyyMMddHHmmss", null);

                    SetSystemDateTime.SetLocalTime(new SystemTime()
                    {
                        Year = (ushort)date.Year,
                        Month = (ushort)date.Month,
                        DayOfWeek = (ushort)date.DayOfWeek,
                        Day = (ushort)date.Day,
                        Hour = (ushort)date.Hour,
                        Minute = (ushort)date.Minute,
                        Second = (ushort)date.Second
                    });

                    Console.WriteLine("Success to set time stamp [" + tick + "]");

                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Failed to get time stamp from internet");
            }

            Console.ReadKey();
        }

    }

    public class SetSystemDateTime//自定义类SetSystemDateTime，用于设置系统日期,为了使用DllImportAttribute类（DllImportAttribute类是指可以将属性应用于方法，
                                  //并由非托管动态链接库（DLL）作为静态入口点公开），
                                  //需要引入命名空间：using System.Runtime.InteropServices;
    {
        [DllImportAttribute("Kernel32.dll")]//使用包含要导入的方法的 DLL 的名称初始化 DllImportAttribute 类的新实例。
        public static extern void GetLocalTime(SystemTime st);//C#要设置系统时间必须要调用Win32的API，而其中相关的函数就是SetSystemTime(), GetSystemTimer(), SetLocalTime(), GetLocalTime(),
                                                              //这似乎是用VC写的函数，在VC++中是可以直接调用的。
                                                              //对于这两个函数，其输入参数必须是一个下面这样的结构体，其成员变量类型必须是ushort，成员变量不能改变顺序。
        [DllImportAttribute("Kernel32.dll")]
        public static extern void SetLocalTime(SystemTime st);
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public class SystemTime//自定义类SystemTime用于定义日期类
    {
        public ushort Year;//年
        public ushort Month;//月
        public ushort DayOfWeek;//星期
        public ushort Day;//日
        public ushort Hour;//小时
        public ushort Minute;//分
        public ushort Second;//秒
    }

}
