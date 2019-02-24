using System;
using System.Diagnostics;

namespace StellaClient.Time
{
    public class LinuxTimeSetter : ISystemTimeSetter
    {
        private long epochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        public void AdjustTimeWithTicks(long ticks)
        {
            long unixTimeOld = (DateTime.UtcNow.Ticks - epochTicks);
            long unixTimeNew = unixTimeOld + ticks;

            long unixTimeSeconds = unixTimeNew / TimeSpan.TicksPerSecond;
            // TODO add miliseconds int miliseconds = TimeSpan.TicksPerMillisecond

            // date --set="2011-12-07 01:20:15.962"  && date --rfc-3339=ns
            string returnValue = RunBashCommand($"date -s @{unixTimeSeconds}");
        }

        private string RunBashCommand(string cmd)
        {
            string escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}