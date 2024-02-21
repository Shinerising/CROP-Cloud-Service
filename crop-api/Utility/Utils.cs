using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CROP.API.Utility
{
    public static class Utils
    {
        public static bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static async Task<string> ExecuteCommand(string command)
        {
            string result = "";
            await Task.Run(() =>
            {
                try
                {
                    using Process proc = new();
                    if (IsWindows)
                    {
                        proc.StartInfo.FileName = "powershell";
                        proc.StartInfo.Arguments = "-Command \" " + command + " \"";
                    }
                    else
                    {
                        proc.StartInfo.FileName = "/bin/sh";
                        proc.StartInfo.Arguments = "-c \" " + command + " \"";
                    }
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
            return result.Trim();
        }
    }
}