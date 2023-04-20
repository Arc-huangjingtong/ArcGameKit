using System;

namespace HaiPackage
{
    public static class GTime
    {
        /// <summary>
        /// 获取某个时间与当前时间的差值 
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns></returns>
        public static TimeSpan GetDifference(long date)
        {
            return GetDifference(new DateTime(new DateTime(1970, 1, 1).Ticks + long.Parse(date.ToString()) * 10000).ToLocalTime());
        }

        /// <summary>
        /// 获取某个时间与当前时间的差值 
        /// </summary>
        /// <param name="date">时间</param>
        /// <returns></returns>
        public static TimeSpan GetDifference(DateTime date)
        {
            return DateTime.Now > date.ToLocalTime() ? DateTime.Now - date.ToLocalTime() : date.ToLocalTime() - DateTime.Now;
        }
    }
}