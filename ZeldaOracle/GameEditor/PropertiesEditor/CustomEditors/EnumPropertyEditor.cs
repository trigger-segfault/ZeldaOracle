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
using ZeldaOracle.Common.Util;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class EnumPropertyEditor : EnumComboBoxEditor {

		Type enumType;
		VarType baseType;

		protected override IEnumerable CreateItemsSource(PropertyItem propertyItem) {
			CustomPropertyDescriptor propertyDescriptor = (CustomPropertyDescriptor)propertyItem.PropertyDescriptor;
			baseType = propertyDescriptor.Property.VarType;
			string typeName = propertyDescriptor.Documentation.EditorSubType;
			enumType = GameUtil.FindType(typeName, false);
			return GetValues(enumType);
		}

		private static IEnumerable<object> GetValues(Type enumType) {
			if (enumType != null)
				return EnumHelper.GetBrowsableValues(enumType);
			return Enumerable.Empty<object>();
		}

		protected override IValueConverter CreateValueConverter() {
			return new EnumValueConverter(enumType, baseType);
		}
	}


	public class EnumValueConverter : IValueConverter {
		Type enumType;
		VarType baseType;

		public EnumValueConverter(Type enumType, VarType baseType) {
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
				case VarType.Integer: return (int)value;
				case VarType.String: return value.ToString();
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
				case VarType.Integer: return (int)value;
				case VarType.String: return value.ToString();
				}
			}
			return null;
		}
	}
}
