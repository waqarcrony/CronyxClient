
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CronyxApp.Services.Constants;
using CronyxLib.Services;
using Newtonsoft.Json;

namespace CronyxLib
{
    public class APICall
    {
        private string? _bearerToken = "";
        public APICall(string? token = null)
        {
            _bearerToken = token;
        }
        public async Task<AuthResponse<T>> Execute<T>(string url, object requestBody)
        {
            var client = new HttpClient();
            if (_bearerToken != "")
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            }


            var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(string.Concat(Utility.ServerURL, url), content);
                response.EnsureSuccessStatusCode(); // Throws if not 2xx

                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = System.Text.Json.JsonSerializer.Deserialize<AuthResponse<T>>(responseContent, options);
                return result!;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }


        }


        public async Task<List<DeviceVm>> GetDevices()
        {
            var request = new { }; // if needed

            var rs = await HttpClientHelper.SendRequestAsync(
                HttpMethod.Get,
                Utility.ServerURL + "Global/GetDevices",
                JsonConvert.SerializeObject(request)
            );

            var data = JsonConvert.DeserializeObject<List<DeviceVm>>(rs.SuccessContents.ToString());

            if (data == null || data?.Count == 0)
                throw new Exception("No devices found");

            return data;
        }
    }

    public class AuthResponse<T>
    {
        public bool Success { get; set; }
        public T data { get; set; }
        public string Message { get; set; }
    }

    public class DeviceVm
    {
        // User
        public string LicenseNo { get; set; }
        public string StoreName { get; set; }
        public string ConnectorPackageType { get; set; }
        public bool IsReportingActive { get; set; }
        public bool FaultPending { get; set; }
        public bool TextNotification { get; set; }
        public bool EmailNotification { get; set; }
        public int PrimaryStoreID { get; set; }

        // Device
        public int DeviceId { get; set; }
        public int UserDBID { get; set; }
        public string DeviceName { get; set; }
        public int DeviceTypeID { get; set; }
        public int? AttachedToID { get; set; }
        public string MACAddress { get; set; }
        public string IPorName { get; set; }
        public string InterfaceType { get; set; }
        public bool IsActive { get; set; }
        public string SerialNumber { get; set; }
        public DateTime? ActivatedOn { get; set; }
        public DateTime? WarrantyExpires { get; set; }

        // Device Type
        public string DeviceType { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string DeviceDescription { get; set; }
        public int? DevicePhotoID { get; set; }

        // Extra
        public bool DeviceCheckActive { get; set; }
        public string FullLog { get; set; }
        public string LastStatus { get; set; }

        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
        public string Parameter3 { get; set; }
        public string Parameter4 { get; set; }
        public string Parameter5 { get; set; }
        public string Parameter6 { get; set; }
        public string Parameter7 { get; set; }
    }

}
