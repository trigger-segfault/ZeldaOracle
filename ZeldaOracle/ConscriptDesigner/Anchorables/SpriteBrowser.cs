﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ConscriptDesigner.Anchorables {
	public class SpriteBrowser : RequestCloseAnchorable {
		
		private SpriteBrowserControl browser;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the sprite browser.</summary>
		public SpriteBrowser() {
			Border border = CreateBorder();
			this.browser = new SpriteBrowserControl();
			border.Child = this.browser;

			Closed += OnAnchorableClosed;

			Title = "Sprite Browser";
			Content = border;
		}


		//-----------------------------------------------------------------------------
		// Control
		//-----------------------------------------------------------------------------
		
		public void RefreshList() {
			browser.RefreshList();
		}

		public void ClearList() {
			browser.ClearList();
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
