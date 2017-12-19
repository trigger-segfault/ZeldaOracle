using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ConscriptDesigner.Anchorables;
using ConscriptDesigner.Content;
using ConscriptDesigner.Util;
using ConscriptDesigner.Windows;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xceed.Wpf.AvalonDock.Layout;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Game;

namespace ConscriptDesigner.Control {
	public static class DesignerControl {
		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private static MainWindow mainWindow;
		private static ContentRoot project;
		private static GraphicsDevice graphicsDevice;
		private static ContentManager contentManager;
		private static Task<ScriptReaderException> busyTask;
		private static ScriptReaderException lastScriptError;
		private static DispatcherTimer updateTimer;
		private static HashSet<IRequestClosePanel> openAnchorables;
		private static List<IRequestClosePanel> closingAnchorables;

		private static ClipboardListener clipboardListener;

		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		public static void Initialize(MainWindow mainWindow) {
			clipboardListener = new ClipboardListener();
			GameSettings.DesignerMode = true;
			DesignerControl.mainWindow = mainWindow;
			openAnchorables = new HashSet<IRequestClosePanel>();
			closingAnchorables = new List<IRequestClosePanel>();
			//project = new ContentRoot();
			//project.LoadContentProject(ContentProjectFile);
			updateTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.2), DispatcherPriority.ApplicationIdle, delegate { Update(); }, Application.Current.Dispatcher);
		}


		//-----------------------------------------------------------------------------
		// Anchorables
		//-----------------------------------------------------------------------------
		
		public static RequestCloseDocument CreateDocumentAnchorable() {
			RequestCloseDocument anchorable = new RequestCloseDocument();
			mainWindow.DockDocument(anchorable);
			return anchorable;
		}

		public static IRequestClosePanel GetActiveAnchorable() {
			foreach (var anchorable in openAnchorables) {
				if (anchorable.IsActive)
					return anchorable;
			}
			return null;
		}

		public static IContentAnchorable GetActiveContent() {
			foreach (var anchorable in openAnchorables) {
				if (anchorable.IsActive)
					return anchorable.Content as IContentAnchorable;
			}
			return null;
		}

		public static IEnumerable<IRequestClosePanel> GetOpenAnchorables() {
			return openAnchorables;
		}

		public static IEnumerable<IContentAnchorable> GetOpenContent() {
			foreach (var anchorable in openAnchorables) {
				if (anchorable.Content is IContentAnchorable)
					yield return (IContentAnchorable) anchorable.Content;
			}
		}

		public static IEnumerable<IContentAnchorable> GetModifiedContent() {
			foreach (var anchorable in openAnchorables) {
				if (anchorable.Content is IContentAnchorable) {
					var content = (IContentAnchorable) anchorable.Content;
					if (content.IsModified)
						yield return content;
				}
			}
		}
		
		public static void AddOpenAnchorable(IRequestClosePanel anchorable) {
			openAnchorables.Add(anchorable);
		}

		public static void RemoveOpenAnchorable(IRequestClosePanel anchorable) {
			openAnchorables.Remove(anchorable);
		}

		public static void AddClosingAnchorable(IRequestClosePanel anchorable) {
			closingAnchorables.Add(anchorable);
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		public static void Update() {
			if (busyTask != null) {
				if (busyTask.IsCompleted) {
					lastScriptError = busyTask.Result;
					busyTask = null;
					CommandManager.InvalidateRequerySuggested();
				}
			}
			if (closingAnchorables.Any()) {
				List<IContentAnchorable> needsSaving = new List<IContentAnchorable>();
				List<string> needsSavingFiles = new List<string>();
				foreach (IRequestClosePanel anchorable in closingAnchorables) {
					if (anchorable.Content is IContentAnchorable) {
						var content = (IContentAnchorable) anchorable.Content;
						if (content.IsModified) {
							needsSaving.Add(content);
							needsSavingFiles.Add(content.ContentFile.Path);
						}
					}
				}
				MessageBoxResult result = MessageBoxResult.Yes;
				if (needsSaving.Any()) {
					result = SaveChangesWindow.Show(mainWindow, needsSavingFiles);
					if (result == MessageBoxResult.Yes) {
						foreach (IContentAnchorable content in needsSaving) {
							content.Save();
						}
					}
				}
				if (result != MessageBoxResult.Cancel) {
					foreach (IRequestClosePanel anchorable in closingAnchorables) {
						anchorable.ForceClose();
					}
				}
				closingAnchorables.Clear();
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public static void ShowExceptionMessage(Exception ex, string verb, string name) {
			var result = TriggerMessageBox.Show(mainWindow, MessageIcon.Error, "An error occurred while trying to " +
				verb + " '" + project.Name + "'! Would you like to see the error?", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes)
				ErrorMessageBox.Show(ex, true);
		}

		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		private static ScriptReaderException CompileContentTask() {
			mainWindow.Dispatcher.Invoke(SaveAll);
			if (mainWindow.OutputTerminal != null)
				mainWindow.OutputTerminal.Clear();
			try {
				Stopwatch watch = Stopwatch.StartNew();
				foreach (ContentFile file in project.GetAllFiles()) {
					if (file.ShouldCompile) {
						string outPath = file.OutputFilePath;
						string outDir = Path.GetDirectoryName(outPath);
						if (!Directory.Exists(outDir))
							Directory.CreateDirectory(outDir);
						Console.WriteLine("Building " + file.Path.Replace('/', '\\') + " -> " + outPath.Replace('/', '\\'));
						file.Compile();
					}
				}
				UpdateContentFolder(project);
				Console.WriteLine("----------------------------------------------------------------");
				Console.WriteLine("Finished! Duration: " + watch.Elapsed.RoundUpToNearestSecond().ToString(@"hh\:mm\:ss"));
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
				Console.WriteLine("Stack Trace:\n" + ex.StackTrace);
			}
			return null;
		}

		private static ScriptReaderException RunConscriptsTask() {
			mainWindow.Dispatcher.Invoke(SaveAll);
			if (mainWindow.OutputTerminal != null)
				mainWindow.OutputTerminal.Clear();
			Resources.Uninitialize();
			
			try {
				Stopwatch watch = Stopwatch.StartNew();
				UpdateContentFolder(project);

				Resources.Initialize(contentManager, graphicsDevice);
				GameData.Initialize();
				Console.WriteLine("----------------------------------------------------------------");
				Console.WriteLine("Finished! Duration: " + watch.Elapsed.RoundUpToNearestSecond().ToString(@"hh\:mm\:ss"));
			}
			catch (ScriptReaderException ex) {
				ex.PrintMessage();
				Console.WriteLine("Stack Trace:\n" + ex.StackTrace);
				return ex;
			}
			catch (LoadContentException ex) {
				ex.PrintMessage();
				Console.WriteLine("Stack Trace:\n" + ex.StackTrace);
			}
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
				Console.WriteLine("Stack Trace:\n" + ex.StackTrace);
			}
			return null;
		}

		private static void UpdateContentFolder(ContentFolder folder) {
			HashSet<string> existingFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (string file in Directory.GetFiles(folder.OutputFilePath)) {
				existingFiles.Add(Path.GetFileName(file));
			}
			foreach (string file in Directory.GetDirectories(folder.OutputFilePath)) {
				existingFiles.Add(Path.GetFileName(file));
			}
			foreach (ContentFile file in folder.GetLocalFiles()) {
				string inPath = file.FilePath;
				string outPath = file.OutputFilePath;
				if (outPath == null)
					continue;
				string outName = Path.GetFileName(outPath);
				if (existingFiles.Contains(outName)) {
					if (file.ShouldCopyToOutput && File.GetLastWriteTimeUtc(inPath) != File.GetLastWriteTimeUtc(outPath)) {
						File.Copy(inPath, outPath, true);
					}
					existingFiles.Remove(outName);
				}
				else if (file.IsFolder) {
					if (!Directory.Exists(outPath))
						Directory.CreateDirectory(outPath);
					UpdateContentFolder((ContentFolder) file);
				}
				else if (file.ShouldCopyToOutput) {
					File.Copy(inPath, outPath, true);
				}
			}

			// Remove files that are no longer needed
			foreach (string leftoverFile in existingFiles) {
				string filePath = Path.Combine(folder.OutputFilePath, leftoverFile);
				if (Directory.Exists(filePath))
					Directory.Delete(filePath, true);
				else
					File.Delete(Path.Combine(folder.OutputFilePath, leftoverFile));
			}
		}


		//-----------------------------------------------------------------------------
		// Commands
		//-----------------------------------------------------------------------------

		public static bool RequestSaveAll(out bool errorOccurred) {
			List<IContentAnchorable> needsSaving = new List<IContentAnchorable>();
			List<string> needsSavingFiles = new List<string>();
			if (project.IsProjectModified)
				needsSavingFiles.Add(project.Name);
			foreach (var content in GetModifiedContent()) {
				needsSaving.Add(content);
				needsSavingFiles.Add(content.ContentFile.Path);
			}
			MessageBoxResult result = MessageBoxResult.Yes;
			errorOccurred = false;
			if (needsSavingFiles.Any()) {
				result = SaveChangesWindow.Show(mainWindow, needsSavingFiles);
				if (result == MessageBoxResult.Yes) {
					try {
						if (project.IsProjectModified)
							project.SaveContentProject();
					}
					catch (Exception ex) {
						ShowExceptionMessage(ex, "save", project.Name);
						errorOccurred = true;
					}
					if (!errorOccurred) {
						foreach (IContentAnchorable content in needsSaving) {
							try {
								content.Save();
							}
							catch (Exception ex) {
								ShowExceptionMessage(ex, "save", content.ContentFile.Name);
								errorOccurred = true;
								break;
							}
						}
					}
				}
			}
			return (result != MessageBoxResult.Cancel);
		}

		public static bool RequestClose() {
			if (IsProjectOpen) {
				bool errorOccurred = false;
				bool result = RequestSaveAll(out errorOccurred);
				if (errorOccurred) {
					MessageBoxResult result2 = TriggerMessageBox.Show(mainWindow, MessageIcon.Question, "Would you still like to close after an error occured?",
					"Continue closing?", MessageBoxButton.YesNo);
					return (result2 != MessageBoxResult.No);
				}
				return result;
			}
			return true;
		}

		public static void Undo() {
			var content = GetActiveContent();
			if (content != null) content.Undo();
			CommandManager.InvalidateRequerySuggested();
		}

		public static void Redo() {
			var content = GetActiveContent();
			if (content != null) content.Redo();
			CommandManager.InvalidateRequerySuggested();
		}

		public static void Close() {
			bool errorOccurred = false;
			bool result = RequestSaveAll(out errorOccurred);
			if (errorOccurred) {
				MessageBoxResult result2 = TriggerMessageBox.Show(mainWindow, MessageIcon.Question, "Would you still like to close the project after an error occured?",
					"Continue closing?", MessageBoxButton.YesNo);
				result = (result2 != MessageBoxResult.No);
			}
			if (result) {
				foreach (IRequestClosePanel anchorable in openAnchorables) {
					if (anchorable is RequestCloseDocument) {
						anchorable.ForceClose();
					}
				}
				if (mainWindow.ProjectExplorer != null)
					mainWindow.ProjectExplorer.Cleanup();
				if (mainWindow.OutputTerminal != null)
					mainWindow.OutputTerminal.Clear();
				project = null;
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public static void OpenProject(string path) {
			try {
				ContentRoot newProject = new ContentRoot();
				newProject.LoadContentProject(path);
				project = newProject;
				mainWindow.OpenProjectExplorer();
				mainWindow.OpenOutputConsole();
				if (mainWindow.ProjectExplorer != null)
					mainWindow.ProjectExplorer.Project = newProject;
				CommandManager.InvalidateRequerySuggested();
			}
			catch (Exception ex) {
				ShowExceptionMessage(ex, "open", Path.GetFileName(path));
			}
		}

		public static void Open() {
			if (IsProjectOpen) {
				Close();
			}
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Content Project Files|*.contentproj";
			dialog.FilterIndex = 0;
			dialog.CheckFileExists = true;
			var result = dialog.ShowDialog(mainWindow);
			if (result.HasValue && result.Value) {
				OpenProject(dialog.FileName);
			}
		}

		public static void Save() {
			var content = GetActiveContent();
			try {
				if (content != null) content.Save();
			}
			catch (Exception ex) {
				var result = TriggerMessageBox.Show(mainWindow, MessageIcon.Error, "An error occurred while trying to save '" +
					content.ContentFile.Name + "'! Would you like to see the error?", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
					ErrorMessageBox.Show(ex, true);
			}
			CommandManager.InvalidateRequerySuggested();
		}

		public static void SaveAll() {
			foreach (var content in GetModifiedContent()) {
				content.Save();
			}
			project.SaveContentProject();
			CommandManager.InvalidateRequerySuggested();
		}

		public static void RunConscripts() {
			if (busyTask == null) {
				busyTask = Task.Run(() => { return RunConscriptsTask(); });
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public static void CompileContent() {
			if (busyTask == null) {
				busyTask = Task.Run(() => { return CompileContentTask(); });
				CommandManager.InvalidateRequerySuggested();
			}
		}


		//-----------------------------------------------------------------------------
		// Command Properties
		//-----------------------------------------------------------------------------

		public static bool CanSave {
			get {
				var content = GetActiveContent();
				return (content != null ? content.IsModified : false);
			}
		}

		public static bool CanUndo {
			get {
				var content = GetActiveContent();
				return (content != null ? content.CanUndo : false);
			}
		}

		public static bool CanRedo {
			get {
				var content = GetActiveContent();
				return (content != null ? content.CanRedo : false);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		public static string DesignerContentDirectory {
			get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), contentManager.RootDirectory); }
		}

		public static MainWindow MainWindow {
			get { return mainWindow; }
		}

		public static ContentRoot Project {
			get { return project; }
		}

		public static GraphicsDevice GraphicsDevice {
			get { return graphicsDevice; }
			set { graphicsDevice = value; }
		}

		public static ContentManager ContentManager {
			get { return contentManager; }
			set { contentManager = value; }
		}

		public static ClipboardListener ClipboardListener {
			get { return clipboardListener; }
		}

		public static bool IsBusy {
			get { return busyTask != null; }
		}

		public static bool IsInTextEditor {
			get { return GetActiveContent() is ConscriptEditor; }
		}

		public static bool IsProjectOpen {
			get { return project != null; }
		}
	}
}
