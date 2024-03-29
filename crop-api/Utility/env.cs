namespace CROP.API.Utility

{
    public static class Env
    {
        public const string PostgresConnection = "DATABASE_URL";
        public const string RedisConnection = "REDIS_URL";
        public const string CORS = "CORS";
        public const string ConfigFolder = "CONFIG_PATH";
        public const string UserFolder = "USER_PATH";
        public const string StorageFolder = "STORAGE_PATH";

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