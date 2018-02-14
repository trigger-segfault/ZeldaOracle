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
	public class Point2ISingleAxisPropertyEditor : IntegerUpDownEditor {

		protected CustomPropertyDescriptor PropertyDescriptor {
			get { return PropertyItem.PropertyDescriptor as CustomPropertyDescriptor; }
		}

		protected override IValueConverter CreateValueConverter() {
			string subtype = PropertyDescriptor.Documentation.EditorSubType;
			string[] parts = subtype.Split(new char[] { ':' }, 2);

			bool vertical = string.Compare(parts[0], "vertical", true) == 0;
			int otherValue = 0;
			if (parts.Length == 2)
				int.TryParse(parts[1], out otherValue);

			return new Point2ISingleAxisValueConverter(vertical, otherValue);
		}
	}

	public class Point2ISingleAxisValueConverter : IValueConverter {

		private bool vertical;
		private int otherValue;

		public Point2ISingleAxisValueConverter(bool vertical, int otherValue) {
			this.vertical = vertical;
			this.otherValue = otherValue;
		}

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is int) {
				return Point2I.FromBoolean(vertical, (int) value, otherValue);
			}
			else if (value is Point2I) {
				return ((Point2I) value)[vertical];
			}
			return null;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is int) {
				return Point2I.FromBoolean(vertical, (int) value, otherValue);
			}
			else if (value is Point2I) {
				return ((Point2I) value)[vertical];
			}
			return null;
		}
	}
}
