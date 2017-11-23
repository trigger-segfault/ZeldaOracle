using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaEditor.Control;
using ZeldaOracle.Game;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using System.Collections;
using Xceed.Wpf.Toolkit.PropertyGrid;
using System.Windows.Data;
using System.Globalization;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class EnumPropertyEditor : EnumComboBoxEditor {

		Type enumType;
		PropertyType baseType;

		protected override IEnumerable CreateItemsSource(PropertyItem propertyItem) {
			CustomPropertyDescriptor propertyDescriptor = (CustomPropertyDescriptor)propertyItem.PropertyDescriptor;
			baseType = propertyDescriptor.Property.Type;
			string typeName = propertyDescriptor.Documentation.EditorSubType;
			enumType = GetTypeByName(typeName);
			return GetValues(enumType);
		}

		private static object[] GetValues(Type enumType) {
			List<object> list = new List<object>();
			if (enumType != null) {
				foreach (FieldInfo fieldInfo in from x in enumType.GetFields()
												where x.IsLiteral
												select x) {
					object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(BrowsableAttribute), false);
					if (customAttributes.Length != 1 || ((BrowsableAttribute)customAttributes[0]).Browsable) {
						list.Add(fieldInfo.GetValue(enumType));
					}
				}
			}
			return list.ToArray();
		}

		private static Type GetTypeByName(string className) {
			Type[] assemblyTypes = typeof(Property).Assembly.GetTypes();
			for (int j = 0; j < assemblyTypes.Length; j++) {
				if (assemblyTypes[j].Name == className) {
					return assemblyTypes[j];
				}
			}
			throw new Exception("Unknown editor subtype");
		}

		protected override IValueConverter CreateValueConverter() {
			return new EnumValueConverter(enumType, baseType);
		}
	}


	public class EnumValueConverter : IValueConverter {
		Type enumType;
		PropertyType baseType;

		public EnumValueConverter(Type enumType, PropertyType baseType) {
			this.enumType = enumType;
			this.baseType = baseType;
		}

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is int?) {
				try { return Enum.ToObject(enumType, ((int?)value).Value); } catch { }
			}
			else if (value is string) {
				try { return Enum.Parse(enumType, value as string, true); } catch { }
			}
			else if (value.GetType() == enumType) {
				switch (baseType) {
				case PropertyType.Integer: return (int)value;
				case PropertyType.String: return value.ToString();
				}
			}
			return null;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is int?) {
				try { return Enum.ToObject(enumType, ((int?)value).Value); } catch { }
			}
			else if (value is string) {
				try { return Enum.Parse(enumType, value as string, true); } catch { }
			}
			else if (value.GetType() == enumType) {
				switch (baseType) {
				case PropertyType.Integer: return (int)value;
				case PropertyType.String: return value.ToString();
				}
			}
			return null;
		}
	}
}
