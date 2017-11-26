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

		protected EditorControl EditorControl {
			get { return PropertyDescriptor.EditorControl; }
		}
		protected CustomPropertyDescriptor PropertyDescriptor {
			get { return PropertyItem.PropertyDescriptor as CustomPropertyDescriptor; }
		}

		protected override bool IsReadOnly { get { return false; } }

		protected override void OpenWindow() {
			string message = (string)PropertyItem.Value;
			string result = TextMessageEditor.Show(Window.GetWindow(Editor), message, EditorControl);

			if (result != null)
				SetValue(result);
		}
	}
}
