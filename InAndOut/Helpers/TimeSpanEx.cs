using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InAndOut.Helpers
{
    public static class TimeSpanEx
    {
       
        public static TimeSpan RoundToSeconds(this TimeSpan span)
        {
            var spanTicks = span.Ticks;
            var seconds = spanTicks/TimeSpan.TicksPerSecond;
            var remainder = spanTicks % TimeSpan.TicksPerSecond;
            if (remainder > TimeSpan.TicksPerSecond / 2)
                seconds++;
            return TimeSpan.FromSeconds(seconds);
        }
    }
}
