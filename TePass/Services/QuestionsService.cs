using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TePass.Models;


namespace TePass.Services
{
    public class QuestionsService
    {
        const string Url = "https://tedevelopment.herokuapp.com/api/Questions/";
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
        public async Task<IEnumerable<Question>> GetQuestByTestId(int id)
        {
            HttpClient client = GetClient();
            var x = await client.GetAsync(Url + id);
            if (x.IsSuccessStatusCode)
            {
                string result = await client.GetStringAsync(Url + id);
                return JsonSerializer.Deserialize<IEnumerable<Question>>(result, options);
            }
            else return null;
        }
    }
}
