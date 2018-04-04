using System;
using System.ComponentModel;
using System.Globalization;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Converters {
	public class DirectionConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context,
			Type sourceType)
		{
			if (sourceType == typeof(int) || sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context,
			CultureInfo culture, object value)
		{
			if (value is int) {
				return (Direction) (int) value;
			}
			if (value is string) {
				Direction result;
				if (!Direction.TryParse((string) value, true, out result))
					throw new ArgumentException("Invalid direction string!");
				return result;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context,
			CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(int)) {
				return (int) (Direction) value;
			}
			if (destinationType == typeof(string)) {
				Direction direction = (Direction) value;
				return direction.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
