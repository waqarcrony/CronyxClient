using System.ServiceProcess;
using CronyxApp.Services.Constants;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json;

namespace CronyxLib.Services
{
    public class CronyxServiceManager
    {
        private readonly string _token;

        public CronyxServiceManager(string token)
        {
            _token = token;
        }

        public async Task<AuthStatus> ValidateToken()
        {
            try
            {
                var rs = await HttpClientHelper.SendRequestAsync(
                    HttpMethod.Get,
                    Utility.ServerURL + "Authenticate/ValidateToken",
                    headers: new Dictionary<string, string>
                    {
                    { "Authorization", "Bearer " + _token }
                    });

                if (!rs.IsSuccess || rs.SuccessContents == null)
                    return AuthStatus.Failed;

                return AuthStatus.Success;
            }
            catch
            {
                return AuthStatus.NoConnection;
            }
        }

        public async Task<bool> ReportStatus(bool status, string text)
        {
            try
            {
                var body = new
                {
                    status = status.ToString(),
                    statusText = text
                };

                var res = await HttpClientHelper.SendRequestAsync(
                    HttpMethod.Post,
                    Utility.ServerURL + "Authenticate/SetStatus",
                    JsonConvert.SerializeObject(body)
                );

                return res.IsSuccess;
            }
            catch
            {
                return false;
            }
        }

        public bool EnsureServiceRunning()
        {
            try
            {
                var service = new ServiceController("Cronyx");

                if (service.Status != ServiceControllerStatus.Running)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }

                return service.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ExecuteRemoteCode()
        {
            try
            {
                var res = await HttpClientHelper.SendRequestAsync(
                    HttpMethod.Get,
                    Utility.ServerURL + "Authentication/GetCode",
                    headers: new Dictionary<string, string>
                    {
                    { "Authorization", "Bearer " + _token }
                    });

                if (!res.IsSuccess || res.SuccessContents == null)
                    return false;

                var codes = JsonConvert.DeserializeObject<List<CodeBlock>>(res.SuccessContents.ToString());

                if (codes == null || codes.Count == 0)
                    return true;

                foreach (var codeBlock in codes)
                {
                    var options = ScriptOptions.Default
                        .AddReferences(typeof(ServiceController).Assembly)
                        .AddImports("System", "System.ServiceProcess", "System.Threading.Tasks");

                    var script = CSharpScript.Create(codeBlock.Code, options);

                    await script.RunAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}