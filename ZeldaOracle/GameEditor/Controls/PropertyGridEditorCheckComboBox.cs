using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace ZeldaEditor.Controls {
	public class PropertyGridEditorCheckComboBox : CheckComboBox {
		static PropertyGridEditorCheckComboBox() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorCheckComboBox),
					   new FrameworkPropertyMetadata(typeof(PropertyGridEditorCheckComboBox)));
		}
	}
}
