using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace PosClient.Services.Constants
{
    public static class Utility
    {
        public static string defaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public static string defaultDateFormat = "yyyy-MM-dd";
        public static string defaultTimeFormat = "hh:mm tt";

        //public static string ServerURL = "https://cronyconnector.azurewebsites.net/api/";
        public static string ServerURL = "https://localhost:7182/api/";

        public static string GetMac()
        {
            var macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up && nic.GetPhysicalAddress().ToString() != ""
                orderby nic.Description
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();

            return macAddr;
        }
    }

    public static class SecureStorage
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Cronyx", "token.dat");

        public static void SaveToken(string token)
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            byte[] data = Encoding.UTF8.GetBytes(token);

            byte[] encrypted = ProtectedData.Protect(
                data,
                null,
                DataProtectionScope.CurrentUser); // tied to logged-in user

            File.WriteAllBytes(FilePath, encrypted);
        }

        public static string GetToken()
        {
            if (!File.Exists(FilePath))
                return null;

            byte[] encrypted = File.ReadAllBytes(FilePath);

            byte[] decrypted = ProtectedData.Unprotect(
                encrypted,
                null,
                DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(decrypted);
        }
    }

    #region VMs
    public class ApiResponseViewModel
    {
        public bool IsSuccess { get; set; }
        public dynamic? FailReason { get; set; }
        public dynamic? FailReasonContent { get; set; }
        public dynamic? SuccessContents { get; set; }
        public dynamic? SuggestedContents { get; set; }
    }
    #endregion
}
