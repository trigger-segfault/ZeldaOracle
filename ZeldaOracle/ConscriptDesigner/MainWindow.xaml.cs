using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ConscriptDesigner.Control;
using ConscriptDesigner.WinForms;
using Xceed.Wpf.AvalonDock.Layout;

namespace ConscriptDesigner {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private bool supressEvents;

		private OutputConsole outputConsole;
		private ProjectExplorer projectExplorer;

		private DispatcherTimer checkOutdatedTimer;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MainWindow() {
			supressEvents = true;
			InitializeComponent();

			dummyHost.Child = new DummyGraphicsDeviceControl();
			Application.Current.Activated += OnApplicationActivated;

			this.checkOutdatedTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.1),
				DispatcherPriority.ApplicationIdle, delegate {
					DesignerControl.CheckForOutdatedFiles(true);
					checkOutdatedTimer.Stop();
				}, Dispatcher);
			this.checkOutdatedTimer.Stop();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		private void OnLoaded(object sender, RoutedEventArgs e) {
			DesignerControl.Initialize(this);
			supressEvents = false;
			
			OnOutputConsoleCommand();
			OnProjectExplorerCommand();

			string[] args = Environment.GetCommandLineArgs();
			if (args.Length > 1) {
				DesignerControl.OpenProject(args[1]);
			}
		}

		private void OnApplicationActivated(object sender, EventArgs e) {
			// HACK: Prevent text editor keeping mouse down focus after closing any dialogs.
			checkOutdatedTimer.Start();
		}

		private void OnClosing(object sender, CancelEventArgs e) {
			e.Cancel = !DesignerControl.RequestClose();
		}
		
		private void OnActiveAnchorableChanged(object sender, EventArgs e) {
			CommandManager.InvalidateRequerySuggested();
		}

		private void OnAnchorableClosed(object sender, EventArgs e) {
			IRequestCloseAnchorable anchorable = sender as IRequestCloseAnchorable;
			if (anchorable is ProjectExplorer) {
				projectExplorer.Clear();
				projectExplorer = null;
			}
			else if (anchorable is OutputConsole) {
				outputConsole = null;
			}
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

		public void DockDocument(RequestCloseDocument anchorable) {
			//dockingManager.Layout.RootPanel.Children.Add(anchorable);
			LayoutDocumentPane docPane = dockingManager.Layout.Descendents().FirstOrDefault(l => l is LayoutDocumentPane) as LayoutDocumentPane;
			if (docPane != null)
				docPane.Children.Add(anchorable);
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

		}

		private void OnReplaceCommand(object sender, ExecutedRoutedEventArgs e) {

		}

		private void OnGotoLineCommand(object sender, ExecutedRoutedEventArgs e) {

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

		private void OnSpriteBrowserCommand(object sender, ExecutedRoutedEventArgs e) {

		}

		private void OnTileBrowserCommand(object sender, ExecutedRoutedEventArgs e) {

		}

		private void OnRunConscriptsCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.RunConscripts();
		}

		private void OnCompileContentCommand(object sender, ExecutedRoutedEventArgs e) {
			DesignerControl.CompileContent();
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

		private void CanExecuteInTextEditor(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
			e.CanExecute = DesignerControl.IsInTextEditor;
		}

		private void CanExecuteIsProjectOpen(object sender, CanExecuteRoutedEventArgs e) {
			if (supressEvents) return;
			e.CanExecute = DesignerControl.IsProjectOpen;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ProjectExplorer ProjectExplorer {
			get { return projectExplorer; }
		}

		public OutputConsole OutputConsole {
			get { return outputConsole; }
		}
	}
}
