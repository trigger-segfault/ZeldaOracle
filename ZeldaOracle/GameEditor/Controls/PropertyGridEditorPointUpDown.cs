using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZeldaEditor.Controls {
	public class PropertyGridEditorPointUpDown : PointUpDown {
		static PropertyGridEditorPointUpDown() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorPointUpDown),
				new FrameworkPropertyMetadata(typeof(PropertyGridEditorPointUpDown)));
		}
	}
}
