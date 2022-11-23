using Checkers.Server.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Checkers.Server.DataManagement
{
    public class UserService : IUserService
    {
        public string Path { get; } = @"C:\Users\EgorKuzmin\source\repos\Lab2_Checkers\users.json";

        public UserService()
        {
            if (!File.Exists(Path))
                using (var writer = File.Open(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) { }
        }
        public UserService(string path)
        {
            Path = path;
            if (!File.Exists(Path))
                using (var writer = File.Open(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) { }
        }

        public void CreateUser(User model)
        {
            var json = File.ReadAllText(Path);

            if (string.IsNullOrEmpty(json))
            {
                var jArray = new JArray();
                var tempUser = JObject.Parse(JsonConvert.SerializeObject(model));
                jArray.Add(tempUser);
                var jsonResult = JsonConvert.SerializeObject(jArray, Formatting.Indented);
                File.WriteAllText(Path, jsonResult);
                return;
            }
          
            var jsonArray = JArray.Parse(json);
            var newUser = JObject.Parse(JsonConvert.SerializeObject(model));
            jsonArray.Add(newUser);
            string newJsonResult = JsonConvert.SerializeObject(jsonArray, Formatting.Indented);
            File.WriteAllText(Path, newJsonResult);
        }

        public IEnumerable<User> GetUsers()
        {
            List<User> users = new List<User>();
            var json = File.ReadAllText(Path);

            if (string.IsNullOrEmpty(json))
            {
                return users;
            }

            users = JsonConvert.DeserializeObject<List<User>>(json);
            return users;
        }

        public void UpdateUser(User model)
        {
            var json = File.ReadAllText(Path);
            var jsonArray = JArray.Parse(json);
            jsonArray.First(e => e["Nickname"].Value<string>() == model.Nickname)["VictoriesCount"] = model.VictoriesQuantity;
            string jsonResult = JsonConvert.SerializeObject(jsonArray, Formatting.Indented);
            File.WriteAllText(Path, jsonResult);
        }
    }
}
