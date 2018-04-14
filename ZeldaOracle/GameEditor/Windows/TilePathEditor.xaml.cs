using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;
using ZeldaWpf.Windows;
using ZeldaEditor.Control;
using ZeldaEditor.Controls;
using ZeldaEditor.WinForms;

namespace ZeldaEditor.Windows {
	/// <summary>
	/// Interaction logic for TilePathEditor.xaml
	/// </summary>
	public partial class TilePathEditor : Window {
		
		private ObservableCollection<TilePathCommandItem> commands;
		private bool repeats;
		private bool modified;
		private string savedPath;
		private Properties properties;
		private string propertyName;
		private TilePathCommandItem dragItem;

		private TilePathDisplay pathDisplay;


		private TilePathEditor(EditorControl editorControl,
			Properties properties, string propertyName)
		{
			InitializeComponent();
			commands = new ObservableCollection<TilePathCommandItem>();
			itemsControl.ItemsSource = commands;
			modified = false;
			this.properties = properties;
			this.propertyName = propertyName;
			savedPath = properties.Get<string>(propertyName, "");


			TileDataInstance tile =
				(TileDataInstance) properties.PropertyObject;
			pathDisplay = new TilePathDisplay(tile.Room, tile);
			pathDisplay.EditorControl = editorControl;
			host.Width = pathDisplay.Width;
			host.Height = pathDisplay.Height;
			host.Child = pathDisplay;

			ParsePath(savedPath);

			// Update view settings
			checkBoxAllPaths.IsChecked = pathDisplay.RunAllPaths;
			checkBoxHighlightPath.IsChecked = pathDisplay.HighlightPath;
			checkBoxFadeTiles.IsChecked = pathDisplay.FadeTiles;
		}

		private TilePathEditor(EditorControl editorControl, Room room) {
			InitializeComponent();
			commands = new ObservableCollection<TilePathCommandItem>();
			modified = false;
			properties = null;
			propertyName = "";
			savedPath = "";

			// Hide editor
			borderEditor.Visibility = Visibility.Collapsed;

			// Hide view setting
			stackPanelView.Visibility = Visibility.Collapsed;

			// Remove excess window height
			SizeToContent = SizeToContent.WidthAndHeight;

			pathDisplay = new TilePathDisplay(room, null);
			pathDisplay.EditorControl = editorControl;
			host.Width = pathDisplay.Width;
			host.Height = pathDisplay.Height;
			host.Child = pathDisplay;

			// Update view settings
			checkBoxFadeTiles.IsChecked = pathDisplay.FadeTiles;

			// Change window title to viewer
			Title = "Tile Path Viewer";
		}


		public TilePathCommandItem MakeItem() {
			TilePathCommandItem item = new TilePathCommandItem();

			item.Insert += OnInsertCommand;
			item.Remove += OnRemoveCommand;
			item.Modified += OnCommandModified;
			item.DragStarted += OnCommandDragStarted;
			item.DragCompleted += OnCommandDragCompleted;
			item.DragDelta += OnCommandDragDelta;

			return item;
		}

		private void OnCommandDragDelta(object sender, DragDeltaEventArgs e) {
			int currentIndex = IndexOfItem(sender);
			double y = Mouse.GetPosition(itemsControl).Y;
			double cmdHeight = commands[0].ActualHeight;
			int newIndex = GMath.Clamp((int) (y / cmdHeight), 0, commands.Count - 1);
			if (newIndex != currentIndex) {
				commands.Move(currentIndex, newIndex);
			}
		}

		private void OnCommandDragCompleted(object sender, DragCompletedEventArgs e) {
			dragItem.Margin = new Thickness(0);
			OnCommandModified();
		}

		private void OnCommandDragStarted(object sender, DragStartedEventArgs e) {
			dragItem = (TilePathCommandItem) sender;
			dragItem.Margin = new Thickness(5, 0, -5, 0);
		}

		private void OnAddCommand(object sender, RoutedEventArgs e) {
			var item = MakeItem();
			commands.Add(item);
			OnCommandModified();
		}

		private void OnInsertCommand(object sender, RoutedEventArgs e) {
			int index = IndexOfItem(sender);
			var item = MakeItem();
			commands.Insert(index, item);
			OnCommandModified();
		}

		private void OnRemoveCommand(object sender, RoutedEventArgs e) {
			int index = IndexOfItem(sender);
			commands.RemoveAt(index);
			OnCommandModified();
		}

		private int IndexOfItem(object item) {
			return commands.IndexOf((TilePathCommandItem) item);
		}

		private void OnCommandModified(object sender = null, RoutedEventArgs e = null) {
			if (!modified) {
				modified = true;
				CommandManager.InvalidateRequerySuggested();
			}
			// Update the path for the path display
			properties.Set(propertyName, PathToString());
			pathDisplay.Restart();
		}

		private void OnSave(object sender, ExecutedRoutedEventArgs e) {
			modified = false;
			savedPath = PathToString();
		}

		private void CanSave(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = modified = true;
		}

		private void OnFinished(object sender, RoutedEventArgs e) {
			savedPath = PathToString();
			DialogResult = true;
		}

		private void OnDeleteTilePath(object sender, RoutedEventArgs e) {
			var result = MessageBoxResult.Yes;
			if (commands.Any()) {
				result = TriggerMessageBox.Show(this, MessageIcon.Warning,
					"Are you sure you want to delete the tile path?",
					"Delete Path", MessageBoxButton.YesNo);
			}
			if (result == MessageBoxResult.Yes) {
				savedPath = "";
				DialogResult = true;
			}
		}

		private void OnRepeatsChanged(object sender, RoutedEventArgs e) {
			repeats = buttonRepeats.IsChecked.Value;
			OnCommandModified();
		}

		private void OnRunAllPathsChanged(object sender, RoutedEventArgs e) {
			pathDisplay.RunAllPaths = checkBoxAllPaths.IsChecked.Value;
		}

		private void OnHighlightPathChanged(object sender, RoutedEventArgs e) {
			pathDisplay.HighlightPath = checkBoxHighlightPath.IsChecked.Value;
		}

		private void ParsePath(string str) {
			commands.Clear();
			repeats = false;

			string[] strCommands = str.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
			if (strCommands.Length == 0)
				return;

			for (int i = 0; i < strCommands.Length; i++) {
				string[] tokens = strCommands[i].Split(new char[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
				if (tokens.Length == 0)
					continue;

				TilePathCommandItem item = null;

				string cmd = tokens[0];
				string param = (tokens.Length > 1 ? tokens[1] : "");

				Direction direction;
				if (Direction.TryParse(cmd, false, out direction)) {
					int distance;
					if (int.TryParse(param, out distance)) {
						item = MakeItem();
						item.CommandType = (TilePathCommandTypes) (int) direction;
						item.IntParam = distance;
					}
				}
				else if (cmd == TilePath.SpeedCmd) {
					float speed;
					if (float.TryParse(param, out speed)) {
						item = MakeItem();
						item.CommandType = TilePathCommandTypes.Speed;
						item.FloatParam = speed;
					}
				}
				else if (cmd == TilePath.PauseCmd) {
					int time;
					if (int.TryParse(param, out time)) {
						item = MakeItem();
						item.CommandType = TilePathCommandTypes.Pause;
						item.IntParam = time;
					}
				}
				else if (cmd == TilePath.RepeatCmd)
					repeats = true;

				if (item != null)
					commands.Add(item);
			}

			buttonRepeats.IsChecked = repeats;
			scrollViewer.ScrollToBottom();
		}

		private string PathToString() {
			string str = "";

			foreach (var command in commands) {
				str += command.CommandType.ToString().ToLower() + " ";
				if (command.CommandType == TilePathCommandTypes.Speed)
					str += command.FloatParam;
				else
					str += command.IntParam;
				str += "; ";
			}

			if (repeats)
				str += TilePath.RepeatCmd + ";";
			else
				str = str.TrimEnd();

			return str;
		}


		public static string ShowEditor(Window owner, EditorControl editorControl,
			Properties properties, string propertyName)
		{
			TilePathEditor editor = new TilePathEditor(editorControl,
				properties, propertyName);
			string oldPath = properties.Get<string>(propertyName, "");
			editor.Owner = owner;
			editor.ShowDialog();
			properties.Set(propertyName, oldPath);
			return editor.savedPath;
		}

		public static void ShowViewer(Window owner, EditorControl editorControl, Room room) {
			TilePathEditor editor = new TilePathEditor(editorControl, room);
			editor.Owner = owner;
			editor.ShowDialog();
		}

		private void OnRestart(object sender, RoutedEventArgs e) {
			pathDisplay.Restart();
		}

		private void OnFadeTilesChanged(object sender, RoutedEventArgs e) {
			pathDisplay.FadeTiles = checkBoxFadeTiles.IsChecked.Value;
		}
	}
}
