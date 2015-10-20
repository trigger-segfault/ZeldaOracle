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
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.PropertiesEditor;
using ZeldaOracle.Common.Geometry;

using FormsControl = System.Windows.Forms.Control;

namespace ZeldaEditor {

	public partial class EditorForm : Form {
		
		private LevelDisplay		levelDisplay;
		private TileDisplay			tileDisplay;
		private EditorControl		editorControl;

		private ToolStripButton[]	toolButtons;

		private delegate void HotKeyAction();

		private Dictionary<Keys, HotKeyAction> hotKeyCommands;

		FormsControl activeControl;

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

			activeControl = null;

			treeViewWorld.NodeMouseDoubleClick += delegate(object sender, TreeNodeMouseClickEventArgs e) {
				if (e.Node.Name == "level") {
					editorControl.OpenLevel(e.Node.Index);
				}
				else if (e.Node.Name == "world") {
					editorControl.OpenObjectProperties(editorControl.World);
				}
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
				buttonToolSquare,
				buttonToolFill,
				buttonToolSelection,
				buttonToolEyedropper
			};

			this.hotKeyCommands = new Dictionary<Keys, HotKeyAction>();
			this.hotKeyCommands.Add(Keys.PageUp, delegate() { cycleLayerUpToolStripMenuItem_Click(null, null); });
			this.hotKeyCommands.Add(Keys.PageDown, delegate() { cycleLayerUpToolStripMenuItem1_Click(null, null); });
			this.hotKeyCommands.Add(Keys.M, delegate() { buttonTool_Click(this.buttonToolPointer, null); });
			this.hotKeyCommands.Add(Keys.P, delegate() { buttonTool_Click(this.buttonToolPlace, null); });
			this.hotKeyCommands.Add(Keys.O, delegate() { buttonTool_Click(this.buttonToolSquare, null); });
			this.hotKeyCommands.Add(Keys.F, delegate() { buttonTool_Click(this.buttonToolFill, null); });
			this.hotKeyCommands.Add(Keys.S, delegate() { buttonTool_Click(this.buttonToolSelection, null); });
			this.hotKeyCommands.Add(Keys.K, delegate() { buttonTool_Click(this.buttonToolEyedropper, null); });
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

		private void EditorForm_Load(object sender, EventArgs e) {
			foreach (FormsControl c in (sender as FormsControl).Controls) {
				c.GotFocus += OnControlFocus;
				EditorForm_Load(c, null);
			}
		}

		private void OnControlFocus(object sender, EventArgs e) {
			this.activeControl = sender as FormsControl;
		}

		// Use this for shortcut keys that won't work on their own.
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			bool typing = this.activeControl is TextBoxBase;
			if (!typing && hotKeyCommands.ContainsKey(keyData)) {
				hotKeyCommands[keyData]();
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
			get { return treeViewWorld; }
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

		public ToolStripButton ButtonTestLevelPlace {
			get { return buttonTestLevelPlace; }
		}

		public ContextMenuStrip ContenxtMenuGeneral {
			get { return contextMenuGeneral; }
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
			if (!editorControl.IsWorldOpen)
				MessageBox.Show(this, "No world is open.", "Test World");
			else if (editorControl.World.LevelCount == 0)
				MessageBox.Show(this, "Cannot test a world with no levels.", "Test World");
			else
				editorControl.TestWorld();
		}

		// Test Level At Position
		private void testLevelAtPositionToolStripMenuItem_Click(object sender, EventArgs e) {
			editorControl.PlayerPlaceMode = buttonTestLevelPlace.Checked;
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

		private void renameToolStripMenuItem_Click(object sender, EventArgs e) {
			if (treeViewWorld.SelectedNode.Name == "level") {
				int levelIndex = treeViewWorld.SelectedNode.Index;
				if (RenameForm.Show(this, editorControl.World.GetLevelAt(levelIndex).Properties.GetString("id")) == DialogResult.OK) {
					editorControl.World.GetLevelAt(levelIndex).Properties.Set("id", RenameForm.NewName);
					editorControl.RefreshWorldTreeView();
				}
			}
		}

		private void duplicateToolStripMenuItem_Click(object sender, EventArgs e) {

		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {

		}

		private void resizeToolStripMenuItem_Click(object sender, EventArgs e) {
			if (LevelResizeShiftForm.ShowResize(this, editorControl.Level.Dimensions) == DialogResult.OK) {
				editorControl.Level.Resize(LevelResizeShiftForm.LevelSize);
			}
		}

		private void shiftToolStripMenuItem_Click(object sender, EventArgs e) {
			if (LevelResizeShiftForm.ShowShift(this, editorControl.Level.Dimensions) == DialogResult.OK) {
				editorControl.Level.Shift(LevelResizeShiftForm.LevelShift);
			}
		}

		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			CustomPropertyDescriptor propertyDescriptor = e.ChangedItem.PropertyDescriptor as CustomPropertyDescriptor;
			if (propertyDescriptor.Property.Action != null)
				propertyDescriptor.Property.Action(editorControl.PropertyGridControl.EditedObject, e.ChangedItem.Value);
			if (propertyDescriptor.Property.Name == "id") {
				IPropertyObject propObject = propertyDescriptor.Property.Properties.PropertyObject;
				if (propObject is World || propObject is Level) {
					editorControl.RefreshWorldTreeView();
				}
			}
		}

		private void renameToolStripMenuItem1_Click(object sender, EventArgs e) {
			if (treeViewWorld.SelectedNode.Name == "world") {
				if (RenameForm.Show(this, editorControl.World.Properties.GetString("id")) == DialogResult.OK) {
					editorControl.World.Properties.Set("id", RenameForm.NewName);
					editorControl.RefreshWorldTreeView();
				}
			}
		}

		private void editPropertiesToolStripMenuItem_Click(object sender, EventArgs e) {
			if (treeViewWorld.SelectedNode.Name == "world") {
				editorControl.OpenObjectProperties(editorControl.World);
			}
		}

		// Make sure the right clicked node doesn't change back after selecting an item in the content menu.
		private void treeViewLevels_MouseClick(object sender, MouseEventArgs e) {
			// Only check with right click so pressing the pluses and minuses don't change the selection.
			if (e.Button == MouseButtons.Right)
				treeViewWorld.SelectedNode = treeViewWorld.GetNodeAt(e.X, e.Y);
		}

		private void treeViewLevels_AfterSelect(object sender, TreeViewEventArgs e) {
			if (e.Node.Name == "level" || e.Node.Name == "area" || e.Node.Name == "dungeon" || e.Node.Name == "script") {
				int count = 0;
				if (e.Node.Name == "level")
					count = editorControl.World.LevelCount;
				buttonTreeViewEdit.Enabled = true;
				buttonTreeViewRename.Enabled = true;
				buttonTreeViewDuplicate.Enabled = true;
				buttonTreeViewDelete.Enabled = true;
				buttonTreeViewMoveUp.Enabled = (e.Node.Index > 0);
				buttonTreeViewMoveDown.Enabled = (e.Node.Index + 1 < e.Node.Parent.Nodes.Count);
				buttonTreeViewResize.Enabled = (e.Node.Name == "level");
				buttonTreeViewShift.Enabled = (e.Node.Name == "level");
			}
			else {
				buttonTreeViewRename.Enabled = (e.Node.Name == "world");
				buttonTreeViewEdit.Enabled = (e.Node.Name == "world");
				buttonTreeViewDuplicate.Enabled = false;
				buttonTreeViewDelete.Enabled = false;
				buttonTreeViewMoveUp.Enabled = false;
				buttonTreeViewMoveDown.Enabled = false;
				buttonTreeViewResize.Enabled = false;
				buttonTreeViewShift.Enabled = false;
			}
		}

		private void buttonMoveUp_Click(object sender, EventArgs e) {
			TreeNode node = treeViewWorld.SelectedNode;
			if (node.Name == "level") {
				editorControl.World.MoveLevel(node.Index, -1, true);
				editorControl.RefreshWorldTreeView();
				treeViewWorld.SelectedNode = treeViewWorld.Nodes[0].Nodes[0].Nodes[node.Index - 1];
			}
		}

		private void buttonTreeViewMoveDown_Click(object sender, EventArgs e) {
			TreeNode node = treeViewWorld.SelectedNode;
			if (node.Name == "level") {
				editorControl.World.MoveLevel(node.Index, 1, true);
				editorControl.RefreshWorldTreeView();
				treeViewWorld.SelectedNode = treeViewWorld.Nodes[0].Nodes[0].Nodes[node.Index + 1];
			}
		}

		private void buttonTreeViewNewLevel_Click(object sender, EventArgs e) {
			using (LevelAddForm form = new LevelAddForm()) {
				if (form.ShowDialog(this) == DialogResult.OK) {
					Level level = new Level(form.LevelName, form.LevelWidth, form.LevelHeight,
						form.LevelLayerCount, form.LevelRoomSize, form.LevelZone);
					editorControl.AddLevel(level, true);
					treeViewWorld.SelectedNode = treeViewWorld.Nodes[0].Nodes[0].Nodes[treeViewWorld.Nodes[0].Nodes[0].Nodes.Count - 1];
				}
			}
		}

		private void buttonTreeViewDelete_Click(object sender, EventArgs e) {
			if (treeViewWorld.SelectedNode.Name == "level") {
				if (MessageBox.Show(this, "Would you like to delete this level?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes) {
					int index = treeViewWorld.SelectedNode.Index;
					editorControl.World.RemoveLevelAt(treeViewWorld.SelectedNode.Index);
					editorControl.RefreshWorldTreeView();
					if (editorControl.World.LevelCount == 0) {
						editorControl.CloseLevel();
						treeViewWorld.SelectedNode = treeViewWorld.Nodes[0].Nodes[0];
						treeViewLevels_AfterSelect(null, new TreeViewEventArgs(treeViewWorld.SelectedNode));
					}
					else {
						editorControl.OpenLevel(GMath.Max(0, index - 1));
						treeViewWorld.SelectedNode = treeViewWorld.Nodes[0].Nodes[0].Nodes[GMath.Max(0, index - 1)];
					}
				}
			}
		}

		private void buttonTreeViewRename_Click(object sender, EventArgs e) {
			if (treeViewWorld.SelectedNode.Name == "world") {
				if (RenameForm.Show(this, editorControl.World.Properties.GetString("id")) == DialogResult.OK) {
					editorControl.World.Properties.Set("id", RenameForm.NewName);
					editorControl.RefreshWorldTreeView();
				}
			}
			else if (treeViewWorld.SelectedNode.Name == "level") {
				int levelIndex = treeViewWorld.SelectedNode.Index;
				if (RenameForm.Show(this, editorControl.World.GetLevelAt(levelIndex).Properties.GetString("id")) == DialogResult.OK) {
					editorControl.World.GetLevelAt(levelIndex).Properties.Set("id", RenameForm.NewName);
					editorControl.RefreshWorldTreeView();
				}
			}
		}

		private void buttonTreeViewResize_Click(object sender, EventArgs e) {
			if (LevelResizeShiftForm.ShowResize(this, editorControl.Level.Dimensions) == DialogResult.OK) {
				editorControl.Level.Resize(LevelResizeShiftForm.LevelSize);
			}
		}

		private void buttonTreeViewShift_Click(object sender, EventArgs e) {
			if (LevelResizeShiftForm.ShowShift(this, editorControl.Level.Dimensions) == DialogResult.OK) {
				editorControl.Level.Shift(LevelResizeShiftForm.LevelShift);
			}
		}

		private void buttonTreeViewEdit_Click(object sender, EventArgs e) {
			if (treeViewWorld.SelectedNode.Name == "world") {
				editorControl.OpenObjectProperties(editorControl.World);
			}
			else if (treeViewWorld.SelectedNode.Name == "level") {
				editorControl.OpenLevel(treeViewWorld.SelectedNode.Index);
			}
		}

		//-----------------------------------------------------------------------------


	}

}
