using System.Windows;
using ZeldaOracle.Common.Geometry;

namespace ConscriptDesigner.Windows {
	/// <summary>
	/// Interaction logic for ResizeLevelWindow.xaml
	/// </summary>
	public partial class ResizeTilesetWindow : Window {
		private Point2I newSize;

		public ResizeTilesetWindow(Point2I currentSize) {
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
			ResizeTilesetWindow window = new ResizeTilesetWindow(dimensions);
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
