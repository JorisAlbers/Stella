namespace StellaClient.Time
{
    public interface ISystemTimeSetter
    {
        /// <summary>
        /// The number of ticks, positive or negative, to add to the current system time.
        /// </summary>
        /// <param name="ticks"></param>
        void AdjustTimeWithTicks(long ticks);

        /// <summary>
        /// Checks if the system time is already sync via a NTP server.
        /// </summary>
        /// <returns></returns>
        bool TimeIsNTPSynced();
    }
}