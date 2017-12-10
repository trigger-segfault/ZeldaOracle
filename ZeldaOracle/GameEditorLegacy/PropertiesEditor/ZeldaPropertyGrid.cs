using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;
using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.PropertiesEditor {

	public class ZeldaPropertyGrid : PropertyGrid {

		private EditorControl editorControl;
		private IPropertyObject propertyObject;
		private PropertiesContainer	propertiesContainer;
		private Dictionary<string, CustomPropertyEditor> typeEditors;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ZeldaPropertyGrid() {
			editorControl		= null;
			propertyObject		= null;
			propertiesContainer	= new PropertiesContainer(this);
			typeEditors			= new Dictionary<string, CustomPropertyEditor>();

			this.SelectedObject			= propertiesContainer;
			this.PropertyValueChanged	+= OnPropertyChange;
		}

		public void Initialize(EditorControl editorControl) {
			this.editorControl = editorControl;

			// Create custom property editor types.
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

			// Initialize the property editors.
			foreach (CustomPropertyEditor editor in typeEditors.Values.Where(e => e != null)) {
				editor.Initialize(this);
			}
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
		// Properties Methods
		//-----------------------------------------------------------------------------
		
		public void OpenProperties(Properties properties, IPropertyObject propertyObject) {
			OpenProperties(propertyObject);
		}

		public void OpenProperties(IPropertyObject propertyObject) {
			this.propertyObject = propertyObject;
			propertiesContainer.Set(propertyObject.Properties);
			Refresh();
		}
		
		public void RefreshProperties() {
			Refresh();
		}
		
		public void CloseProperties() {
			propertiesContainer.Clear();
			Refresh();
		}
		

		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		private void OnPropertyChange(object sender, PropertyValueChangedEventArgs e) {
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
		
		public EditorControl EditorControl { 
			get { return editorControl; }
			set { editorControl = value; }
		}

		public Properties Properties {
			get { return propertyObject.Properties; }
		}
		
		public IPropertyObject PropertyObject {
			get { return propertyObject; }
		}
	}
}
