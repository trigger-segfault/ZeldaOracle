using System.Windows;
using Xceed.Wpf.Toolkit;

namespace ZeldaWpf.Controls {
	public class PropertyGridEditorCheckComboBox : CheckComboBox {
		static PropertyGridEditorCheckComboBox() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorCheckComboBox),
					   new FrameworkPropertyMetadata(typeof(PropertyGridEditorCheckComboBox)));
		}
	}
}
