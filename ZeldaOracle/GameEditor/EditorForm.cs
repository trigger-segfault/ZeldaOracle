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
			this.comboBoxWorldLayer.Items.Add("Events");
			this.comboBoxWorldLayer.SelectedIndex = 0;

			// Create tools.
			this.toolButtons	= new ToolStripButton[] {
				buttonToolPointer,
				buttonToolPlace,
				buttonToolSelection,
				buttonToolEyedropper
			};
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		// Prompt the user to save unsaved changes if there are any. Returns
		// the result of the prompt dialogue (yes/no/cancel), or 'yes' if
		// there were no unsaved changes.
		private DialogResult PromptSaveChanges() {
			if (!editorControl.IsWorldOpen || !editorControl.HasMadeChanges)
				return DialogResult.Yes;

			// Show the dialogue.
			string worldName = editorControl.WorldFileName;
			DialogResult result = MessageBox.Show("Do you want to save changes to " + worldName + "?", "Unsaved Changes",
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
			
			if (result == DialogResult.Yes)
				SaveWorld();
			
			return result;
		}

		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		// Use this for shortcut keys that won't work on their own.
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if (keyData == (Keys.PageUp)) {
				cycleLayerUpToolStripMenuItem_Click(null, null);
				return true;
			}
			if (keyData == Keys.PageDown) {
				cycleLayerUpToolStripMenuItem1_Click(null, null);
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		// Attempt to save the world automatically first, or open a dialogue
		// if the world isn't from a file.
		private void SaveWorld() {
			if (editorControl.IsWorldFromFile)
				editorControl.SaveFileAs(editorControl.WorldFilePath); // Save to file.
			else
				ShowSaveWorldDialog(); // Open Save as dialogue
		}

		// Open a save file dialogue to save the world.
		private void ShowSaveWorldDialog() {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Zelda world files (*.zwd)|*.zwd";
			saveFileDialog.ValidateNames = true;

			if (saveFileDialog.ShowDialog() == DialogResult.OK) {
				Console.WriteLine("Saving file as " + saveFileDialog.FileName + ".");
				editorControl.SaveFileAs(saveFileDialog.FileName);
			}
		}

		// Open an open file dialogue to open a world file.
		private void ShowOpenWorldDialog() {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.DereferenceLinks = true;
			openFileDialog.Filter = "Zelda world files (*.zwd)|*.zwd";

			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				Console.WriteLine("Opened file " + openFileDialog.FileName + ".");
				editorControl.OpenFile(openFileDialog.FileName);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		// Make sure to check for unsaved changes when closing.
		protected override void OnFormClosing(FormClosingEventArgs e) {
			if (PromptSaveChanges() == DialogResult.Cancel)
				e.Cancel = true;
			else
				base.OnFormClosing(e);
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

		// Play animations
		private void buttonAnimations_Click(object sender, EventArgs e) {
			editorControl.PlayAnimations = (sender as ToolStripButton).Checked;
		}

		// Select tileset
		private void comboBoxTilesets_SelectedIndexChanged(object sender, EventArgs e) {
			if ((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex] != "")
				editorControl.ChangeTileset((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex]);
			levelDisplay.Focus();
		}

		// Select zone
		private void comboBoxZone_SelectedIndexChanged(object sender, EventArgs e) {
			if ((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex] != "")
				editorControl.ChangeZone((string)(sender as ToolStripComboBox).Items[(sender as ToolStripComboBox).SelectedIndex]);
			levelDisplay.Focus();
		}

		// Select layer
		private void comboBoxWorldLayer_SelectedIndexChanged(object sender, EventArgs e) {
			if (comboBoxWorldLayer.SelectedIndex == comboBoxWorldLayer.Items.Count - 1) {
				editorControl.EventMode = true;
			}
			else {
				// TODO: Change tileset to event tileset.
				editorControl.EventMode = false;
				editorControl.CurrentLayer = comboBoxWorldLayer.SelectedIndex;
				if (editorControl.CurrentTool != null)
					editorControl.CurrentTool.OnChangeLayer();
			}
			if (editorControl.PropertyGridControl != null)
				editorControl.PropertyGridControl.CloseProperties();
			levelDisplay.Focus();
		}

		// Tool buttons
		private void buttonTool_Click(object sender, EventArgs e) {
			for (int i = 0; i < toolButtons.Length; i++) {
				if (toolButtons[i] == sender)
					editorControl.ChangeTool(i);
			}
		}

		// Hide below layers
		private void hideBelowToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.BelowTileDrawMode = TileDrawModes.Hide;
			hideBelowToolStripMenuItem.Checked = true;
			fadeBelowToolStripMenuItem.Checked = false;
			showBelowToolStripMenuItem.Checked = false;
		}

		// Fade below layers
		private void fadeBelowToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.BelowTileDrawMode = TileDrawModes.Fade;
			hideBelowToolStripMenuItem.Checked = false;
			fadeBelowToolStripMenuItem.Checked = true;
			showBelowToolStripMenuItem.Checked = false;
		}
		
		// Show below layers
		private void showBelowToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.BelowTileDrawMode = TileDrawModes.Show;
			hideBelowToolStripMenuItem.Checked = false;
			fadeBelowToolStripMenuItem.Checked = false;
			showBelowToolStripMenuItem.Checked = true;
		}
		
		// Hide above layers
		private void hideAboveToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.AboveTileDrawMode = TileDrawModes.Hide;
			hideAboveToolStripMenuItem.Checked = true;
			fadeAboveToolStripMenuItem.Checked = false;
			showAboveToolStripMenuItem.Checked = false;
		}
		
		// Fade above layers
		private void fadeAboveToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.AboveTileDrawMode = TileDrawModes.Fade;
			hideAboveToolStripMenuItem.Checked = false;
			fadeAboveToolStripMenuItem.Checked = true;
			showAboveToolStripMenuItem.Checked = false;
		}

		// Show above layers
		private void showAboveToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.AboveTileDrawMode = TileDrawModes.Show;
			hideAboveToolStripMenuItem.Checked = false;
			fadeAboveToolStripMenuItem.Checked = false;
			showAboveToolStripMenuItem.Checked = true;
		}

		// Show Rewards
		private void showRewardsToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.ShowRewards = showRewardsToolStripMenuItem.Checked;
		}

		// Show Room Borders
		private void showRoomBordersToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.RoomSpacing = showRoomBordersToolStripMenuItem.Checked ? 1 : 0;
		}

		// Show Events
		private void showEventsToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.ShowEvents = showEventsToolStripMenuItem.Checked;
		}

		// Show grid
		private void buttonWorldGrid_Click(object sender, EventArgs e) {
			editorControl.ShowGrid = buttonWorldGrid.Checked;
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

		public PropertyGrid PropertyGridEvents {
			get { return propertyGrid1; }
		}

		public ToolStripButton ButtonTestPlayerPlace {
			get { return buttonTestPlayerPlace; }
		}


		//-----------------------------------------------------------------------------
		// File Menu Buttons
		//-----------------------------------------------------------------------------

		// New World...
		private void newWorldToolStripMenuItem_Click(object sender, EventArgs e) {
			if (PromptSaveChanges() != DialogResult.Cancel) {
				// TODO: New World
			}
		}

		//-----------------------------------------------------------------------------

		// Open World...
		private void openWorldToolStripMenuItem_Click(object sender, EventArgs e) {
			if (PromptSaveChanges() != DialogResult.Cancel)
				ShowOpenWorldDialog();
		}

		//-----------------------------------------------------------------------------

		// Close World
		private void closeWorldToolStripMenuItem_Click(object sender, EventArgs e) {
			if (PromptSaveChanges() != DialogResult.Cancel)
				editorControl.CloseFile();
		}

		//-----------------------------------------------------------------------------

		// Save World...
		private void saveWorldToolStripMenuItem_Click(object sender, EventArgs e) {
			SaveWorld();
		}

		// Save World As...
		private void saveWorldAsToolStripMenuItem_Click(object sender, EventArgs e) {
			ShowSaveWorldDialog();
		}
		
		//-----------------------------------------------------------------------------

		// Exit
		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Close();
		}


		//-----------------------------------------------------------------------------
		// Edit Menu Buttons
		//-----------------------------------------------------------------------------

		// Undo
		private void undoToolStripMenuItem_Click(object sender, EventArgs e) {

		}

		// Redo
		private void redoToolStripMenuItem_Click(object sender, EventArgs e) {

		}
		
		//-----------------------------------------------------------------------------

		// Cut
		private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.CurrentTool.Cut();
		}

		// Copy
		private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.CurrentTool.Copy();
		}

		// Paste
		private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.CurrentTool.Paste();
		}

		// Delete
		private void deleteToolStripMenuItem1_Click(object sender, EventArgs e) {
			editorControl.CurrentTool.Delete();
		}
		
		//-----------------------------------------------------------------------------

		// Select All
		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.CurrentTool.SelectAll();
		}

		// Deselect
		private void deselectToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.CurrentTool.Deselect();
		}

		
		//-----------------------------------------------------------------------------
		// World Menu Buttons
		//-----------------------------------------------------------------------------


		// Test Level
		private void testLevelToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.TestWorld();
		}

		// Test Level At Position
		private void testLevelAtPositionToolStripMenuItem_Click(object sender, EventArgs e) {
			//editorControl.PlayerPlaceMode = buttonTestPlayerPlace.Checked;
			editorControl.PlayerPlaceMode = true;
		}

		// Add Level...
		private void addLevelToolStripMenuItem_Click(object sender, EventArgs e) {
			using (LevelAddForm form = new LevelAddForm()) {
				if (form.ShowDialog(this) == DialogResult.OK) {
					Level level = new Level(form.LevelName, form.LevelWidth, form.LevelHeight,
						form.LevelLayerCount, form.LevelRoomSize, form.LevelZone);
					editorControl.AddLevel(level, true);
				}
			}
		}

		// Cycle Layer Up
		private void cycleLayerUpToolStripMenuItem_Click(object sender, EventArgs e) {
			comboBoxWorldLayer.SelectedIndex = 
				(comboBoxWorldLayer.SelectedIndex + 1) % comboBoxWorldLayer.Items.Count;
		}
		
		// Cycle Layer Down
		private void cycleLayerUpToolStripMenuItem1_Click(object sender, EventArgs e) {
			comboBoxWorldLayer.SelectedIndex = 
				(comboBoxWorldLayer.SelectedIndex + comboBoxWorldLayer.Items.Count - 1) % comboBoxWorldLayer.Items.Count;
		}

		//-----------------------------------------------------------------------------


	}

}
