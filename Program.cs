using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Rabbit
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Credentials = new NetworkCredential("oaf_admin", "oaf_admin1"),
            };

            HttpClient _client = new HttpClient(httpClientHandler);
            HttpResponseMessage response = _client.GetAsync("http://localhost:15672/api/users/").Result;

            var users = response.Content.ReadAsStringAsync().Result;
            var allUsers = JsonConvert.DeserializeObject<List<RabbitUser>>(users);
            HttpResponseMessage permissions;
            foreach(var user in allUsers)
            {
                permissions = _client.GetAsync($"http://localhost:15672/api/users/{user.name}/permissions").Result;
                user.permissions = JsonConvert.DeserializeObject<List<Permissions>>(permissions.Content.ReadAsStringAsync().Result).FirstOrDefault();
                Console.WriteLine(JsonConvert.SerializeObject(user.permissions));
            }
        }
    }

    public class RabbitUser
    {
        public  string name {get; set;}
        public string password_hash {get; set;}
        public string hashing_algorithm {get; set;}
        public string tags {get; set;}
        public Permissions permissions {get; set;}
    }

    public class Permissions
    {
        public string user {get; set;}
        public string vhost {get; set;}
        public string configure {get; set;}
        public string write {get; set; }
        public string read {get; set; }
    }
}
