using System.Windows;

namespace ZeldaWpf.Controls {
	public class PropertyGridEditorPointUpDown : PointUpDown {
		static PropertyGridEditorPointUpDown() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorPointUpDown),
				new FrameworkPropertyMetadata(typeof(PropertyGridEditorPointUpDown)));
		}
	}
}
