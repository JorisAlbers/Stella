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
            if(!RunBashCommand($"date -s @{unixTimeSeconds}", out string returnMessage))
            {
                throw new Exception($"Failed to set the time. Return message of process: {returnMessage}");
            }
        }

        public bool TimeIsNTPSynced()
        {
            if(RunBashCommand("timedatectl status", out string returnMessage))
            {
                // Parse the message.
                // We are looking for "NTP synchronized : "
                string[] split = returnMessage.Split('\n');
                for(int i=0; i< split.Length;i++)
                {
                    string[] lineSplit = split[i].Split(':');
                    Console.WriteLine($"{lineSplit[0]} - {lineSplit[1]}");
                    if(lineSplit[0].Contains("NTP synchronized"))
                    {
                        return lineSplit[1].Contains("yes");
                    }
                }
                throw new Exception($"Failed to parse the timedatectl status message. Return message of process: {returnMessage}");
            }

            throw new Exception($"Failed to get the timedatectl status. Return message of process: {returnMessage}");
        }

        private bool RunBashCommand(string cmd, out string returnMessage)
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
            returnMessage = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return process.ExitCode == 0;
        }
    }
}