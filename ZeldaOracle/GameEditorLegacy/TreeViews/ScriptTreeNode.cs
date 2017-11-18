using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaEditor.Control;
using ZeldaEditor.PropertiesEditor.CustomEditors;
using ZeldaEditor.Scripting;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;

namespace ZeldaEditor.TreeViews {
	
	public class ScriptTreeNode : IWorldTreeViewNode {
		private Script script;

		public ScriptTreeNode(Script script) {
			this.script = script;
			int imageIndex = (script.HasErrors ? 9 : 8);
			ImageIndex			= imageIndex;
			SelectedImageIndex	= imageIndex;
			Text				= script.ID;
			Name				= "script";
		}

		public override void Open(EditorControl editorControl) {
			using (ScriptEditor form = new ScriptEditor(script, editorControl)) {
				if (form.ShowDialog(editorControl.EditorForm) == DialogResult.OK) {
					editorControl.RefreshWorldTreeView();
				}
			}
		}

		public override void Delete(EditorControl editorControl) {
			DialogResult result = MessageBox.Show(editorControl.EditorForm,
				"You are about to delete the script '" + script.ID + "'. This will be permanent. Continue?", "Confirm",
				MessageBoxButtons.YesNo, MessageBoxIcon.Information,
				MessageBoxDefaultButton.Button2);

			if (result == DialogResult.Yes) {
				editorControl.World.RemoveScript(script);
				editorControl.RefreshWorldTreeView();
			}
		}

		public override void Rename(string name) {
			script.ID = name;
		}

		public override void Duplicate(EditorControl editorControl, string suffix) {
			Script duplicate = new Script(script);
			duplicate.ID += suffix;
			editorControl.World.AddScript(duplicate);
			editorControl.RefreshWorldTreeView();
		}
	}
}
