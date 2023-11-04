using CROP.API.Data;
using CROP.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CROP.API.Services
{
    public static class PostgresService
    {
        public static void InitializeDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
            var context = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
            dbInitializer.Initialize(context);
        }
    }

    public class DbInitializer
    {
        public void Initialize(PostgresDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();
            var users = LoadXmlData<List<UserData>>("users", "./Config/user.xml");
            if (users != null)
            {
                var hasher = new PasswordHasher<UserData>();
                foreach (var user in users)
                {
                    user.Password = hasher.HashPassword(user, user.Password);
                    _ = dbContext.Users.Any(_user => _user.Id == user.Id) ? dbContext.Users.Update(user) : dbContext.Users.Add(user);
                }
            }

            var stations = LoadXmlData<List<StationInfo>>("stations", "./Config/station.xml");
            if (stations != null) {
                foreach (var station in stations.Where(station => !station.IsDisabled)) {
                    if (!dbContext.Stations.Any(_station => _station.Id == station.Id)) {
                        dbContext.Stations.Add(station);
                    }
                    else
                    {
                        dbContext.Stations.Update(station);
                    }

                    if (!dbContext.TagRecords.Any(tag => tag.Station == station.Id && tag.Name == "Alarm")) {
                        dbContext.TagRecords.Add(new TagRecord { Station = station.Id, Name = "Alarm" });
                    }
                    if (!dbContext.TagRecords.Any(tag => tag.Station == station.Id && tag.Name == "Record")) {
                        dbContext.TagRecords.Add(new TagRecord { Station = station.Id, Name = "Record" });
                    }
                }
            }

            dbContext.SaveChanges();
        }
        public static T? LoadXmlData<T>(string root, string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        var serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));
                        return (T?)serializer.Deserialize(stream);
                    }
                }
            }
            catch
            {
            }
            return default;
        }
    }
}
