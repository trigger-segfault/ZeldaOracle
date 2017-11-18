using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid;
using ZeldaEditor.Control;
using ZeldaEditor.Windows;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class TextMessagePropertyEditor : WindowPropertyEditor {

		protected override bool IsReadOnly { get { return false; } }

		protected override void OpenWindow(PropertyItem propertyItem) {
			string message = (string)propertyItem.Value;
			string result = TextMessageEditor.Show(Window.GetWindow(Editor), message, EditorControl.Instance);

			if (result != null)
				SetValue(result);
		}
	}
}
