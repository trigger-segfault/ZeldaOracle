using System.Windows;
using System.Windows.Controls;

namespace ZeldaWpf.Controls {
	public class PropertyGridEditorWindowButton : Control {

		public static readonly DependencyProperty TextProperty =
			TextBox.TextProperty.AddOwner(typeof(PropertyGridEditorWindowButton));

		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty IsReadOnlyProperty =
			TextBox.IsReadOnlyProperty.AddOwner(typeof(PropertyGridEditorWindowButton));

		public bool IsReadOnly {
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}

		public static readonly RoutedEvent ClickEvent =
			Button.ClickEvent.AddOwner(typeof(PropertyGridEditorWindowButton));

		public event RoutedEventHandler Click {
			add { AddHandler(ClickEvent, value); }
			remove { RemoveHandler(ClickEvent, value); }
		}

		static PropertyGridEditorWindowButton() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorWindowButton),
					   new FrameworkPropertyMetadata(typeof(PropertyGridEditorWindowButton)));
		}
	}
}
