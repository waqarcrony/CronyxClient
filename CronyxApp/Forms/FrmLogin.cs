using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using System.ServiceProcess;
using CronyxApp.Services.Constants;
using CronyxLib;
using CronyxLib.Services;
using Newtonsoft.Json;
using BackgroundWorker = CronyxLib.Services.BackgroundWorker;

namespace CronyxApp
{
    public partial class FrmLogin : Form
    {
        NotifyIcon _trayIcon;

        public FrmLogin()
        {
            InitializeComponent();

            _trayIcon = new NotifyIcon
            {
                Icon = new Icon("app.ico"),
                Visible = true,
                Text = "Cronyx Monitor"
            };

            _trayIcon.Click += (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };

            _trayIcon.BalloonTipClicked += (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            };

            this.Resize += (s, e) =>
            {
                if (this.WindowState == FormWindowState.Minimized)
                    this.Hide();
            };

            this.FormClosing += (s, e) =>
            {
                ShowBalloon("Running in background", 1000);
                this.Hide();
                e.Cancel = true;
            };
        }

        private void ShowBalloon(string text, int timeout)
        {
            if (!string.IsNullOrEmpty(text))
                _trayIcon.BalloonTipText = text;

            _trayIcon.ShowBalloonTip(timeout);
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtEmail.Text))
                    throw new Exception("Enter email");

                if (string.IsNullOrWhiteSpace(TxtPass.Text))
                    throw new Exception("Enter password");

                UseWaitCursor = true;

                var request = new
                {
                    EmailOrUsername = TxtEmail.Text.Trim(),
                    Pass = TxtPass.Text.Trim()
                };

                var rs = await HttpClientHelper.SendRequestAsync(
                    HttpMethod.Post,
                    Utility.ServerURL + "Authenticate/Login",
                    JsonConvert.SerializeObject(request)
                );

                UseWaitCursor = false;

                if (!rs.IsSuccess || rs.SuccessContents == null)
                    throw new Exception(rs.FailReason ?? "Server error");

                var data = JsonConvert.DeserializeObject<List<LoginResponseVM>>(rs.SuccessContents.ToString());

                if (data == null || data.Count == 0)
                    throw new Exception("Invalid credentials");

                LoginResponseVM selected;

                if (data.Count == 1)
                {
                    selected = data[0];
                }
                else
                {
                    using (var frm = new FrmSelectStore(data))
                    {
                        if (frm.ShowDialog() != DialogResult.OK)
                            return;

                        selected = frm.SelectedStore;
                    }
                }

                if (selected == null || string.IsNullOrEmpty(selected.Token))
                    throw new Exception("Invalid store selection");

                if (!TokenService.SaveToken(selected.Token))
                    throw new Exception("Token save failed");

                ShowBalloon("Login Successful", 1000);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                UseWaitCursor = false;
                MessageBox.Show("Error occurred: " + ex.Message);
            }
        }

        private void FrmLogin_Shown(object sender, EventArgs e)
        {
#if DEBUG
            TxtEmail.Text = "muhammadali@cronysoft.com";
            TxtPass.Text = "Abcd@1234";
#endif
        }
    }
}
