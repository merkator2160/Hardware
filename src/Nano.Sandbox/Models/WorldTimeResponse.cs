using System;

namespace Nano.Sandbox.Models
{
	public class WorldTimeResponse
	{
		public String abbreviation { get; set; }

		////[JsonProperty("client_ip")]
		//public String ClientIp { get; set; }

		//[JsonProperty("client_ip")]
		public String datetime { get; set; }

		////[JsonProperty("day_of_week")]
		//public Int32 DayOfWeek { get; set; }

		////[JsonProperty("day_of_year")]
		//public Int32 DayOfYear { get; set; }

		//public Boolean Dst { get; set; }

		////[JsonProperty("dst_from")]
		//public String DstFrom { get; set; }

		////[JsonProperty("dst_offset")]
		//public Int32 DstOffset { get; set; }

		////[JsonProperty("dst_until")]
		//public String DstUntil { get; set; }

		////[JsonProperty("raw_offset")]
		//public Int32 RawOffset { get; set; }

		//public String TimeZone { get; set; }
		//public Int32 UnixTime { get; set; }

		////[JsonProperty("utc_datetime")]
		//public DateTime UtcDateTime { get; set; }

		////[JsonProperty("utc_offset")]
		//public DateTime UtcOffset { get; set; }

		////[JsonProperty("week_number")]
		//public Int32 WeekNumber { get; set; }
	}
}