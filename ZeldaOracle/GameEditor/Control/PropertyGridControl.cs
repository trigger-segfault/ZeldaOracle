using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;
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

			propertiesContainer = new PropertiesContainer(this);
			propertyGrid.SelectedObject = propertiesContainer;

			// Create property editor types.
			typeEditors = new Dictionary<string, CustomPropertyEditor>();
			typeEditors["sprite"]			= new ResourcePropertyEditor<Sprite>();
			typeEditors["animation"]		= new ResourcePropertyEditor<Animation>();
			typeEditors["collision_model"]	= new ResourcePropertyEditor<CollisionModel>();
			typeEditors["song"]				= new ResourcePropertyEditor<Song>();
			typeEditors["sound"]			= new ResourcePropertyEditor<Sound>();
			typeEditors["zone"]				= new ResourcePropertyEditor<Zone>();
			typeEditors["reward"]			= new RewardPropertyEditor(editorControl.RewardManager);
			typeEditors["text_message"]		= new TextMessagePropertyEditor();
			typeEditors["script"]			= null;
			typeEditors["sprite_index"]		= new SpriteIndexComboBox();
			typeEditors["direction"]		= new DirectionPropertyEditor();
			typeEditors["angle"]			= null;
			typeEditors["enum"]				= new EnumComboBox();
			typeEditors["enum_flags"]		= null;

			// Initialize property type editors.
			foreach (KeyValuePair<string, CustomPropertyEditor> entry in typeEditors) {
				if (entry.Value != null)
					entry.Value.Initialize(this);
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
		// Properties
		//-----------------------------------------------------------------------------

		public TileDataInstance TileData {
			get { return editedObject as TileDataInstance; }
		}

		public IPropertyObject EditedObject {
			get { return editedObject; }
			set { editedObject = value; }
		}
	}
}
