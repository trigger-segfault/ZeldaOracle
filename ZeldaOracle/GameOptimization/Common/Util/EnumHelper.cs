using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ZeldaOracle.Common.Util {
	/// <summary>A static class with helper methods for enumerations.</summary>
	public static class EnumHelper {
		/// <summary>Gets the description attribute of an enum.</summary>
		public static string ToDescription(this Enum value) {
			if (value == null || !Enum.IsDefined(value.GetType(), value))
				return "";

			FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
			if (fieldInfo != null) {
				DescriptionAttribute[] attributes =
					fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
					as DescriptionAttribute[];
				if (attributes != null && attributes.Length > 0) {
					return attributes[0].Description;
				}
			}

			return value.ToString();
		}

		/// <summary>Returns a collection of all enum values.</summary>
		public static E[] GetValues<E>() where E : struct {
			return (E[]) Enum.GetValues(typeof(E));
		}

		/// <summary>Returns a collection of all enum names.</summary>
		public static string[] GetNames<E>() where E : struct {
			return Enum.GetNames(typeof(E));
		}

		/// <summary>Returns a collection of all Browsable enum values.</summary>
		public static IEnumerable<E> GetBrowsableValues<E>() where E : struct {
			foreach (FieldInfo fieldInfo in from x in typeof(E).GetFields()
				where x.IsLiteral && x.IsBrowsable() select x)
			{
				yield return (E) fieldInfo.GetValue(null);
			}
		}

		/// <summary>Returns a collection of all Browsable enum values.</summary>
		public static IEnumerable<object> GetBrowsableValues(Type enumType) {
			foreach (FieldInfo fieldInfo in from x in enumType.GetFields()
				where x.IsLiteral && x.IsBrowsable() select x)
			{
				yield return fieldInfo.GetValue(null);
			}
		}

		/// <summary>Returns a collection of all Browsable enum names.</summary>
		public static IEnumerable<string> GetBrowsableNames<E>() where E : struct {
			return GetBrowsableNames(typeof(E));
		}

		/// <summary>Returns a collection of all Browsable enum names.</summary>
		public static IEnumerable<string> GetBrowsableNames(Type enumType) {
			foreach (FieldInfo fieldInfo in from x in enumType.GetFields()
				where x.IsLiteral && x.IsBrowsable() select x)
			{
				yield return fieldInfo.Name;
			}
		}
	}
}
