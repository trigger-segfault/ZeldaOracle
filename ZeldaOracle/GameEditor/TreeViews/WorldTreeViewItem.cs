using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
//using ZeldaEditor.PropertiesEditor.CustomEditors;
using System.Windows.Media;
using ZeldaEditor.Util;
using ZeldaOracle.Common.Scripting;

namespace ZeldaEditor.TreeViews {
	
	public class WorldTreeViewItem : IWorldTreeViewItem {
		private World world;
		
		public WorldTreeViewItem(World world) {
			this.world = world;
			Source              = EditorImages.World;
			Header				= world.ID;
			Tag					= "world";
			IsExpanded          = true;
		}

		public override void Open(EditorControl editorControl) {
			editorControl.OpenProperties(world);
		}

		public override void Rename(EditorControl editorControl, string name) {
			world.ID = name;
			Header = name;
			editorControl.IsModified = true;
		}

		public override IIDObject IDObject { get { return world; } }
	}
}
