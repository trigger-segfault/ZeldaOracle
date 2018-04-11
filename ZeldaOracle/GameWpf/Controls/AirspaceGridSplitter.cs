using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ZeldaOracle.Common.Geometry;
using ZeldaWpf.Util;

namespace ZeldaWpf.Controls {
	/// <summary>A gridsplitter control that draws over WinForms controls.</summary>
	public class AirspaceGridSplitter : GridSplitter {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The brush color of the drag preview.</summary>
		private static readonly SolidColorBrush DragBrush =
			WpfHelper.ColorBrush(64, 64, 64).AsFrozen();
		/// <summary>The opacity of the drag preview.</summary>
		private const double DragOpacity = 0.75;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The window used to preview the dragging result.</summary>
		private Window airspaceWindow;
		/// <summary>The relative position of the grip while dragging.</summary>
		private Point gripPosition;
		/// <summary>The grid containing this gridsplitter.</summary>
		private Grid grid;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the airspace gridsplitter.</summary>
		public AirspaceGridSplitter() {
			PreviewStyle = new Style();
		}


		//-----------------------------------------------------------------------------
		// Override Events
		//-----------------------------------------------------------------------------

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
				if (ResizeDirection == GridResizeDirection.Columns ||
					(ResizeDirection == GridResizeDirection.Auto &&
					Grid.GetColumn(this) != 0))
				{
					mouse.Y = 0;
					mouse.X -= gripPosition.X;

					mouse = this.PointToScreen(mouse);
					Point gridPoint = grid.PointToScreen(new Point(0, 0));

					double maxX = gridPoint.X + grid.ActualWidth -
						airspaceWindow.Width;
					mouse.X = GMath.Clamp(mouse.X, gridPoint.X, maxX);
				}
				else if (ResizeDirection == GridResizeDirection.Rows ||
					(ResizeDirection == GridResizeDirection.Auto &&
					Grid.GetRow(this) != 0))
				{
					mouse.X = 0;
					mouse.Y -= gripPosition.Y;

					mouse = this.PointToScreen(mouse);
					Point gridPoint = grid.PointToScreen(new Point(0, 0));


					double maxY = gridPoint.Y + grid.ActualHeight -
						airspaceWindow.Height;
					mouse.Y = GMath.Clamp(mouse.Y, gridPoint.Y, maxY);
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


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Attemps to close the airspace window if it's open.</summary>
		private void CloseAirspaceWindow() {
			if (airspaceWindow != null) {
				airspaceWindow.Close();
			}
		}
	}
}
