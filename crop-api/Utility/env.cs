namespace CROP.API.Services
{    
    public static class Env
    {
        public const string PostgresConnection = "DATABASE_URL";
        public const string RedisConnection = "REDIS_URL";
        public const string SecretKey = "SECRET_KEY";
    
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2) {
                    continue;
                }

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}