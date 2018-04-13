using System.Windows;

namespace ZeldaWpf.Controls {
	public class PropertyGridEditorVectorUpDown : VectorUpDown {
		static PropertyGridEditorVectorUpDown() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorVectorUpDown),
				new FrameworkPropertyMetadata(typeof(PropertyGridEditorVectorUpDown)));
		}
	}
}
