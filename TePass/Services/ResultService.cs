using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TePass.Models;

namespace TePass.Services
{
    public class ResultService
    {
        const string Url = "https://tedevelopment.herokuapp.com/api/Result/";
        readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        private HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }
        public async Task<Result> Get(int testId)
        {
            HttpClient client = GetClient();
            var x = await client.GetAsync(Url + testId);
            if (x.IsSuccessStatusCode)
            {
                string result = await client.GetStringAsync(Url + testId);
                return JsonSerializer.Deserialize<Result>(result, options);
            }
            else return null;
        }
        public async Task<Result> Add(Result result)
        {
            HttpClient client = GetClient();
            var response = await client.PostAsync(Url,
                new StringContent(
                    JsonSerializer.Serialize(result),
                    Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonSerializer.Deserialize<Result>(
                await response.Content.ReadAsStringAsync(), options);
        }
    }
}
