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
	public class DirectionPropertyEditor : EnumComboBoxEditor {

		// Until Angles is actually treated as an enum in-game
		public enum Directions {
			None = -1,
			Right = 0,
			Up = 1,
			Left = 2,
			Down = 3
		}

		Type enumType;
		VarType baseType;

		protected override IEnumerable CreateItemsSource(PropertyItem propertyItem) {
			CustomPropertyDescriptor propertyDescriptor = (CustomPropertyDescriptor)propertyItem.PropertyDescriptor;
			baseType = propertyDescriptor.Property.VarType;
			string typeName = propertyDescriptor.Documentation.EditorSubType;
			enumType = typeof(Directions);
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
