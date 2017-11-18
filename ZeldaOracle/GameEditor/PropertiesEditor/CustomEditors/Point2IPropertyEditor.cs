using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaOracle.Common.Geometry;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class Point2IPropertyEditor : TextBoxEditor {
		protected override IValueConverter CreateValueConverter() {
			return new Point2IValueConverter();
		}
	}

	public class Point2IValueConverter : IValueConverter {
		
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string) {
				try {
					return Point2I.Parse(value as string);
				}
				catch { }
			}
			else if (value is Point2I) {
				Point2I point = (value as Point2I?).Value;
				return point.X + "," + point.Y;
			}
			return null;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string) {
				try {
					return Point2I.Parse(value as string);
				}
				catch { }
			}
			else if (value is Point2I) {
				Point2I point = (value as Point2I?).Value;
				return point.X + "," + point.Y;
			}
			return null;
		}
	}
}
