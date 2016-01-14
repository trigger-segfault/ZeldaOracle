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
using ZeldaEditor.ObjectsEditor;
using ZeldaEditor.PropertiesEditor;
using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaEditor.Scripting;
using ZeldaEditor.Tools;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;

using FormsControl = System.Windows.Forms.Control;

namespace ZeldaEditor {

	public partial class EditorForm : Form {
		
		private LevelDisplay		levelDisplay;
		private TileDisplay			tileDisplay;
		private EditorControl		editorControl;

		private ObjectEditor		objectEditorForm;

		private ToolStripButton[]	toolButtons;

		private delegate void HotKeyAction();

		private Dictionary<Keys, HotKeyAction> hotKeyCommands;

		private FormsControl activeControl;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public EditorForm() {
			InitializeComponent();

			// Create the editor control instance.
			editorControl = new EditorControl(this);

			// Setup the world tree view.
			worldTreeView.EditorControl = editorControl;

			// Create the level display.
			levelDisplay				= new LevelDisplay();
			levelDisplay.EditorControl	= editorControl;
			levelDisplay.Name			= "levelDisplay";
			levelDisplay.Dock			= DockStyle.Fill;
			levelDisplay.EditorForm		= this;
			panelWorld.Controls.Add(this.levelDisplay);
			
			// Create the tileset display.
			tileDisplay					= new TileDisplay();
			tileDisplay.EditorControl	= editorControl;
			tileDisplay.Name			= "tileDisplay";
			tileDisplay.Dock			= DockStyle.Fill;
			tileDisplay.EditorForm		= this;
			panelTiles2.Controls.Add(tileDisplay);

			activeControl = null;

			objectEditorForm = null;

			statusLabelTask.Text = null;

			this.comboBoxWorldLayer.Items.Add("Layer 1");
			this.comboBoxWorldLayer.Items.Add("Layer 2");
			this.comboBoxWorldLayer.Items.Add("Layer 3");
			this.comboBoxWorldLayer.Items.Add("Events");
			this.comboBoxWorldLayer.SelectedIndex = 0;

			// Create tools.
			this.toolButtons = new ToolStripButton[] {
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

		// Tile Properties...
		private void tilePropertiesToolStripMenuItem_Click(object sender, EventArgs e) {
			IPropertyObject obj = editorControl.PropertyGridControl.EditedObject;
			if (obj is IPropertyObject) {
				if (objectEditorForm == null || objectEditorForm.IsDisposed)
					objectEditorForm = new ObjectEditor(editorControl);
				objectEditorForm.SetObject((IPropertyObject) obj);
				objectEditorForm.Show(this);

				/*
				using (Form form = new ObjectEditor((TileDataInstance) obj)) {
					//if (form.ShowDialog(this) == DialogResult.OK) {
					form.Show();
					if (form.DialogResult == DialogResult.OK) {

					}
					else {

					}
				}*/
			}
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

		// Add New Level...
		private void addLevelToolStripMenuItem_Click(object sender, EventArgs e) {
			using (LevelAddForm form = new LevelAddForm(editorControl.World)) {
				if (form.ShowDialog(this) == DialogResult.OK) {
					Level level = new Level(form.LevelName, form.LevelWidth, form.LevelHeight,
						form.LevelLayerCount, form.LevelRoomSize, form.LevelZone);
					editorControl.AddLevel(level, true);
				}
			}
		}
		
		// Add New Dungeon...
		private void buttonAddDungeon_Click(object sender, EventArgs e) {
			using (DungeonAddForm form = new DungeonAddForm(editorControl.World)) {
				if (form.ShowDialog(this) == DialogResult.OK) {
					Dungeon dungeon = form.CreateDungeon();
					editorControl.World.AddDungeon(dungeon);
					editorControl.RefreshWorldTreeView();
				}
			}
		}
		
		// Create Script...
		private void buttonAddScript_Click(object sender, EventArgs e) {
			Script script = new Script();
			script.Name = "untitled";
			script.Code = "";
			worldTreeView.RefreshScripts();
			editorControl.AddScript(script);

			using (ScriptEditor form = new ScriptEditor(script, editorControl)) {
				if (form.ShowDialog(this) == DialogResult.OK) {
				}
				else {
					editorControl.World.RemoveScript(script);
				}
			
				worldTreeView.RefreshScripts();
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
		// Property Grid events
		//-----------------------------------------------------------------------------

		// Property value changed.
		private void propertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			editorControl.PropertyGridControl.OnPropertyChange(sender, e);
		}
		
		
		//-----------------------------------------------------------------------------
		// World Tree View button events
		//-----------------------------------------------------------------------------
		
		// Update which tool strip buttons are enabled based on which node type is selected.
		private void treeViewLevels_AfterSelect(object sender, TreeViewEventArgs e) {
			if (e.Node.Name == "level" || e.Node.Name == "area" || e.Node.Name == "dungeon" || e.Node.Name == "script") {
				buttonTreeViewEdit.Enabled		= true;
				buttonTreeViewRename.Enabled	= true;
				buttonTreeViewDuplicate.Enabled	= true;
				buttonTreeViewDelete.Enabled	= true;
				buttonTreeViewMoveUp.Enabled	= (e.Node.Index > 0);
				buttonTreeViewMoveDown.Enabled	= (e.Node.Index + 1 < e.Node.Parent.Nodes.Count);
				buttonTreeViewResize.Enabled	= (e.Node.Name == "level");
				buttonTreeViewShift.Enabled		= (e.Node.Name == "level");
			}
			else {
				buttonTreeViewRename.Enabled	= (e.Node.Name == "world");
				buttonTreeViewEdit.Enabled		= (e.Node.Name == "world");
				buttonTreeViewDuplicate.Enabled	= false;
				buttonTreeViewDelete.Enabled	= false;
				buttonTreeViewMoveUp.Enabled	= false;
				buttonTreeViewMoveDown.Enabled	= false;
				buttonTreeViewResize.Enabled	= false;
				buttonTreeViewShift.Enabled		= false;
			}
		}

		// Edit node.
		private void buttonTreeViewEdit_Click(object sender, EventArgs e) {
			worldTreeView.OpenNode(worldTreeView.SelectedNode);
		}

		// Rename node.
		private void buttonTreeViewRename_Click(object sender, EventArgs e) {
			worldTreeView.RenameNode();
		}

		// Duplicate node.
		private void buttonTreeViewDuplicate_Click(object sender, EventArgs e) {
			worldTreeView.DuplicateNode();
		}

		// Delete node.
		private void buttonTreeViewDelete_Click(object sender, EventArgs e) {
			worldTreeView.DeleteNode();
		}

		// Move node up.
		private void buttonMoveUp_Click(object sender, EventArgs e) {
			worldTreeView.MoveNodeUp();
		}
		
		// Move node down.
		private void buttonTreeViewMoveDown_Click(object sender, EventArgs e) {
			worldTreeView.MoveNodeDown();
		}

		// Resize level node.
		private void buttonTreeViewResize_Click(object sender, EventArgs e) {
			if (LevelResizeShiftForm.ShowResize(this, editorControl.Level.Dimensions) == DialogResult.OK) {
				editorControl.Level.Resize(LevelResizeShiftForm.LevelSize);
			}
		}

		// Shift level node.
		private void buttonTreeViewShift_Click(object sender, EventArgs e) {
			if (LevelResizeShiftForm.ShowShift(this, editorControl.Level.Dimensions) == DialogResult.OK) {
				editorControl.Level.Shift(LevelResizeShiftForm.LevelShift);
			}
		}

		//-----------------------------------------------------------------------------
		
		private void levelTabControl_Selected(object sender, TabControlEventArgs e) {
			/*if (levelTabControl.SelectedTab == levelTabPageTiles) {
				Console.WriteLine("Tiles tab selected!");
				levelDisplay.Parent = panelWorld;
			}
			else if (levelTabControl.SelectedTab == levelTabPageTriggers) {
				Console.WriteLine("Triggers tab selected!");
				levelDisplay.Parent = panelTriggersLevelDisplay;
			}*/
		}


		// Open room properties.
		private void roomPropertiesToolStripMenuItem_Click(object sender, EventArgs e) {
			Point2I selectedRoom = editorControl.SelectedRoom;
			if (selectedRoom.X >= 0 && selectedRoom.Y >= 0) {
				Room room = editorControl.Level.GetRoomAt(selectedRoom);

				if (objectEditorForm == null || objectEditorForm.IsDisposed)
					objectEditorForm = new ObjectEditor(editorControl);
				objectEditorForm.SetObject(room);
				objectEditorForm.Show(this);
			}
		}
		
		// Open room properties.
		private void roomPropertiesToolStripMenuItem1_Click(object sender, EventArgs e) {
			Room room = editorControl.LevelDisplay.SelectedRoom;

			if (room != null) {
				editorControl.OpenObjectProperties(room);
				if (objectEditorForm == null || objectEditorForm.IsDisposed)
					objectEditorForm = new ObjectEditor(editorControl);
				objectEditorForm.SetObject(room);
				objectEditorForm.Show(this);
			}
		}
		
		//-----------------------------------------------------------------------------
	}

}
