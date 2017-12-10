using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.PropertiesEditor;
using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.Control {
	public class PropertyGridControl {

		private EditorControl		editorControl;
		private PropertyGrid		propertyGrid;
		private PropertiesContainer	propertiesContainer;
		private Dictionary<string, CustomPropertyEditor> typeEditors;
		private IPropertyObject		editedObject;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PropertyGridControl(EditorControl editorControl, PropertyGrid propertyGrid) {
			this.editorControl	= editorControl;
			this.propertyGrid	= propertyGrid;

			propertiesContainer = new PropertiesContainer(null);
			propertyGrid.SelectedObject = propertiesContainer;

			// Create property editor types.
			typeEditors = new Dictionary<string, CustomPropertyEditor>();
			typeEditors["sprite"]			= new ResourcePropertyEditor<SpriteOld>();
			typeEditors["animation"]		= new ResourcePropertyEditor<AnimationOld>();
			typeEditors["collision_model"]	= new ResourcePropertyEditor<CollisionModel>();
			typeEditors["song"]				= new ResourcePropertyEditor<Song>();
			typeEditors["sound"]			= new ResourcePropertyEditor<Sound>();
			typeEditors["zone"]				= new ResourcePropertyEditor<Zone>();
			typeEditors["reward"]			= new RewardPropertyEditor(editorControl.RewardManager);
			typeEditors["text_message"]		= new TextMessagePropertyEditor();
			typeEditors["script"]			= new ScriptPropertyEditor();
			typeEditors["sprite_index"]		= new SpriteIndexComboBox();
			typeEditors["direction"]		= new DirectionPropertyEditor();
			typeEditors["angle"]			= null;
			typeEditors["enum"]				= new EnumComboBox();
			typeEditors["enum_flags"]		= null;
			typeEditors["dungeon"]			= new DungeonPropertyEditor();
			typeEditors["level"]			= new LevelPropertyEditor();
			/*
			// Initialize property type editors.
			foreach (KeyValuePair<string, CustomPropertyEditor> entry in typeEditors) {
				if (entry.Value != null)
					entry.Value.Initialize(this);
			}*/
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public UITypeEditor GetUITypeEditor(string editorType) {
			if (!typeEditors.ContainsKey(editorType))
				return null;
			return typeEditors[editorType];
		}


		//-----------------------------------------------------------------------------
		// Methods
		//-----------------------------------------------------------------------------

		public void OpenProperties(Properties properties, IPropertyObject editedObject) {
			this.editedObject = editedObject;
			propertiesContainer.Set(properties);
			propertyGrid.Refresh();
		}
		
		public void CloseProperties() {
			editedObject = null;
			propertiesContainer.Clear();
			propertyGrid.Refresh();
		}
		

		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public void OnPropertyChange(object sender, PropertyValueChangedEventArgs e) {
			CustomPropertyDescriptor propertyDescriptor = e.ChangedItem.PropertyDescriptor as CustomPropertyDescriptor;
			Property property = propertyDescriptor.Property;
			PropertyDocumentation propertyDoc = property.Documentation;
			
			// Handle special property editor-types.
			if (propertyDoc != null && propertyDoc.EditorType == "script") {
				string oldValue = e.OldValue.ToString();
				string newValue = e.ChangedItem.Value.ToString();

				Script oldScript = editorControl.World.GetScript(oldValue);
				Script newScript = editorControl.World.GetScript(newValue);
				bool isNewScriptInvalid = (newScript == null && newValue.Length > 0);

				// When a script property is changed from a hidden script to something else.
				if (oldScript != null && oldScript.IsHidden && newScript != oldScript) {
					// Delete the old script from the world (because it is now unreferenced).
					editorControl.World.RemoveScript(oldScript);
					Console.WriteLine("Deleted unreferenced script '" + oldValue + "'");

					// Don't allow the user to reference other hidden scripts.
					if (newScript != null && newScript.IsHidden)
						isNewScriptInvalid = true;
				}

				// Show a message if the script is invalid.
				if (isNewScriptInvalid)
					MessageBox.Show("'" + newValue + "' is not a valid script name.");
			}

		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public TileDataInstance TileData {
			get { return editedObject as TileDataInstance; }
		}

		public IPropertyObject EditedObject {
			get { return editedObject; }
			set { editedObject = value; }
		}

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}
	}
}
