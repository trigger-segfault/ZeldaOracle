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
	public partial class LevelAddForm : Form {

		private World world;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public LevelAddForm(World world) {
			InitializeComponent();

			this.world = world;

			comboBoxRoomSize.SelectedIndex = 0;

			comboBoxZone.Items.Clear();

			foreach (KeyValuePair<string, Zone> entry in Resources.GetDictionary<Zone>()) {
				comboBoxZone.Items.Add(entry.Key);
			}
			comboBoxZone.SelectedIndex = 0;
		}


		//-----------------------------------------------------------------------------
		// Level Methods
		//-----------------------------------------------------------------------------

		public Level CreateLevel() {
			return null;
		}


		//-----------------------------------------------------------------------------
		// Form Events
		//-----------------------------------------------------------------------------

		private void buttonAdd_Click(object sender, EventArgs e) {
			// Verify the ID is unique.
			if (world.GetLevel(textBoxLevelName.Text) != null) {
				MessageBox.Show("A level with the ID '" + textBoxLevelName.Text + "' already exists!",
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

		public string LevelName {
			get { return textBoxLevelName.Text; }
		}
		
		public int LevelWidth {
			get { return (int) numericLevelWidth.Value; }
		}
		
		public int LevelHeight {
			get { return (int) numericLevelHeight.Value; }
		}
		
		public Point2I LevelRoomSize {
			get {
				if (comboBoxRoomSize.SelectedIndex == 1)
					return GameSettings.ROOM_SIZE_LARGE;
				return GameSettings.ROOM_SIZE_SMALL;
			}
		}
		
		public Zone LevelZone {
			get { return Resources.Get<Zone>(comboBoxZone.Text); }
		}
	}
}
