using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZeldaOracle.Common.Geometry;
using ZeldaWpf.WinForms;
using Cursor = System.Windows.Forms.Cursor;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using Key = System.Windows.Input.Key;
using MouseButton = System.Windows.Input.MouseButton;
using ModifierKeys = System.Windows.Input.ModifierKeys;
using FormsControl = System.Windows.Forms.Control;
using ZeldaWpf.Util;
using ZeldaOracle.Common.Util;

namespace ZeldaWpf.Tools {
	/// <summary>The base event arguments for WinForms tools.</summary>
	public class WinFormsToolEventArgs {

		/// <summary>Gets the control this event was sent from.</summary>
		public GraphicsDeviceControl Sender { get; }
		/// <summary>Gets if the mouse is inside the control.</summary>
		public bool IsInside { get; }
		/// <summary>Gets the current button for this mouse event.</summary>
		public MouseButton Button { get; }
		/// <summary>Gets the mouse position.</summary>
		public Point2I Position { get; }
		/// <summary>Gets the mouse wheel delta.</summary>
		public int Delta { get; }
		/// <summary>Gets the number of mouse clicks.</summary>
		public int Clicks { get; }
		public bool IsOpposite(MouseButton button) {
			return	(Button == MouseButton.Left && button == MouseButton.Right) ||
					(Button == MouseButton.Right && button == MouseButton.Left);
		}
		public bool IsLeftOrRight {
			get { return (Button == MouseButton.Left || Button == MouseButton.Right); }
		}

		public WinFormsToolEventArgs(GraphicsDeviceControl sender) {
			Sender = sender;
			IsInside = sender.IsMouseOver;
			Button = WpfCasting.NoButton;
			Position = sender.ScrollPosition +
					sender.PointToClient(Cursor.Position).ToPoint2I();
			Delta = 0;
			Clicks = 0;
		}

		public WinFormsToolEventArgs(GraphicsDeviceControl sender, MouseEventArgs e)
			: this(sender)
		{
			Button = e.Button.ToWpfMouseButton();
			Position = e.Location.ToPoint2I();
			Delta = e.Delta;
			Clicks = e.Clicks;
		}
	}
}
