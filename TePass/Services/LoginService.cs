﻿using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TePass.Models;

namespace TePass.Services
{
    class LoginService
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
        public async Task<User> Add(User user)
        {
            HttpClient client = GetClient();
            User perm = await Get(user.Login);
            if (perm == null)
            {
                var response = await client.PostAsync(Url,
                    new StringContent(
                        JsonSerializer.Serialize(user),
                        Encoding.UTF8, "application/json"));
                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                return JsonSerializer.Deserialize<User>(
                    await response.Content.ReadAsStringAsync(), options);
            }
            else return null;

        }
        public async Task<User> Update(User user)
        {
            HttpClient client = GetClient();
            var response = await client.PutAsync(Url,
                new StringContent(
                    JsonSerializer.Serialize(user),
                    Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonSerializer.Deserialize<User>(
                await response.Content.ReadAsStringAsync(), options);
        }
        public async Task<User> Delete(int id)
        {
            HttpClient client = GetClient();
            var response = await client.DeleteAsync(Url + id);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonSerializer.Deserialize<User>(
               await response.Content.ReadAsStringAsync(), options);
        }
    }
}