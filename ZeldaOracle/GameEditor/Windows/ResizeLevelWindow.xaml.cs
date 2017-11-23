using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Geometry;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for ResizeLevelWindow.xaml
	/// </summary>
	public partial class ResizeLevelWindow : Window {
		private Point2I newSize;

		public ResizeLevelWindow(Point2I currentSize) {
			InitializeComponent();

			spinnerWidth.Value = currentSize.X;
			spinnerHeight.Value = currentSize.Y;
			spinnerWidth.Focus();
		}

		private void OnResize(object sender, RoutedEventArgs e) {
			newSize = new Point2I(spinnerWidth.Value.Value, spinnerHeight.Value.Value);
			DialogResult = true;
			Close();
		}


		public static bool Show(Window owner, ref Point2I dimensions) {
			ResizeLevelWindow window = new ResizeLevelWindow(dimensions);
			window.Owner = owner;
			var result = window.ShowDialog();
			if (result.HasValue && result.Value) {
				dimensions = window.newSize;
				return true;
			}
			return false;
		}
	}
}
