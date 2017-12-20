using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConscriptDesigner.Util {
	/// <summary>Miscellaneous extensions that don't need their own class.</summary>
	public static class Extensions {

		/// <summary>Rounds the timespan up to the nearest second.</summary>
		public static TimeSpan RoundUpToNearestSecond(this TimeSpan span) {
			return TimeSpan.FromSeconds(Math.Ceiling(span.TotalSeconds));
		}
	}
}
