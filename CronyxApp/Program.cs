using Microsoft.Win32;
using System.Diagnostics;
using System.ServiceProcess;

namespace CronyxApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isFirstInstance;
            using (Mutex mtx = new Mutex(true, "CronyxAppMutex", out isFirstInstance))
            {
                if (!isFirstInstance)
                {
                    // Already running, exit
                    return;
                }

                ApplicationConfiguration.Initialize();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new Status());
            }
        
           
        }


      
    }


}