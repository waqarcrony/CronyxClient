using System.Net.NetworkInformation;
using PosClient.Services.Constants;

namespace PosClient
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                var rs = await HttpClientHelper.SendRequestAsync(HttpMethod.Get,
                                                                 url: Utility.ServerURL + "Authenticate/GetToken?licenseNumber="
                                                                        + TxtLicense.Text.Trim()
                                                                        + "&macAddress=" + TxtMacAddr.Text.Trim()
                                                                 );
                UseWaitCursor = false;
                if (rs.IsSuccess && !string.IsNullOrEmpty(rs.SuccessContents) && rs.SuccessContents?.ToString().Length > 10)
                {
                    SecureStorage.SaveToken(rs.SuccessContents);

                    FrmMain frmMain = new FrmMain();
                    frmMain.Show();

                    this.DialogResult = DialogResult.OK;
                    this.Hide();
                }
                else
                {
                    MessageBox.Show($"Connection to server failed. Reason: {rs.FailReason}");
                }
            }
            catch (Exception ex)
            {
                UseWaitCursor = false;
                MessageBox.Show($"An error occurred while connecting to the server. Error details: {ex.Message}");
            }
        }

        private void FrmLogin_Shown(object sender, EventArgs e)
        {
#if DEBUG
            TxtLicense.Text = "7861102lic";
#endif
            TxtMacAddr.Text = Utility.GetMac();
        }
    }
}
