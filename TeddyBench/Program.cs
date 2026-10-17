using System;
using System.Windows.Forms;

namespace TeddyBench
{
    static class Program
    {
        public static TeddyMain MainClass;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;

            MainClass = new TeddyMain();
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                MainClass.Shown += delegate {
                    MainClass.ApplyLinuxUi();
                    MainClass.ConfigureLinuxPlayback();
                    MainClass.ConfigureLinuxPlaybackButton();
                };
            Application.Run(MainClass);
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MainClass.ReportException("Main Application Thread", e.Exception);
        }
    }
}
