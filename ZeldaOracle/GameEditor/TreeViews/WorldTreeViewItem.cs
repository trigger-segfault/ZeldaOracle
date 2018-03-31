using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.TreeViews {

	public class WorldTreeViewItem : IWorldTreeViewItem {

		private World world;
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public WorldTreeViewItem(World world) {
			this.world = world;
			Source              = EditorImages.World;
			Header				= world.ID;
			Tag					= "world";
			IsExpanded          = true;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Open(EditorControl editorControl) {
			OpenProperties(editorControl);
		}

		public override void OpenProperties(EditorControl editorControl) {
			editorControl.OpenProperties(world);
			editorControl.EditorWindow.OpenObjectEditor(world);
		}

		public override void Rename(EditorControl editorControl, string name) {
			world.ID = name;
			Header = name;
			editorControl.IsModified = true;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public override IIDObject IDObject { get { return world; } }
	}
}
