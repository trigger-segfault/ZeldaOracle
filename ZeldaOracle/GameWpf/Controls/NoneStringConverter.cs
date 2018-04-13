using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ZeldaWpf.Controls {
	/// <summary>A converter for representing an empty string as "(none)".</summary>
	public class NoneStringConverter : IValueConverter {

		/// <summary>Converts an empty string to "(none)".</summary>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string) {
				string str = value as string;
				if (str == "")
					str = "(none)";
				return str;
			}
			return "";
		}

		/// <summary>Converts "(none)" to an empty string.</summary>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string) {
				string str = value as string;
				if (str == "(none)")
					str = "";
				return str;
			}
			return "";
		}
	}
}
