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
                                                                 url: Utility.ServerURL + "Authenticate/ValidateLicense?licenseNumber="
                                                                        + TxtLicense.Text.Trim()
                                                                        + "&macAddress=" + TxtMacAddr.Text.Trim()
                                                                 );
                UseWaitCursor = false;
                if (rs.IsSuccess && rs.SuccessContents != null && rs.SuccessContents?.ToString().Length > 10)
                {
                    MessageBox.Show("Connection to server successful.");
                }
                else
                {
                    MessageBox.Show($"Connection to server failed. Reason: {rs.FailReason}");
                }
            }
            catch (Exception ex)
            {
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
