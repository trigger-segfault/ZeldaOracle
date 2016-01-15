using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;
using ZeldaEditor.Scripting;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.ObjectsEditor.CustomControls;
using ZeldaOracle.Game.Tiles.Custom;

namespace ZeldaEditor.ObjectsEditor {

	public partial class ObjectEditor : Form {

		private EditorControl editorControl;
		private TileDataInstance tile;
		private IPropertyObject propertyObject;
		private CustomObjectEditorControl customControl;
		private Dictionary<Type, CustomObjectEditorControl> customTileTypeControls;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public ObjectEditor(EditorControl editorControl) {
			InitializeComponent();

			rawPropertyGrid.Initialize(editorControl);

			// Setup the custom control types.
			customTileTypeControls = new Dictionary<Type, CustomObjectEditorControl>();
			customTileTypeControls[typeof(TileButton)]			= new ObjectControlsTileButton();
			customTileTypeControls[typeof(TileLantern)]			= new ObjectControlsTileLantern();
			customTileTypeControls[typeof(TileLever)]			= new ObjectControlsTileLever();
			customTileTypeControls[typeof(TileColorSwitch)]		= new ObjectControlsTileColorSwitch();
			customTileTypeControls[typeof(TileChest)]			= new ObjectControlsTileChest();
			customTileTypeControls[typeof(TileReward)]			= new ObjectControlsTileReward();
			customTileTypeControls[typeof(TileSign)]			= new ObjectControlsTileSign();
			customTileTypeControls[typeof(TileOwl)]				= new ObjectControlsTileSign();			// TileOwl == TileSign
			customTileTypeControls[typeof(TileColorCube)]		= new ObjectControlsTileColorCube();
			customTileTypeControls[typeof(TileColorLantern)]	= new ObjectControlsTileColorLantern();
			customTileTypeControls[typeof(TileColorJumpPad)]	= new ObjectControlsTileColorJumpPad();
			customTileTypeControls[typeof(TileColorTile)]		= new ObjectControlsTileColorJumpPad();	// TileColorTile == TileColorJumpPad
			customTileTypeControls[typeof(TileDoor)]			= new ObjectControlsTileDoor();

			this.editorControl	= editorControl;
			this.tile			= null;
			this.eventsTab.Initialize(this);

			// Solid Types.
			comboBoxSolidType.Items.AddRange(new ComboBoxItem<TileSolidType>[] {
				new ComboBoxItem<TileSolidType>(TileSolidType.NotSolid,		"Not Solid"),
				new ComboBoxItem<TileSolidType>(TileSolidType.Solid,		"Solid"),
				new ComboBoxItem<TileSolidType>(TileSolidType.HalfSolid,	"Half Solid"),
				new ComboBoxItem<TileSolidType>(TileSolidType.Ledge,		"Ledge"),
			});
			
			// Ledge Directions.
			comboBoxLedgeDirection.Items.AddRange(new ComboBoxItem<int>[] {
				new ComboBoxItem<int>(Directions.Down,	"Down"),
				new ComboBoxItem<int>(Directions.Up,	"Up"),
				new ComboBoxItem<int>(Directions.Left,	"Left"),
				new ComboBoxItem<int>(Directions.Right,	"Right"),
			});

			// Collision Models.
			foreach (KeyValuePair<string, CollisionModel> model in ZeldaOracle.Common.Content.Resources.GetResourceDictionary<CollisionModel>()) {
				comboBoxCollisionModel.Items.Add(new ComboBoxItem<CollisionModel>(model.Value, model.Key));
			}

			// Environment Types.
			comboBoxMovementType.Items.AddRange(new ComboBoxItem<TileEnvironmentType>[] {
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Normal,		"Normal"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Stairs,		"Stairs"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Ladder,		"Ladder"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Ice,			"Ice"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Puddle,		"Puddle"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Grass,		"Grass"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Hole,			"Hole"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Water,		"Water"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.DeepWater,	"Deep Water"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Ocean,		"Ocean"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Waterfall,	"Waterfall"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Whirlpool,	"Whirlpool"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Lava,			"Lava"),
				new ComboBoxItem<TileEnvironmentType>(TileEnvironmentType.Lavafall,		"Lavafall"),
			});

			// Pickupable Bracelet Level.
			comboBoxBraceletLevel.Items.AddRange(new string[] {
				"Level 1 (Power Bracelet)",
				"Level 2 (Power Gloves)",
			});

			// Cuttable Swordable Level.
			comboBoxSwordLevel.Items.AddRange(new string[] {
				"Level 1 (Wooden Sword)",
				"Level 2 (Noble Sword)",
				"Level 3 (Master Sword)",
			});

			// Move direction.
			comboBoxMoveDirection.Items.AddRange(new ComboBoxItem<int>[] {
				new ComboBoxItem<int>(-1,				"Any"),
				new ComboBoxItem<int>(Directions.Right,	"Right"),
				new ComboBoxItem<int>(Directions.Left,	"Left"),
				new ComboBoxItem<int>(Directions.Up,	"Up"),
				new ComboBoxItem<int>(Directions.Down,	"Down"),
			});
		}

		
		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void SetObject(IPropertyObject propertyObject) {
			this.propertyObject = propertyObject;
			
			// Determine the name of the base type of the property object.
			string baseTypeName = "Object";
			if (propertyObject is TileDataInstance)
				baseTypeName = "Tile";
			else if (propertyObject is EventTileDataInstance)
				baseTypeName = "Event Tile";
			else if (propertyObject is Room)
				baseTypeName = "Room";
			else if (propertyObject is Level)
				baseTypeName = "Level";
			else if (propertyObject is World)
				baseTypeName = "World";
			else if (propertyObject is Dungeon)
				baseTypeName = "Dungeon";
			else if (propertyObject is Script)
				baseTypeName = "Script";

			// Determine the type of the object.
			Type tileType = null;
			tile = null;
			if (propertyObject is TileDataInstance) {
				tile = (TileDataInstance) propertyObject;
				tileType = tile.Type;
				if (tileType == null)
					tileType = typeof(Tile);
			}
			else if (propertyObject is EventTileDataInstance) {
				tileType = ((EventTileDataInstance) propertyObject).Type;
				if (tileType == null)
					tileType = typeof(EventTile);
			}
			
			// Only show Tile Interactions tab page for tiles.
			if (propertyObject is TileDataInstance) {
				if (!tabControl1.TabPages.Contains(tabPageTileInteractions))
					tabControl1.TabPages.Add(tabPageTileInteractions);
			}
			else {
				if (tabControl1.TabPages.Contains(tabPageTileInteractions))
					tabControl1.TabPages.Remove(tabPageTileInteractions);
			}
			
			// Setup the raw properties grid.
			rawPropertyGrid.OpenProperties(propertyObject);

			// Set the form title.
			if (tileType != null) {
				Text = String.Format("{0} Properties [{1}]", baseTypeName, tileType.Name);
				groupBoxCustomObjectControls.Text = tileType.Name;
			}
			else {
				Text = String.Format("{0} Properties", baseTypeName);
				groupBoxCustomObjectControls.Text = baseTypeName;
			}

			// Remove the custom control for the previous tile type.
			if (customControl != null) {
				panelCustomObjectControl.Controls.Remove(customControl);
				//groupBoxCustomObjectControls.Controls.Remove(customControl);
				customControl = null;
			}
			// Add the custom control for the tile type.
			if (tileType != null && customTileTypeControls.ContainsKey(tileType)) {
				customControl = customTileTypeControls[tileType];
				customControl.Initialize(this);
				customControl.Dock = DockStyle.Fill;
				panelCustomObjectControl.Controls.Add(customControl);
				//groupBoxCustomObjectControls.Controls.Add(customControl);
			}
			
			// Setup events tab.
			eventsTab.SetupObject(propertyObject);
					
			// ID.
			textBoxId.Text = propertyObject.Properties.GetString("id", "");

			// Spawn type.
			checkBoxStartDisabled.Checked	= !propertyObject.Properties.GetBoolean("enabled", true);
			checkBoxPoofEffect.Checked		= propertyObject.Properties.GetBoolean("spawn_poof_effect", false);
			
			if (tile != null) {
				// Size.
				numberBoxWidth.Value = tile.Size.X;
				numberBoxHeight.Value = tile.Size.Y;

				// Solid type.
				TileSolidType solidType = (TileSolidType)
					tile.Properties.GetInteger("solidity", (int) TileSolidType.NotSolid);
				ComboBoxItem<TileSolidType>.SelectComboBoxItem(comboBoxSolidType, solidType);
				
				// Ledge Direction.
				int ledgeDir = tile.Properties.GetInteger("ledge_direction", Directions.Down);
				ComboBoxItem<int>.SelectComboBoxItem(comboBoxLedgeDirection, ledgeDir);

				// Collision model.
				ComboBoxItem<CollisionModel>.SelectComboBoxItem(comboBoxCollisionModel, tile.CollisionModel);

				// Environment type.
				TileEnvironmentType envType = (TileEnvironmentType)
					tile.Properties.GetInteger("environment_type", (int) TileEnvironmentType.Normal);
				ComboBoxItem<TileEnvironmentType>.SelectComboBoxItem(comboBoxMovementType, envType);
				
				//  Interactions.
				checkBoxCuttable.Checked			= tile.Flags.HasFlag(TileFlags.Cuttable);
				checkBoxPickupable.Checked			= tile.Flags.HasFlag(TileFlags.Pickupable);
				checkBoxMovable.Checked				= tile.Flags.HasFlag(TileFlags.Movable);
				checkBoxBurnable.Checked			= tile.Flags.HasFlag(TileFlags.Burnable);
				checkBoxBombable.Checked			= tile.Flags.HasFlag(TileFlags.Bombable);
				checkBoxDigable.Checked				= tile.Flags.HasFlag(TileFlags.Digable);
				checkBoxBoomerangable.Checked		= tile.Flags.HasFlag(TileFlags.Boomerangable);
				checkBoxSwitchable.Checked			= tile.Flags.HasFlag(TileFlags.Switchable);
				checkBoxBreakOnSwitch.Checked		= !tile.Flags.HasFlag(TileFlags.SwitchStays);
				checkBoxMoveOnce.Checked			= tile.Properties.GetBoolean("move_once", false);
				comboBoxSwordLevel.SelectedIndex	= tile.Properties.GetInteger("cuttable_sword_level", 0);
				comboBoxBraceletLevel.SelectedIndex	= tile.Properties.GetInteger("pickupable_bracelet_level", 0);
				checkBoxDisableOnDestroy.Checked	= tile.Properties.GetBoolean("disable_on_destroy", false);
				
				// Move direction.
				ComboBoxItem<int>.SelectComboBoxItem(comboBoxMoveDirection,
					tile.Properties.GetInteger("move_direction", -1));
			}

			// Setup custom controls.
			if (customControl != null)
				customControl.SetupObject(propertyObject);
		}

		public void ApplyChanges() {
			// Id.
			propertyObject.Properties.Exists("id");
			propertyObject.Properties.Set("id", textBoxId.Text);

			// Spawn Type.
			propertyObject.Properties.Set("enabled", !checkBoxStartDisabled.Checked);
			propertyObject.Properties.Set("spawn_poof_effect", checkBoxPoofEffect.Checked);
			
			if (tile != null) {
				// Size.
				tile.Size = new Point2I((int) numberBoxWidth.Value, (int) numberBoxHeight.Value);
			
				// Solid Type
				tile.Properties.Set("solidity",
					(int) ComboBoxItem<TileSolidType>.GetComboBoxValue(comboBoxSolidType));
				
				// Ledge direction
				tile.Properties.Set("ledge_direction",
					ComboBoxItem<int>.GetComboBoxValue(comboBoxLedgeDirection));

				// Collision Model.
				tile.CollisionModel = ComboBoxItem<CollisionModel>.GetComboBoxValue(comboBoxCollisionModel);

				// Environment Type.
				tile.Properties.Set("environment_type",
					(int) ComboBoxItem<TileEnvironmentType>.GetComboBoxValue(comboBoxMovementType));

				//  Interactions.
				tile.SetFlags(TileFlags.Cuttable,		checkBoxCuttable.Checked);
				tile.SetFlags(TileFlags.Pickupable,		checkBoxPickupable.Checked);
				tile.SetFlags(TileFlags.Movable,		checkBoxMovable.Checked);
				tile.SetFlags(TileFlags.Burnable,		checkBoxBurnable.Checked);
				tile.SetFlags(TileFlags.Bombable,		checkBoxBombable.Checked);
				tile.SetFlags(TileFlags.Digable,		checkBoxDigable.Checked);
				tile.SetFlags(TileFlags.Boomerangable,	checkBoxBoomerangable.Checked);
				tile.SetFlags(TileFlags.Switchable,		checkBoxSwitchable.Checked);
				tile.SetFlags(TileFlags.SwitchStays,	!checkBoxBreakOnSwitch.Checked);
				tile.Properties.Set("move_once", checkBoxMoveOnce.Checked);
				tile.Properties.Set("cuttable_sword_level", comboBoxSwordLevel.SelectedIndex);
				tile.Properties.Set("pickupable_bracelet_level", comboBoxBraceletLevel.SelectedIndex);
				tile.Properties.Set("disable_on_destroy", checkBoxDisableOnDestroy.Checked);

				// Move direction
				int moveDir = ((ComboBoxItem<int>) comboBoxMoveDirection.SelectedItem).Value;
				tile.Properties.Set("move_direction", moveDir);
			}

			// Apply events.
			eventsTab.ApplyChanges();
			
			// Apply custom controls.
			if (customControl != null)
				customControl.ApplyChanges(propertyObject);

			rawPropertyGrid.RefreshProperties();
		}
		
		
		//-----------------------------------------------------------------------------
		// Form Events
		//-----------------------------------------------------------------------------
		
		// Done.
		private void buttonDone_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
			ApplyChanges();
			Close();
		}

		// Apply.
		private void buttonApply_Click(object sender, EventArgs e) {
			ApplyChanges();
		}

		// Cancel.
		private void buttonCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
		
		
		//-----------------------------------------------------------------------------
		// Event Tab Events.
		//-----------------------------------------------------------------------------
		/*
		private void radioButtonEventNone_CheckedChanged(object sender, EventArgs e) {

		}

		private void radioButtonEventCustomCode_CheckedChanged(object sender, EventArgs e) {
			groupBoxCustomCode.Enabled = radioButtonEventCustomCode.Checked;
		}

		private void radioButtonEventScript_CheckedChanged(object sender, EventArgs e) {
			groupBoxScript.Enabled = radioButtonEventScript.Checked;
		}

		// Selected event changes.
		private void listBoxEvents_SelectedIndexChanged(object sender, EventArgs e) {
			if (listBoxEvents.SelectedIndex >= 0) {
				// An event was selected.
				Property property = eventProperties[listBoxEvents.SelectedIndex];
				PropertyDocumentation doc = property.GetRootDocumentation();

				panelEventProperties.Visible	= true;
				//labelEventName.Text				= "Event: " + listBoxEvents.Items[listBoxEvents.SelectedIndex].ToString();
				//labelEventDescription.Text		= "(description goes here)";

				labelEventName.Text				= "Event: " + (doc != null ? doc.ReadableName : property.Name);
				labelEventDescription.Text		= (doc != null ? doc.Description : "");

				string scriptName = property.StringValue;
				Script script = editorControl.World.GetScript(scriptName);
				
				comboBoxScript.Text = "";

				if (script == null) {
					// No action.
					radioButtonEventNone.Checked = true;
				}
				else if (script.IsHidden) {
					// Custom Code.
					radioButtonEventCustomCode.Checked = true;
				}
				else {
					// Script.
					radioButtonEventScript.Checked = true;
					comboBoxScript.Text = scriptName;
				}
			}
			else {
				// No event is selected.
				panelEventProperties.Visible = false;
			}
		}

		// Edit Code.
		private void buttonEditCode_Click(object sender, EventArgs e) {
			Property property = eventProperties[listBoxEvents.SelectedIndex];
			string scriptName = property.StringValue;
			Script script = editorControl.World.GetScript(scriptName);
			
			using (ScriptEditor form = new ScriptEditor(script, editorControl)) {
				if (form.ShowDialog(editorControl.EditorForm) == DialogResult.OK) {

				}
				else {

				}
			}
		}

		// Edit Script.
		private void buttonEditScript_Click(object sender, EventArgs e) {
			Property property = eventProperties[listBoxEvents.SelectedIndex];
			string scriptName = property.StringValue;
			Script script = editorControl.World.GetScript(scriptName);
			
			using (ScriptEditor form = new ScriptEditor(script, editorControl)) {
				if (form.ShowDialog(editorControl.EditorForm) == DialogResult.OK) {

				}
				else {

				}
			}
		}
		*/

		public EditorControl EditorControl {
			get { return editorControl; }
		}

		public IPropertyObject PropertyObject {
			get { return propertyObject; }
		}

		private void checkBoxCuttable_CheckedChanged(object sender, EventArgs e) {
			comboBoxSwordLevel.Enabled = checkBoxCuttable.Checked;
			labelSwordLevel.Enabled = checkBoxCuttable.Checked;
		}

		private void checkBoxPickupable_CheckedChanged(object sender, EventArgs e) {
			comboBoxBraceletLevel.Enabled = checkBoxPickupable.Checked;
			labelBraceletLevel.Enabled = checkBoxPickupable.Checked;
		}

		private void checkBoxMovable_CheckedChanged(object sender, EventArgs e) {
			checkBoxMoveOnce.Enabled = checkBoxMovable.Checked;
			comboBoxMoveDirection.Enabled = checkBoxMovable.Checked;
			labelMoveDirection.Enabled = checkBoxMovable.Checked;
		}

		private void checkBoxSwitchable_CheckedChanged(object sender, EventArgs e) {
			checkBoxBreakOnSwitch.Enabled = checkBoxSwitchable.Checked;
		}

		private void comboBoxSolidType_SelectedIndexChanged(object sender, EventArgs e) {
			bool isLedge = (((ComboBoxItem<TileSolidType>) comboBoxSolidType.SelectedItem).Value == TileSolidType.Ledge);
			comboBoxLedgeDirection.Enabled = isLedge;
			labelLedgeDirection.Enabled = isLedge;
		}

		private void rawPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			SetObject(propertyObject);
		}
	}
	/*
	public class TileEnvTypeItem {
		public TileEnvironmentType Type { get; set; }
		public string Name { get; set; }

		public TileEnvTypeItem(TileEnvironmentType type, string name) {
			this.Type = type;
			this.Name = name;
		}

		public override string ToString() {
			return Name;
		}
	}
	public class TileSolidTypeItem {
		public TileSolidType Type { get; set; }
		public string Name { get; set; }

		public TileSolidTypeItem(TileSolidType type, string name) {
			this.Type = type;
			this.Name = name;
		}

		public override string ToString() {
			return Name;
		}
	}
	public class CollisionModelItem {
		public CollisionModel Model { get; set; }
		public string Name { get; set; }

		public CollisionModelItem(CollisionModel model, string name) {
			this.Model = model;
			this.Name = name;
		}

		public override string ToString() {
			return Name;
		}
	}*/

	public class ComboBoxItem<T> {
		public T Value { get; set; }
		public string Name { get; set; }
		
		public ComboBoxItem(T value, string name) {
			this.Value = value;
			this.Name = name;
		}

		public override string ToString() {
			return Name;
		}
		
		public static T GetComboBoxValue(ComboBox comboBox) {
			return ((ComboBoxItem<T>) comboBox.SelectedItem).Value;
		}

		public static void SelectComboBoxItem(ComboBox comboBox, T value) {
			comboBox.SelectedIndex = 0;
			foreach (ComboBoxItem<T> item in comboBox.Items) {
				if (item.Value.Equals(value)) {
					comboBox.SelectedItem = item;
					break;
				}
			}
		}
	}
}
