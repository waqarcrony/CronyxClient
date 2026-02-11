using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            string exePath = $"\"{AppDomain.CurrentDomain.BaseDirectory}Cronyx.exe\"";

            // sc create command
            //RunCommand($"sc create Cronyx binPath= {exePath}  obj= \"NT AUTHORITY\\NetworkService\" start= auto");

            RunCommand($"sc create Cronyx binPath= {exePath}  obj= \"LocalSystem\" start= auto");
            //obj= \"LocalSystem\"
            RunCommand("sc description Cronyx \"Cronyx Background Service\"");
            RunCommand("sc start Cronyx");
            //obj= "NT AUTHORITY\NetworkService" start= auto
            Console.WriteLine("Service installed and started. at "+exePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to install/start service: " + ex.Message);
            Environment.Exit(1); // Important: exit with 1 to indicate failure
        }
    }

    static void RunCommand(string command)
    {
        Process p = new Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.Arguments = "/c " + command;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;
        p.Start();
        p.WaitForExit();
    }
}
