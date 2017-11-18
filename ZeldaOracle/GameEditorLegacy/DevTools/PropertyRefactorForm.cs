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
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.DevTools {
	public partial class PropertyRefactorForm : Form {
		
		private EditorControl editorControl;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PropertyRefactorForm(EditorControl editorControl) {
			InitializeComponent();
			this.editorControl = editorControl;

			// Add items to the "Look In" combo box.
			comboBoxLookIn.Items.Add("Entire World");
			comboBoxLookIn.Items.Add("Current Level");
			comboBoxLookIn.Items.Add("Current Selection");
			comboBoxLookIn.SelectedIndex = 0;
			
			// Set the button to be pressed with the Enter key.
			AcceptButton = buttonFindAll;
			
			KeyPreview = true;
		}

		//-----------------------------------------------------------------------------
		// Form Events
		//-----------------------------------------------------------------------------

		// Close the form when the ESCAPE key is pressed.
		private void PropertyRefactorForm_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Escape) {
				this.Close();
			}
		}
		
		// Find All.
		private void buttonFindAll_Click(object sender, EventArgs e) {
			// Create the find settings.
			PropertyFindSettings findSettings = new PropertyFindSettings() {
				PropertyName = textBoxFindName.Text,
				Scope = (ObjectFindScope) comboBoxLookIn.SelectedIndex
			};

			Console.WriteLine("Finding properties with the name {0}:", findSettings.PropertyName);

			// Search through object's properties.
			int resultCount = 0;
			foreach (Property property in GetProperties(findSettings)) {
				IPropertyObject propertyObject = property.Properties.PropertyObject;

				if (propertyObject is BaseTileData) {
					Console.WriteLine(" - {0}", ((BaseTileData) propertyObject).Name);
				}
				else if (propertyObject is BaseTileDataInstance) {
					Type type = ((BaseTileDataInstance) propertyObject).Type;

					if (type != null)
						Console.WriteLine(" - {0}", type.Name);
					else
						Console.WriteLine(" - Tile");
				}
				resultCount++;
			}

			Console.WriteLine("{0} results found", resultCount);
		}

		// Replace All.
		private void buttonReplaceAll_Click(object sender, EventArgs e) {
			// Create the find settings.
			PropertyFindSettings findSettings = new PropertyFindSettings() {
				PropertyName = textBoxFindName.Text,
				Scope = (ObjectFindScope) comboBoxLookIn.SelectedIndex
			};
			string replaceName = textBoxReplaceName.Text;

			Console.WriteLine("Replacing properties with the name {0} to the name {1}:", findSettings.PropertyName, replaceName);

			// Search through object's properties.
			int resultCount = 0;
			foreach (Property property in GetProperties(findSettings)) {
				IPropertyObject propertyObject = property.Properties.PropertyObject;
				property.Name = replaceName;
				resultCount++;
			}

			Console.WriteLine("{0} properties modified", resultCount);
		}


		//-----------------------------------------------------------------------------
		// Property Find Methods
		//-----------------------------------------------------------------------------
		
		// Return an enumerable lits of properties found with the given find settings.
		private IEnumerable<Property> GetProperties(PropertyFindSettings findSettings) {
			foreach (IPropertyObject propertyObject in GetPropertyObjects(findSettings.Scope)) {
				foreach (Property property in propertyObject.Properties.GetProperties()) {
					if (property.Name == findSettings.PropertyName)
						yield return property;
				}
			}
		}
		
		// Return an enumerable lits of placed tiles in the given level.
		private IEnumerable<IPropertyObject> GetPropertyObjects(ObjectFindScope scope) {
			// Search tile data.
			foreach (KeyValuePair<string, TileData> entry in Resources.GetResourceDictionary<TileData>())
				yield return entry.Value;

			// Search event tile data.
			foreach (KeyValuePair<string, EventTileData> entry in Resources.GetResourceDictionary<EventTileData>())
				yield return entry.Value;

			// Search placed tiles.
			if (scope == ObjectFindScope.EntireWorld) {
				foreach (Level level in editorControl.World.Levels) {
					foreach (IPropertyObject obj in GetPropertyObjectsInLevel(level))
						yield return obj;
				}
			}
			else if (scope == ObjectFindScope.CurrentLevel) {
				foreach (IPropertyObject obj in GetPropertyObjectsInLevel(editorControl.Level))
					yield return obj;
			}
			else if (scope == ObjectFindScope.CurrentSelection) {
				// TODO: Current selection.
				//editorControl.LevelDisplay.SelectionGrid
			}
		}
		
		// Return an enumerable lits of placed tiles in the given level.
		private IEnumerable<IPropertyObject> GetPropertyObjectsInLevel(Level level) {
			foreach (Room room in level.GetRooms()) {
				foreach (TileDataInstance tile in room.GetTiles())
					yield return tile;
				foreach (EventTileDataInstance tile in room.EventData)
					yield return tile;
			}
		}
	}


	public enum ObjectFindScope {
		EntireWorld = 0,
		CurrentLevel,
		CurrentSelection
	}

	public class PropertyFindSettings {
		public string PropertyName { get; set; }
		public ObjectFindScope Scope { get; set; }
	}

	public class FindResult {
		public Property Property { get; set; }
		public IPropertyObject PropertyObject { get; set; }
	}
}
