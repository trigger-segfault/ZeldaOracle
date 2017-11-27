using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class AnglePropertyEditor : EnumComboBoxEditor {

		// Until Angles is actually treated as an enum in-game
		private enum Angles {
			None = -1,
			Right = 0,
			UpRight = 1,
			Up = 2,
			UpLeft = 3,
			Left = 4,
			DownLeft = 5,
			Down = 6,
			DownRight = 7
		}

		Type enumType;
		PropertyType baseType;

		protected override IEnumerable CreateItemsSource(PropertyItem propertyItem) {
			CustomPropertyDescriptor propertyDescriptor = (CustomPropertyDescriptor)propertyItem.PropertyDescriptor;
			baseType = propertyDescriptor.Property.Type;
			string typeName = propertyDescriptor.Documentation.EditorSubType;
			enumType = typeof(Angles);
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

		protected override IValueConverter CreateValueConverter() {
			return new EnumValueConverter(enumType, baseType);
		}
	}
}
