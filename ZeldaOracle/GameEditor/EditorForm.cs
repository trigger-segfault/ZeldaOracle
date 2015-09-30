using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;
using ZeldaEditor.Tools;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor {

	public partial class EditorForm : Form {
		
		private LevelDisplay		levelDisplay;
		private TileDisplay			tileDisplay;
		private EditorControl		editorControl;
		//private PropertiesContainer	propertiesContainer;

		private ToolStripButton[]	toolButtons;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EditorForm() {
			InitializeComponent();

			editorControl				= new EditorControl();
			editorControl.EditorForm	= this;

			// Create the level display.
			levelDisplay				= new LevelDisplay();
			levelDisplay.EditorControl	= editorControl;
			levelDisplay.Name			= "levelDisplay";
			levelDisplay.Dock			= DockStyle.Fill;
			levelDisplay.EditorForm		= this;
			panelWorld.Controls.Add(this.levelDisplay);

			tileDisplay					= new TileDisplay();
			tileDisplay.EditorControl	= editorControl;
			tileDisplay.Name			= "tileDisplay";
			tileDisplay.Dock			= DockStyle.Fill;
			tileDisplay.EditorForm		= this;
			panelTiles2.Controls.Add(tileDisplay);

			treeViewLevels.ExpandAll();
			treeViewLevels.NodeMouseDoubleClick += delegate(object sender, TreeNodeMouseClickEventArgs e) {
				editorControl.OpenLevel(e.Node.Index);
			};
			treeViewLevels.AfterLabelEdit += delegate(object sender, NodeLabelEditEventArgs e) {
				Console.WriteLine("Renamed level to " + e.Label);
				int levelIndex = e.Node.Index;
				Level level = editorControl.World.GetLevel(levelIndex);
				level.Name = e.Label;
			};

			this.comboBoxWorldLayer.Items.Add("Layer 1");
			this.comboBoxWorldLayer.Items.Add("Layer 2");
			this.comboBoxWorldLayer.Items.Add("Layer 3");
			this.comboBoxWorldLayer.SelectedIndex = 0;

			// Create tools.
			this.toolButtons	= new ToolStripButton[] {
				buttonToolPointer,
				buttonToolPlace,
				buttonToolSelection,
				buttonToolEyedropper
			};
		}


		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if (keyData == (Keys.Control | Keys.O)) {
				MessageBox.Show("Hotkey pressed!");
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		public void OnToolChange(int toolIndex) {
			for (int i = 0; i < toolButtons.Length; i++)
				toolButtons[i].Checked = (i == toolIndex);
		}


		//-----------------------------------------------------------------------------
		// Form Event Handlers
		//-----------------------------------------------------------------------------

		// Open a file.
		private void buttonLoad_Click(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.DereferenceLinks = true;
			openFileDialog.Filter = "Zelda world files (*.zwd)|*.zwd";

			openFileDialog.ShowDialog();

			if (openFileDialog.FileName != String.Empty) {
				Console.WriteLine("Opened file " + openFileDialog.FileName + ".");
				editorControl.OpenFile(openFileDialog.FileName);
			}
		}

		// Save the file.
		private void buttonSave_Click(object sender, EventArgs e) {
			buttonSaveAs_Click(sender, e);
		}
		
		// Save the file as.
		private void buttonSaveAs_Click(object sender, EventArgs e) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Zelda world files (*.zwd)|*.zwd";
			saveFileDialog.ValidateNames = true;

			saveFileDialog.ShowDialog();

			if (saveFileDialog.FileName != String.Empty) {
				Console.WriteLine("Saving file as " + saveFileDialog.FileName + ".");
				editorControl.SaveFileAs(saveFileDialog.FileName);
			}
		}

		// Add a new level to the world.
		private void buttonAddLevel_Click(object sender, EventArgs e) {
			using (LevelAddForm form = new LevelAddForm()) {
				if (form.ShowDialog(this) == DialogResult.OK) {
					Level level = new Level(form.LevelName, form.LevelWidth, form.LevelHeight,
						form.LevelLayerCount, form.LevelRoomSize, form.LevelZone);
					editorControl.AddLevel(level, true);
				}
			}
		}

		private void buttonAnimations_Click(object sender, EventArgs e) {
			editorControl.PlayAnimations = (sender as ToolStripButton).Checked;
		}

		private void comboBoxTilesets_SelectedIndexChanged(object sender, EventArgs e) {
			if ((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex] != "")
				editorControl.ChangeTileset((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex]);
			levelDisplay.Focus();
		}

		private void comboBoxZone_SelectedIndexChanged(object sender, EventArgs e) {
			if ((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex] != "")
				editorControl.ChangeZone((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex]);
			levelDisplay.Focus();
		}

		private void comboBoxWorldLayer_SelectedIndexChanged(object sender, EventArgs e) {
			editorControl.CurrentLayer = this.comboBoxWorldLayer.SelectedIndex;
			levelDisplay.Focus();
		}

		private void buttonTool_Click(object sender, EventArgs e) {
			for (int i = 0; i < toolButtons.Length; i++) {
				if (toolButtons[i] == sender)
					editorControl.ChangeTool(i);
			}
		}

		private void hideBelowToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.BelowTileDrawMode = TileDrawModes.Hide;
			hideBelowToolStripMenuItem.Checked = true;
			fadeBelowToolStripMenuItem.Checked = false;
			showBelowToolStripMenuItem.Checked = false;
		}

		private void fadeBelowToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.BelowTileDrawMode = TileDrawModes.Fade;
			hideBelowToolStripMenuItem.Checked = false;
			fadeBelowToolStripMenuItem.Checked = true;
			showBelowToolStripMenuItem.Checked = false;
		}

		private void showBelowToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.BelowTileDrawMode = TileDrawModes.Show;
			hideBelowToolStripMenuItem.Checked = false;
			fadeBelowToolStripMenuItem.Checked = false;
			showBelowToolStripMenuItem.Checked = true;
		}

		private void hideAboveToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.AboveTileDrawMode = TileDrawModes.Hide;
			hideAboveToolStripMenuItem.Checked = true;
			fadeAboveToolStripMenuItem.Checked = false;
			showAboveToolStripMenuItem.Checked = false;
		}

		private void fadeAboveToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.AboveTileDrawMode = TileDrawModes.Fade;
			hideAboveToolStripMenuItem.Checked = false;
			fadeAboveToolStripMenuItem.Checked = true;
			showAboveToolStripMenuItem.Checked = false;
		}

		private void showAboveToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.AboveTileDrawMode = TileDrawModes.Show;
			hideAboveToolStripMenuItem.Checked = false;
			fadeAboveToolStripMenuItem.Checked = false;
			showAboveToolStripMenuItem.Checked = true;
		}

		private void showRewardsToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.ShowRewards = showRewardsToolStripMenuItem.Checked;
		}

		private void buttonWorldGrid_Click(object sender, EventArgs e) {
			editorControl.ShowGrid = buttonWorldGrid.Checked;
		}

		private void buttonTest_Click(object sender, EventArgs e) {
			editorControl.TestWorld();
		}

		private void toolStripButton1_Click(object sender, EventArgs e) {
			editorControl.PlayerPlaceMode = buttonTestPlayerPlace.Checked;
		}

		private void showRoomBordersToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.RoomSpacing = showRoomBordersToolStripMenuItem.Checked ? 1 : 0;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TreeView LevelTreeView {
			get { return treeViewLevels; }
		}

		public ContextMenuStrip ContextMenuLevelSelect {
			get { return contextMenuLevelSelect; }
		}

		public bool PlayAnimations {
			get { return buttonAnimations.Checked; }
		}

		public ToolStripStatusLabel StatusBarLabelTileLoc {
			get { return statusBarLabelTileLoc; }
		}

		public ToolStripStatusLabel StatusBarLabelRoomLoc {
			get { return statusBarLabelRoomLoc; }
		}

		public LevelDisplay LevelDisplay {
			get { return levelDisplay; }
		}

		public TileDisplay TileDisplay {
			get { return tileDisplay; }
		}

		public ToolStripComboBox ComboBoxTilesets {
			get { return comboBoxTilesets; }
		}

		public ToolStripComboBox ComboBoxZones {
			get { return comboBoxZones; }
		}

		public PropertyGrid PropertyGrid {
			get { return propertyGrid; }
		}

		public Label PropertyGridTitle {
			get { return propertyGridTitle; }
		}

		public ToolStripButton ButtonTestPlayerPlace {
			get { return buttonTestPlayerPlace; }
		}

	}

}
