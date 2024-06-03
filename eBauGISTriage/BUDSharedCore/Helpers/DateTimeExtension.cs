using System;

namespace BUDSharedCore.Helpers
{
    public static class DateTimeExtension
    {
        public static DateTime IfWeekendThenMonday(this DateTime dt)
        {
            DateTime result = dt;

            if (result.DayOfWeek == DayOfWeek.Saturday)
            {
                result = result.AddDays(2);
            }

            if (result.DayOfWeek == DayOfWeek.Sunday)
            {
                result = dt.AddDays(1);
            }

            return result;
        }

        public static DateTime IfWeekendThenFriday(this DateTime dt)
        {
            DateTime result = dt;

            if (result.DayOfWeek == DayOfWeek.Saturday)
            {
                result = result.AddDays(-1);
            }

            if (result.DayOfWeek == DayOfWeek.Sunday)
            {
                result = dt.AddDays(-2);
            }

            return result;
        }
    }
}
