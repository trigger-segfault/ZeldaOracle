using System.Windows;
using ZeldaEditor.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Scripting;
using ZeldaEditor.WinForms;
using ZeldaOracle.Game.Worlds;
using System.Windows.Input;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for ObjectEditor.xaml
	/// </summary>
	public partial class ObjectEditor : Window {

		// TriggerEditor triggerEditor
		private EditorControl editorControl;
		private TilePreview tilePreview;
		/// <summary>The object to edit properties for.</summary>
		private object obj;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ObjectEditor(EditorControl editorControl, object obj) {
			InitializeComponent();
			
			this.editorControl = editorControl;
			this.obj = obj;
			
			// Create the tile preview
			tilePreview						= new TilePreview();
			tilePreview.EditorControl		= editorControl;
			tilePreview.Name				= "tilePreview";
			tilePreview.Dock				= System.Windows.Forms.DockStyle.Fill;
			hostTilePreview.Child			= tilePreview;
			
			SetObject(obj);
		}
		
		public static ObjectEditor Show(Window owner, EditorControl editorControl,
			object obj = null)
		{
			ObjectEditor window = new ObjectEditor(editorControl, obj);
			window.Owner = owner;
			window.Show();
			return window;
		}
		
	
		//-----------------------------------------------------------------------------
		// Object Management
		//-----------------------------------------------------------------------------

		/// <summary>Set the object to show properties for.</summary>
		public void SetObject(object obj) {
			this.obj = obj;
			triggerEditor.SetObject(obj as ITriggerObject);

			if (obj is IVariableObject)
				objectVariableEditor.Variables = ((IVariableObject) obj).Variables;
			else
				objectVariableEditor.Variables = null;

			// Set the object preview image and name
			objectPreviewName.Text = "(none)";
			if (obj is BaseTileDataInstance) {
				objectPreviewName.Text =
					((BaseTileDataInstance) obj).BaseData.ResourceName;
				if (obj is TileDataInstance)
					Title = "Tile Properties";
				else
					Title = "Action Tile Properties";
			}
			else if (obj is Area) {
				Title = "Area Properties";
				objectPreviewName.Text = ((Area) obj).ID;
			}
			else if (obj is Room) {
				Title = "Room Properties";
				objectPreviewName.Text = "Room";
				if (!string.IsNullOrWhiteSpace(((Room) obj).ID))
					objectPreviewName.Text += " - " + ((Room) obj).ID;
			}
			else if (obj is Level) {
				Title = "Level Properties";
				objectPreviewName.Text = ((Level) obj).ID;
			}
			else if (obj is World) {
				Title = "World Properties";
				objectPreviewName.Text = "World";
			}
			tilePreview.UpdateTile(obj as BaseTileDataInstance);
		}


		//-----------------------------------------------------------------------------
		// UI Callbacks
		//-----------------------------------------------------------------------------


	}
}
