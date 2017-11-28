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
using ZeldaEditor.Undo;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaEditor.WinForms;

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

		private TreeViewItem internalScriptsNode;

		private TreeViewItem customScriptWorldNode;
		private TreeViewItem customScriptDungeonNode;
		private Dictionary<string, TreeViewItem> customScriptLevelNodes;

		private ContextMenu contextMenuWorld;
		private ContextMenu contextMenuLevel;
		private ContextMenu contextMenuDungeon;
		private ContextMenu contextMenuScript;
		private ContextMenu contextMenuEvent;

		private WpfMouseWheelMessageFilter messageFilter;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public WorldTreeView() {
			InitializeComponent();
			editorControl = null;
			worldNode = null;

			InitWorldContextMenu();
			InitLevelContextMenu();
			InitDungeonContextMenu();
			InitScriptContextMenu();
			InitEventContextMenu();

			customScriptLevelNodes = new Dictionary<string, TreeViewItem>();
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

			this.messageFilter = new WpfMouseWheelMessageFilter(treeView);
			this.messageFilter.AddFilter();
		}

		private void InitWorldContextMenu() {
			ImageMenuItem menuItem;
			contextMenuWorld = new ContextMenu();
			menuItem = new ImageMenuItem(EditorImages.Rename, "Rename");
			menuItem.Click += OnRename;
			contextMenuWorld.Items.Add(menuItem);
			menuItem = new ImageMenuItem(EditorImages.Edit, "Edit");
			menuItem.Click += OnEdit;
			contextMenuWorld.Items.Add(menuItem);
		}

		private void InitLevelContextMenu() {
			ImageMenuItem menuItem;
			contextMenuLevel = new ContextMenu();
			menuItem = new ImageMenuItem(EditorImages.LevelAdd, "Create Level");
			menuItem.Click += OnAdd;
			contextMenuLevel.Items.Add(menuItem);
			menuItem = new ImageMenuItem(EditorImages.LevelDuplicate, "Duplicate");
			menuItem.Click += OnDuplicate;
			contextMenuLevel.Items.Add(menuItem);

			contextMenuLevel.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.LevelDelete, "Delete");
			menuItem.Click += OnDelete;
			contextMenuLevel.Items.Add(menuItem);

			contextMenuLevel.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Rename, "Rename");
			menuItem.Click += OnRename;
			contextMenuLevel.Items.Add(menuItem);

			contextMenuLevel.Items.Add(new Separator());
			
			menuItem = new ImageMenuItem(EditorImages.LevelResize, "Resize");
			menuItem.Click += OnResizeLevel;
			contextMenuLevel.Items.Add(menuItem);

			menuItem = new ImageMenuItem(EditorImages.LevelShift, "Shift");
			menuItem.Click += OnShiftLevel;
			contextMenuLevel.Items.Add(menuItem);

			contextMenuLevel.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Edit, "Edit");
			menuItem.Click += OnEdit;
			contextMenuLevel.Items.Add(menuItem);
		}

		private void OnResizeLevel(object sender, RoutedEventArgs e) {
			Level level = (treeView.SelectedItem as LevelTreeViewItem).Level;
			EditorCommands.ResizeLevel.Execute(level, null);
		}

		private void OnShiftLevel(object sender, RoutedEventArgs e) {
			Level level = (treeView.SelectedItem as LevelTreeViewItem).Level;
			EditorCommands.ShiftLevel.Execute(level, null);
		}

		private void InitDungeonContextMenu() {
			ImageMenuItem menuItem;
			contextMenuDungeon = new ContextMenu();
			menuItem = new ImageMenuItem(EditorImages.DungeonAdd, "Create Dungeon");
			menuItem.Click += OnAdd;
			contextMenuDungeon.Items.Add(menuItem);
			menuItem = new ImageMenuItem(EditorImages.DungeonDuplicate, "Duplicate");
			menuItem.Click += OnDuplicate;
			contextMenuDungeon.Items.Add(menuItem);

			contextMenuDungeon.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.DungeonDelete, "Delete");
			menuItem.Click += OnDelete;
			contextMenuDungeon.Items.Add(menuItem);

			contextMenuDungeon.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Rename, "Rename");
			menuItem.Click += OnRename;
			contextMenuDungeon.Items.Add(menuItem);

			contextMenuDungeon.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Edit, "Edit");
			menuItem.Click += OnEdit;
			contextMenuDungeon.Items.Add(menuItem);
		}


		private void InitScriptContextMenu() {
			ImageMenuItem menuItem;
			contextMenuScript = new ContextMenu();
			menuItem = new ImageMenuItem(EditorImages.ScriptAdd, "Create Script");
			menuItem.Click += OnAdd;
			contextMenuScript.Items.Add(menuItem);
			menuItem = new ImageMenuItem(EditorImages.ScriptDuplicate, "Duplicate");
			menuItem.Click += OnDuplicate;
			contextMenuScript.Items.Add(menuItem);

			contextMenuScript.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.ScriptDelete, "Delete");
			menuItem.Click += OnDelete;
			contextMenuScript.Items.Add(menuItem);

			contextMenuScript.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Rename, "Rename");
			menuItem.Click += OnRename;
			contextMenuScript.Items.Add(menuItem);

			contextMenuScript.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Edit, "Edit");
			menuItem.Click += OnEdit;
			contextMenuScript.Items.Add(menuItem);
		}

		private void InitEventContextMenu() {
			ImageMenuItem menuItem;
			contextMenuEvent = new ContextMenu();
			menuItem = new ImageMenuItem(EditorImages.GotoOwner, "Goto Owner");
			menuItem.Click += OnGotoOwner;
			contextMenuEvent.Items.Add(menuItem);

			contextMenuEvent.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Edit, "Edit");
			menuItem.Click += OnEdit;
			contextMenuEvent.Items.Add(menuItem);
		}

		//-----------------------------------------------------------------------------
		// Tree Node Mutators
		//-----------------------------------------------------------------------------

		public void SelectItem(TreeViewItem parent, int index) {
			(parent.Items[index] as TreeViewItem).IsSelected = true;
		}

		public int GetItemIndex(TreeViewItem parent, object child) {
			for (int i = 0; i < parent.Items.Count; i++) {
				if (parent.Items[i] == child)
					return i;
			}
			return -1;
		}

		//-----------------------------------------------------------------------------
		// Tree Refresh Methods
		//-----------------------------------------------------------------------------

		public void RefreshWorld() {
			if (treeView.Items.Count == 0) {
				RefreshTree();
				return;
			}
			worldNode.Header = editorControl.World.ID;
		}

		public void RefreshLevels() {
			if (treeView.Items.Count == 0) {
				RefreshTree();
				return;
			}
			levelsNode.Items.Clear();
			World world = editorControl.World;

			for (int i = 0; i < world.LevelCount; i++) {
				LevelTreeViewItem levelNode = new LevelTreeViewItem(world.GetLevelAt(i), editorControl);
				levelNode.ContextMenu = contextMenuLevel;
				levelsNode.Items.Add(levelNode);
			}
		}

		public void RefreshScripts(bool refreshScripts, bool refreshEvents) {
			if (treeView.Items.Count == 0) {
				RefreshTree();
				return;
			}
			if (refreshEvents) {
				if (internalScriptsNode == null)
					internalScriptsNode = new FolderTreeViewItem("Events", false);
				internalScriptsNode.Items.Clear();
				
				TreeViewItem newCustomScriptWorldNode = new FolderTreeViewItem("World", IsNodeExpanded(customScriptWorldNode));
				TreeViewItem newCustomScriptDungeonNode = new FolderTreeViewItem("Dungeons", IsNodeExpanded(customScriptDungeonNode));
				Dictionary<string, TreeViewItem> newCustomScriptLevelNodes = new Dictionary<string, TreeViewItem>();

				foreach (Event evnt in editorControl.EventCache) {
					IEventObject eventObject = evnt.Events.EventObject;
					string level = null;
					string dungeon = null;
					EventTreeViewItem eventNode = new EventTreeViewItem(evnt, editorControl);
					eventNode.ContextMenu = contextMenuEvent;
					if (eventObject is BaseTileDataInstance) {
						level = ((BaseTileDataInstance)eventObject).Room.Level.ID;
					}
					else if (eventObject is Room) {
						level = ((Room)eventObject).Level.ID;
					}
					else if (eventObject is Level) {
						level = ((Level)eventObject).ID;
					}
					else if (eventObject is Dungeon) {
						dungeon = ((Dungeon)eventObject).ID;
					}
					if (level != null) {
						if (!newCustomScriptLevelNodes.ContainsKey(level)) {
							TreeViewItem previous;
							customScriptLevelNodes.TryGetValue(level, out previous);
							newCustomScriptLevelNodes.Add(level, new FolderTreeViewItem(
								"Level '" + level + "'", IsNodeExpanded(previous)));
						}
						newCustomScriptLevelNodes[level].Items.Add(eventNode);
					}
					else if (dungeon != null) {
						newCustomScriptDungeonNode.Items.Add(eventNode);
					}
					else {
						newCustomScriptWorldNode.Items.Add(eventNode);
					}
				}
				customScriptWorldNode = newCustomScriptWorldNode;
				customScriptDungeonNode = newCustomScriptDungeonNode;
				customScriptLevelNodes = newCustomScriptLevelNodes;

				if (customScriptWorldNode.Items.Count > 0) {
					internalScriptsNode.Items.Add(customScriptWorldNode);
				}
				if (customScriptDungeonNode.Items.Count > 0) {
					internalScriptsNode.Items.Add(customScriptDungeonNode);
				}
				foreach (TreeViewItem levelNode in customScriptLevelNodes.Values) {
					internalScriptsNode.Items.Add(levelNode);
				}
			}

			if (refreshScripts) {
				scriptsNode.Items.Clear();
				List<Script> scripts = editorControl.World.Scripts.Values.ToList();
				scripts.Sort((a, b) => { return AlphanumComparator.Compare(a.ID, b.ID, true); });
				foreach (Script script in scripts) {
					ScriptTreeViewItem scriptNode = new ScriptTreeViewItem(script, editorControl);
					scriptNode.ContextMenu = contextMenuScript;
					scriptsNode.Items.Add(scriptNode);
				}
				scriptsNode.Items.Add(internalScriptsNode);
			}
		}

		private bool IsNodeExpanded(TreeViewItem node, bool defaultExpanded = false) {
			return (node != null ? node.IsExpanded : defaultExpanded);
		}

		public void RefreshDungeons() {
			if (treeView.Items.Count == 0) {
				RefreshTree();
				return;
			}
			dungeonsNode.Items.Clear();

			foreach (Dungeon dungeon in editorControl.World.GetDungeons()) {
				DungeonTreeViewItem dungeonNode = new DungeonTreeViewItem(dungeon);
				dungeonNode.ContextMenu = contextMenuDungeon;
				dungeonsNode.Items.Add(dungeonNode);
			}
		}

		public void RefreshTree() {
			World world = editorControl.World;

			if (!editorControl.IsWorldOpen) {
				treeView.Items.Clear(); // Don't show a tree if no world is open.
			}
			else {
				// Create the tree node skeleton (world, levels, scripts, etc.)
				if (treeView.Items.Count == 0)
					CreateTreeSkeleton();

				RefreshLevels();
				RefreshDungeons();
				RefreshScripts(true, true);
			}
			UpdateButtons();
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		// Create the base node folders.
		private void CreateTreeSkeleton() {
			World world = editorControl.World;

			if (worldNode != null) {
				worldNode.Items.Clear();
			}
			else {
				worldNode           = new WorldTreeViewItem(world);
				worldNode.Header    = editorControl.World.ID;
			}

			if (levelsNode != null) {
				levelsNode.Items.Clear();
			}
			else {
				levelsNode          = new ImageTreeViewItem(EditorImages.LevelGroup, "Levels", true);
				levelsNode.Tag      = "levels";
			}
			if (dungeonsNode != null) {
				dungeonsNode.Items.Clear();
			}
			else {
				dungeonsNode        = new ImageTreeViewItem(EditorImages.DungeonGroup, "Dungeons", true);
				dungeonsNode.Tag    = "dungeons";
			}
			if (scriptsNode != null) {
				scriptsNode.Items.Clear();
			}
			else {
				scriptsNode         = new ImageTreeViewItem(EditorImages.ScriptGroup, "Scripts", true);
				scriptsNode.Tag     = "scripts";
			}


			treeView.Items.Clear();
			treeView.Items.Add(worldNode);
			worldNode.Items.Add(levelsNode);
			worldNode.Items.Add(dungeonsNode);
			worldNode.Items.Add(scriptsNode);

			worldNode.ContextMenu = contextMenuWorld;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public EditorControl EditorControl {
			get { return editorControl; }
			set { editorControl = value; }
		}

		public bool IsMouseOverTreeView {
			get { return treeView.IsMouseOver; }
		}

		private void OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			UpdateButtons();
		}
		
		private void OnTreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
			if (treeViewItem is IWorldTreeViewItem)
				((IWorldTreeViewItem)treeViewItem).Open(editorControl);
		}
		private void OnTreeViewMouseRightButtonDown(object sender, MouseEventArgs e) {
			TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

			if (treeViewItem != null) {
				treeViewItem.Focus();
				e.Handled = true;
			}
		}

		private static TreeViewItem VisualUpwardSearch(DependencyObject source) {
			while (source != null && !(source is TreeViewItem))
				source = VisualTreeHelper.GetParent(source);

			return source as TreeViewItem;
		}
		private void OnTreeViewMouseDown(object sender, MouseButtonEventArgs e) {
			TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

			//treeViewItem.IsSelected = true;
		}
		private void OnGotoOwner(object sender, RoutedEventArgs e) {
			IEventObject eventObject = (treeView.SelectedItem as EventTreeViewItem).Event.Events.EventObject;
			if (eventObject is BaseTileDataInstance)
				editorControl.GotoTile(eventObject as BaseTileDataInstance);
			else if (eventObject is Room)
				editorControl.GotoRoom(eventObject as Room);
			else if (eventObject is Level)
				editorControl.OpenLevel(eventObject as Level);
			else
				editorControl.OpenProperties(eventObject);
		}

		private void OnEdit(object sender, RoutedEventArgs e) {
			(treeView.SelectedItem as IWorldTreeViewItem).Open(editorControl);
		}

		private void OnRename(object sender, RoutedEventArgs e) {
			IWorldTreeViewItem node = treeView.SelectedItem as IWorldTreeViewItem;
			string newID = RenameWindow.Show(Window.GetWindow(this),
				editorControl.World, node.IDObject);

			if (newID != null) {
				ActionRenameID action = new ActionRenameID(node.IDObject, newID);
				editorControl.PushAction(action, ActionExecution.Execute);
				/*node.Rename(editorControl, newID);
				editorControl.EditorWindow.UpdatePropertyPreview(editorControl.PropertyGrid.PropertyObject);
				editorControl.IsModified = true;*/
			}
		}

		private void OnAdd(object sender, RoutedEventArgs e) {
			if (treeView.SelectedItem is LevelTreeViewItem) {
				EditorCommands.AddNewLevel.Execute(null, null);
			}
			else if (treeView.SelectedItem is DungeonTreeViewItem) {
				EditorCommands.AddNewDungeon.Execute(null, null);
			}
			else if (treeView.SelectedItem is ScriptTreeViewItem) {
				EditorCommands.AddNewScript.Execute(null, null);
			}
		}

		private void OnDuplicate(object sender, RoutedEventArgs e) {
			(treeView.SelectedItem as IWorldTreeViewItem).Duplicate(editorControl);
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
			int selectedIndex = GetItemIndex(parent, treeView.SelectedItem);
			EditorAction action = null;
			if (treeView.SelectedItem is LevelTreeViewItem) {
				action = new ActionMoveLevel((treeView.SelectedItem as LevelTreeViewItem).Level, direction);
			}
			else if (treeView.SelectedItem is DungeonTreeViewItem) {
				action = new ActionMoveDungeon((treeView.SelectedItem as DungeonTreeViewItem).Dungeon, direction);
			}
			if (action != null) {
				//editorControl.RefreshWorldTreeView();
				editorControl.PushAction(action, ActionExecution.Execute);
				SelectItem(parent, selectedIndex + direction);
			}
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
			else if (treeView.SelectedItem is IWorldTreeViewItem)
			{
				buttonEdit.IsEnabled = true;
				buttonDuplicate.IsEnabled = true;
				buttonDelete.IsEnabled = true;

				if (!(treeView.SelectedItem is EventTreeViewItem))
					buttonRename.IsEnabled = true;

				if (!(treeView.SelectedItem is ScriptTreeViewItem)) {
					TreeViewItem parent = GetParentNode(treeView.SelectedItem);
					int selectedIndex = GetItemIndex(parent, treeView.SelectedItem);
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
		private void OnTreeViewRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e) {
			e.Handled = true;
		}

		public void FocusOnTreeView() {
			treeView.Focus();
		}
	}
}
