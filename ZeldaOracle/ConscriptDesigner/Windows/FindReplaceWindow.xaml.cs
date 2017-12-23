using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;
using ConscriptDesigner.Controls.TextEditing;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.NRefactory;
using ZeldaOracle.Common.Util;

namespace ConscriptDesigner.Windows {
	/// <summary>
	/// Interaction logic for FindAndReplaceDialog.xaml
	/// </summary>
	public partial class FindReplaceWindow : Window {

		//-----------------------------------------------------------------------------
		// Classes
		//-----------------------------------------------------------------------------

		/// <summary>A simple start/length selection.</summary>
		private struct Selection {

			//-----------------------------------------------------------------------------
			// Constants
			//-----------------------------------------------------------------------------

			/// <summary>An empty selection with a start offset and length of zero.</summary>
			public static readonly Selection Empty = new Selection();
			/// <summary>An selection using the invalid identifier.</summary>
			public static readonly Selection Invalid = new Selection() { Start = -1 };


			//-----------------------------------------------------------------------------
			// Members
			//-----------------------------------------------------------------------------

			/// <summary>The start offset of the selection.</summary>
			public int Start { get; set; }
			/// <summary>The length of the selection.</summary>
			public int Length { get; set; }
			/// <summary>The end of the selection.</summary>
			public int End {
				get { return Start + Length; }
				set { Length = value - Start; }
			}
			/// <summary>Returns true if the start is using an invalid identifier.</summary>
			public bool IsInvalid {
				get { return Start == -1; }
			}


			//-----------------------------------------------------------------------------
			// Static Constructors
			//-----------------------------------------------------------------------------

			/// <summary>Creates a selection from the text editor's selection.</summary>
			public static Selection FromTextEditor(TextEditor editor) {
				Selection selection = new Selection();
				selection.Start = editor.SelectionStart;
				selection.Length = editor.SelectionLength;
				return selection;
			}

			/// <summary>Creates a selection from a match.</summary>
			public static Selection FromMatch(Match match) {
				Selection selection = new Selection();
				selection.Start = match.Index;
				selection.Length = match.Length;
				return selection;
			}

			/// <summary>Creates a selection from a start offset and length.</summary>
			public static Selection FromStartLength(int start, int length) {
				Selection selection = new Selection();
				selection.Start = start;
				selection.Length = length;
				return selection;
			}

			/// <summary>Creates a selection from a start offset and no length.</summary>
			public static Selection FromStart(int start) {
				Selection selection = new Selection();
				selection.Start = start;
				return selection;
			}
		}

		/// <summary>A scope for searching the project in.</summary>
		private enum Scopes {
			None = -1,
			[Description("Current Document")]
			CurrentDocument,
			//[Description("Selection")]
			//Selection,
			[Description("All Open Documents")]
			AllOpenDocuments,
			[Description("Entire Project")]
			EntireProject
		}


		//-----------------------------------------------------------------------------
		// Static Members
		//-----------------------------------------------------------------------------

		private static bool lastCaseSensitive = true;
		private static bool lastWholeWord = false;
		private static bool lastRegex = false;
		private static bool lastWrapSearch = true;
		private static bool lastSearchUp = false;
		private static bool lastLiveSearch = true;
		private static string lastReplaceText = "";
		private static Scopes lastScope = Scopes.CurrentDocument;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The current script being hooked into.</summary>
		private ContentScript lastScript;
		/// <summary>The colorizer for search results.</summary>
		private ColorizeSearchResultsBackgroundRenderer searchColorizor;
		/// <summary>Used as a hack to focus on the find text box after changing modes.</summary>
		private DispatcherTimer focusTimer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the find and replace window.</summary>
		public FindReplaceWindow(bool replace) {
			InitializeComponent();

			checkBoxCaseSensitive.IsChecked = lastCaseSensitive;
			checkBoxWholeWord.IsChecked = lastWholeWord;
			checkBoxRegex.IsChecked = lastRegex;
			checkBoxWrapSearch.IsChecked = lastWrapSearch;
			checkBoxSearchUp.IsChecked = lastSearchUp;
			checkBoxLiveSearch.IsChecked = lastLiveSearch;
			textBoxReplace.Text = lastReplaceText;

			searchColorizor = new ColorizeSearchResultsBackgroundRenderer();

			Scopes[] scopes = (Scopes[])Enum.GetValues(typeof(Scopes));
			foreach (Scopes scope in scopes) {
				if (scope == Scopes.None)
					continue;
				ComboBoxItem item = new ComboBoxItem();
				item.Content = scope.ToDescription();
				item.Tag = scope;
				comboBoxScope.Items.Add(item);
				if (scope == lastScope)
					comboBoxScope.SelectedItem = item;
			}

			ContentScript script = ActiveScript;
			if (script != null && script.IsOpen) {
				textBoxFind.Text = script.TextEditor.TextArea.Selection.GetText();
			}

			focusTimer = new DispatcherTimer(
				TimeSpan.FromSeconds(0.05),
				DispatcherPriority.ApplicationIdle,
				delegate {
					if (tabControl.SelectedIndex == 0)
						textBoxFind.Focus();
					else
						textBoxReplaceFind.Focus();
					focusTimer.Stop();
				}, Dispatcher);
			focusTimer.Stop();

			if (replace)
				ReplaceMode();
			else
				FindMode();

			DesignerControl.ActiveAnchorableChanged += OnActiveAnchorableChanged;
			lastScript = null;
			OnActiveAnchorableChanged();
		}


		//-----------------------------------------------------------------------------
		// Showing
		//-----------------------------------------------------------------------------

		/// <summary>Shows and returns the find and replace window.</summary>
		public static FindReplaceWindow Show(Window owner, bool replace, EventHandler onClose) {
			FindReplaceWindow window = new FindReplaceWindow(replace);
			window.Owner = owner;
			window.Show();
			window.Closed += onClose;
			return window;
		}


		//-----------------------------------------------------------------------------
		// Find and Replace
		//-----------------------------------------------------------------------------

		/// <summary>Switches to find mode.</summary>
		public void FindMode() {
			tabControl.SelectedIndex = 0;
			// HACK: Focus on the textbox after a split second.
			// Otherwise the tabitem will steal focus.
			focusTimer.Start();
		}

		/// <summary>Switches to replace mode.</summary>
		public void ReplaceMode() {
			tabControl.SelectedIndex = 1;
			// HACK: Focus on the textbox after a split second.
			// Otherwise the tabitem will steal focus.
			focusTimer.Start();
		}

		/// <summary>Finds the next search result.</summary>
		public void FindNext() {
			string find = SearchText;
			if (string.IsNullOrEmpty(find)) {
				SystemSounds.Asterisk.Play();
				return;
			}

			ContentScript script;
			Selection selection;
			if (FindNext(find, out script, out selection)) {
				NavigateToSelection(script, selection);
			}
			else {
				SystemSounds.Asterisk.Play();
			}
		}

		/// <summary>Replaces the next search result.</summary>
		public void ReplaceNext() {
			if (tabControl.SelectedIndex != 1)
				return;

			string find = textBoxReplaceFind.Text;
			string replace = textBoxReplace.Text;
			if (string.IsNullOrEmpty(find)) {
				SystemSounds.Asterisk.Play();
				return;
			}


			ContentFile startFile = DesignerControl.GetActiveContentFile();
			Selection startSelection = Selection.Invalid;
			if (startFile == null)
				startFile = FindNextScript();
			else if (startFile is ContentScript) {
				ContentScript startScript = (ContentScript) startFile;
				if (startScript.IsOpen) {
					TextEditor editor = startScript.TextEditor;
					Regex regex = GetRegex(find);
					string input = editor.TextArea.Selection.GetText();
					int start = editor.SelectionStart;
					Match match = regex.Match(input);
					if (match.Success && match.Index == 0 && match.Length == input.Length) {
						editor.Document.Replace(editor.SelectionStart,
							editor.SelectionLength, replace);
					}
					startSelection = Selection.FromStartLength(start, replace.Length);
				}
			}

			if (startFile == null) {
				SystemSounds.Asterisk.Play();
				return;
			}

			ContentScript script;
			Selection selection;
			if (FindNext(find, out script, out selection, startFile,
				startSelection, replace: true)) {
				NavigateToSelection(script, selection);
			}
			else {
				SystemSounds.Asterisk.Play();
			}
		}

		/// <summary>Replaces all matching results.</summary>
		public void ReplaceAll() {
			if (tabControl.SelectedIndex != 1)
				return;

			string find = textBoxReplaceFind.Text;
			string replace = textBoxReplace.Text;
			if (string.IsNullOrEmpty(find)) {
				SystemSounds.Asterisk.Play();
				return;
			}

			int replaceCount = 0;
			ContentScript startScript = null;
			ContentScript lastScript = null;
			ContentFile startFile = DesignerControl.GetActiveContentFile();
			if (startFile == null)
				startFile = FindNextScript(replaceAll: true);
			Selection startSelection = Selection.Invalid;
			if (startFile is ContentScript) {
				startScript = (ContentScript) startFile;
				if (startScript.IsOpen) {
					TextEditor editor = startScript.TextEditor;
					Regex regex = GetRegex(find);
					string input = editor.TextArea.Selection.GetText();
					int start = editor.SelectionStart;
					Match match = regex.Match(input);
					if (match.Success && match.Index == 0 && match.Length == input.Length) {
						editor.BeginChange();
						editor.Document.Replace(editor.SelectionStart,
							editor.SelectionLength, replace);
						startSelection = Selection.FromStartLength(start, replace.Length);
						lastScript = startScript;
						replaceCount++;
					}
				}
			}

			if (startFile == null) {
				SystemSounds.Asterisk.Play();
				return;
			}

			ContentFile currentFile = startFile;
			ContentScript script;
			Selection selection;
			Selection currentSelection = startSelection;
			while (FindNext(find, out script, out selection, startFile, startSelection,
				currentFile, currentSelection, true)) {
				if (script != lastScript) {
					if (lastScript != null) {
						if (!lastScript.IsOpen)
							lastScript.Save(false);
						else
							lastScript.TextEditor.EndChange();
					}
					if (script.IsOpen) {
						script.TextEditor.BeginChange();
					}
				}
				if (script.IsOpen) {
					TextEditor editor = script.TextEditor;
					editor.Document.Replace(selection.Start, selection.Length, replace);
					currentSelection = Selection.FromStartLength(selection.Start, replace.Length);
				}
				else {
					script.LoadedText = script.LoadedText.Remove(selection.Start, selection.Length);
					script.LoadedText = script.LoadedText.Insert(selection.Start, replace);
					currentSelection = Selection.FromStartLength(selection.Start, replace.Length);
				}
				if (startSelection.IsInvalid)
					startSelection = currentSelection;
				currentFile = script;
				lastScript = script;
				replaceCount++;
			}
			if (lastScript != null && lastScript.IsOpen) {
				lastScript.TextEditor.EndChange();
			}
			TriggerMessageBox.Show(this, MessageIcon.Info, "Replaced " + replaceCount + " match" +
				(replaceCount != 1 ? "es" : "") + ".", "Replace All");
			if (startScript != null && startScript.IsOpen && replaceCount > 0) {
				startScript.TextEditor.SelectionLength = 0;
			}
		}


		//-----------------------------------------------------------------------------
		// Commands Execution
		//-----------------------------------------------------------------------------

		private void OnFindCommand(object sender, ExecutedRoutedEventArgs e) {
			FindMode();
		}

		private void OnReplaceCommand(object sender, ExecutedRoutedEventArgs e) {
			ReplaceMode();
		}

		private void OnFindNextCommand(object sender, ExecutedRoutedEventArgs e) {
			FindNext();
		}

		private void OnReplaceNextCommand(object sender, ExecutedRoutedEventArgs e) {
			ReplaceNext();
		}

		private void OnReplaceAllCommand(object sender, ExecutedRoutedEventArgs e) {
			ReplaceAll();
		}


		//-----------------------------------------------------------------------------
		// Commands Can Execute
		//-----------------------------------------------------------------------------

		private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------
		
		private void OnClosing(object sender, CancelEventArgs e) {
			lastCaseSensitive = checkBoxCaseSensitive.IsChecked == true;
			lastWholeWord = checkBoxWholeWord.IsChecked == true;
			lastRegex = checkBoxRegex.IsChecked == true;
			lastWrapSearch = checkBoxWrapSearch.IsChecked == true;
			lastSearchUp = checkBoxSearchUp.IsChecked == true;
			lastLiveSearch = checkBoxLiveSearch.IsChecked == true;
			lastReplaceText = textBoxReplace.Text;
			lastScope = Scope;
			if (lastScript != null && lastScript.IsOpen)
				lastScript.TextEditor.TextArea.TextView.BackgroundRenderers.Remove(searchColorizor);
		}

		private void OnActiveAnchorableChanged(object sender = null, EventArgs e = null) {
			ContentScript newScript = ActiveScript;
			if (lastScript != newScript) {
				UnhookScript(lastScript);
				HookScript(newScript);
				lastScript = newScript;
				if (newScript != null)
					UpdateSearch();
			}
		}

		private void OnTextEditorTextChanged(object sender, EventArgs e) {
			string find = textBoxFind.Text;
			if (!string.IsNullOrEmpty(find)) {
				UpdateSearch();
			}
		}

		private void OnFindNext(object sender = null, RoutedEventArgs e = null) {
			FindNext();
		}

		private void OnReplaceNext(object sender = null, RoutedEventArgs e = null) {
			ReplaceNext();
		}

		private void OnReplaceAll(object sender = null, RoutedEventArgs e = null) {
			ReplaceAll();
		}

		private void OnFindTextChanged(object sender, TextChangedEventArgs e) {
			UpdateSearch();
			FindFirstResult();
		}

		private void OnCheckboxChanged(object sender, RoutedEventArgs e) {
			UpdateSearch();
			FindFirstResult();
		}

		private void OnScopeChanged(object sender, SelectionChangedEventArgs e) {

		}

		
		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Hooks the scripts text editor.</summary>
		private void HookScript(ContentScript script) {
			if (script == null) return;
			script.TextEditor.TextChanged += OnTextEditorTextChanged;
			script.TextEditor.TextArea.TextView.BackgroundRenderers.Add(searchColorizor);
		}

		/// <summary>Unhooks the scripts text editor.</summary>
		private void UnhookScript(ContentScript script) {
			if (script == null) return;
			script.TextEditor.TextChanged -= OnTextEditorTextChanged;
			script.TextEditor.TextArea.TextView.BackgroundRenderers.Remove(searchColorizor);
		}

		/// <summary>Finds the next script in the list.</summary>
		private ContentScript FindNextScript(ContentFile active = null, ContentFile startFile = null,
			bool replaceAll = false, Scopes scope = Scopes.None) {
			if (active == null) {
				active = DesignerControl.GetActiveContentFile();
			}
			if (scope == Scopes.None)
				scope = Scope;
			if (scope != Scopes.AllOpenDocuments && scope != Scopes.EntireProject)
				return active as ContentScript;
			ContentScript result = null;
			if (active == null) {
				if (DesignerControl.Project.LocalFileCount > 0)
					result = FindNextScriptSearchUp(DesignerControl.Project, startFile ?? null, scope);
			}
			else {
				result = FindNextScriptSearchDown(active.Parent, active, startFile ?? active, scope);
				// Search from the opposite end back to the active file if we found nothing
				if (result == null && (checkBoxWrapSearch.IsChecked == true || replaceAll))
					result = FindNextScriptSearchUp(DesignerControl.Project, startFile ?? active, scope);
			}
			return result;
		}

		/// <summary>Finds the next script by searching down through folders from the starting file.</summary>
		private ContentScript FindNextScriptSearchDown(ContentFolder folder, ContentFile startChild,
			ContentFile startFile, Scopes scope) {
			bool searchUp = checkBoxSearchUp.IsChecked == true;
			int startIndex = folder.IndexOfFile(startChild);
			startIndex += (searchUp ? -1 : 1);
			ContentScript result = null;
			if (startIndex >= 0 && startIndex < folder.LocalFileCount)
				result = FindNextScriptSearchUp(folder, startFile, scope, startIndex);
			if (result == null && folder.Parent != null) {
				result = FindNextScriptSearchDown(folder.Parent, folder, startFile, scope);
			}
			return result;
		}

		/// <summary>Finds the next script by searching up through a folder and subfolders.</summary>
		private ContentScript FindNextScriptSearchUp(ContentFolder folder, ContentFile startFile,
			Scopes scope, int startIndex = -1) {
			bool searchUp = checkBoxSearchUp.IsChecked == true;
			ContentScript result = null;
			if (searchUp) {
				if (startIndex == -1)
					startIndex = folder.LocalFileCount - 1;
				for (int i = startIndex; i >= 0 && result == null; i--) {
					ContentFile file = folder.GetFileAt(i);
					if (file == startFile)
						return startFile as ContentScript;
					if (file is ContentScript) {
						if (file.IsOpen || scope == Scopes.EntireProject)
							return (ContentScript) file;
					}
					else if (file.IsFolder)
						result = FindNextScriptSearchUp((ContentFolder) file, startFile, scope);
				}
			}
			else {
				if (startIndex == -1)
					startIndex = 0;
				for (int i = startIndex; i < folder.LocalFileCount && result == null; i++) {
					ContentFile file = folder.GetFileAt(i);
					if (file == startFile)
						return startFile as ContentScript;
					if (file is ContentScript) {
						if (file.IsOpen || scope == Scopes.EntireProject)
							return (ContentScript) file;
					}
					else if (file.IsFolder)
						result = FindNextScriptSearchUp((ContentFolder) file, startFile, scope);
				}
			}
			return result;
		}

		/// <summary>Finds the next search result.</summary>
		private bool FindNext(string searchText, out ContentScript script, out Selection selection,
			ContentFile startFile = null, Selection? startSelectionIn = null, ContentFile currentFile = null,
			Selection? currentSelectionIn = null, bool replaceAll = false, bool replace = false,
			Scopes scope = Scopes.None) {
			selection = Selection.Empty;
			script = null;

			Match match = Match.Empty;
			Regex regex = GetRegex(searchText);
			bool searchUp = regex.Options.HasFlag(RegexOptions.RightToLeft);

			if (replaceAll) {
				replace = true;
				searchUp = false;
			}

			Selection currentSelection =
				(currentSelectionIn.HasValue ? currentSelectionIn.Value : Selection.Invalid);
			Selection startSelection =
				(startSelectionIn.HasValue ? startSelectionIn.Value : Selection.Invalid);
			script = startFile as ContentScript;
			if (currentFile != null)
				script = currentFile as ContentScript;
			if (currentFile == null) {
				script = ActiveScript;
				currentFile = script;
				if (script != null) {
					if (!replaceAll && currentSelection.IsInvalid)
						currentSelection = Selection.FromTextEditor(script.TextEditor);
				}
				else {
					script = FindNextScript(replaceAll: replaceAll, scope: scope);
					currentFile = script;
				}
			}
			else if (script != null) {
				if (script.IsOpen && !replaceAll && currentSelection.IsInvalid) {
					currentSelection = Selection.FromTextEditor(script.TextEditor);
				}
			}

			if (startFile == null)
				startFile = currentFile;

			if (script == null)
				return false;


			int startEnd = -1;
			int currentEnd = 0;
			bool loaded = script.LoadedText != null || script.IsOpen;
			if (!loaded) {
				loaded = script.LoadText();
			}
			if (loaded) {
				if (currentSelection.IsInvalid) {
					if (searchUp)
						currentSelection = Selection.FromStart(script.LoadedText.Length);
					else
						currentSelection = Selection.FromStart(0);
				}
				int start = (searchUp == replace) ? currentSelection.Start : currentSelection.End;
				currentEnd = start;
				if (!replace && currentSelection.Length > 0) {
					if (searchUp)
						start = Math.Max(0, start - 1);
					else
						start = Math.Min(script.LoadedText.Length, start + 1);
				}
				if (!startSelection.IsInvalid)
					startEnd = (searchUp == replace) ? startSelection.Start : startSelection.End;
				match = regex.Match(script.LoadedText, start);
				if (!replace) {
					currentEnd = Math.Max(0, currentEnd - 1);
					if (startEnd != -1)
						startEnd = Math.Max(0, startEnd - 1);
				}
				if (match.Success) {
					if (searchUp && match.Index + match.Length > startEnd && startEnd != -1) { }
					else if (!searchUp && match.Index < startEnd && startEnd != -1) { }
					if (searchUp && match.Index + match.Length > currentEnd) { }
					else if (!searchUp && match.Index < currentEnd) { }
					else {
						selection = Selection.FromMatch(match);
						return true;
					}
				}

				currentEnd = searchUp ? currentSelection.End : currentSelection.End;
				if (!startSelection.IsInvalid)
					startEnd = (searchUp == replace) ? startSelection.End : startSelection.Start;
			}
			script = FindNextScript(currentFile, startFile, replaceAll, scope);
			currentSelection = Selection.Invalid;


			while (script != null) {
				loaded = script.LoadedText != null || script.IsOpen;
				if (!loaded) {
					if (script == startFile)
						return false;
					loaded = script.LoadText();
				}
				if (loaded) {
					int start = searchUp ? script.LoadedText.Length : 0;
					match = regex.Match(script.LoadedText, start);
					if (match.Success) {
						if (script == startFile && replace) {
							if (searchUp && match.Index < startEnd && startEnd != -1)
								return false;
							if (!searchUp && match.Index + match.Length > startEnd && startEnd != -1)
								return false;
							if (searchUp && match.Index < currentEnd)
								return false;
							if (!searchUp && match.Index + match.Length > currentEnd)
								return false;
						}
						selection = Selection.FromMatch(match);
						return true;
					}
				}
				if (script == startFile)
					return false;
				script = FindNextScript(script, startFile, replaceAll, scope);
			}

			return false;
		}

		/// <summary>Navigates to the selection in the text editor.</summary>
		private void NavigateToSelection(ContentScript script, Selection selection) {
			if (!script.IsOpen) {
				if (!script.Open(false))
					return;
			}
			script.SelectText(selection.Start, selection.Length, false);
		}

		/// <summary>Constructs the regex from the checkbox settings.</summary>
		private Regex GetRegex(string textToFind, bool leftToRight = false) {
			RegexOptions options = RegexOptions.None;
			if (checkBoxSearchUp.IsChecked == true && !leftToRight)
				options |= RegexOptions.RightToLeft;
			if (checkBoxCaseSensitive.IsChecked == false)
				options |= RegexOptions.IgnoreCase;

			if (checkBoxRegex.IsChecked == true) {
				return new Regex(textToFind, options);
			}
			else {
				string pattern = Regex.Escape(textToFind);
				if (checkBoxWholeWord.IsChecked == true)
					pattern = "\\b" + pattern + "\\b";
				return new Regex(pattern, options);
			}
		}

		/// <summary>Used for live searching while typing.</summary>
		private void FindFirstResult() {
			if (lastScript != null && checkBoxLiveSearch.IsChecked == true) {
				TextEditor editor = lastScript.TextEditor;
				string find = SearchText;
				ContentScript script;
				Selection selection;
				Selection currentSelection = Selection.FromStart(editor.SelectionStart);
				if (FindNext(find, out script, out selection, currentFile: lastScript,
					startSelectionIn: currentSelection, currentSelectionIn: currentSelection,
					scope: Scopes.CurrentDocument)) {
					lastScript.SelectText(selection.Start, selection.Length, false);
				}
			}
		}

		/// <summary>Updates the search results and highlighting.</summary>
		private void UpdateSearch() {
			string find = SearchText;
			Regex regex = null;
			searchColorizor.CurrentResults.Clear();
			if (!string.IsNullOrEmpty(find) && lastScript != null) {
				regex = GetRegex(find);
				string text = lastScript.LoadedText;
				Match match = regex.Match(text);

				while (match.Success) {
					searchColorizor.CurrentResults.Add(new SearchResult(match));
					match = regex.Match(text, match.Index + 1);
				}
			}

			if (lastScript != null && lastScript.IsOpen) {
				lastScript.TextEditor.TextArea.TextView.InvalidateLayer(KnownLayer.Selection);
			}
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the search text.</summary>
		private string SearchText {
			get {
				if (tabControl.SelectedIndex == 0)
					return textBoxFind.Text;
				else
					return textBoxReplaceFind.Text;
			}
		}

		/// <summary>Gets the active anchorable as a content script.</summary>
		private ContentScript ActiveScript {
			get { return DesignerControl.GetActiveContentFile() as ContentScript; }
		}

		/// <summary>Gets the current find scope.</summary>
		private Scopes Scope {
			get { return (Scopes) ((ComboBoxItem) comboBoxScope.SelectedItem).Tag; }
		}
	}
}
