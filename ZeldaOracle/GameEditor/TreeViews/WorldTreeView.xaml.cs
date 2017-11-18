using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
//using ZeldaEditor.PropertiesEditor.CustomEditors;
using System.Windows.Controls;
using System.Windows.Media;
using ZeldaEditor.Util;
using ZeldaEditor.Controls;
using System.Windows.Input;
using System.Windows;
using ZeldaEditor.Windows;

namespace ZeldaEditor.TreeViews {
	/// <summary>
	/// Interaction logic for WorldTreeView.xaml
	/// </summary>
	public partial class WorldTreeView : UserControl {


		private EditorControl editorControl;
		private ImageTreeViewItem worldNode;
		private ImageTreeViewItem levelsNode;
		private ImageTreeViewItem dungeonsNode;
		private ImageTreeViewItem scriptsNode;
		private TreeViewItem hiddenScriptsNode;
		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		
		public WorldTreeView() {
			InitializeComponent();
			editorControl = null;
			worldNode = null;

			// Open nodes on double click.
			// TODO: Reimplement
			/*NodeMouseDoubleClick += delegate(object sender, TreeNodeMouseClickEventArgs e) {
				OpenNode(e.Node);
			};*/

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

			// TODO: Reimplement
			// Make sure the right clicked node doesn't change back after selecting an item in the content menu.
			/*MouseClick += delegate(object sender, MouseEventArgs e) {
				// Only check with right click so pressing the pluses and minuses don't change the selection.
				if (e.Button == MouseButtons.Right)
					SelectedNode = GetNodeAt(e.X, e.Y);
				if (SelectedNode.IsEditing)
					SelectedNode.EndEdit(true);
			};*/
			
			treeView.Items.Clear();
		}


		//-----------------------------------------------------------------------------
		// Tree Node Mutators
		//-----------------------------------------------------------------------------

		public void OpenNode(TreeViewItem node) {
			if (node is IWorldTreeViewItem)
				((IWorldTreeViewItem)node).Open(editorControl);
		}

		public void RenameNode() {
			if (treeView.SelectedItem is IWorldTreeViewItem) {
				//SelectedNode.BeginEdit();

				// TODO: Reimplement
				/*if (RenameForm.Show(this, SelectedNode.Text) == DialogResult.OK) {
					((IWorldTreeViewNode) SelectedNode).Rename(RenameForm.NewName);
					RefreshTree();
				}*/
			}
		}

		public void DuplicateNode() {
			TreeViewItem node = treeView.SelectedItem as TreeViewItem;
			if (node is IWorldTreeViewItem)
				((IWorldTreeViewItem)node).Duplicate(editorControl, " - Copy");
		}

		public void DeleteNode() {
			TreeViewItem node = treeView.SelectedItem as TreeViewItem;
			if (node is IWorldTreeViewItem)
				((IWorldTreeViewItem)node).Delete(editorControl);
		}

		public void MoveNodeUp() {
			if (treeView.SelectedItem is LevelTreeViewItem) {
				int selectedIndex = GetNodeIndex(levelsNode, treeView.SelectedItem);
				editorControl.World.MoveLevel(selectedIndex, -1, true);
				editorControl.RefreshWorldTreeView();
				SelectItem(levelsNode, selectedIndex - 1);
			}
		}

		public void MoveNodeDown() {
			if (treeView.SelectedItem is LevelTreeViewItem) {
				int selectedIndex = GetNodeIndex(levelsNode, treeView.SelectedItem);
				editorControl.World.MoveLevel(selectedIndex, +1, true);
				editorControl.RefreshWorldTreeView();
				SelectItem(levelsNode, selectedIndex + 1);
			}
		}

		public void SelectItem(TreeViewItem parent, int index) {
			(parent.Items[index] as TreeViewItem).IsSelected = true;
		}

		public int GetNodeIndex(TreeViewItem parent, object child) {
			for (int i = 0; i < parent.Items.Count; i++) {
				if (parent.Items[i] == child)
					return i;
			}
			return -1;
		}

		//-----------------------------------------------------------------------------
		// Tree Refresh Methods
		//-----------------------------------------------------------------------------

		public void RefreshLevels() {
			levelsNode.Items.Clear();
			World world = editorControl.World;

			for (int i = 0; i < world.Levels.Count; i++) {
				LevelTreeViewItem levelNode = new LevelTreeViewItem(world.Levels[i]);
				//levelNode.ContextMenuStrip = editorControl.EditorWindow.contextMenuLevelSelect;
				levelsNode.Items.Add(levelNode);
			}
		}

		public void RefreshScripts() {
			bool expanded = (hiddenScriptsNode != null && hiddenScriptsNode.IsExpanded);
			scriptsNode.Items.Clear();
			hiddenScriptsNode = new FolderTreeViewItem("Internal", expanded);

			List<Script> scripts = editorControl.World.Scripts.Values.ToList();
			scripts.Sort((a, b) => { return AlphanumComparator.Compare(a.ID, b.ID, true); } );
			foreach (Script script in scripts) {
				ScriptTreeViewItem scriptNode = new ScriptTreeViewItem(script);
				//scriptNode.ContextMenuStrip = editorControl.EditorWindow.contextMenuScriptNode;
				if (!script.IsHidden) {
					scriptsNode.Items.Add(scriptNode);
				}
				else {
					hiddenScriptsNode.Items.Add(scriptNode);
				}
			}
			scriptsNode.Items.Add(hiddenScriptsNode);
		}

		public void RefreshDungeons() {
			dungeonsNode.Items.Clear();

			foreach (Dungeon dungeon in editorControl.World.Dungeons) {
				DungeonTreeViewItem dungeonNode = new DungeonTreeViewItem(dungeon);
				//dungeonNode.ContextMenuStrip = editorControl.EditorForm.contextMenuDungeonNode;
				dungeonsNode.Items.Add(dungeonNode);
			}
		}

		public void RefreshTree() {
			World world = editorControl.World;

			if (world == null) {
				treeView.Items.Clear(); // Don't show a tree if no world is open.
			}
			else {
				// Create the tree node skeleton (world, levels, scripts, etc.)
				if (treeView.Items.Count == 0)
					CreateTreeSkeleton();

				worldNode.Header = world.ID;

				RefreshLevels();
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

			worldNode           = new WorldTreeViewItem(world);

			levelsNode          = new ImageTreeViewItem(EditorImages.LevelGroup, "Levels", true);
			levelsNode.Tag      = "levels";
			dungeonsNode        = new ImageTreeViewItem(EditorImages.DungeonGroup, "Dungeons", true);
			dungeonsNode.Tag    = "dungeons";
			scriptsNode         = new ImageTreeViewItem(EditorImages.ScriptGroup, "Scripts", true);
			scriptsNode.Tag		= "scripts";


			treeView.Items.Add(worldNode);
			worldNode.Items.Add(levelsNode);
			worldNode.Items.Add(dungeonsNode);
			worldNode.Items.Add(scriptsNode);

			//worldNode.ContextMenuStrip = editorControl.EditorWindow.ContenxtMenuGeneral;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}

		private void OnSelectionChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e) {
			UpdateButtons();
		}

		//private TreeViewItem pressedTreeViewItem;

		private void OnTreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			OpenNode(treeView.SelectedItem as TreeViewItem);
		}
		private void OnTreeViewMouseRightButtonDown(object sender, MouseEventArgs e) {
			TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

			if (treeViewItem != null) {
				treeViewItem.Focus();
				e.Handled = true;
			}
		}

		static TreeViewItem VisualUpwardSearch(DependencyObject source) {
			while (source != null && !(source is TreeViewItem))
				source = VisualTreeHelper.GetParent(source);

			return source as TreeViewItem;
		}
		private void OnTreeViewMouseDown(object sender, MouseButtonEventArgs e) {
			TreeViewItem item = sender as TreeViewItem;
			item.IsSelected = true;
		}

		private void OnEdit(object sender, RoutedEventArgs e) {
			(treeView.SelectedItem as IWorldTreeViewItem).Open(editorControl);
		}

		private void OnRename(object sender, RoutedEventArgs e) {
			IWorldTreeViewItem node = treeView.SelectedItem as IWorldTreeViewItem;
			string newName = RenameWindow.Show(Window.GetWindow(this),
				editorControl.World, node.IDObject);

			if (newName != null) {
				node.Rename(editorControl.World, newName);
				editorControl.EditorWindow.UpdatePropertyPreview(editorControl.PropertyGrid.PropertyObject);
				editorControl.IsModified = true;
			}
		}

		private void OnDuplicate(object sender, RoutedEventArgs e) {
			//(treeView.SelectedItem as IWorldTreeViewNode).Duplicate(editorControl);
		}

		private void OnDelete(object sender, RoutedEventArgs e) {
			(treeView.SelectedItem as IWorldTreeViewItem).Delete(editorControl);
		}

		private void OnMoveUp(object sender, RoutedEventArgs e) {
			MoveSelected(-1);
		}

		private void OnMoveDown(object sender, RoutedEventArgs e) {
			MoveSelected(+1);
		}

		private void MoveSelected(int direction) {
			TreeViewItem parent = GetParentNode(treeView.SelectedItem);
			int selectedIndex = GetNodeIndex(parent, treeView.SelectedItem);
			if (treeView.SelectedItem is LevelTreeViewItem) {
				editorControl.World.MoveLevel(selectedIndex, direction, true);
			}
			else if (treeView.SelectedItem is DungeonTreeViewItem) {
				editorControl.World.MoveDungeon(selectedIndex, direction, true);
			}
			editorControl.RefreshWorldTreeView();
			SelectItem(parent, selectedIndex + direction);
		}

		private TreeViewItem GetParentNode(object node) {
			return (node as TreeViewItem).Parent as TreeViewItem;
		}

		private void UpdateButtons() {
			buttonEdit.IsEnabled = false;
			buttonRename.IsEnabled = false;
			buttonDuplicate.IsEnabled = false;
			buttonDelete.IsEnabled = false;
			buttonMoveUp.IsEnabled = false;
			buttonMoveDown.IsEnabled = false;
			
			if (treeView.SelectedItem is WorldTreeViewItem) {
				buttonEdit.IsEnabled = true;
				buttonRename.IsEnabled = true;
			}
			else if (treeView.SelectedItem is IWorldTreeViewItem) {
				buttonEdit.IsEnabled = true;
				buttonRename.IsEnabled = true;
				buttonDuplicate.IsEnabled = true;
				buttonDelete.IsEnabled = true;

				if (!(treeView.SelectedItem is ScriptTreeViewItem)) {
					TreeViewItem parent = GetParentNode(treeView.SelectedItem);
					int selectedIndex = GetNodeIndex(parent, treeView.SelectedItem);
					if (selectedIndex > 0)
						buttonMoveUp.IsEnabled = true;
					if (selectedIndex + 1 < parent.Items.Count)
						buttonMoveDown.IsEnabled = true;
				}
			}
		}

		private void OnTreeViewMouseEnter(object sender, MouseEventArgs e) {
			treeView.Focus();
		}
	}
}
