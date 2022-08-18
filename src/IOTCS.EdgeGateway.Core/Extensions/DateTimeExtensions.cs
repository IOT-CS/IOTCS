namespace System
{
	/// <summary>
	/// Date time extension methods<br/>
	/// 时间的扩展函数<br/>
	/// </summary>
	public static class DateTimeExtensions {

		/// <summary>类型转换提供者</summary>
		/// <remarks>重载默认提供者<seealso cref="DefaultConvert"/>并赋值给<see cref="Convert"/>可改变所有类型转换的行为</remarks>
		public static DefaultConvert Convert { get; set; } = new DefaultConvert();

		/// <summary>
		/// Truncate datetime, only keep seconds<br/>
		/// 去掉时间中秒数后的部分<br/>
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var time = DateTime.UtcNow;
		/// var truncated = time.Truncate();
		/// </code>
		/// </example>
		public static DateTime Truncate(this DateTime time) {
			return time.AddTicks(-(time.Ticks % TimeSpan.TicksPerSecond));
		}

		/// <summary>
		/// Return unix style timestamp<br/>
		/// Return a minus value if the time early than 1970-1-1<br/>
		/// The given time will be converted to UTC time first<br/>
		/// 获取unix风格的时间戳<br/>
		/// 如果时间早于1970年1月1日则返回值是负数<br/>
		/// 参数会先转换为UTC时间<br/>
		/// </summary>
		/// <param name="time">The time</param>
		/// <returns></returns>
		/// <example>
		/// <code language="cs">
		/// var time = new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc);
		/// var timeStamp = (int)time.ToTimestamp().TotalSeconds; // == 86400
		/// </code>
		/// </example>
		public static TimeSpan ToTimestamp(this DateTime time) {
			return time.ToUniversalTime() - new DateTime(1970, 1, 1);
		}

		/// <summary>转为时间日期，转换失败时返回最小时间。支持字符串、整数（Unix秒）</summary>
		/// <param name="value">待转换对象</param>
		/// <returns></returns>
		public static DateTime ToDateTime(this Object value) => Convert.ToDateTime(value, DateTime.MinValue);

		/// <summary>时间日期转为yyyy-MM-dd HH:mm:ss完整字符串，对UTC时间加后缀</summary>
		/// <remarks>最常用的时间日期格式，可以无视各平台以及系统自定义的时间格式</remarks>
		/// <param name="value">待转换对象</param>
		/// <returns></returns>
		public static String ToFullString(this DateTime value) => Convert.ToFullString(value);
	}
}
