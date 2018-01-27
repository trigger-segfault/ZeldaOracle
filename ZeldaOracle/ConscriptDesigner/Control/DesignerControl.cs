using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripts;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Worlds;

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
		private static Thread busyThread;
		private static bool busyTaskIsConscripts;
		private static ScriptReaderException lastScriptError;
		private static DispatcherTimer updateTimer;
		private static DispatcherTimer modifiedTimer;
		private static List<IRequestCloseAnchorable> openAnchorables;
		private static List<IRequestCloseAnchorable> closingAnchorables;

		private static RewardManager rewardManager;

		private static Zone previewZone;
		private static string previewZoneID;
		private static ObservableCollection<string> previewZones;
		private static Stopwatch animationWatch;
		private static int previewScale;
		private static DispatcherTimer animationTimer;

		private static string resourceAutoCompleteType;
		private static bool playAnimations;

		private static Tileset selectedTileset;
		private static BaseTileData selectedTileData;
		private static Point2I selectedTileLocation;


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		public static void Initialize(MainWindow mainWindow) {
			GameSettings.DesignerMode = true;
			DesignerControl.mainWindow = mainWindow;
			mainWindow.ActiveAnchorableChanged += delegate {
				if (ActiveAnchorableChanged != null)
					ActiveAnchorableChanged(null, EventArgs.Empty);
			};
			openAnchorables = new List<IRequestCloseAnchorable>();
			closingAnchorables = new List<IRequestCloseAnchorable>();
			updateTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.05), DispatcherPriority.Render, Update, Application.Current.Dispatcher);
			modifiedTimer = new DispatcherTimer(TimeSpan.FromSeconds(3), DispatcherPriority.ApplicationIdle, delegate { CheckForOutdatedFiles(); }, Application.Current.Dispatcher);
			animationTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.05), DispatcherPriority.Render, Update, Application.Current.Dispatcher);

			previewZone = null;
			previewZoneID = "default";
			previewZones = new ObservableCollection<string>();
			resourceAutoCompleteType = "";
			playAnimations = false;
			animationWatch = Stopwatch.StartNew();
			previewScale = 1;
			selectedTileset = null;
			selectedTileData = null;
			selectedTileLocation = -Point2I.One;
		}

		public static void SetGraphics(GraphicsDevice graphicsDevice, ContentManager contentManager) {
			DesignerControl.graphicsDevice = graphicsDevice;
			DesignerControl.contentManager = contentManager;
			if (IsProjectOpen) {
				RunConscripts();
			}
		}

		//-----------------------------------------------------------------------------
		// Events
		//-----------------------------------------------------------------------------

		public static event EventHandler FinishedBuilding;

		public static event EventHandler ProjectOpened;
		public static event EventHandler ProjectClosed;
		public static event EventHandler ResourcesUnloaded;
		public static event EventHandler ResourcesLoaded;

		public static event EventHandler ActiveAnchorableChanged;

		public static event EventHandler PreviewScaleChanged;
		public static event EventHandler PreviewInvalidated;

		//-----------------------------------------------------------------------------
		// Anchorables
		//-----------------------------------------------------------------------------

		public static void DockDocument(RequestCloseDocument document) {
			mainWindow.DockDocument(document);
		}

		public static IRequestCloseAnchorable GetActiveAnchorable() {
			return mainWindow.ActiveAnchorable;
		}

		public static ContentFile GetActiveContentFile() {
			IRequestCloseAnchorable anchorable = mainWindow.ActiveAnchorable;
			if (anchorable is IContentFileContainer) {
				return ((IContentFileContainer) anchorable).File;
			}
			return null;
		}

		public static ICommandAnchorable GetActiveCommandAnchorable() {
			IRequestCloseAnchorable anchorable = mainWindow.ActiveAnchorable;
			if (anchorable is ICommandAnchorable) {
				return (ICommandAnchorable) anchorable;
			}
			return null;
		}

		public static IEnumerable<IRequestCloseAnchorable> GetOpenAnchorables() {
			return openAnchorables;
		}

		public static IEnumerable<ContentFile> GetOpenContentFiles() {
			foreach (var anchorable in openAnchorables) {
				if (anchorable is IContentFileContainer)
					yield return ((IContentFileContainer) anchorable).File;
			}
		}

		public static IEnumerable<ContentFile> GetModifiedContentFiles() {
			foreach (ContentFile file in project.GetAllFiles()) {
				if (file.IsModified)
					yield return file;
			}
		}
		
		public static void AddOpenAnchorable(IRequestCloseAnchorable anchorable) {
			openAnchorables.Add(anchorable);
		}

		public static void RemoveOpenAnchorable(IRequestCloseAnchorable anchorable) {
			openAnchorables.Remove(anchorable);
		}

		public static void AddClosingAnchorable(IRequestCloseAnchorable anchorable) {
			closingAnchorables.Add(anchorable);
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		private static void Update(object sender, EventArgs e) {
			if (busyTask != null) {
				if (busyTask.IsCompleted) {
					if (busyTaskIsConscripts)
						lastScriptError = busyTask.Result;
					busyTask = null;
					busyThread = null;
					CommandManager.InvalidateRequerySuggested();
					if (FinishedBuilding != null)
						FinishedBuilding(null, EventArgs.Empty);
					if (busyTaskIsConscripts) {
						LoadPreviewZones();
						if (ResourcesLoaded != null)
							ResourcesLoaded(null, EventArgs.Empty);
					}
				}
			}
			if (closingAnchorables.Any()) {
				List<ContentFile> needsSaving = new List<ContentFile>();
				List<string> needsSavingFiles = new List<string>();
				bool activeClosing = false;
				bool tilesetEditorNeedsSaving = false;
				List<IRequestCloseAnchorable> newClosingAnchorables = new List<IRequestCloseAnchorable>();
				foreach (IRequestCloseAnchorable anchorable in closingAnchorables) {
					newClosingAnchorables.Add(anchorable);
					if (anchorable.IsActive)
						activeClosing = true;
					if (anchorable is IContentFileContainer) {
						var content = (IContentFileContainer) anchorable;
						if (content.File != null && content.File.IsModified) {
							needsSaving.Add(content.File);
							needsSavingFiles.Add(content.File.Path);
						}
					}
					else if (anchorable is TilesetEditor) {
						TilesetEditor editor = (TilesetEditor) anchorable;
						if (editor.IsModified) {
							needsSavingFiles.Add("Tileset Editor");
							tilesetEditorNeedsSaving = true;
						}
					}
				}
				// Clear this now so that this doesn't get called again while in a dialog
				closingAnchorables.Clear();

				MessageBoxResult result = MessageBoxResult.Yes;
				bool errorOccurred = false;
				if (needsSavingFiles.Any()) {
					result = SaveChangesWindow.Show(mainWindow, needsSavingFiles, false);
					if (result == MessageBoxResult.Yes) {
						foreach (ContentFile file in needsSaving) {
							if (!file.Save(true))
								errorOccurred = true;
						}

						if (tilesetEditorNeedsSaving) {
							if (!mainWindow.TilesetEditor.Save(true))
								errorOccurred = true;
						}
					}
				}
				if (errorOccurred) {
					result = TriggerMessageBox.Show(mainWindow, MessageIcon.Error,
						"An error occurred while trying to save the modified files. " +
						"Would you still like to close them?", "Save Error", MessageBoxButton.YesNo);
					if (result == MessageBoxResult.No)
						result = MessageBoxResult.Cancel;
				}
				if (result != MessageBoxResult.Cancel) {
					foreach (IRequestCloseAnchorable anchorable in newClosingAnchorables) {
						anchorable.ForceClose();
					}
					if (activeClosing) {
						mainWindow.InvalidateActiveAnchorable();
					}
				}
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public static void CheckForOutdatedFiles(bool force = false) {
			if (IsProjectOpen && (mainWindow.IsActive || force)) {
				if (project.IsFileOutdated()) {
					var result = TriggerMessageBox.Show(mainWindow, MessageIcon.Warning,
						"The content project file has been modified outside the designer. " +
						"Would you like to reload the project?", "Reload Project", MessageBoxButton.YesNo);
					if (result == MessageBoxResult.Yes) {
						OpenProject(project.ProjectFile);
					}
					else {
						project.UpdateLastModified();
						project.IsProjectModified = true;
					}
				}
				else {
					List<ContentFile> files = new List<ContentFile>();
					List<string> fileNames = new List<string>();
					foreach (ContentFile file in project.GetAllFiles()) {
						if (file.IsOpen && file.IsFileOutdated()) {
							files.Add(file);
							fileNames.Add(file.Path);
						}
					}
					if (files.Any()) {
						bool errorOccurred = false;
						var result = SaveChangesWindow.Show(mainWindow, fileNames, true);
						foreach (ContentFile file in files) {
							if (result == MessageBoxResult.Yes) {
								if (!file.Reload(true)) {
									errorOccurred = true;
									file.IsModifiedOverride = true;
									file.UpdateLastModified();
								}
							}
							else {
								file.IsModifiedOverride = true;
								file.UpdateLastModified();
							}
						}
						if (errorOccurred) {
							TriggerMessageBox.Show(mainWindow, MessageIcon.Error, "An error occurred while " +
								"trying to save all modified files!", "Save Error");
						}
					}
				}
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
			busyThread = Thread.CurrentThread;
			mainWindow.Dispatcher.Invoke(() => SaveAll(true));
			if (mainWindow.OutputConsole != null)
				mainWindow.OutputConsole.Clear();
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
			catch (ThreadAbortException) { }
			catch (Exception ex) {
				Console.WriteLine(ex.Message);
				Console.WriteLine("Stack Trace:\n" + ex.StackTrace);
			}
			return null;
		}

		private static ScriptReaderException RunConscriptsTask() {
			busyThread = Thread.CurrentThread;
			mainWindow.Dispatcher.Invoke(() => SaveAll(true));
			if (mainWindow.OutputConsole != null)
				mainWindow.OutputConsole.Clear();
			Resources.Uninitialize();

			try {
				Stopwatch watch = Stopwatch.StartNew();
				UpdateContentFolder(project);

				Resources.Initialize(contentManager, graphicsDevice);
				rewardManager = new RewardManager(null);
				GameData.Initialize(rewardManager);

				//Console.WriteLine("Loading Rewards");
				//rewardManager = new RewardManager(null);
				//GameData.LoadRewards(rewardManager);

				Console.WriteLine("----------------------------------------------------------------");
				Console.WriteLine("Finished! Duration: " + watch.Elapsed.RoundUpToNearestSecond().ToString(@"hh\:mm\:ss"));
			}
			catch (ThreadAbortException) { }
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
					if (file.ShouldCopyToOutput /*&& File.GetLastWriteTimeUtc(inPath) != File.GetLastWriteTimeUtc(outPath)*/) {
						File.Copy(inPath, outPath, true);
					}
					existingFiles.Remove(outName);
				}
				else if (file.ShouldCopyToOutput) {
					File.Copy(inPath, outPath, true);
				}

				if (file.IsFolder) {
					if (!Directory.Exists(outPath))
						Directory.CreateDirectory(outPath);
					UpdateContentFolder((ContentFolder) file);
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
		
		private static void LoadPreviewZones() {
			List<string> sortedList = new List<string>();
			foreach (var pair in Resources.GetResourceDictionary<Zone>()) {
				sortedList.Add(pair.Key);
			}
			sortedList.Sort((a, b) => AlphanumComparator.Compare(a, b, true));
			previewZones.Clear();
			foreach (string zone in sortedList) {
				previewZones.Add(zone);
			}
			if (!Resources.ContainsResource<Zone>(previewZoneID)) {
				if (previewZones.Any())
					previewZoneID = previewZones[0];
			}
			previewZone = Resources.GetResource<Zone>(previewZoneID);
		}


		//-----------------------------------------------------------------------------
		// Commands
		//-----------------------------------------------------------------------------

		public static void InvalidatePreview() {
			if (PreviewInvalidated != null)
				PreviewInvalidated(null, EventArgs.Empty);
		}

		public static bool RequestSaveAll(out bool errorOccurred) {
			List<ContentFile> needsSaving = new List<ContentFile>();
			List<string> needsSavingFiles = new List<string>();
			if (project.IsProjectModified)
				needsSavingFiles.Add(project.Name);
			foreach (ContentFile file in GetModifiedContentFiles()) {
				needsSaving.Add(file);
				needsSavingFiles.Add(file.Path);
			}
			if (mainWindow.TilesetEditor != null && mainWindow.TilesetEditor.IsModified) {
				needsSavingFiles.Add("Tileset Editor");
			}
			MessageBoxResult result = MessageBoxResult.Yes;
			errorOccurred = false;
			if (needsSavingFiles.Any()) {
				result = SaveChangesWindow.Show(mainWindow, needsSavingFiles, false);
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
						foreach (ContentFile file in needsSaving) {
							try {
								if (!file.Save(true))
									errorOccurred = true;
							}
							catch (Exception) {
								//ShowExceptionMessage(ex, "save", file.Name);
								errorOccurred = true;
								break;
							}
						}
					}
					if (mainWindow.TilesetEditor != null && mainWindow.TilesetEditor.IsModified) {
						if (!mainWindow.TilesetEditor.Save(true))
							errorOccurred = true;
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
			var anchorable = GetActiveCommandAnchorable();
			if (anchorable != null) anchorable.Undo();
			CommandManager.InvalidateRequerySuggested();
		}

		public static void Redo() {
			var anchorable = GetActiveCommandAnchorable();
			if (anchorable != null) anchorable.Redo();
			CommandManager.InvalidateRequerySuggested();
		}

		public static void Cut() {
			var anchorable = GetActiveCommandAnchorable();
			if (anchorable != null) anchorable.Cut();
			CommandManager.InvalidateRequerySuggested();
		}

		public static void Copy() {
			var anchorable = GetActiveCommandAnchorable();
			if (anchorable != null) anchorable.Copy();
			CommandManager.InvalidateRequerySuggested();
		}

		public static void Paste() {
			var anchorable = GetActiveCommandAnchorable();
			if (anchorable != null) anchorable.Paste();
			CommandManager.InvalidateRequerySuggested();
		}

		public static void Delete() {
			var anchorable = GetActiveCommandAnchorable();
			if (anchorable != null) anchorable.Delete();
			CommandManager.InvalidateRequerySuggested();
		}

		public static void Close() {
			if (IsProjectOpen) {
				bool errorOccurred = false;
				bool result = RequestSaveAll(out errorOccurred);
				if (errorOccurred) {
					MessageBoxResult result2 = TriggerMessageBox.Show(mainWindow, MessageIcon.Question, "Would you still like to close the project after an error occured?",
					"Continue closing?", MessageBoxButton.YesNo);
					result = (result2 != MessageBoxResult.No);
				}
				if (result) {
					//mainWindow.SaveLayout();
					ProjectUserSettings.Save();
					while (openAnchorables.Any()) {
						openAnchorables[0].ForceClose();
					}
					if (mainWindow.OutputConsole != null)
						mainWindow.OutputConsole.Clear();
					if (mainWindow.ProjectExplorer != null)
						mainWindow.ProjectExplorer.Clear();
					project.Cleanup();
					project = null;
					if (ResourcesUnloaded != null)
						ResourcesUnloaded(null, EventArgs.Empty);
					if (ProjectClosed != null)
						ProjectClosed(null, EventArgs.Empty);
					Resources.Uninitialize();
					selectedTileset = null;
					selectedTileData = null;
					selectedTileLocation = -Point2I.One;
					CommandManager.InvalidateRequerySuggested();
				}
			}
		}

		public static void OpenProject(string path) {
			if (IsProjectOpen) {
				Close();
			}
			try {
				while (openAnchorables.Any()) {
					openAnchorables[0].ForceClose();
				}
				ContentRoot newProject = new ContentRoot();
				newProject.LoadContentProject(path);
				project = newProject;
				//mainWindow.LoadLayout();
				if (IsGraphicsLoaded)
					RunConscripts();
				if (ProjectOpened != null)
					ProjectOpened(null, EventArgs.Empty);
				if (!ProjectUserSettings.Load()) {
					mainWindow.OpenOutputConsole();
					mainWindow.OpenProjectExplorer();
				}
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
			var file = GetActiveContentFile();
			try {
				if (file != null) {
					file.Save(false);
				}
				else {
					var anchorable = GetActiveAnchorable();
					if (anchorable is TilesetEditor) {
						mainWindow.TilesetEditor.Save(false);
					}
				}
			}
			catch (Exception ex) {
				ShowExceptionMessage(ex, "save", file.Name);
			}
			CommandManager.InvalidateRequerySuggested();
		}

		public static void SaveAll(bool excludeProject = false) {
			bool errorOccurred = false;
			foreach (var file in GetModifiedContentFiles()) {
				if (!file.Save(true))
					errorOccurred = true;
			}
			if (mainWindow.TilesetEditor != null) {
				if (!mainWindow.TilesetEditor.Save(true))
					errorOccurred = true;
			}
			try {
				if (!excludeProject && project.IsProjectModified)
					project.SaveContentProject();
			}
			catch (Exception ex) {
				ShowExceptionMessage(ex, "save", project.Name);
			}
			if (errorOccurred) {
				TriggerMessageBox.Show(mainWindow, MessageIcon.Error, "An error occurred while " +
					"trying to save all files!", "Save Error");
			}
			CommandManager.InvalidateRequerySuggested();
		}

		public static void RunConscripts() {
			if (busyTask == null) {
				busyTaskIsConscripts = true;
				if (ResourcesUnloaded != null)
					ResourcesUnloaded(null, EventArgs.Empty);
				previewZones.Clear();
				previewZone = null;
				selectedTileset = null;
				selectedTileData = null;
				selectedTileLocation = -Point2I.One;
				busyTask = Task.Run(() => RunConscriptsTask());
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public static void CompileContent() {
			if (busyTask == null) {
				busyTaskIsConscripts = false;
				busyTask = Task.Run(() => CompileContentTask());
				CommandManager.InvalidateRequerySuggested();
			}
		}

		public static void CancelBuild() {
			if (busyTask != null && busyThread != null) {
				busyThread.Abort();
				busyThread = null;
				busyTask = null;
				CommandManager.InvalidateRequerySuggested();
				if (mainWindow.OutputConsole != null)
					mainWindow.OutputConsole.NewLine();
				Console.WriteLine("----------------------------------------------------------------");
				Console.WriteLine("Canceled!");
			}
		}

		public static void GotoError() {
			if (HasError) {
				ContentFile file = project.Get(lastScriptError.FileName);
				if (file != null) {
					ContentScript script = file as ContentScript;
					if (script == null) {
						TriggerMessageBox.Show(mainWindow, MessageIcon.Warning, "Cannot go to the error " +
						"location because the offending file is not a conscript!", "Not a Conscript");
					}
					else {
						script.GotoLocation(lastScriptError.LineNumber, lastScriptError.ColumnNumber);
					}
				}
				else {
					TriggerMessageBox.Show(mainWindow, MessageIcon.Warning, "Cannot go to the error " +
						"location because the offending file no longer exists in the project!",
						"Missing File");
				}
			}
		}

		public static void PlaySound(ContentSound sound) {
			mainWindow.PlaySound(sound);
		}

		public static void RestartAnimations() {
			animationWatch.Restart();
			if (PreviewInvalidated != null)
				PreviewInvalidated(null, EventArgs.Empty);
		}

		public static void LaunchGame() {
			SaveAll();
			UpdateContentFolder(project);
			Process.Start("ZeldaOracle.exe", "-dev");
		}

		public static void LaunchEditor() {
			SaveAll();
			UpdateContentFolder(project);
			Process.Start("ZeldaEditor.exe");
		}


		//-----------------------------------------------------------------------------
		// Command Properties
		//-----------------------------------------------------------------------------

		public static bool CanSave {
			get {
				var content = GetActiveContentFile();
				if (content != null) {
					return content.IsModified;
				}
				var anchorable = GetActiveAnchorable();
				if (anchorable is TilesetEditor) {
					return mainWindow.TilesetEditor.IsModified;
				}
				return false;
			}
		}

		public static bool CanUndo {
			get {
				var content = GetActiveContentFile();
				return (content != null ? content.CanUndo : false);
			}
		}

		public static bool CanRedo {
			get {
				var content = GetActiveContentFile();
				return (content != null ? content.CanRedo : false);
			}
		}

		public static bool CanCut {
			get {
				var anchorable = GetActiveCommandAnchorable();
				return (anchorable != null ? anchorable.CanCut : false);
			}
		}

		public static bool CanCopy {
			get {
				var anchorable = GetActiveCommandAnchorable();
				return (anchorable != null ? anchorable.CanCopy : false);
			}
		}

		public static bool CanPaste {
			get {
				var anchorable = GetActiveCommandAnchorable();
				return (anchorable != null ? anchorable.CanPaste : false);
			}
		}

		public static bool CanDelete {
			get {
				var anchorable = GetActiveCommandAnchorable();
				return (anchorable != null ? anchorable.CanDelete : false);
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
		}

		public static ContentManager ContentManager {
			get { return contentManager; }
		}

		public static bool IsBusy {
			get { return busyTask != null; }
		}

		public static bool IsInTextEditor {
			get { return GetActiveContentFile() is ContentScript; }
		}

		public static bool IsProjectOpen {
			get { return project != null; }
		}

		public static bool HasError {
			get { return lastScriptError != null; }
		}

		public static string ProjectSettingsFile {
			get { return project.ProjectFile + ".designer.user"; }
		}

		public static bool IsGraphicsLoaded {
			get { return graphicsDevice != null && contentManager != null; }
		}

		public static RewardManager RewardManager {
			get { return rewardManager; }
		}

		public static ObservableCollection<string> PreviewZones {
			get { return previewZones; }
		}

		public static string PreviewZoneID {
			get { return previewZoneID; }
			set {
				if (previewZoneID != value && value != null) {
					previewZoneID = value;
					previewZone = Resources.GetResource<Zone>(previewZoneID);
					if (PreviewInvalidated != null)
						PreviewInvalidated(null, EventArgs.Empty);
				}
			}
		}

		public static Zone PreviewZone {
			get { return previewZone; }
		}

		public static bool PlayAnimations {
			get { return playAnimations; }
			set {
				if (playAnimations != value) {
					playAnimations = value;
					if (value)
						animationWatch.Restart();
					else if (PreviewInvalidated != null)
						PreviewInvalidated(null, EventArgs.Empty);
				}
			}
		}

		public static float PlaybackTime {
			get {
				if (playAnimations)
					return (float) (animationWatch.Elapsed.TotalSeconds * 60.0);
				return 0f;
			}
		}

		public static int PreviewScale {
			get { return previewScale; }
			set {
				if (previewScale != value) {
					previewScale = value;
					if (PreviewScaleChanged != null)
						PreviewScaleChanged(null, EventArgs.Empty);
				}
			}
		}

		public static Tileset SelectedTileset {
			get { return selectedTileset; }
			set { selectedTileset = value; }
		}

		public static BaseTileData SelectedTileData {
			get { return selectedTileData; }
			set { selectedTileData = value; }
		}

		public static Point2I SelectedTileLocation {
			get { return selectedTileLocation; }
			set { selectedTileLocation = value; }
		}
	}
}
