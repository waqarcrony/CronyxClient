
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CronyxLib
{
    public class APICall
    {
        public static string APIServer = "http://localhost:5131/";
        private string _bearerToken = "";
        public APICall(string token)
        {
            _bearerToken = token;
        }
        public async Task<AuthResponse<T>> Execute<T>(string url,object requestBody)
        {
            var client = new HttpClient();
            if (_bearerToken != "")
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            }
            

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(string.Concat(APIServer,url), content);
                response.EnsureSuccessStatusCode(); // Throws if not 2xx

                string responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<AuthResponse<T>>(responseContent, options);
                return result!;

            }
            catch (Exception ex)
            {
                throw new Exception($"Error: {ex.Message}");
            }
           
        
        }
    }

    public class AuthResponse<T>
    {
        public bool Success { get; set; }
        public T data { get; set; }
        public string Message { get; set; }
    }
}
