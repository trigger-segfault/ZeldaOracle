using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConscriptDesigner.Util {
	public static class Extensions {

		public static TimeSpan RoundUpToNearestSecond(this TimeSpan span) {
			return TimeSpan.FromSeconds(Math.Ceiling(span.TotalSeconds));
		}
	}
}
