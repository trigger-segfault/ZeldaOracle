using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ConscriptDesigner.Control;
using Xceed.Wpf.AvalonDock.Layout;

namespace ConscriptDesigner.Anchorables {
	/// <summary>A layout document that conforms to the designer's dialog for close requesting.</summary>
	public class RequestCloseDocument : LayoutDocument, IRequestCloseAnchorable {
		
		/// <summary>True if the anchorable has already been force-closed.</summary>
		private bool forceClosed;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the document.</summary>
		public RequestCloseDocument() {
			this.forceClosed = false;
			Closing += OnDocumentClosing;
			Closed += OnDocumentClosed;
			DesignerControl.AddOpenAnchorable(this);
		}


		//-----------------------------------------------------------------------------
		// Closing
		//-----------------------------------------------------------------------------

		/// <summary>Forces the document closed.</summary>
		public void ForceClose() {
			if (!forceClosed) {
				forceClosed = true;
				Closing -= OnDocumentClosing;
				Close();
			}
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called when the document is closing.</summary>
		private void OnDocumentClosing(object sender, CancelEventArgs e) {
			e.Cancel = true;
			DesignerControl.AddClosingAnchorable(this);
		}

		/// <summary>Called after the document is closed.</summary>
		private void OnDocumentClosed(object sender, EventArgs e) {
			DesignerControl.RemoveOpenAnchorable(this);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Creates a border to surround the anchorable.</summary>
		public static Border CreateBorder() {
			return RequestCloseAnchorable.CreateBorder();
		}
	}
}
