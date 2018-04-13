using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ZeldaEditor.Control;
//using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaEditor.Scripting;
using ZeldaEditor.Undo;
using ZeldaWpf.Util;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaWpf.Windows;

namespace ZeldaEditor.TreeViews {
	
	public class AreaTreeViewItem : IWorldTreeViewItem {

		private Area area;
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public AreaTreeViewItem(Area area) {
			this.area = area;
			Source  = EditorImages.Area;
			Header				= area.ID;
			Tag					= "area";
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Open(EditorControl editorControl) {
			OpenProperties(editorControl);
		}

		public override void OpenProperties(EditorControl editorControl) {
			// Open the areas's properties
			editorControl.OpenProperties(area);
			editorControl.EditorWindow.OpenObjectEditor(area);
		}

		public override void Delete(EditorControl editorControl) {
			MessageBoxResult result = TriggerMessageBox.Show(editorControl.EditorWindow, MessageIcon.Warning,
				"Are you sure you want to delete the area '" + area.ID + "'?", "Confirm",
				MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				ActionDeleteArea action = new ActionDeleteArea(area);
				editorControl.PushAction(action, ActionExecution.Execute);
				/*editorControl.World.RemoveArea(area);
				editorControl.EditorWindow.TreeViewWorld.RefreshAreas();
				editorControl.IsModified = true;*/
			}
		}

		public override void Rename(EditorControl editorControl, string name) {
			editorControl.World.RenameArea(area, name);
			Header = area.ID;
			editorControl.IsModified = true;
		}

		public override void Duplicate(EditorControl editorControl) {
			// Dummy area for the rename window
			Area duplicate = new Area();
			string newName = RenameWindow.Show(Window.GetWindow(this), editorControl.World, duplicate);
			if (newName != null) {
				duplicate = new Area(area);
				EditorAction action = new ActionDuplicateArea(area, duplicate);
				editorControl.PushAction(action, ActionExecution.Execute);
			}
		}

		public Area Area {
			get { return area; }
		}

		public override IIDObject IDObject { get { return area; } }
	}
}
