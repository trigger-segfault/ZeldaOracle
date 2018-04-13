using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ZeldaWpf.Controls {
	public class IsNullConverter : IValueConverter {

		/// <summary>Converts an empty string to "(none)".</summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value == null;
		}

		/// <summary>Converts "(none)" to an empty string.</summary>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
