using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaEditor.Controls;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class BooleanPropertyEditor : TypeEditor<CheckBox> {

		protected override void SetValueDependencyProperty() {
			base.ValueProperty = ToggleButton.IsCheckedProperty;
		}

		protected override CheckBox CreateEditor() {
			return new PropertyGridEditorBooleanCheckBox();
		}
	}
}
