using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Util {
	public static class EnumHelper {
		public static string ToDescription(this Enum value) {
			if (value == null) {
				return string.Empty;
			}

			if (!Enum.IsDefined(value.GetType(), value)) {
				return string.Empty;
			}

			FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
			if (fieldInfo != null) {
				DescriptionAttribute[] attributes =
					fieldInfo.GetCustomAttributes(typeof (DescriptionAttribute), false) as DescriptionAttribute[];
				if (attributes != null && attributes.Length > 0) {
					return attributes[0].Description;
				}
			}

			return value.ToString();
		}
	}
}
