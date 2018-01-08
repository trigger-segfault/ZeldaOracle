using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		private bool supressEvents;

		private ProjectExplorer projectExplorer;
		private OutputConsole outputConsole;
		private SpriteBrowser spriteBrowser;
		private SpriteSourceBrowser spriteSourceBrowser;
		private StyleBrowser styleBrowser;
		private TileDataBrowser tileDataBrowser;

		private FindReplaceWindow findReplaceWindow;
		private PlaybackWindow playbackWindow;

		private DispatcherTimer checkOutdatedTimer;
		private DispatcherTimer loadedTimer;
		private DispatcherTimer displayTimer;

		private IRequestCloseAnchorable activeAnchorable;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MainWindow() {
			supressEvents = true;
			InitializeComponent();

			// Splash window mode
			Width = 400;
			Height = 260;
			WindowStyle = WindowStyle.None;
			ResizeMode = ResizeMode.NoResize;
			dockPanel.Visibility = Visibility.Hidden;

			Application.Current.Activated += OnApplicationActivated;

			this.checkOutdatedTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.1),
				DispatcherPriority.ApplicationIdle, delegate {
					DesignerControl.CheckForOutdatedFiles(true);
					checkOutdatedTimer.Stop();
				}, Dispatcher);
			this.checkOutdatedTimer.Stop();
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

			Visibility = Visibility.Collapsed;
			displayTimer = new DispatcherTimer(
				TimeSpan.FromMilliseconds(0.1),
				DispatcherPriority.ApplicationIdle,
				delegate {
					DisplayWindow();
					displayTimer.Stop();
					displayTimer = null;
				}, Dispatcher);
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
			supressEvents = false;

			OnOutputConsoleCommand();
			OnProjectExplorerCommand();

			loadedTimer = new DispatcherTimer(
				TimeSpan.FromSeconds(0.1),
				DispatcherPriority.ApplicationIdle,
				delegate {
					Initialize();
					loadedTimer.Stop();
					loadedTimer = null;
				}, Dispatcher);
		}

		private void OnApplicationActivated(object sender, EventArgs e) {
			// HACK: Prevent text editor keeping mouse down focus after closing any dialogs.
			checkOutdatedTimer.Start();
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
				spriteBrowser.RefreshList();
			if (spriteSourceBrowser != null)
				spriteSourceBrowser.RefreshList();
			if (styleBrowser != null)
				styleBrowser.RefreshList();
			if (tileDataBrowser != null)
				tileDataBrowser.RefreshList();
		}

		private void OnResourcesUnloaded(object sender, EventArgs e) {
			if (spriteBrowser != null)
				spriteBrowser.ClearList();
			if (spriteSourceBrowser != null)
				spriteSourceBrowser.ClearList();
			if (styleBrowser != null)
				styleBrowser.ClearList();
			if (tileDataBrowser != null)
				tileDataBrowser.ClearList();
		}

		private void OnFinishedBuilding(object sender, EventArgs e) {
			//if (spriteBrowser != null)
			//	spriteBrowser.RefreshList();
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
			if (ActiveAnchorableChanged != null && activeAnchorable != oldAnchorable)
				ActiveAnchorableChanged(this, EventArgs.Empty);
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

		public void DockDocument(RequestCloseDocument anchorable) {
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
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsLoaded)
					spriteBrowser.RefreshList();
			}
			else {
				spriteBrowser.IsActive = true;
			}
		}

		private void OnSpriteSourceBrowserCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (spriteSourceBrowser == null) {
				spriteSourceBrowser = new SpriteSourceBrowser();
				spriteSourceBrowser.Closed += OnAnchorableClosed;
				spriteSourceBrowser.AddToLayout(dockingManager, AnchorableShowStrategy.Right);
				var pane = spriteSourceBrowser.Parent as LayoutAnchorablePane;
				pane.DockWidth = new GridLength(250);
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsLoaded)
					spriteSourceBrowser.RefreshList();
			}
			else {
				spriteSourceBrowser.IsActive = true;
			}
		}

		private void OnStyleBrowserCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (styleBrowser == null) {
				styleBrowser = new StyleBrowser();
				styleBrowser.Closed += OnAnchorableClosed;
				styleBrowser.AddToLayout(dockingManager, AnchorableShowStrategy.Right);
				var pane = styleBrowser.Parent as LayoutAnchorablePane;
				pane.DockWidth = new GridLength(250);
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsLoaded)
					styleBrowser.RefreshList();
			}
			else {
				styleBrowser.IsActive = true;
			}
		}

		private void OnTileDataBrowserCommand(object sender = null, ExecutedRoutedEventArgs e = null) {
			if (tileDataBrowser == null) {
				tileDataBrowser = new TileDataBrowser();
				tileDataBrowser.Closed += OnAnchorableClosed;
				tileDataBrowser.AddToLayout(dockingManager, AnchorableShowStrategy.Right);
				var pane = tileDataBrowser.Parent as LayoutAnchorablePane;
				pane.DockWidth = new GridLength(250);
				if (DesignerControl.IsProjectOpen && ZeldaResources.IsLoaded)
					tileDataBrowser.RefreshList();
			}
			else {
				tileDataBrowser.IsActive = true;
			}
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
			if (supressEvents) return;
			e.CanExecute = DesignerControl.CanSave;
		}

		private void CanUndo(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
			e.CanExecute = DesignerControl.CanUndo;
		}

		private void CanRedo(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
			e.CanExecute = DesignerControl.CanRedo;
		}

		private void CanExecuteIsBusy(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
			e.CanExecute = !DesignerControl.IsBusy && DesignerControl.IsProjectOpen;
		}

		private void CanExecuteIsInTextEditor(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
			e.CanExecute = DesignerControl.IsInTextEditor;
		}

		private void CanExecuteIsProjectOpen(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
			e.CanExecute = DesignerControl.IsProjectOpen;
		}

		private void CanExecuteIsBuilding(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
			e.CanExecute = DesignerControl.IsBusy;
		}

		private void CanExecuteHasError(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
			e.CanExecute = DesignerControl.HasError;
		}

		private void CanExecuteIsFindAndReplaceOpen(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
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

		public IRequestCloseAnchorable ActiveAnchorable {
			get { return activeAnchorable; }
		}

		public FindReplaceWindow FindAndReplaceWindow {
			get { return findReplaceWindow; }
		}
	}
}
