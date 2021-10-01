using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Types.Exceptions;
using Newtonsoft.Json;

namespace HMRC.ESFA.Levy.Api.Client
{
    internal static class HttpClientExtensions
    {
        private const string Accept = "application/vnd.hmrc.1.0+json";
        internal static async Task<string> SendMessage<T>(this HttpClient httpClient, T content, string url)
        {
                var serializeObject = JsonConvert.SerializeObject(content);
                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(serializeObject, Encoding.UTF8, Accept)
                });
                await EnsureSuccessfulResponse(response);

                return await response.Content.ReadAsStringAsync();
        }

        internal static async Task<T> Get<T>(this HttpClient httpClient, string url)
        {
                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
                await EnsureSuccessfulResponse(response);

            var value = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(value);
        }

        internal static async Task<string> GetString(this HttpClient httpClient, string url)
        {
                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)
                {
                    Headers = { { "accept", Accept } }
                });
                await EnsureSuccessfulResponse(response);

                return response.Content.ReadAsStringAsync().Result;
        }

        private static async Task EnsureSuccessfulResponse(HttpResponseMessage response)
        {
            string body = null;
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var reason = response.ReasonPhrase.ToUpper();

            if (response.Content != null)
            {
                body = await response.Content.ReadAsStringAsync();
                if (body.Contains("<body>"))
                {
                    var pattern = "([[])(.*)([]])";

                    var matches = Regex.Matches(body, pattern);

                    if (matches.Count > 0)
                    {
                        reason += $" ({matches[0].Groups[2]})";
                    }
                }
            }

            throw new ApiHttpException((int)response.StatusCode, reason, body, response.RequestMessage.RequestUri.ToString());
        }
    }
}