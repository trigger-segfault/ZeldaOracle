using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZeldaEditor.Control;
//using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaEditor.Scripting;
using ZeldaEditor.Util;
using ZeldaEditor.Windows;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.TreeViews {
	
	public class ScriptTreeViewItem : IWorldTreeViewItem {
		private Script script;
		
		public ScriptTreeViewItem(Script script) {
			this.script = script;
			Source = (script.HasErrors ? EditorImages.ScriptError : EditorImages.Script);
			Header				= script.ID;
			Tag					= "script";
		}

		public override void Open(EditorControl editorControl) {
			if (ScriptEditor.Show(editorControl.EditorWindow, script, editorControl, false)) {
				editorControl.EditorWindow.TreeViewWorld.RefreshScripts();
			}
		}

		public override void Delete(EditorControl editorControl) {
			MessageBoxResult result = TriggerMessageBox.Show(editorControl.EditorWindow, MessageIcon.Info,
				"You are about to delete the script '" + script.ID + "'. This will be permanent. Continue?", "Confirm",
				MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				editorControl.World.RemoveScript(script);
				editorControl.EditorWindow.TreeViewWorld.RefreshScripts();
				editorControl.IsModified = true;
			}
		}

		public override void Rename(World world, string name) {
			world.RenameScript(script, name);
			Header = script.ID;
		}

		public override void Duplicate(EditorControl editorControl, string suffix) {
			/*Script duplicate = new Script(script);
			duplicate.ID += suffix;
			editorControl.World.AddScript(duplicate);
			editorControl.RefreshWorldTreeView();*/
		}

		public override IIDObject IDObject { get { return script; } }
	}
}
