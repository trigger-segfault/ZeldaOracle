using System.Windows;
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
