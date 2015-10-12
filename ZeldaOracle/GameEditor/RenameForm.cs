using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Content;

namespace ZeldaEditor {
	public partial class RenameForm : Form {

		private static string newName;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RenameForm() {
			InitializeComponent();
			
			buttonOK.DialogResult		= DialogResult.OK;
			buttonCancel.DialogResult	= DialogResult.Cancel;
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void textBoxName_TextChanged(object sender, EventArgs e) {
			newName = textBoxName.Text;
		}

		private void textBoxName_KeyUp(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) {
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void RenameForm_Shown(object sender, EventArgs e) {
			textBoxName.Focus();
		}


		//-----------------------------------------------------------------------------
		// Show
		//-----------------------------------------------------------------------------

		public static DialogResult Show(IWin32Window owner, string name) {
			using (RenameForm form = new RenameForm()) {
				form.textBoxName.Text = name;
				RenameForm.newName = name;
				return form.ShowDialog(owner);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public static string NewName {
			get { return newName; }
		}
	}
}
