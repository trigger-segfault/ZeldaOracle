using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
using Xceed.Wpf.Toolkit;
using ZeldaEditor.Controls;
using ZeldaOracle.Common.Util;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class EnumFlagPropertyEditor : TypeEditor<CheckComboBox>, ITypeEditor {

		Type enumType;
		PropertyType baseType;

		protected EditorControl EditorControl {
			get { return PropertyDescriptor.EditorControl; }
		}
		protected CustomPropertyDescriptor PropertyDescriptor {
			get { return PropertyItem.PropertyDescriptor as CustomPropertyDescriptor; }
		}

		protected override void SetValueDependencyProperty() {
			base.ValueProperty = CheckComboBox.SelectedValueProperty;
		}
		
		protected override CheckComboBox CreateEditor() {
			var checkComboBox = new PropertyGridEditorCheckComboBox();
			//var checkComboBox = new CheckComboBox();
			checkComboBox.Delimiter = ", ";
			checkComboBox.Loaded += CheckComboBox_Loaded;
			return checkComboBox;
		}

		private void CheckComboBox_Loaded(object sender, System.Windows.RoutedEventArgs e) {
			base.ResolveValueBinding(PropertyItem);
		}

		protected override void ResolveValueBinding(PropertyItem propertyItem) {
			this.SetItemsSource(propertyItem);
		}
		
		private void SetItemsSource(PropertyItem propertyItem) {
			base.Editor.ItemsSource = this.CreateItemsSource(propertyItem);
		}

		protected IEnumerable CreateItemsSource(PropertyItem propertyItem) {
			baseType = PropertyDescriptor.Property.Type;
			string typeName = PropertyDescriptor.Documentation.EditorSubType;
			enumType = GameUtil.FindType(typeName, false);
			return GetValues(enumType);
		}

		private static IEnumerable<String> GetValues(Type enumType) {
			if (enumType != null)
				return EnumHelper.GetBrowsableNames(enumType);
			return Enumerable.Empty<string>();
		}

		protected override IValueConverter CreateValueConverter() {
			return new EnumFlagValueConverter(enumType, baseType);
		}
	}


	public class EnumFlagValueConverter : IValueConverter {
		Type enumType;
		PropertyType baseType;

		public EnumFlagValueConverter(Type enumType, PropertyType baseType) {
			this.enumType = enumType;
			this.baseType = baseType;
		}

		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is int) {
				List<string> excludes = new List<string>();
				foreach (FieldInfo fieldInfo in from x in enumType.GetFields()
												where x.IsLiteral
												select x) {
					object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(BrowsableAttribute), false);
					if (customAttributes.Length == 1 && !((BrowsableAttribute)customAttributes[0]).Browsable) {
						excludes.Add(fieldInfo.Name);
					}
					else if ((int)fieldInfo.GetValue(enumType) == 0) {
						excludes.Add(fieldInfo.Name);
					}
				}

				try {
					string str = Enum.ToObject(enumType, ((int?)value).Value).ToString();
					foreach (string exclude in excludes) {
						str = str.Replace(exclude + ", ", "");
						str = str.Replace(", " + exclude, "");
						str = str.Replace(exclude, "");
					}
					return str;
				}
				catch { }
			}
			else if (value is string) {
				try {
					if ((string)value == "")
						return 0;
					return (int)Enum.Parse(enumType, value as string, true);
				}
				catch { }
			}
			return null;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is int) {
				List<string> excludes = new List<string>();
				foreach (FieldInfo fieldInfo in from x in enumType.GetFields()
												where x.IsLiteral
												select x) {
					object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(BrowsableAttribute), false);
					if (customAttributes.Length == 1 && !((BrowsableAttribute)customAttributes[0]).Browsable) {
						excludes.Add(fieldInfo.Name);
					}
					else if ((int)fieldInfo.GetValue(enumType) == 0) {
						excludes.Add(fieldInfo.Name);
					}
				}

				try {
					string str = Enum.ToObject(enumType, ((int?)value).Value).ToString();
					foreach (string exclude in excludes) {
						str = str.Replace(exclude + ", ", "");
						str = str.Replace(", " + exclude, "");
						str = str.Replace(exclude, "");
					}
					return str;
				}
				catch { }
			}
			else if (value is string) {
				try {
					if ((string)value == "")
						return 0;
					return (int)Enum.Parse(enumType, value as string, true);
				}
				catch { }
			}
			return null;
		}
	}
}
