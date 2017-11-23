using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ZeldaEditor.Controls {
	public class PropertyGridEditorBooleanCheckBox : CheckBox {

		static PropertyGridEditorBooleanCheckBox() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorBooleanCheckBox),
					   new FrameworkPropertyMetadata(typeof(PropertyGridEditorBooleanCheckBox)));
		}
	}
}
