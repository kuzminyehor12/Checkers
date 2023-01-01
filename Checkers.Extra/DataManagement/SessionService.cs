using Checkers.Server.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Checkers.Extra.DataManagement
{
    public class SessionService : ISessionService
    {
        public string Path { get; } = @"C:\Users\EgorKuzmin\source\repos\Lab2_Checkers\session.json";
        public SessionService()
        {
            if (!File.Exists(Path))
                using (var writer = File.Open(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) { }
        }

        public SessionService(string path)
        {
            Path = path;
            if (!File.Exists(Path))
                using (var writer = File.Open(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)) { }
        }
        public void CreateSession(Session model)
        {
            var session = JObject.Parse(JsonConvert.SerializeObject(model));
            var jsonResult = JsonConvert.SerializeObject(session, Formatting.Indented);
            File.WriteAllText(Path, jsonResult);
        }

        public Session GetSession()
        {
            var json = File.ReadAllText(Path);
            var session = new Session();

            if (string.IsNullOrEmpty(json))
            {
                return session;
            }

            session = JsonConvert.DeserializeObject<Session>(json);
            return session;
        }

        public void RemoveSession()
        {
            File.Delete(Path);
        }

        public void UpdateSession(Session model)
        {
            var json = File.ReadAllText(Path);

            if (string.IsNullOrEmpty(json))
            {
                CreateSession(model);
                return;
            }

            var jObject = JObject.Parse(json);
            jObject["FirstPlayerSessionVictoriesCount"] = model.FirstPlayerSessionVictoriesCount;
            jObject["SecondPlayerSessionVictoriesCount"] = model.SecondPlayerSessionVictoriesCount;
            string jsonResult = JsonConvert.SerializeObject(jObject, Formatting.Indented);
            File.WriteAllText(Path, jsonResult);
        }
    }
}
