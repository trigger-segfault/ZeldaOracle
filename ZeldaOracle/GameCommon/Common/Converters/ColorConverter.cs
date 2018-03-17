using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Converters {
	public class ColorConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context,
			Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context,
			CultureInfo culture, object value)
		{
			if (value is string) {
				try {
					return Color.Parse((string) value);
				}
				catch { }
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				Color color = (Color) value;
				return color.R + "," + color.G + "," + color.B + "," + color.A;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
