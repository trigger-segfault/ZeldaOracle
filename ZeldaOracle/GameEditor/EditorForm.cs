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

			string worldName = "untitled";
			if (editorControl.IsWorldFromFile)
				worldName = editorControl.WorldFileName;
			DialogResult result = MessageBox.Show("Do you want to save changes to " + worldName + "?", "Unsave Changes",
				MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
			
			if (result == DialogResult.Yes)
				SaveWorld();
			
			return result;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		// Attempt to save the world automatically first, or open a dialogue
		// if the world isn't from a file.
		private void SaveWorld() {
			if (editorControl.IsWorldFromFile)
				editorControl.SaveFileAs(editorControl.WorldFileName); // Save to file.
			else
				SaveWorldAs(); // Open Save as dialogue
		}

		// Open a save file dialogue to save the world.
		private void SaveWorldAs() {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Zelda world files (*.zwd)|*.zwd";
			saveFileDialog.ValidateNames = true;

			if (saveFileDialog.ShowDialog() == DialogResult.OK) {
				Console.WriteLine("Saving file as " + saveFileDialog.FileName + ".");
				editorControl.SaveFileAs(saveFileDialog.FileName);
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
			if (PromptSaveChanges() != DialogResult.Cancel) {
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.DereferenceLinks = true;
				openFileDialog.Filter = "Zelda world files (*.zwd)|*.zwd";

				if (openFileDialog.ShowDialog() == DialogResult.OK) {
					Console.WriteLine("Opened file " + openFileDialog.FileName + ".");
					editorControl.OpenFile(openFileDialog.FileName);
				}
			}
		}

		//-----------------------------------------------------------------------------

		// Save World...
		private void saveWorldToolStripMenuItem_Click(object sender, EventArgs e) {
			SaveWorld();
		}

		// Save World As...
		private void saveWorldAsToolStripMenuItem_Click(object sender, EventArgs e) {
			SaveWorldAs();
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

		}

		// Copy
		private void copyToolStripMenuItem_Click(object sender, EventArgs e) {

		}

		// Paste
		private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {

		}

		// Delete
		private void deleteToolStripMenuItem1_Click(object sender, EventArgs e) {

		}
		
		//-----------------------------------------------------------------------------

		// Select All
		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {

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

		//-----------------------------------------------------------------------------


	}

}
