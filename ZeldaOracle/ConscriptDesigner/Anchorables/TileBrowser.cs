﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using ConscriptDesigner.Control;

namespace ConscriptDesigner.Anchorables {
	public class TileBrowser : RequestCloseAnchorable {

		private TileBrowserControl browser;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the tile browser.</summary>
		public TileBrowser() {
			Border border = CreateBorder();
			this.browser = new TileBrowserControl();
			border.Child = this.browser;

			Closed += OnAnchorableClosed;

			Title = "Tile Browser";
			Content = border;
		}


		//-----------------------------------------------------------------------------
		// Control
		//-----------------------------------------------------------------------------

		public void Reload() {
			browser.Reload();
		}

		public void Unload() {
			browser.Unload();
		}


		//-----------------------------------------------------------------------------
		// XML Serialization
		//-----------------------------------------------------------------------------

		public override void ReadXml(XmlReader reader) {
			base.ReadXml(reader);
			DesignerControl.MainWindow.TileBrowser = this;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called to force cleanup during close.</summary>
		private void OnAnchorableClosed(object sender, EventArgs e) {
			browser.Dispose();
		}
	}
}
