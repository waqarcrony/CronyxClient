using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
        public static string ServerURL = "https://localhost:7098/api/";

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
