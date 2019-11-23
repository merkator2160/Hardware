using System;

namespace Clock.Programmer.Extensions
{
    public static class PosixDateTime
    {
        private static readonly DateTime _posixZeroPoint;


        static PosixDateTime()
        {
            _posixZeroPoint = new DateTime(1970, 1, 1);
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public static UInt64 ToPosixTimeMs(this DateTime time)
        {
            if (time < _posixZeroPoint)
                return 0;

            return (UInt64)time.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
        public static DateTime FromPosixTimeMs(this UInt64 posixTime)
        {
            return new DateTime(1970, 1, 1).Add(TimeSpan.FromMilliseconds(posixTime));
        }
        public static Int32 ToPosixTimeSec(this DateTime time)
        {
            if (time < _posixZeroPoint)
                return 0;

            return (Int32)time.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
        public static DateTime FromPosixTimeSec(this Int32 posixTime)
        {
            return new DateTime(1970, 1, 1).Add(TimeSpan.FromSeconds(posixTime));
        }
    }
}