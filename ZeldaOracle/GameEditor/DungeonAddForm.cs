using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor {
	public partial class DungeonAddForm : Form {

		private World world;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public DungeonAddForm(World world) {
			InitializeComponent();

			this.world = world;
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------
		
		public Dungeon CreateDungeon() {
			return new Dungeon(textBoxID.Text, textBoxName.Text);
		}

		
		//-----------------------------------------------------------------------------
		// Form Events
		//-----------------------------------------------------------------------------

		private void buttonAdd_Click(object sender, EventArgs e) {
			// Verify the ID is unique.
			if (world.GetDungoen(textBoxID.Text) != null) {
				MessageBox.Show("A dungeon with the ID '" + textBoxID.Text + "' already exists!",
					Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else {
				DialogResult = DialogResult.OK;
				Close();
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public string DungeonID {
			get { return textBoxID.Text; }
		}
		
		public string DungeonName {
			get { return textBoxName.Text; }
		}
	}
}
