using System;
using StellaClient.Time;

namespace EndToEndTests
{
    public class TimeTests
    {
        public void Start()
        {
            bool run = true;
            while(run)
            {
                Console.WriteLine("Time Test");
                Console.WriteLine("e - exit time test");
                Console.WriteLine("n - Check if system time is NTP synced");
                Console.WriteLine("s - Sets the time a year ahead");
                Console.WriteLine();

                string input = Console.ReadLine();
                switch(input)
                {
                    case "e":
                        run = false;
                        break;
                    case "n":
                        SystemTimeSycned();
                        break;       
                    case "s":
                        SetSystemTime();
                        break;        
                    default:
                        Console.WriteLine($"Unknown command {input}");
                        break;
                }

            }

        }

        private void SystemTimeSycned()
        {
            var timeSetter = new LinuxTimeSetter();
            Console.WriteLine($"System time NTP synced : {timeSetter.TimeIsNTPSynced()}");
        }

        private void SetSystemTime()
        {
            var timeSetter = new LinuxTimeSetter();
            timeSetter.AdjustTimeWithTicks(TimeSpan.FromDays(365).Ticks);
            Console.WriteLine($"Set system time a year ahead");
        }

    }
}