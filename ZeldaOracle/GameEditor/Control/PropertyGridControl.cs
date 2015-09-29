using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.Control {
	public class PropertyGridControl {

		private EditorControl		editorControl;
		private PropertyGrid		propertyGrid;
		private PropertiesContainer	propertiesContainer;
		private Dictionary<string, UITypeEditor> typeEditors;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PropertyGridControl(EditorControl editorControl, PropertyGrid propertyGrid) {
			this.editorControl	= editorControl;
			this.propertyGrid	= propertyGrid;

			propertiesContainer = new PropertiesContainer(this);
			propertyGrid.SelectedObject = propertiesContainer;
			typeEditors = new Dictionary<string,UITypeEditor>();

			typeEditors["sprite"]			= new ResourcePropertyEditor<Sprite>();
			typeEditors["animation"]		= new ResourcePropertyEditor<Animation>();
			typeEditors["collision_model"]	= new ResourcePropertyEditor<CollisionModel>();
			typeEditors["zone"]				= new ResourcePropertyEditor<Zone>();
			typeEditors["reward"]			= new RewardPropertyEditor(editorControl.RewardManager);
			
			typeEditors["text_message"]		= new TextMessagePropertyEditor();
			typeEditors["script"]			= null;

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

		public void OpenProperties(Properties properties, Properties baseProperties) {
			propertiesContainer.Set(properties, baseProperties);
			propertyGrid.Refresh();
		}
		
		public void CloseProperties() {
			propertiesContainer.Clear();
			propertyGrid.Refresh();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
	}
}
