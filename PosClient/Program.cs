using PosClient.Services.Constants;

namespace PosClient
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var token = SecureStorage.GetToken();
            var mac = Utility.GetMac();

            if (!string.IsNullOrEmpty(token) && IsTokenValid(token, mac))
            {
                Application.Run(new FrmMain());
                return;
            }

            using (var login = new FrmLogin())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new FrmMain());
                }
            }
        }

        static bool IsTokenValid(string token, string mac)
        {
            try
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                client.DefaultRequestHeaders.Add("macAddress", mac);

                var res = client
                    .GetAsync(Utility.ServerURL + "Authenticate/ValidateToken")
                    .GetAwaiter()
                    .GetResult();

                return res.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}