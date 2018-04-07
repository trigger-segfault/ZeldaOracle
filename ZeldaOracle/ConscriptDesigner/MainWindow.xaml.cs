using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ConscriptDesigner.Anchorables;
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;
using ConscriptDesigner.Util;
using ConscriptDesigner.Windows;
using ConscriptDesigner.WinForms;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Xceed.Wpf.AvalonDock.Themes;
using ZeldaResources = ZeldaOracle.Common.Content.Resources;

namespace ConscriptDesigner {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private bool suppressEvents;

		private ProjectExplorer projectExplorer;
		private OutputConsole outputConsole;
		private SpriteBrowser spriteBrowser;
		private SpriteSourceBrowser spriteSourceBrowser;
		private StyleBrowser styleBrowser;
		private TileDataBrowser tileDataBrowser;
		private TilesetBrowser tilesetBrowser;
		private TileBrowser tileBrowser;
		private TilesetEditor tilesetEditor;

		private FindReplaceWindow findReplaceWindow;
		private PlaybackWindow playbackWindow;

		private StoppableTimer checkOutdatedTimer;
		private StoppableTimer loadedTimer;
		private StoppableTimer displayTimer;
		private StoppableTimer focusTimer;

		private IRequestCloseAnchorable activeAnchorable;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MainWindow() {
			// Prevent System.Windows.Data Error: 4
			PresentationTraceSources.DataBindingSource.Switch.Level =
				SourceLevels.Critical;

			suppressEvents = true;
			InitializeComponent();

			// Splash window mode
			Width = 400;
			Height = 260;
			WindowStyle = WindowStyle.None;
			ResizeMode = ResizeMode.NoResize;
			dockPanel.Visibility = Visibility.Hidden;

			Application.Current.Activated += OnApplicationActivated;

			for (int i = 1; i <= 3; i++) {
				ComboBoxItem item = new ComboBoxItem();
				item.Content = "x" + i + " Scale";
				item.Tag = i;
				comboBoxScales.Items.Add(item);
			}
			comboBoxScales.SelectedIndex = 0;


			this.checkOutdatedTimer = StoppableTimer.Create(
				TimeSpan.FromSeconds(0.1),
				DispatcherPriority.ApplicationIdle, delegate {
					DesignerControl.CheckForOutdatedFiles(true);
					checkOutdatedTimer.Stop();
				});
			/*this.checkOutdatedTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.1),
				DispatcherPriority.ApplicationIdle, delegate {
					DesignerControl.CheckForOutdatedFiles(true);
					checkOutdatedTimer.Stop();
				}, Dispatcher);
			this.checkOutdatedTimer.Stop();*/

			Application.Current.Activated += OnApplicationActivated;
			Application.Current.Deactivated += OnApplicationDeactivated;

			focusTimer = StoppableTimer.Create(
				TimeSpan.FromMilliseconds(16),
				DispatcherPriority.Render,
				delegate {
					if (activeAnchorable != null)
						activeAnchorable.Focus();
					focusTimer.Stop();
				});
			/*focusTimer = new DispatcherTimer(
				TimeSpan.FromMilliseconds(16),
				DispatcherPriority.Render,
				delegate {
					if (activeAnchorable != null)
						activeAnchorable.Focus();
					focusTimer.Stop();
				}, Dispatcher);
			focusTimer.Stop();*/
		}


		//-----------------------------------------------------------------------------
		// Setup
		//-----------------------------------------------------------------------------
		
		// HACK: This function is used to setup loading of the layout
		// in a way that prevents GraphicsDevice.Reset from failing.
		private void Initialize() {
			dummyHost.Child = new DummyGraphicsDeviceControl();
			string[] args = Environment.GetCommandLineArgs();
			if (args.Length > 1) {
				DesignerControl.OpenProject(args[1]);
			}
			else {
				ProjectUserSettings.LoadDefaults();
			}

			Visibility = Visibility.Collapsed;
			displayTimer = StoppableTimer.StartNew(
				TimeSpan.FromMilliseconds(0.1),
				DispatcherPriority.ApplicationIdle,
				delegate {
					DisplayWindow();
					displayTimer.Stop();
					displayTimer = null;
				});
			/*displayTimer = new DispatcherTimer(
				TimeSpan.FromMilliseconds(0.1),
				DispatcherPriority.ApplicationIdle,
				delegate {
					DisplayWindow();
					displayTimer.Stop();
					displayTimer = null;
				}, Dispatcher);*/
		}

		private void DisplayWindow() {
			// Disable splash window mode
			WindowStyle = WindowStyle.SingleBorderWindow;
			ResizeMode = ResizeMode.CanResize;
			splash.Visibility = Visibility.Collapsed;
			dockPanel.Visibility = Visibility.Visible;

			// Center window
			Width = ProjectUserSettings.Window.Width;
			Height = ProjectUserSettings.Window.Height;
			Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
			Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
			if (ProjectUserSettings.Window.Maximized)
				WindowState = WindowState.Maximized;
			Visibility = Visibility.Visible;
		}


		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public event EventHandler ActiveAnchorableChanged;


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnLoaded(object sender, RoutedEventArgs e) {
			DesignerControl.Initialize(this);
			DesignerControl.FinishedBuilding += OnFinishedBuilding;
			DesignerControl.ProjectClosed += OnProjectClosed;
			DesignerControl.ResourcesLoaded += OnResourcesLoaded;
			DesignerControl.ResourcesUnloaded += OnResourcesUnloaded;
			suppressEvents = false;

			loadedTimer = StoppableTimer.StartNew(
				TimeSpan.FromSeconds(0.1),
				DispatcherPriority.ApplicationIdle,
				delegate {
					Initialize();
					loadedTimer.Stop();
					loadedTimer = null;
				});
			/*loadedTimer = new DispatcherTimer(
				TimeSpan.FromSeconds(0.1),
				DispatcherPriority.ApplicationIdle,
				delegate {
					Initialize();
					loadedTimer.Stop();
					loadedTimer = null;
				}, Dispatcher);*/
		}

		private void OnApplicationActivated(object sender, EventArgs e) {
			DesignerControl.IsActive = true;
			// HACK: Prevent text editor keeping mouse down focus after closing any dialogs.
			checkOutdatedTimer.Start();
		}

		private void OnApplicationDeactivated(object sender, EventArgs e) {
			DesignerControl.IsActive = false;
		}

		private void OnClosing(object sender, CancelEventArgs e) {
			e.Cancel = !DesignerControl.RequestClose();
			if (!e.Cancel) {
				if (DesignerControl.IsProjectOpen) {
					//SaveLayout();
					ProjectUserSettings.Save();
				}
				// Prevent the output console from throwing an
				// exception from still trying to be written to.
				if (outputConsole != null) {
					outputConsole.ForceClose();
				}
			}
		}

		private void OnProjectClosed(object sender, EventArgs e) {
			OnResourcesUnloaded(sender, e);
			if (findReplaceWindow != null)
				findReplaceWindow.Close();
		}

		private void OnResourcesLoaded(object sender, EventArgs e) {
			if (spriteBrowser != null)
				spriteBrowser.Reload();
			if (spriteSourceBrowser != null)
				spriteSourceBrowser.Reload();
			if (styleBrowser != null)
				styleBrowser.Reload();
			if (tileDataBrowser != null)
				tileDataBrowser.Reload();
			if (tilesetBrowser != null)
				tilesetBrowser.Reload();
			if (tileBrowser != null)
				tileBrowser.Reload();
			if (tilesetEditor != null)
				tilesetEditor.Reload();
			comboBoxZones.ItemsSource = DesignerControl.PreviewZones;
			comboBoxZones.SelectedItem = DesignerControl.PreviewZoneID;
			comboBoxTilePalettes.ItemsSource = DesignerControl.PreviewTilePalettes;
			comboBoxTilePalettes.SelectedItem = DesignerControl.PreviewTilePaletteID;
			comboBoxEntityPalettes.ItemsSource = DesignerControl.PreviewEntityPalettes;
			comboBoxEntityPalettes.SelectedItem = DesignerControl.PreviewEntityPaletteID;
		}

		private void OnResourcesUnloaded(object sender, EventArgs e) {
			if (spriteBrowser != null)
				spriteBrowser.Unload();
			if (spriteSourceBrowser != null)
				spriteSourceBrowser.Unload();
			if (styleBrowser != null)
				styleBrowser.Unload();
			if (tileDataBrowser != null)
				tileDataBrowser.Unload();
			if (tilesetBrowser != null)
				tilesetBrowser.Unload();
			if (tileBrowser != null)
				tileBrowser.Unload();
			if (tilesetEditor != null)
				tilesetEditor.Unload();
			suppressEvents = true;
			comboBoxZones.ItemsSource = null;
			comboBoxZones.Items.Clear();
			comboBoxTilePalettes.ItemsSource = null;
			comboBoxTilePalettes.Items.Clear();
			comboBoxEntityPalettes.ItemsSource = null;
			comboBoxEntityPalettes.Items.Clear();
			suppressEvents = false;
		}

		private void OnFinishedBuilding(object sender, EventArgs e) {
			
		}
		
		private void OnActiveAnchorableChanged(object sender = null, EventArgs e = null) {
			IRequestCloseAnchorable oldAnchorable = activeAnchorable;
			activeAnchorable = null;
			foreach (IRequestCloseAnchorable anchorable in DesignerControl.GetOpenAnchorables()) {
				if (anchorable.IsActive) {
					activeAnchorable = anchorable;
					break;
				}
			}
			CommandManager.InvalidateRequerySuggested();
			if (ActiveAnchorableChanged != null && activeAnchorable != oldAnchorable) {
				// HACK: Focus after a split second so focus actually transfers.
				focusTimer.Start();
				ActiveAnchorableChanged(this, EventArgs.Empty);
			}
		}

		private void OnAnchorableClosed(object sender, EventArgs e) {
			IRequestCloseAnchorable anchorable = sender as IRequestCloseAnchorable;
			if (anchorable is ProjectExplorer)
				projectExplorer = null;
			else if (anchorable is OutputConsole)
				outputConsole = null;
			else if (anchorable is SpriteBrowser)
				spriteBrowser = null;
			else if (anchorable is SpriteSourceBrowser)
				spriteSourceBrowser = null;
			else if (anchorable is StyleBrowser)
				styleBrowser = null;
			else if (anchorable is TileDataBrowser)
				tileDataBrowser = null;
			else if (anchorable is TilesetBrowser)
				tilesetBrowser = null;
			else if (anchorable is TileBrowser)
				tileBrowser = null;
			else if (anchorable is TilesetEditor)
				tilesetEditor = null;
			CommandManager.InvalidateRequerySuggested();
		}

		private void OnWindowClosed(object sender, EventArgs e) {
			Window window = sender as Window;
			if (window is FindReplaceWindow)
				findReplaceWindow = null;
			if (window is PlaybackWindow)
				playbackWindow = null;

			// HACK: Prevent minimizing to Visual Studio after closing
			// a tool window that has called a message box.
			Activate();
		}


		//-----------------------------------------------------------------------------
		// Dock Anchorables
		//-----------------------------------------------------------------------------

		public void OpenProjectExplorer() {
			OnProjectExplorerCommand();
		}

		public void OpenOutputConsole() {
			OnOutputConsoleCommand();
		}

		public void OpenSpriteBrowser() {
			OnSpriteBrowserCommand();
		}

		public void OpenSpriteSourceBrowser() {
			OnSpriteSourceBrowserCommand();
		}

		public void OpenStyleBrowser() {
			OnStyleBrowserCommand();
		}

		public void OpenTileDataBrowser() {
			OnTileDataBrowserCommand();
		}

		public void OpenTilesetBrowser() {
			OnTilesetBrowserCommand();
		}

		public void OpenTileBrowser() {
			OnTileBrowserCommand();
		}

		public void OpenTilesetEditor() {
			OnTilesetEditorCommand();
		}

		public void DockDocument(RequestCloseDocument anchorable) {
			//dockingManager.Layout.RootPanel.Children.Add(anchorable);
			LayoutDocumentPane docPane = dockingManager.Layout.Descendents().FirstOrDefault(l => l is LayoutDocumentPane) as LayoutDocumentPane;
			if (docPane != null)
				docPane.Children.Add(anchorable);
		}

		public void DockDocument(RequestCloseAnchorable anchorable) {
			//dockingManager.Layout.RootPanel.Children.Add(anchorable);
			LayoutDocumentPane docPane = dockingManager.Layout.Descendents().FirstOrDefault(l => l is LayoutDocumentPane) as LayoutDocumentPane;
			if (docPane != null)
				docPane.Children.Add(anchorable);
		}

		public void InvalidateActiveAnchorable() {
			OnActiveAnchorableChanged();
		}

		public void LoadLayout() {
			if (File.Exists(DesignerControl.ProjectSettingsFile)) {
				try {
					var serializer = new XmlLayoutSerializer(dockingManager);
					using (var stream = new StreamReader(DesignerControl.ProjectSettingsFile))
						serializer.Deserialize(stream);
					//dockingManager.Theme = new VS2010Theme();
					return;
				}
				catch (Exception) {

				}
			}
			BuildDefaultLayoutRoot();
			OpenProjectExplorer();
			OpenOutputConsole();
		}

		public void SaveLayout() {
			try {
				var serializer = new XmlLayoutSerializer(dockingManager);
				using (var stream = new StreamWriter(DesignerControl.ProjectSettingsFile))
					serializer.Serialize(stream);
			}
			catch (Exception) {

			}
		}

		public IEnumerable<ContentFile> GetOrderedOpenContentFiles() {
			foreach (var descendant in dockingManager.Layout.Descendents()) {
				if (descendant is IContentFileContainer) {
					yield return ((IContentFileContainer) descendant).File;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Actions
		//-----------------------------------------------------------------------------
		
		public void PlaySound(ContentSound sound) {
			if (playbackWindow == null) {
				playbackWindow = PlaybackWindow.Show(this, sound, OnWindowClosed);
			}
			else {
				playbackWindow.PlaySound(sound);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private void BuildDefaultLayoutRoot() {
			/*LayoutRoot root = new LayoutRoot();
			root.RootPanel.Children.Clear();

			LayoutPanel panel1 = new LayoutPanel();
			panel1.Orientation = Orientation.Vertical;
			root.RootPanel = panel1;

			LayoutPanel panel2 = new LayoutPanel();
			panel2.Orientation = Orientation.Horizontal;
			panel1.Children.Add(panel2);

			LayoutAnchorablePane anchorablePane1 = new LayoutAnchorablePane();
			anchorablePane1.DockWidth = new GridLength(250);
			panel2.Children.Add(anchorablePane1);

			LayoutPanel panel3 = new LayoutPanel();
			panel3.Orientation = Orientation.Vertical;
			panel2.Children.Add(panel3);

			LayoutDocumentPane documentPane = new LayoutDocumentPane();
			panel3.Children.Add(documentPane);

			LayoutAnchorablePane anchorablePane2 = new LayoutAnchorablePane();
			anchorablePane2.DockHeight = new GridLength(200);
			panel3.Children.Add(anchorablePane2);

			dockingManager.Layout = root;*/
		}

		//-----------------------------------------------------------------------------
		// Commands
		//-----------------------------------------------------------------------------

		private void OnOpenCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.Open();
		}

		private void OnSaveCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.Save();
		}

		private void OnSaveAllCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.SaveAll();
		}

		private void OnCloseCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.Close();
		}

		private void OnExitCommand(object sender, ExecutedRoutedEventArgs e) {
			Close();
		}

		private void OnUndoCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.Undo();
		}

		private void OnRedoCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.Redo();
		}

		private void OnCutCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.Cut();
		}

		private void OnCopyCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.Copy();
		}

		private void OnPasteCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.Paste();
		}

		private void OnDeleteCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.Delete();
		}

		private void OnFindCommand(object sender, ExecutedRoutedEventArgs e) {
			if (findReplaceWindow == null) {
				findReplaceWindow = FindReplaceWindow.Show(this, false, OnWindowClosed);
			}
			else {
				findReplaceWindow.FindMode();
			}
		}

		private void OnReplaceCommand(object sender, ExecutedRoutedEventArgs e) {
			if (findReplaceWindow == null) {
				findReplaceWindow = FindReplaceWindow.Show(this, true, OnWindowClosed);
			}
			else {
				findReplaceWindow.ReplaceMode();
			}
		}

		private void OnFindNextCommand(object sender, ExecutedRoutedEventArgs e) {
			findReplaceWindow.FindNext();
		}

		private void OnReplaceNextCommand(object sender, ExecutedRoutedEventArgs e) {
			findReplaceWindow.ReplaceNext();
		}

		private void OnReplaceAllCommand(object sender, ExecutedRoutedEventArgs e) {
			findReplaceWindow.ReplaceAll();
		}

		private void OnGotoLineCommand(object sender, ExecutedRoutedEventArgs e) {
			GotoWindow.Show(this);
		}

		private void OnProjectExplorerCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (projectExplorer == null) {
				projectExplorer = new ProjectExplorer();
				projectExplorer.Project = DesignerControl.Project;
				projectExplorer.Closed += OnAnchorableClosed;
				projectExplorer.AddToLayout(dockingManager, AnchorableShowStrategy.Left);
				var pane = projectExplorer.Parent as LayoutAnchorablePane;
				pane.DockWidth = new GridLength(300);
			}
			else {
				projectExplorer.IsActive = true;
			}
		}

		private void OnOutputConsoleCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (outputConsole == null) {
				outputConsole = new OutputConsole();
				outputConsole.Closed += OnAnchorableClosed;
				outputConsole.AddToLayout(dockingManager, AnchorableShowStrategy.Bottom);
				var pane = outputConsole.Parent as LayoutAnchorablePane;
				pane.DockHeight = new GridLength(180);
			}
			else {
				outputConsole.IsActive = true;
			}
		}

		private void OnSpriteBrowserCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (spriteBrowser == null) {
				spriteBrowser = new SpriteBrowser();
				spriteBrowser.Closed += OnAnchorableClosed;
				spriteBrowser.AddToLayout(dockingManager, AnchorableShowStrategy.Right);
				var pane = spriteBrowser.Parent as LayoutAnchorablePane;
				pane.DockWidth = new GridLength(250);
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsInitialized)
					spriteBrowser.Reload();
			}
			spriteBrowser.IsActive = true;
		}

		private void OnSpriteSourceBrowserCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (spriteSourceBrowser == null) {
				spriteSourceBrowser = new SpriteSourceBrowser();
				spriteSourceBrowser.Closed += OnAnchorableClosed;
				spriteSourceBrowser.AddToLayout(dockingManager, AnchorableShowStrategy.Right);
				var pane = spriteSourceBrowser.Parent as LayoutAnchorablePane;
				pane.DockWidth = new GridLength(250);
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsInitialized)
					spriteSourceBrowser.Reload();
			}
			spriteSourceBrowser.IsActive = true;
		}

		private void OnStyleBrowserCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (styleBrowser == null) {
				styleBrowser = new StyleBrowser();
				styleBrowser.Closed += OnAnchorableClosed;
				styleBrowser.AddToLayout(dockingManager, AnchorableShowStrategy.Right);
				var pane = styleBrowser.Parent as LayoutAnchorablePane;
				pane.DockWidth = new GridLength(250);
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsInitialized)
					styleBrowser.Reload();
			}
			styleBrowser.IsActive = true;
		}

		private void OnTileDataBrowserCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (tileDataBrowser == null) {
				tileDataBrowser = new TileDataBrowser();
				tileDataBrowser.Closed += OnAnchorableClosed;
				tileDataBrowser.AddToLayout(dockingManager, AnchorableShowStrategy.Right);
				var pane = tileDataBrowser.Parent as LayoutAnchorablePane;
				pane.DockWidth = new GridLength(250);
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsInitialized)
					tileDataBrowser.Reload();
			}
			tileDataBrowser.IsActive = true;
		}

		private void OnTilesetBrowserCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (tilesetBrowser == null) {
				tilesetBrowser = new TilesetBrowser();
				tilesetBrowser.Closed += OnAnchorableClosed;
				tilesetBrowser.AddToLayout(dockingManager, AnchorableShowStrategy.Right);
				var pane = tilesetBrowser.Parent as LayoutAnchorablePane;
				pane.DockWidth = new GridLength(250);
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsInitialized)
					tilesetBrowser.Reload();
			}
			tilesetBrowser.IsActive = true;
		}

		private void OnTileBrowserCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (tileBrowser == null) {
				tileBrowser = new TileBrowser();
				tileBrowser.Closed += OnAnchorableClosed;
				tileBrowser.AddToLayout(dockingManager, AnchorableShowStrategy.Right);
				var pane = tileBrowser.Parent as LayoutAnchorablePane;
				pane.DockWidth = new GridLength(250);
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsInitialized)
					tileBrowser.Reload();
			}
			tileBrowser.IsActive = true;
		}

		private void OnTilesetEditorCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (tilesetEditor == null) {
				tilesetEditor = new TilesetEditor();
				tilesetEditor.Closed += OnAnchorableClosed;
				DockDocument(tilesetEditor);
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsInitialized)
					tilesetEditor.Reload();
			}
			tilesetEditor.IsActive = true;
		}

		private void OnLaunchGameCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.LaunchGame();
		}

		private void OnLaunchEditorCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.LaunchEditor();
		}

		private void OnRunConscriptsCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.RunConscripts();
		}

		private void OnCompileContentCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.CompileContent();
		}

		private void OnCancelBuildCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.CancelBuild();
		}

		private void OnGotoErrorCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.GotoError();
		}


		//-----------------------------------------------------------------------------
		// Can Execute Commands
		//-----------------------------------------------------------------------------

		private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}

		private void CanSave(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = DesignerControl.CanSave;
		}

		private void CanUndo(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = DesignerControl.CanUndo;
		}

		private void CanRedo(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = DesignerControl.CanRedo;
		}

		private void CanCut(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = DesignerControl.CanCut;
		}

		private void CanCopy(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = DesignerControl.CanCopy;
		}

		private void CanPaste(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = DesignerControl.CanPaste;
		}

		private void CanDelete(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = DesignerControl.CanDelete;
		}

		private void CanExecuteIsBusy(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = !DesignerControl.IsBusy && DesignerControl.IsProjectOpen;
		}

		private void CanExecuteIsInTextEditor(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = DesignerControl.IsInTextEditor;
		}

		private void CanExecuteIsProjectOpen(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = DesignerControl.IsProjectOpen;
		}

		private void CanExecuteIsBuilding(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = !DesignerControl.IsBusy;
		}

		private void CanExecuteHasError(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = DesignerControl.HasError;
		}

		private void CanExecuteIsFindAndReplaceOpen(object sender, CanExecuteRoutedEventArgs e) {
			if (suppressEvents) return;
			e.CanExecute = findReplaceWindow != null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ProjectExplorer ProjectExplorer {
			get { return projectExplorer; }
			set { projectExplorer = value; }
		}

		public OutputConsole OutputConsole {
			get { return outputConsole; }
			set { outputConsole = value; }
		}

		public SpriteBrowser SpriteBrowser {
			get { return spriteBrowser; }
			set { spriteBrowser = value; }
		}

		public SpriteSourceBrowser SpriteSourceBrowser {
			get { return spriteSourceBrowser; }
			set { spriteSourceBrowser = value; }
		}

		public StyleBrowser StyleBrowser {
			get { return styleBrowser; }
			set { styleBrowser = value; }
		}

		public TileDataBrowser TileDataBrowser {
			get { return tileDataBrowser; }
			set { tileDataBrowser = value; }
		}

		public TilesetBrowser TilesetBrowser {
			get { return tilesetBrowser; }
			set { tilesetBrowser = value; }
		}

		public TileBrowser TileBrowser {
			get { return tileBrowser; }
			set { tileBrowser = value; }
		}

		public TilesetEditor TilesetEditor {
			get { return tilesetEditor; }
			set { tilesetEditor = value; }
		}

		public IRequestCloseAnchorable ActiveAnchorable {
			get { return activeAnchorable; }
		}

		public FindReplaceWindow FindAndReplaceWindow {
			get { return findReplaceWindow; }
		}

		private void OnScaleChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			DesignerControl.PreviewScale = (int) ((FrameworkElement) comboBoxScales.SelectedItem).Tag;
		}

		private void OnZoneChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			DesignerControl.PreviewZoneID = (string) comboBoxZones.SelectedItem;
		}

		private void OnTilePaletteChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			DesignerControl.PreviewTilePaletteID = (string) comboBoxTilePalettes.SelectedItem;
		}

		private void OnEntityPaletteChanged(object sender, SelectionChangedEventArgs e) {
			if (suppressEvents) return;
			DesignerControl.PreviewEntityPaletteID = (string) comboBoxEntityPalettes.SelectedItem;
		}

		private void OnPlayAnimations(object sender, RoutedEventArgs e) {
			DesignerControl.PlayAnimations = buttonPlayAnimations.IsChecked.Value;
			buttonRestartAnimations.IsEnabled = DesignerControl.PlayAnimations;
		}

		private void OnRestartAnimations(object sender, RoutedEventArgs e) {
			DesignerControl.RestartAnimations();
		}
	}
}
