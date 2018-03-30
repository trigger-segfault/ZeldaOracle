using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaOracle.Game.Control.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaEditor.Control;
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
	
	/// <summary>Commands used by the WorldTreeView.</summary>
	public static class WorldTreeViewCommands {
		public static readonly RoutedUICommand Rename = new RoutedUICommand(
			"RenameLevel", "Rename Level", typeof(EditorCommands));

		public static readonly RoutedUICommand Delete = new RoutedUICommand(
			"DeleteLevel", "Delete Level", typeof(EditorCommands));

		public static readonly RoutedUICommand Duplicate = new RoutedUICommand(
			"DuplicateLevel", "Duplicate Level", typeof(EditorCommands));

		public static readonly RoutedUICommand EditProperties = new RoutedUICommand(
			"EditProperties", "Properties", typeof(EditorCommands));

		public static readonly RoutedUICommand CreateLevel = new RoutedUICommand(
			"CreateLevel", "Create Level", typeof(EditorCommands));

		public static readonly RoutedUICommand ResizeLevel = new RoutedUICommand(
			"ResizeLevel", "Resize Level", typeof(EditorCommands));

		public static readonly RoutedUICommand ShiftLevel = new RoutedUICommand(
			"ShiftLevel", "Shift Rooms", typeof(EditorCommands));

		public static readonly RoutedUICommand Edit = new RoutedUICommand(
			"Edit", "Edit", typeof(EditorCommands));

		public static readonly RoutedUICommand MoveDown = new RoutedUICommand(
			"MoveDown", "Move Down", typeof(EditorCommands));

		public static readonly RoutedUICommand MoveUp = new RoutedUICommand(
			"MoveUp", "Move Up", typeof(EditorCommands));

		public static readonly RoutedUICommand GoToOwner = new RoutedUICommand(
			"GoToOwner", "Go to Owner", typeof(EditorCommands));
	}

	/// <summary>
	/// Interaction logic for WorldTreeView.xaml
	/// </summary>
	public partial class WorldTreeView : UserControl {		

		private EditorControl editorControl;
		private ImageTreeViewItem worldNode;
		private ImageTreeViewItem levelsNode;
		private ImageTreeViewItem areasNode;
		private ImageTreeViewItem scriptsNode;
		private TreeViewItem internalScriptsNode;
		private TreeViewItem customScriptWorldNode;
		private TreeViewItem customScriptAreaNode;
		private Dictionary<string, TreeViewItem> customScriptLevelNodes;

		private ContextMenu contextMenuWorld;
		private ContextMenu contextMenuLevel;
		private ContextMenu contextMenuArea;
		private ContextMenu contextMenuScript;
		private ContextMenu contextMenuEvent;

		private WpfFocusMessageFilter messageFilter;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public WorldTreeView() {
			InitializeComponent();
			editorControl = null;
			worldNode = null;

			// Initialize context menus
			InitWorldContextMenu();
			InitLevelContextMenu();
			InitAreaContextMenu();
			InitScriptContextMenu();
			InitEventContextMenu();

			customScriptLevelNodes = new Dictionary<string, TreeViewItem>();

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

			messageFilter = new WpfFocusMessageFilter(treeView);
			messageFilter.AddFilter();
		}

		private void InitWorldContextMenu() {
			ImageMenuItem menuItem;
			contextMenuWorld = new ContextMenu();
			menuItem = new ImageMenuItem(EditorImages.Rename, "Rename");
			menuItem.Click += OnRename;
			contextMenuWorld.Items.Add(menuItem);
			menuItem = new ImageMenuItem(EditorImages.Edit, "Properties...");
			menuItem.Click += OnEditProperties;
			contextMenuWorld.Items.Add(menuItem);
		}

		private void InitLevelContextMenu() {
			ImageMenuItem menuItem;

			contextMenuLevel = new ContextMenu();

			menuItem = new ImageMenuItem(EditorImages.LevelAdd, "Create Level");
			menuItem.Command = WorldTreeViewCommands.CreateLevel;
			contextMenuLevel.Items.Add(menuItem);
			menuItem = new ImageMenuItem(EditorImages.LevelDuplicate, "Duplicate");
			menuItem.Command = WorldTreeViewCommands.Duplicate;
			contextMenuLevel.Items.Add(menuItem);

			contextMenuLevel.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.LevelDelete, "Delete");
			menuItem.Command = WorldTreeViewCommands.Delete;
			contextMenuLevel.Items.Add(menuItem);

			contextMenuLevel.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Rename, "Rename");
			menuItem.Command = WorldTreeViewCommands.Rename;
			contextMenuLevel.Items.Add(menuItem);

			contextMenuLevel.Items.Add(new Separator());
			
			menuItem = new ImageMenuItem(EditorImages.LevelResize, "Resize");
			menuItem.Command = WorldTreeViewCommands.ResizeLevel;
			contextMenuLevel.Items.Add(menuItem);
			menuItem = new ImageMenuItem(EditorImages.LevelShift, "Shift");
			menuItem.Command = WorldTreeViewCommands.ShiftLevel;
			contextMenuLevel.Items.Add(menuItem);

			contextMenuLevel.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Property, "Properties");
			menuItem.Command = WorldTreeViewCommands.EditProperties;
			contextMenuLevel.Items.Add(menuItem);
			menuItem = new ImageMenuItem(EditorImages.Edit, "Edit");
			menuItem.Command = WorldTreeViewCommands.EditProperties;
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

		private void InitAreaContextMenu() {
			ImageMenuItem menuItem;
			
			contextMenuArea = new ContextMenu();

			menuItem = new ImageMenuItem(EditorImages.AreaAdd, "Create Area");
			menuItem.Click += OnCreateArea;
			contextMenuArea.Items.Add(menuItem);
			menuItem = new ImageMenuItem(EditorImages.AreaDuplicate, "Duplicate");
			menuItem.Command = WorldTreeViewCommands.Duplicate;
			contextMenuArea.Items.Add(menuItem);

			contextMenuArea.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.AreaDelete, "Delete");
			menuItem.Command = WorldTreeViewCommands.Delete;
			contextMenuArea.Items.Add(menuItem);

			contextMenuArea.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Rename, "Rename");
			menuItem.Command = WorldTreeViewCommands.Rename;
			contextMenuArea.Items.Add(menuItem);

			contextMenuArea.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Property, "Properties");
			menuItem.Command = WorldTreeViewCommands.EditProperties;
			contextMenuArea.Items.Add(menuItem);
		}


		private void InitScriptContextMenu() {
			ImageMenuItem menuItem;
			
			contextMenuScript = new ContextMenu();

			menuItem = new ImageMenuItem(EditorImages.ScriptAdd, "Create Script");
			menuItem.Click += OnCreateScript;
			contextMenuScript.Items.Add(menuItem);

			menuItem = new ImageMenuItem(EditorImages.ScriptDuplicate, "Duplicate");
			menuItem.Command = WorldTreeViewCommands.Duplicate;
			contextMenuScript.Items.Add(menuItem);

			contextMenuScript.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.ScriptDelete, "Delete");
			menuItem.Command = WorldTreeViewCommands.Delete;
			contextMenuScript.Items.Add(menuItem);

			contextMenuScript.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Rename, "Rename");
			menuItem.Command = WorldTreeViewCommands.Rename;
			contextMenuScript.Items.Add(menuItem);

			contextMenuScript.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Edit, "Edit");
			menuItem.Command = WorldTreeViewCommands.Edit;
			contextMenuScript.Items.Add(menuItem);
		}

		private void InitEventContextMenu() {
			ImageMenuItem menuItem;
			contextMenuEvent = new ContextMenu();
			menuItem = new ImageMenuItem(EditorImages.GotoOwner, "Go to Owner");
			menuItem.Command = WorldTreeViewCommands.GoToOwner;
			contextMenuEvent.Items.Add(menuItem);

			contextMenuEvent.Items.Add(new Separator());

			menuItem = new ImageMenuItem(EditorImages.Edit, "Edit");
			menuItem.Command = WorldTreeViewCommands.Edit;
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
				TreeViewItem newCustomScriptAreaNode = new FolderTreeViewItem("Areas", IsNodeExpanded(customScriptAreaNode));
				Dictionary<string, TreeViewItem> newCustomScriptLevelNodes = new Dictionary<string, TreeViewItem>();

				foreach (Event evnt in editorControl.EventCache) {
					IEventObject eventObject = evnt.Events.EventObject;
					string level = null;
					string area = null;
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
					else if (eventObject is Area) {
						area = ((Area)eventObject).ID;
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
					else if (area != null) {
						newCustomScriptAreaNode.Items.Add(eventNode);
					}
					else {
						newCustomScriptWorldNode.Items.Add(eventNode);
					}
				}
				customScriptWorldNode = newCustomScriptWorldNode;
				customScriptAreaNode = newCustomScriptAreaNode;
				customScriptLevelNodes = newCustomScriptLevelNodes;

				if (customScriptWorldNode.Items.Count > 0) {
					internalScriptsNode.Items.Add(customScriptWorldNode);
				}
				if (customScriptAreaNode.Items.Count > 0) {
					internalScriptsNode.Items.Add(customScriptAreaNode);
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

		public void RefreshAreas() {
			if (treeView.Items.Count == 0) {
				RefreshTree();
				return;
			}
			areasNode.Items.Clear();

			foreach (Area area in editorControl.World.GetAreas()) {
				AreaTreeViewItem areaNode = new AreaTreeViewItem(area);
				areaNode.ContextMenu = contextMenuArea;
				areasNode.Items.Add(areaNode);
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
				RefreshAreas();
				RefreshScripts(true, true);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Create the base node folders.</summary>
		private void CreateTreeSkeleton() {
			World world = editorControl.World;

			if (worldNode != null) {
				worldNode.Items.Clear();
			}
			else {
				worldNode = new WorldTreeViewItem(world);
				worldNode.Header = editorControl.World.ID;
				worldNode.ContextMenu = contextMenuWorld;
			}

			if (levelsNode != null) {
				levelsNode.Items.Clear();
			}
			else {
				levelsNode = new ImageTreeViewItem(EditorImages.LevelGroup, "Levels", true);
				levelsNode.Tag = "levels";
				levelsNode.ContextMenu = contextMenuLevel;
			}
			if (areasNode != null) {
				areasNode.Items.Clear();
			}
			else {
				areasNode = new ImageTreeViewItem(EditorImages.AreaGroup, "Areas", true);
				areasNode.Tag = "areas";
				areasNode.ContextMenu = contextMenuArea;
			}
			if (scriptsNode != null) {
				scriptsNode.Items.Clear();
			}
			else {
				scriptsNode = new ImageTreeViewItem(EditorImages.ScriptGroup, "Scripts", true);
				scriptsNode.Tag = "scripts";
				scriptsNode.ContextMenu = contextMenuScript;
			}

			treeView.Items.Clear();
			treeView.Items.Add(worldNode);
			worldNode.Items.Add(levelsNode);
			worldNode.Items.Add(areasNode);
			worldNode.Items.Add(scriptsNode);
		}

		private static TreeViewItem VisualUpwardSearch(DependencyObject source) {
			while (source != null && !(source is TreeViewItem))
				source = VisualTreeHelper.GetParent(source);
			return source as TreeViewItem;
		}

		private void MoveSelected(int direction) {
			TreeViewItem parent = GetParentNode(treeView.SelectedItem);
			int selectedIndex = GetItemIndex(parent, treeView.SelectedItem);
			EditorAction action = null;
			if (treeView.SelectedItem is LevelTreeViewItem) {
				action = new ActionMoveLevel((treeView.SelectedItem as LevelTreeViewItem).Level, direction);
			}
			else if (treeView.SelectedItem is AreaTreeViewItem) {
				action = new ActionMoveArea((treeView.SelectedItem as AreaTreeViewItem).Area, direction);
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
		

		//-----------------------------------------------------------------------------
		// Command Can Execute
		//-----------------------------------------------------------------------------

		private void IsItemSelected(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = (treeView != null &&
				(treeView.SelectedItem is LevelTreeViewItem ||
				treeView.SelectedItem is AreaTreeViewItem ||
				treeView.SelectedItem is ScriptTreeViewItem ||
				treeView.SelectedItem is WorldTreeViewItem));
		}

		
		//-----------------------------------------------------------------------------
		// Command Execute
		//-----------------------------------------------------------------------------

		private void OnEditProperties(object sender, RoutedEventArgs e) {
			(treeView.SelectedItem as IWorldTreeViewItem).OpenProperties(editorControl);
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
			}
		}

		private void OnCreateLevel(object sender, RoutedEventArgs e) {
			EditorCommands.AddNewLevel.Execute(null, null);
		}

		private void OnCreateArea(object sender, RoutedEventArgs e) {
			EditorCommands.AddNewArea.Execute(null, null);
		}

		private void OnCreateScript(object sender, RoutedEventArgs e) {
			EditorCommands.AddNewScript.Execute(null, null);
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
		
		/// <summary>Select the object which is the owner of an event.</summary>
		private void OnGoToOwner(object sender, RoutedEventArgs e) {
			IEventObject eventObject =
				(treeView.SelectedItem as EventTreeViewItem).Event.Events.EventObject;
			if (eventObject is BaseTileDataInstance)
				editorControl.GotoTile(eventObject as BaseTileDataInstance);
			else if (eventObject is Room)
				editorControl.GotoRoom(eventObject as Room);
			else if (eventObject is Level)
				editorControl.OpenLevel(eventObject as Level);
			else
				editorControl.OpenProperties(eventObject);
		}
		

		//-----------------------------------------------------------------------------
		// UI Event Callbacks
		//-----------------------------------------------------------------------------

		private void OnSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
		}

		private void OnTreeViewMouseEnter(object sender, MouseEventArgs e) {
			treeView.Focus();
		}

		private void OnTreeViewRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e) {
			e.Handled = true;
		}
		
		private void OnTreeViewMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
			if (treeViewItem is IWorldTreeViewItem)
				((IWorldTreeViewItem) treeViewItem).Open(editorControl);
		}

		private void OnTreeViewMouseDown(object sender, MouseButtonEventArgs e) {
		}

		private void OnTreeViewMouseRightButtonDown(object sender, MouseEventArgs e) {
			TreeViewItem treeViewItem = VisualUpwardSearch(
				e.OriginalSource as DependencyObject);
			if (treeViewItem != null) {
				treeViewItem.Focus();
				e.Handled = true;
			}
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
