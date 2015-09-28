using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZeldaEditor {
	public partial class LevelAddForm : Form {

		public LevelAddForm() {
			InitializeComponent();

			comboBoxRoomSize.SelectedIndex = 0;
			comboBoxZone.SelectedIndex = 0;
		}

		private void buttonAdd_Click(object sender, EventArgs e) {
			Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e) {
			Close();
		}
	}
}
