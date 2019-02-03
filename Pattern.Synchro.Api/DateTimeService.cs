using System;

namespace Pattern.Synchro.Api
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime DateTimeNow()
        {
            return DateTime.Now;
        }
    }
}