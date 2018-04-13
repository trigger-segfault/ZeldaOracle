using System.Windows;
using System.Windows.Controls;

namespace ZeldaWpf.Controls {
	public class PropertyGridEditorBooleanCheckBox : CheckBox {

		static PropertyGridEditorBooleanCheckBox() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorBooleanCheckBox),
					   new FrameworkPropertyMetadata(typeof(PropertyGridEditorBooleanCheckBox)));
		}
	}
}
