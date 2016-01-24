using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
using ZeldaEditor.PropertiesEditor.CustomEditors;

namespace ZeldaEditor.TreeViews {
	
	public class WorldTreeView : TreeView {
		
		private EditorControl editorControl;
		private TreeNode worldNode;
		private TreeNode levelsNode;
		private TreeNode areasNode;
		private TreeNode dungeonsNode;
		private TreeNode scriptsNode;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public WorldTreeView() {
			editorControl = null;
			worldNode = null;

			// Open nodes on double click.
			NodeMouseDoubleClick += delegate(object sender, TreeNodeMouseClickEventArgs e) {
				OpenNode(e.Node);
			};

			/*LabelEdit = true; // Allows the editing of labels.

			// Only allow label editing with object nodes.
			BeforeLabelEdit += delegate(object sender, NodeLabelEditEventArgs e) {
				if (!(e.Node is IWorldTreeViewNode))
					e.CancelEdit = true;
			};

			// Update node names after label editing.
			AfterLabelEdit += delegate(object sender, NodeLabelEditEventArgs e) {
				if (e.CancelEdit)
					Console.WriteLine("CANCEL EDIT!!");
				TreeNode node = e.Node;
				if (node is IWorldTreeViewNode)
					((IWorldTreeViewNode) node).Rename(e.Label);
				node.Text = e.Label;
			};*/
			
			// Make sure the right clicked node doesn't change back after selecting an item in the content menu.
			MouseClick += delegate(object sender, MouseEventArgs e) {
				// Only check with right click so pressing the pluses and minuses don't change the selection.
				if (e.Button == MouseButtons.Right)
					SelectedNode = GetNodeAt(e.X, e.Y);
				if (SelectedNode.IsEditing)
					SelectedNode.EndEdit(true);
			};
		}


		//-----------------------------------------------------------------------------
		// Tree Node Mutators
		//-----------------------------------------------------------------------------
		
		public void OpenNode(TreeNode node) {
			if (node is IWorldTreeViewNode)
				((IWorldTreeViewNode) node).Open(editorControl);
		}

		public void RenameNode() {
			if (SelectedNode is IWorldTreeViewNode) {
				//SelectedNode.BeginEdit();
				
				if (RenameForm.Show(this, SelectedNode.Text) == DialogResult.OK) {
					((IWorldTreeViewNode) SelectedNode).Rename(RenameForm.NewName);
					RefreshTree();
				}
			}
		}

		public void DuplicateNode() {
			TreeNode node = SelectedNode;
			if (node is IWorldTreeViewNode)
				((IWorldTreeViewNode) node).Duplicate(editorControl, " - Copy");
		}

		public void DeleteNode() {
			TreeNode node = SelectedNode;
			if (node is IWorldTreeViewNode)
				((IWorldTreeViewNode) node).Delete(editorControl);
		}
		
		public void MoveNodeUp() {
			if (SelectedNode.Name == "level") {
				editorControl.World.MoveLevel(SelectedNode.Index, -1, true);
				editorControl.RefreshWorldTreeView();
				SelectedNode = levelsNode.Nodes[SelectedNode.Index - 1];
			}
		}
		
		public void MoveNodeDown() {
			if (SelectedNode.Name == "level") {
				editorControl.World.MoveLevel(SelectedNode.Index, +1, true);
				editorControl.RefreshWorldTreeView();
				SelectedNode = levelsNode.Nodes[SelectedNode.Index + 1];
			}
		}
		

		//-----------------------------------------------------------------------------
		// Tree Refresh Methods
		//-----------------------------------------------------------------------------

		public void RefreshLevels() {
			levelsNode.Nodes.Clear();
			World world = editorControl.World;
			
			for (int i = 0; i < world.Levels.Count; i++) {
				TreeNode levelNode = new LevelTreeNode(world.Levels[i]);
				levelNode.ContextMenuStrip = editorControl.EditorForm.contextMenuLevelSelect;
				levelsNode.Nodes.Add(levelNode);
			}
		}

		public void RefreshScripts() {
			scriptsNode.Nodes.Clear();
			
			foreach (Script script in editorControl.World.Scripts.Values) {
				if (!script.IsHidden) {
					ScriptTreeNode scriptNode = new ScriptTreeNode(script);
					scriptNode.ContextMenuStrip = editorControl.EditorForm.contextMenuScriptNode;
					scriptsNode.Nodes.Add(scriptNode);
				}
			}
		}

		public void RefreshDungeons() {
			dungeonsNode.Nodes.Clear();
			
			foreach (Dungeon dungeon in editorControl.World.Dungeons.Values) {
				DungeonTreeNode dungeonNode = new DungeonTreeNode(dungeon);
				//dungeonNode.ContextMenuStrip = editorControl.EditorForm.contextMenuDungeonNode;
				dungeonsNode.Nodes.Add(dungeonNode);
			}
		}
		
		public void RefreshAreas() {
			areasNode.Nodes.Clear();
		}

		public void RefreshTree() {
			World world = editorControl.World;

			if (world == null) {
				Nodes.Clear(); // Don't show a tree if no world is open.
			}
			else {
				// Create the tree node skeleton (world, levels, scripts, etc.)
				if (Nodes.Count == 0)
					CreateTreeSkeleton();
				
				worldNode.Text = world.Id;

				RefreshLevels();
				RefreshAreas();
				RefreshDungeons();
				RefreshScripts();
			}
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		// Create the base node folders.
		private void CreateTreeSkeleton() {
			World world = editorControl.World;

			worldNode			= new WorldTreeNode(world);
			
			levelsNode			= new TreeNode("Levels",	1, 1);
			areasNode			= new TreeNode("Areas",		3, 3);
			dungeonsNode		= new TreeNode("Dungeons",	5, 5);
			scriptsNode			= new TreeNode("Scripts",	7, 7);

			levelsNode.Name		= "levels";
			areasNode.Name		= "areas";
			dungeonsNode.Name	= "dungeons";
			scriptsNode.Name	= "scripts";

			Nodes.Add(worldNode);
			worldNode.Nodes.Add(levelsNode);
			worldNode.Nodes.Add(areasNode);
			worldNode.Nodes.Add(dungeonsNode);
			worldNode.Nodes.Add(scriptsNode);

			worldNode.ContextMenuStrip = editorControl.EditorForm.ContenxtMenuGeneral;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorControl EditorControl { 
			get { return editorControl; }
			set { editorControl = value; }
		}
	}
}
