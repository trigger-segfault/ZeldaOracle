using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConscriptDesigner.Control;
using Xceed.Wpf.AvalonDock.Layout;

namespace ConscriptDesigner.Anchorables {
	public class RequestCloseAnchorable : LayoutAnchorable {

		private bool forceClosed;

		public RequestCloseAnchorable() {
			Closing += OnAnchorableClosing;
			Closed += OnAnchorableClosed;
			forceClosed = false;
			DesignerControl.AddOpenAnchorable(this);
		}

		
		private void OnAnchorableClosed(object sender, EventArgs e) {
			DesignerControl.RemoveOpenAnchorable(this);
		}

		public void ForceClose() {
			if (!forceClosed) {
				forceClosed = true;
				Closing -= OnAnchorableClosing;
				Close();
			}
		}


		private void OnAnchorableClosing(object sender, CancelEventArgs e) {
			e.Cancel = true;
			DesignerControl.AddClosingAnchorable(this);
		}
	}
}
