using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class NoneStringConverter : IValueConverter {
			
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string) {
				string str = value as string;
				if (str == "(none)")
					str = "";
				return str;
			}
			return null;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string) {
				string str = value as string;
				if (str == "(none)")
					str = "";
				return str;
			}
			return null;
		}
	}
}
