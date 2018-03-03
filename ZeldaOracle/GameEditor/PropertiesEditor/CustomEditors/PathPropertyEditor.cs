using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZeldaEditor.Control;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	public class PathPropertyEditor : WindowPropertyEditor {

		protected override bool IsReadOnly { get { return true; } }

		protected CustomPropertyDescriptor PropertyDescriptor {
			get { return PropertyItem.PropertyDescriptor as CustomPropertyDescriptor; }
		}
		protected EditorControl EditorControl {
			get { return PropertyDescriptor.EditorControl; }
		}

		protected override void OpenWindow() {
			Properties properties =
				EditorControl.PropertyGrid.PropertyObject.Properties;

			string newPath = TilePathEditor.ShowEditor(Window.GetWindow(Editor),
				EditorControl, properties, PropertyDescriptor.Property.Name);
			
			// Update the properry grid control
			SetValue(newPath);
		}
	}
}
