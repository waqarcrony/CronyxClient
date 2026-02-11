using System.ServiceProcess;
using System.Runtime.Versioning;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text;
using CronyxLib;

namespace CronyxApp
{
    public partial class Status : Form
    {
        NotifyIcon _trayIcon;
        bool inTick = false;
        System.Windows.Forms.Timer _checkTimer;
        private bool _isActivated;

        public bool isActivated
        {
            get => _isActivated;
            set
            {
                _isActivated = value;
                lblAppStatus.Text = value ? "Activated" : "Not Activated";
            }
        }
        public Status()
        {
            InitializeComponent();
            _trayIcon = new NotifyIcon
            {
                Icon = new Icon("app.ico"),
                Visible = false,
                Text = "Cronyx Monitor"
            };
            this.ShowInTaskbar = false;

            this.WindowState = FormWindowState.Minimized;

            this.Hide();

            _trayIcon.BalloonTipText = "Running";
            _trayIcon.BalloonTipTitle = "Cronyx";
            _trayIcon.Visible = true;
            _trayIcon.Click += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };
            _trayIcon.BalloonTipClicked += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };
            this.Resize += (s, e) =>
            {
                if (this.WindowState == FormWindowState.Minimized)

                {
                    // this.Hide();
                    // _trayIcon.BalloonTipText = "Running";

                    //_trayIcon.ShowBalloonTip(1000);
                }
            };
            this.FormClosing += (s, e) =>
            {
                //_trayIcon.BalloonTipText = "Running";

                ShowBalloon("", 1000);

                this.Hide();
                e.Cancel = true;

            };
            this.FormClosed += Status_FormClosed;
           
            _checkTimer = new System.Windows.Forms.Timer();
            _checkTimer.Interval = 15000; // Check every 5 seconds
            _checkTimer.Tick += Timer1_Tick;
        
            //_trayIcon.ShowBalloonTip(1000);

            CallAPI();

        }
        public async Task<bool> RunCodeFromStringAsync()
        {

            bool rt = false;

            try
            {
                CronyxLib.APICall aPICall = new CronyxLib.APICall(txtToken.Text);
                var result2 = await aPICall.Execute<List<CodeBlock>>("Authentication/GetCode", new
                {

                    token = txtToken.Text
                }); 
                if (result2.Success)
                {
                    foreach(CodeBlock codetorun in result2.data)
                    {
                        var code = codetorun.Code;
                        var globals = new ScriptGlobals
                        {
                            tokenValue = txtToken.Text,
                            button1 = this.button1,
                            label1=this.label1,
                            label3 = this.label3,
                            RestartService= CheckandRestartService,
                            ShowBalloonRemote= ShowBalloon
                        };

                        var options = ScriptOptions.Default
                            .AddReferences(typeof(CronyxLib.APICall).Assembly)
                            .AddReferences(typeof(ServiceController).Assembly)
                            .AddImports("System", "System.ServiceProcess", "System.Threading.Tasks", "CronyxLib");

                        var script = CSharpScript.Create<bool>(code, options, typeof(ScriptGlobals));
                        script.Compile(); // Optional, to check for syntax errors

                        var result = await script.RunAsync(globals);
                       
                    }
                    return true;

                 
                }
                else
                {
                   
                    return false;

                }
            }
            catch (Exception ex)
            {
                return false;
            }

          
        }


        private void ShowBalloon(string vStr, int Timeout)
        {
            if (vStr == "") { }
            else
            {
                _trayIcon.BalloonTipText = vStr;
            }
            _trayIcon.ShowBalloonTip(1000);
        }
        private void Status_FormClosed(object? sender, FormClosedEventArgs e)
        {
            ShowBalloon("Shutting Down", 1000);



        }

        private async Task CallAPI()
        {
            txtToken.ReadOnly = true;

            string? token = LoadToken();
            if (token == null)
            {
                Thread.Sleep(1000);
                token = LoadToken();
            }
            if (token == null)
            {
                CronyxActivationRequired();
                return;
            }
            else
            {
                txtToken.Text = token;

                cmdActive.Visible = false;
                AuthStatus aStatus=await Auth();
                if (aStatus==AuthStatus.Success )
                {
                    _checkTimer.Interval = 15000; // Check every 15 seconds

                    CronyxActivated();
                    _checkTimer.Start();
                }
                else if (aStatus==AuthStatus.Failed)
                {
                    _checkTimer.Interval = 15000; // Check every 15 seconds

                    CronyxActivationRequired();
                }
                else
                {
                    //Retry
                    isActivated = false;
                    _checkTimer.Interval = 35000; // Check every 15 seconds

                    _checkTimer.Start();

                }
            }

        }
        private void CronyxActivationRequired()
        {
            isActivated = false;
            ShowBalloon("Activation Required  *******, Please click here", 5000);
            txtToken.Text = new CronyxLib.Crypto().GenerateToken();
            cmdActive.Visible = true;
        }
        private void CronyxActivated()
        {
            ShowBalloon("Cronyx Activated", 1000);
            isActivated = true;
            cmdActive.Visible = false;
        }
        public async Task<AuthStatus> Auth()
        {
            AuthStatus authStatus = AuthStatus.NoConnection;
            
            try
            {
                CronyxLib.APICall aPICall = new CronyxLib.APICall(txtToken.Text);
                var result = await aPICall.Execute<string>("Authentication/Login", new
                {
                    authtype = "TOKEN",
                    login = txtToken.Text,
                    pass = ""
                });
                if (result.Success)
                {

                    authStatus=AuthStatus.Success;
                }
                else
                {
                    authStatus = AuthStatus.Failed;

                }
            }
            catch (Exception ex)
            {
                authStatus = AuthStatus.NoConnection;
            }
            return authStatus;
        }


        public async Task<bool> ReportStatus(bool vStatus,string vStatusText)
        {
            bool rt = false;

            try
            {
                CronyxLib.APICall aPICall = new CronyxLib.APICall(txtToken.Text);
                var result = await aPICall.Execute<string>("Authentication/SetStatus", new
                {
                    status = vStatus.ToString(),
                    statusText = vStatusText,
                    token = txtToken.Text 
                });
                if (result.Success)
                {

                    return true;
                }
                else
                {
                    CronyxActivationRequired();
                    return false;

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SaveToken(string token)
        {
            bool rt = false;
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "alium", "token.dat");
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(token);
                byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.LocalMachine);
                File.WriteAllBytes(filePath, encrypted);
                if (token == LoadToken())
                {
                    rt = true;
                }
            }
            catch { }
            return rt;
        }

        public string? LoadToken()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "alium", "token.dat");

            if (!File.Exists(filePath))
                return null;
            try
            {
                byte[] encrypted = File.ReadAllBytes(filePath);
                byte[] decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.LocalMachine);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }
        private void Status_Load(object? sender, EventArgs e)
        {

            try
            {
                bool isR = IsServiceRunning();

               
                
            }
            catch (Exception ex)
            {
                label1.Text = $"Error {ex.Message} ";

            }

           

        }
        private async void Timer1_Tick(object? sender, EventArgs e)
        {
            if (inTick)
                return;

            inTick = true;
            if (!isActivated)
            {
                try
                {
                    await CallAPI();
                }
                catch { }
                inTick =false;
                return;
            }
            bool isR = false;

            try
            {
                
                isR=IsServiceRunning();
                
            }
            catch (Exception ex)
            {
                label1.Text = $"Error {ex.Message} ";

            }
            try
            {
                await ReportStatus(isR, label1.Text);

                bool rt = await RunCodeFromStringAsync();
                if(rt==false)
                {
                    isActivated = false;
                }
            }
            catch { }
            inTick = false;

        }
        private void StopService()
        {
            bool rt = false;
            string serviceName = "Cronyx";
            var service = new ServiceController(serviceName);

            try
            {
                if (service.Status != ServiceControllerStatus.Running)
                {

                }
                else
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        label1.Text = "Service stopped successfully!";
                        rt = true;
                    }
                    else
                    {
                        label1.Text = "Service did not reach 'Stopped' state.";
                    }

                }
            }
            catch (InvalidOperationException ex) when (ex.InnerException != null)
            {
                label1.Text = $"Stop failed: {ex.InnerException.Message}";
            }
            catch (Exception ex)
            {
                label1.Text = $"Unexpected error: {ex.Message}";
            }
            rt = true;

        }
        
        private bool CheckandRestartService()
        {
            bool rt = false;
            try
            {
                string serviceName = "Cronyx";
                var service = new ServiceController(serviceName);

                if (service.Status != ServiceControllerStatus.Running)
                {
                    label1.Text = "Service is not running. Attempting to start...";

                    try
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));

                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            label1.Text = "Service started successfully!";
                            rt = true;
                        }
                        else
                        {
                            label1.Text = "Service did not reach 'Running' state.";
                        }
                    }
                    catch (InvalidOperationException ex) when (ex.InnerException != null)
                    {
                        label1.Text = $"Start failed: {ex.InnerException.Message}";
                    }
                    catch (Exception ex)
                    {
                        label1.Text = $"Unexpected error: {ex.Message}";
                    }
                }
                else
                {
                    label1.Text = "Service is already running.";
                    rt = true;
                }
            }
            catch (Exception ex)
            {
                label1.Text = $"Service access error: {ex.Message}";
            }
            return  rt;

        }
        [SupportedOSPlatform("windows")]
        bool IsServiceRunning()
        {
            return CheckandRestartService();
        }

        private async void cmdActive_Click(object sender, EventArgs e)
        {
            if (SaveToken(txtToken.Text))
            {
                cmdActive.Visible = false;
                await CallAPI();

            }
            else
            {
                MessageBox.Show("Failed to save token");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {

            bool rt = false;

            try
            {
                CronyxLib.APICall aPICall = new CronyxLib.APICall(txtToken.Text);
                var result2 = await aPICall.Execute<List<UserDeviceInfo>>("Devices/GetDevices", new
                {

                    
                });
                if (result2.Success)
                {
                    int ct = 0;
                    foreach (UserDeviceInfo device in result2.data)
                    {
                        ct++;

                    }
                    label3.Text = $"Total Devices Fetched {ct} ";
                    return ;


                }
                else
                {

                    return ;

                }
            }
            catch (Exception ex)
            {
                return ;
            }
            //bool rt = await RunCodeFromStringAsync();
            //MessageBox.Show(rt.ToString());
            //StopService();
        }
    }
    public delegate void ShowBalloonDelegate(string vMessage, int vDelay);
    public class ScriptGlobals
    {
        public string tokenValue { get; set; } = string.Empty;
        public Button button1 { get; set; } = null!;
        public Label label1 { get; set; } = null!;
        public Label label3 { get; set; } = null!;
        public Func<bool> RestartService { get; set; } = null!;
        public ShowBalloonDelegate ShowBalloonRemote { get; set; } = null!;
    }
  
}
