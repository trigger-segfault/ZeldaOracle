using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ConscriptDesigner.Control;
using ZeldaWpf.Controls;

namespace ConscriptDesigner.Anchorables {
	/// <summary>A layout anchorable that conforms to the designer's dialog for close
	/// requesting.</summary>
	public class RequestCloseAnchorable : TimersLayoutAnchorable,
		IRequestCloseAnchorable
	{
		/// <summary>True if the anchorable has already been force-closed.</summary>
		private bool forceClosed;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the anchorable.</summary>
		public RequestCloseAnchorable() {
			forceClosed = false;
			Hiding += OnAnchorableHiding;
			Closing += OnAnchorableClosing;
			Closed += OnAnchorableClosed;
			DesignerControl.AddOpenAnchorable(this);
		}


		//-----------------------------------------------------------------------------
		// Public Methods
		//-----------------------------------------------------------------------------

		/// <summary>Forces the document closed.</summary>
		public void ForceClose() {
			if (!forceClosed) {
				forceClosed = true;
				Closing -= OnAnchorableClosing;
				Close();
			}
		}

		/// <summary>Focuses on the anchorable's content.</summary>
		public virtual void Focus() {
			if (Content is UIElement)
				((UIElement) Content).Focus();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called when the anchorable is hiding.</summary>
		private void OnAnchorableHiding(object sender, CancelEventArgs e) {
			// HACK: We don't want the X button to hide anchorables when not in the document viewer
			e.Cancel = true;
			Close();
		}

		/// <summary>Called when the anchorable is closing.</summary>
		private void OnAnchorableClosing(object sender, CancelEventArgs e) {
			e.Cancel = true;
			DesignerControl.AddClosingAnchorable(this);
		}

		/// <summary>Called after the anchorable is closed.</summary>
		private void OnAnchorableClosed(object sender, EventArgs e) {
			DesignerControl.RemoveOpenAnchorable(this);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates a border to surround the anchorable.</summary>
		public static Border CreateBorder() {
			Border border = new Border();
			border.BorderThickness = new Thickness(1);
			border.BorderBrush = new SolidColorBrush(Color.FromRgb(41, 57, 86));
			return border;
		}
	}
}
