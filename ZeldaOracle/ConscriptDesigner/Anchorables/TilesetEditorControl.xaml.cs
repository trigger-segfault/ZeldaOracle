using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConscriptDesigner.Anchorables.TilesetEditorTools;
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;
using ConscriptDesigner.Util;
using ConscriptDesigner.Windows;
using ConscriptDesigner.WinForms;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Tiles;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for TilesetEditorControl.xaml
	/// </summary>
	public partial class TilesetEditorControl : UserControl {

		private TilesetEditorDisplay display;

		private List<KeyValuePair<string, Tileset>> tilesets;
		private Tileset tileset;
		private string tilesetName;

		private bool suppressEvents;

		private bool modified;

		private ToggleButton[] toolButtons;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------
		/// <summary>Constructs the tileset browser control.</summary>
		public TilesetEditorControl() {
			this.suppressEvents = true;
			InitializeComponent();

			this.display = new TilesetEditorDisplay();
			this.display.HoverChanged += OnHoverChanged;
			this.display.Modified += OnModified;
			this.display.ToolChanged += OnDisplayToolChanged;
			this.display.ScaleChanged += OnDisplayScaleChanged;
			this.host.Child = this.display;
			this.suppressEvents = false;
			this.tilesets = new List<KeyValuePair<string, Tileset>>();
			this.tileset = null;
			this.tilesetName = "";

			toolButtons = new ToggleButton[] {
				buttonToolPlace,
				buttonToolSelection,
				buttonToolEyedrop
			};
			buttonToolPlace.Tag = display.ToolPlace;
			buttonToolSelection.Tag = display.ToolSelection;
			buttonToolEyedrop.Tag = display.ToolEyedrop;

			if (display.Overwrite) {
				buttonMergeOverwrite.Source = DesignerImages.Overwrite;
				buttonMergeOverwrite.ToolTip = "Overwrite";
			}
			else {
				buttonMergeOverwrite.Source = DesignerImages.Merge;
				buttonMergeOverwrite.ToolTip = "Merge";
			}

			DesignerControl.ResourcesLoaded += OnResourcesLoaded;
			DesignerControl.ResourcesUnloaded += OnResourcesUnloaded;
			DesignerControl.PreviewInvalidated += OnPreviewInvalidated;
			DesignerControl.PreviewScaleChanged += OnPreviewScaleChanged;

			OnHoverChanged();

			UpdateCurrentTool();

			this.suppressEvents = false;
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public event EventHandler ModifiedChanged;

		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		public void Dispose() {
			DesignerControl.ResourcesLoaded -= OnResourcesLoaded;
			DesignerControl.ResourcesUnloaded -= OnResourcesUnloaded;
			DesignerControl.PreviewInvalidated -= OnPreviewInvalidated;
			DesignerControl.PreviewScaleChanged -= OnPreviewScaleChanged;
			display.Dispose();
		}

		public void Reload() {
			suppressEvents = true;
			modified = false;
			if (ModifiedChanged != null)
				ModifiedChanged(this, EventArgs.Empty);
			buttonCancelChanges.IsEnabled = false;

			tilesets.Clear();

			foreach (var pair in ZeldaResources.GetResourceDictionary<Tileset>()) {
				tilesets.Add(new KeyValuePair<string, Tileset>(pair.Key, pair.Value));
			}
			tilesets.Sort((a, b) => AlphanumComparator.Compare(a.Key, b.Key, true));

			comboBoxTilesets.Items.Clear();
			foreach (var pair in tilesets) {
				ComboBoxItem item = new ComboBoxItem();
				item.Content = pair.Key;
				item.Tag = pair.Key;
				comboBoxTilesets.Items.Add(item);
			}
			tileset = ZeldaResources.GetResource<Tileset>(tilesetName);
			if (tilesets.Any() && tileset == null) {
				tilesetName = tilesets[0].Key;
				tileset = tilesets[0].Value;
			}

			comboBoxTilesets.SelectedIndex = tilesets.IndexOf(
				new KeyValuePair<string, Tileset>(tilesetName, tileset));
			if (comboBoxTilesets.SelectedIndex != -1)
				buttonResize.IsEnabled = true;

			UpdateTileset();
			OnHoverChanged();

			comboBoxScales.Items.Clear();
			for (int i = 1; i <= 3; i++) {
				ComboBoxItem item = new ComboBoxItem();
				item.Content = "x" + i + " Scale";
				item.Tag = i;
				comboBoxScales.Items.Add(item);
			}
			comboBoxScales.SelectedIndex = 0;

			suppressEvents = false;
		}

		public void Unload() {
			tileset = null;
			buttonResize.IsEnabled = false;
			display.Unload();
		}

		public void SelectAll() {
			display.SelectAll();
		}


		//-----------------------------------------------------------------------------
		// Tile Setup
		//-----------------------------------------------------------------------------

		private void UpdateTileset() {
			buttonUsePreviewSprites.IsEnabled = (tileset != null);
			if (tileset != null) {
				tileset = new Tileset(tileset);
				buttonUsePreviewSprites.IsChecked = tileset.UsePreviewSprites;
			}
			else {
				buttonUsePreviewSprites.IsChecked = false;
			}
			if (tileset != null)
				display.UpdateList(tileset);
			else
				display.Unload();
		}

		public bool Save(bool silentFail) {
			if (tileset == null || !modified) return true;

			display.CurrentTool.Cancel();
			ContentScript script = DesignerControl.Project.Get(tileset.ConscriptPath) as ContentScript;

			if (script == null) {
				if (silentFail)
					return false;
				TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
					"The script file this tileset originates from no longer exists. Cannot save tileset!",
					"Save Failed");
			}
			else {
				StringBuilder text = new StringBuilder();
				text.AppendLine("TILESET \"" + tileset.ID + "\", " + tileset.Dimensions + ", " +
					tileset.UsePreviewSprites.ToString().ToLower() + ";");
				bool lastRowEmpty = false;
				for (int y = 0; y < tileset.Height; y++) {
					if (!lastRowEmpty)
						text.AppendLine();
					lastRowEmpty = true;
					for (int x = 0; x < tileset.Width; x++) {
						Point2I point = new Point2I(x, y);
						BaseTileData tileData = tileset.GetTileDataAtOrigin(point);
						if (tileData != null) {
							text.AppendLine("SETTILE " + point + ", \"" + tileData.Name + "\";");
							lastRowEmpty = false;
						}
					}
				}
				text.AppendLine();
				text.AppendLine("END;");
				try {
					File.WriteAllText(script.FilePath, text.ToString());
					modified = false;
					buttonCancelChanges.IsEnabled = false;
					if (ModifiedChanged != null)
						ModifiedChanged(this, EventArgs.Empty);
					if (script.IsOpen)
						script.Reload(false);
					else
						script.UnloadText();
					ZeldaResources.SetResource<Tileset>(tileset.ID, tileset);
					int index = tilesets.FindIndex((a) => a.Key == tileset.ID);
					tilesets[index] = new KeyValuePair<string, Tileset>(tileset.ID, tileset);
					if (DesignerControl.MainWindow.TileBrowser != null)
						DesignerControl.MainWindow.TileBrowser.Reload();
					return true;
				}
				catch (Exception ex) {
					if (silentFail)
						return false;
					DesignerControl.ShowExceptionMessage(ex, "save", "tileset " + tileset.ID);
				}
			}
			return false;
		}

		public bool RequestSave(bool silentFail) {
			if (tileset == null || !modified) return true;

			var result = TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
						"Would you like to save changes to current the tileset in the tileset editor?",
						"Save Changes", MessageBoxButton.YesNoCancel);
			if (result == MessageBoxResult.Yes) {
				if (!Save(false))
					return false;
			}
			else if (result == MessageBoxResult.Cancel) {
				comboBoxTilesets.SelectedIndex = tilesets.IndexOf(
					new KeyValuePair<string, Tileset>(tilesetName, tileset));
			}
			return true;
		}

		public void Paste() {
			display.CurrentTool = display.ToolSelection;
			display.ToolSelection.Paste();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnResourcesLoaded(object sender = null, EventArgs e = null) {
			Reload();
		}

		private void OnResourcesUnloaded(object sender = null, EventArgs e = null) {
			Unload();
		}

		private void OnPreviewInvalidated(object sender = null, EventArgs e = null) {
			display.Invalidate();
		}

		private void OnPreviewScaleChanged(object sender = null, EventArgs e = null) {
			display.UpdateScale();
		}

		private void OnHoverChanged(object sender = null, EventArgs e = null) {
			BaseTileData hoverTileData = display.HoverTileData;
			if (hoverTileData == null) {
				textBlockTileName.Text = "";
				statusTileInfo.Content = "";
			}
			else {
				textBlockTileName.Text = hoverTileData.Name;
				if (hoverTileData.Type == null) {
					if (hoverTileData is TileData)
						statusTileInfo.Content = "Type: Tile";
					else
						statusTileInfo.Content = "Type: EventTile";
				}
				else {
					statusTileInfo.Content = "Type: " + hoverTileData.Type.Name;
				}
			}

			Point2I hoverPoint = display.HoverPoint;
			if (hoverPoint == -Point2I.One)
				statusHoverIndex.Content = "(?, ?)";
			else
				statusHoverIndex.Content = hoverPoint.ToString();
		}

		private void OnModified(object sender, EventArgs e) {
			modified = true;
			if (ModifiedChanged != null)
				ModifiedChanged(this, EventArgs.Empty);
			//DesignerControl.InvalidatePreview();
			OnHoverChanged();
			buttonCancelChanges.IsEnabled = true;
		}

		private void OnTilesetChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			if (comboBoxTilesets.SelectedIndex != -1) {
				if (modified) {
					var result = TriggerMessageBox.Show(DesignerControl.MainWindow, MessageIcon.Warning,
						"Would you like to save changes before switching tilesets?",
						"Save Changes", MessageBoxButton.YesNoCancel);
					if (result == MessageBoxResult.Yes) {
						if (!Save(false))
							return;
					}
					else if (result == MessageBoxResult.Cancel) {
						comboBoxTilesets.SelectedIndex = tilesets.IndexOf(
							new KeyValuePair<string, Tileset>(tilesetName, tileset));
					}
					else {
						modified = false;
						buttonCancelChanges.IsEnabled = false;
						if (ModifiedChanged != null)
							ModifiedChanged(this, EventArgs.Empty);
					}
				}
				tilesetName = (string) ((ComboBoxItem) comboBoxTilesets.SelectedItem).Tag;
				tileset = ZeldaResources.GetResource<Tileset>(tilesetName);
				UpdateTileset();
			}
		}

		private void OnScaleChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			display.UpdateScale((int) ((FrameworkElement) comboBoxScales.SelectedItem).Tag);
		}

		private void OnDisplayScaleChanged(object sender, EventArgs e) {
			suppressEvents = true;
			comboBoxScales.SelectedIndex = display.Scale - 1;
			suppressEvents = false;
		}

		private void OnDisplayToolChanged(object sender, EventArgs e) {
			UpdateCurrentTool();
		}

		private void UpdateCurrentTool() {
			for (int i = 0; i < toolButtons.Length; i++)
				toolButtons[i].IsChecked = (toolButtons[i].Tag == display.CurrentTool);
		}

		private void OnPreviewKeyDown(object sender, KeyEventArgs e) {
			if (!(FocusManager.GetFocusedElement(this) is TextBox)) {
				if (Keyboard.Modifiers == ModifierKeys.None) {
					foreach (TilesetEditorTool tool in display.Tools) {
						if (e.Key == tool.HotKey) {
							display.CurrentTool = tool;
							return;
						}
					}
				}

				// Force commands and inputs to execute even when another control may have a command using that key gesture
				foreach (InputBinding inputBinding in this.InputBindings) {
					KeyGesture keyGesture = inputBinding.Gesture as KeyGesture;
					if (keyGesture != null && keyGesture.Key == e.Key && keyGesture.Modifiers == Keyboard.Modifiers) {
						if (inputBinding.Command != null) {
							inputBinding.Command.Execute(0);
							e.Handled = true;
						}
					}
				}

				foreach (CommandBinding cb in this.CommandBindings) {
					RoutedCommand command = cb.Command as RoutedCommand;
					if (command != null) {
						foreach (InputGesture inputGesture in command.InputGestures) {
							KeyGesture keyGesture = inputGesture as KeyGesture;
							if (keyGesture != null && keyGesture.Key == e.Key && keyGesture.Modifiers == Keyboard.Modifiers) {
								command.Execute(0, this);
								e.Handled = true;
							}
						}
					}
				}
			}
		}

		private void OnResizeTileset(object sender, RoutedEventArgs e) {
			display.CurrentTool.Cancel();
			Point2I dimensions = tileset.Dimensions;
			if (ResizeTilesetWindow.Show(Window.GetWindow(this), ref dimensions)) {
				tileset.Resize(dimensions);
				display.UpdateList(tileset);
				display.Modfied();
			}
		}

		private void OnCancelChanges(object sender, RoutedEventArgs e) {
			display.CurrentTool.Cancel();
			var result = TriggerMessageBox.Show(Window.GetWindow(this), MessageIcon.Warning,
				"Are you sure you want to cancel changes made to this tileset?",
				"Cancel Changes", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {
				Reload();
			}
		}

		private void OnToolChanged(object sender, RoutedEventArgs e) {
			display.CurrentTool = (TilesetEditorTool) ((ToggleButton) sender).Tag;
		}

		private void OnUsePreviewSpritesChanged(object sender, RoutedEventArgs e) {
			tileset.UsePreviewSprites = buttonUsePreviewSprites.IsChecked.Value;
			display.Modfied();
			display.Invalidate();
		}

		private void OnMergeOverwriteChanged(object sender, RoutedEventArgs e) {
			display.Overwrite = !display.Overwrite;
			if (display.Overwrite) {
				buttonMergeOverwrite.Source = DesignerImages.Overwrite;
				buttonMergeOverwrite.ToolTip = "Overwrite";
			}
			else {
				buttonMergeOverwrite.Source = DesignerImages.Merge;
				buttonMergeOverwrite.ToolTip = "Merge";
			}
			display.Invalidate();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsModified {
			get { return modified; }
		}

		public bool CanPaste {
			get { return display.ToolSelection.CanPaste; }
		}

		public TilesetEditorTool CurrentTool {
			get { return display.CurrentTool; }
		}
	}
}
