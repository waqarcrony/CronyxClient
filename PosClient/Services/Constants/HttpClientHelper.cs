using System.Text;
using Newtonsoft.Json;

namespace PosClient.Services.Constants
{
    public static class HttpClientHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient(new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(10), // Time to wait for connection (server not responding)
            PooledConnectionLifetime = TimeSpan.FromMinutes(5), // Optional: avoid stale connections
        })
        {
            Timeout = TimeSpan.FromMinutes(1) // Total time for the request (including connect + read/write)
        };

        public static async Task<ApiResponseViewModel> SendRequestAsync(
            HttpMethod method,
            string url,
            string? jsonBody = null,
            MultipartFormDataContent? multiPartFormContent = null,
            Dictionary<string, string>? headers = null)
        {
            var hasInternet = await IsInternetConnected();
            if (!hasInternet)
            {
                return new ApiResponseViewModel
                {
                    IsSuccess = false,
                    FailReason = $"Unable to connect to the server. Please check your internet connection and try again."
                };
            }

            using var request = new HttpRequestMessage(method, url);

            // Add request body if provided
            if (!string.IsNullOrWhiteSpace(jsonBody))
            {
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }
            else if (multiPartFormContent != null)
            {
                request.Content = multiPartFormContent;
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            using var response = await _httpClient.SendAsync(request);
            var rsString = await response.Content.ReadAsStringAsync();

            var rs = JsonConvert.DeserializeObject<ApiResponseViewModel>(rsString);

            return rs;
        }
        public static async Task<bool> IsInternetConnected()
        {
            try
            {
                // Send a simple GET request and check for a successful status code
                var response = await _httpClient.GetAsync("https://dns.google/resolve?name=google.com");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false; // Return false if there's an error (e.g., network issues)
            }
        }

    }
}
