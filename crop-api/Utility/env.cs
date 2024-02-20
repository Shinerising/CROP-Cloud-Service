using System.Diagnostics;

namespace CROP.API.Services
{    
    public static class Env
    {
        public const string PostgresConnection = "DATABASE_URL";
        public const string RedisConnection = "REDIS_URL";
        public const string SecretKey = "SECRET_KEY";
        public const string CORS = "CORS_URL";

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
    public static class Utils
    {
        public static async Task<string> ExecuteCommand(string command)
        {
            string result = "";
            await Task.Run(() =>
            {
                try
                {
                    using Process proc = new();
                    proc.StartInfo.FileName = "/bin/sh";
                    proc.StartInfo.Arguments = "-c \" " + command + " \"";
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardError = true;
                    proc.Start();

                    result += proc.StandardOutput.ReadToEnd();
                    result += proc.StandardError.ReadToEnd();

                    proc.WaitForExit();
                }
                catch
                {

                }
            });
            return result;
        }
    }
}