using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ZeldaEditor.Controls {
	public class AirspaceGridSplitter : GridSplitter {

		private static readonly SolidColorBrush DragBrush = new SolidColorBrush(Color.FromRgb(64, 64, 64));
		private const double DragOpacity = 0.75;

		private Window airspaceWindow;
		private Point gripPosition;
		private Grid grid;

		public AirspaceGridSplitter() {
			PreviewStyle = new Style();
		}

		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			if (e.ChangedButton == MouseButton.Left) {
				grid = base.Parent as Grid;
				if (grid != null) {
					CloseAirspaceWindow();

					airspaceWindow = new Window();
					airspaceWindow.WindowStyle = WindowStyle.None;
					airspaceWindow.ResizeMode = ResizeMode.NoResize;
					airspaceWindow.Background = DragBrush;
					airspaceWindow.Opacity = DragOpacity;
					airspaceWindow.AllowsTransparency = true;
					airspaceWindow.IsHitTestVisible = false;
					airspaceWindow.ShowInTaskbar = false;
					airspaceWindow.Owner = Window.GetWindow(this);
					airspaceWindow.Closed += OnAirspaceWindowClosed;

					Binding b = new Binding("ActualWidth");
					b.Source = this;
					b.Mode = BindingMode.OneWay;
					airspaceWindow.SetBinding(Window.WidthProperty, b);

					b = new Binding("ActualHeight");
					b.Source = this;
					b.Mode = BindingMode.OneWay;
					airspaceWindow.SetBinding(Window.HeightProperty, b);


					Point screen = this.PointToScreen(new Point(0, 0));
					airspaceWindow.Left = screen.X;
					airspaceWindow.Top = screen.Y;

					gripPosition = e.GetPosition(this);
					airspaceWindow.Show();
				}
			}
		}
		protected override void OnMouseUp(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			if (e.ChangedButton == MouseButton.Left) {
				CloseAirspaceWindow();
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if (airspaceWindow != null) {
				Point mouse = e.GetPosition(this);
				if (ResizeDirection == GridResizeDirection.Columns || (ResizeDirection == GridResizeDirection.Auto && Grid.GetColumn(this) != 0)) {
					mouse.Y = 0;
					mouse.X -= gripPosition.X;

					mouse = this.PointToScreen(mouse);
					Point gridPoint = grid.PointToScreen(new Point(0, 0));

					mouse.X = Math.Max(gridPoint.X, Math.Min(gridPoint.X + grid.ActualWidth - airspaceWindow.Width, mouse.X));
				}
				else if (ResizeDirection == GridResizeDirection.Rows || (ResizeDirection == GridResizeDirection.Auto && Grid.GetRow(this) != 0)) {
					mouse.X = 0;
					mouse.Y -= gripPosition.Y;

					mouse = this.PointToScreen(mouse);
					Point gridPoint = grid.PointToScreen(new Point(0, 0));

					mouse.Y = Math.Max(gridPoint.Y, Math.Min(gridPoint.Y + grid.ActualHeight - airspaceWindow.Height, mouse.Y));
				}
				else {
					mouse.X = 0;
					mouse.Y = 0;
				}
				airspaceWindow.Left = mouse.X;
				airspaceWindow.Top = mouse.Y;
			}
		}
		protected override void OnPreviewMouseUp(MouseButtonEventArgs e) {
			// Fix gridsplitter sometimes failing to relocate after a drag
			CancelDrag();
			base.OnPreviewMouseUp(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e) {
			base.OnLostFocus(e);
			CloseAirspaceWindow();
		}
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			base.OnLostMouseCapture(e);
			CloseAirspaceWindow();
		}

		private void OnAirspaceWindowClosed(object sender, EventArgs e) {
			airspaceWindow = null;
		}

		private void CloseAirspaceWindow() {
			if (airspaceWindow != null) {
				airspaceWindow.Close();
			}
		}
	}
}
