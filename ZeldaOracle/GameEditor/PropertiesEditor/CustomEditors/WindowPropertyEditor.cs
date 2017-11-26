using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using ZeldaEditor.Controls;

namespace ZeldaEditor.PropertiesEditor.CustomEditors {
	
	public abstract class WindowPropertyEditor : TypeEditor<PropertyGridEditorWindowButton> {
		
		protected override void SetValueDependencyProperty() {
			base.ValueProperty = PropertyGridEditorWindowButton.TextProperty;
		}

		protected override PropertyGridEditorWindowButton CreateEditor() {
			var control = new PropertyGridEditorWindowButton();
			control.IsReadOnly = IsReadOnly;
			control.Click += OnOpenWindow;
			return control;
		}

		protected abstract bool IsReadOnly { get; }

		protected abstract void OpenWindow();

		private void OnOpenWindow(object sender, RoutedEventArgs e) {
			OpenWindow();
		}

		protected void SetValue(string value) {
			Editor.Text = value;
		}
	}
}
