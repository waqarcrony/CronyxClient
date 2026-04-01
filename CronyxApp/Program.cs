using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using CronyxApp;
using CronyxLib;
using CronyxLib.Services;
using Microsoft.Win32;
using BackgroundWorker = CronyxLib.Services.BackgroundWorker;

namespace CronyxApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        //static void Main()
        //{
        //    bool isFirstInstance;
        //    using (Mutex mtx = new Mutex(true, "CronyxAppMutex", out isFirstInstance))
        //    {
        //        if (!isFirstInstance)
        //        {
        //            // Already running, exit
        //            return;
        //        }

        //        ApplicationConfiguration.Initialize();
        //        Application.EnableVisualStyles();
        //        Application.SetCompatibleTextRenderingDefault(false);

        //        using (var login = new FrmLogin())
        //        {
        //            if (login.ShowDialog() == DialogResult.OK)
        //            {
        //                Application.Run(new FrmMain());
        //            }
        //        }
        //    }
        //}

        [STAThread]
        static async Task Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string? token = TokenService.LoadToken();

            if (!string.IsNullOrEmpty(token))
            {
                var manager = new CronyxServiceManager(token);

                var auth = await manager.ValidateToken();

                if (auth == AuthStatus.Success)
                {
                    Application.Run(new FrmMain());
                    return;
                }
            }

            // fallback to login
            var login = new FrmLogin();

            if (login.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new FrmMain());
            }
        }
    }
}