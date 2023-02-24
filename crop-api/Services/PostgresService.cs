using CROP.API.Data;
using CROP.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;
using System.Xml.Linq;

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
            var users = LoadData<List<UserData>>("./Seeding/user.json");
            if (users != null)
            {
                var hasher = new PasswordHasher<UserData>();
                foreach (var user in users)
                {
                    user.Password = hasher.HashPassword(user, user.Password);
                    _ = dbContext.Users.Any(_user => _user.Id == user.Id) ? dbContext.Users.Update(user) : dbContext.Users.Add(user);
                }
            }

            if (!dbContext.TagRecords.Any(tag => tag.Station == "test" && tag.Name == "Alarm"))
            {
                dbContext.TagRecords.Add(new TagRecord { Station = "test", Name = "Alarm" });
            }
            if (!dbContext.TagRecords.Any(tag => tag.Station == "test" && tag.Name == "Record"))
            {
                dbContext.TagRecords.Add(new TagRecord { Station = "test", Name = "Record" });
            }

            dbContext.SaveChanges();
        }
        public static T? LoadData<T>(string filename)
        {
            string json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
