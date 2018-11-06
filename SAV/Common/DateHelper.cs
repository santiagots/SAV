using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class DateHelper
    {
        public static DateTime getLocal()
        {
            TimeZoneInfo timeInfo = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeInfo);
        }

        public static DateTime GetNextWeekday(DayOfWeek day, DateTime startDate)
        {
            while (startDate.DayOfWeek != day)
                startDate = startDate.AddDays(1);
            return startDate;
        }

        public static DateTime previoustWeekday(DayOfWeek day, DateTime startDate)
        {
            while (startDate.DayOfWeek != day)
                startDate = startDate.AddDays(-1);
            return startDate;
        }
    }
}