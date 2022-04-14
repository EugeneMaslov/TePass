using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TePass.Models;

namespace TePass.Services
{
    public class LoginService
    {
        const string Url = "https://teconservice.herokuapp.com/api/Account/";
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
        public async Task<User> Get(int userId)
        {
            HttpClient client = GetClient();
            var x = await client.GetAsync(Url + "userid/" + userId);
            if (x.IsSuccessStatusCode)
            {
                string result = await client.GetStringAsync(Url + "userid/" + userId);
                return JsonSerializer.Deserialize<User>(result, options);
            }
            else return null;
        }
        public async Task<User> Get(string login)
        {
            HttpClient client = GetClient();
            var x = await client.GetAsync(Url + login);
            if (x.IsSuccessStatusCode)
            {
                string result = await client.GetStringAsync(Url + login);
                return JsonSerializer.Deserialize<User>(result, options);
            }
            else return null;
        }
    }
}
