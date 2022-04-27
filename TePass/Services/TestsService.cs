using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TePass.Models;

namespace TePass.Services
{
    public class TestsService
    {
        const string Url = "https://tedevelopment.herokuapp.com/api/Tests/";
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
        public async Task<IEnumerable<Test>> GetTestByUserId(int id)
        {
            HttpClient client = GetClient();
            var x = await client.GetAsync(Url + id);
            if (x.IsSuccessStatusCode)
            {
                string result = await client.GetStringAsync(Url + id);
                return JsonSerializer.Deserialize<IEnumerable<Test>>(result, options);
            }
            else return null;
        }
        public async Task<Test> GetTestByCode(string code)
        {
            HttpClient client = GetClient();
            var x = await client.GetAsync(Url + "code/" + code);
            if (x.IsSuccessStatusCode)
            {
                string result = await client.GetStringAsync(Url + "code/" + code);
                return JsonSerializer.Deserialize<Test>(result, options);
            }
            else return null;
        }
    }
}
