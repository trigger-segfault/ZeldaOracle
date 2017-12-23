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
using ConscriptDesigner.Content;
using ConscriptDesigner.Control;
using ICSharpCode.AvalonEdit;
using ICSharpCode.NRefactory;
using ZeldaOracle.Common.Util;

namespace ConscriptDesigner.Windows {
	/// <summary>
	/// Interaction logic for FindAndReplaceDialog.xaml
	/// </summary>
	public partial class FindAndReplaceWindow : Window {

		private struct Selection {
			public int Start { get; set; }
			public int Length { get; set; }
			public int End {
				get { return Start + Length; }
				set { Length = value - Start; }
			}
			public bool IsInvalid {
				get { return Start == -1; }
			}

			public static Selection FromTextEditor(TextEditor editor) {
				Selection selection = new Selection();
				selection.Start = editor.SelectionStart;
				selection.Length = editor.SelectionLength;
				return selection;
			}
			public static Selection FromMatch(Match match) {
				Selection selection = new Selection();
				selection.Start = match.Index;
				selection.Length = match.Length;
				return selection;
			}
			public static Selection FromStartLength(int start, int length) {
				Selection selection = new Selection();
				selection.Start = start;
				selection.Length = length;
				return selection;
			}
			public static Selection FromStartEnd(int start, int end) {
				Selection selection = new Selection();
				selection.Start = start;
				selection.Length = end - start;
				return selection;
			}
			public static Selection FromEndLength(int end, int length) {
				Selection selection = new Selection();
				selection.Start = end - length;
				selection.Length = length;
				return selection;
			}
			public static Selection FromStart(int start) {
				Selection selection = new Selection();
				selection.Start = start;
				return selection;
			}
			public static Selection FromConscript(ContentScript script, bool searchUp) {
				if (script.IsOpen)
					return FromTextEditor(script.TextEditor);
				else if (searchUp)
					return FromStart(script.LoadedText.Length);
				else
					return FromStart(0);
			}

			public static readonly Selection Empty = new Selection();
			public static readonly Selection Invalid = new Selection() { Start = -1 };
		}

		private enum Scopes {
			[Description("Current Document")]
			CurrentDocument,
			//[Description("Selection")]
			//Selection,
			[Description("All Open Documents")]
			AllOpenDocuments,
			[Description("Entire Project")]
			EntireProject
		}

		private static bool lastCaseSensitive = true;
		private static bool lastWholeWord = false;
		private static bool lastRegex = false;
		private static bool lastWrapSearch = true;
		private static bool lastSearchUp = false;
		private static Scopes lastScope = Scopes.CurrentDocument;

		public FindAndReplaceWindow(bool replace) {
			InitializeComponent();

			checkBoxCaseSensitive.IsChecked = lastCaseSensitive;
			checkBoxWholeWord.IsChecked = lastWholeWord;
			checkBoxRegex.IsChecked = lastRegex;
			checkBoxWrapSearch.IsChecked = lastWrapSearch;
			checkBoxSearchUp.IsChecked = lastSearchUp;

			Scopes[] scopes = (Scopes[])Enum.GetValues(typeof(Scopes));
			foreach (Scopes scope in scopes) {
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

			if (replace)
				ReplaceMode();
			else
				FindMode();
		}

		private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			lastCaseSensitive = checkBoxCaseSensitive.IsChecked == true;
			lastWholeWord = checkBoxWholeWord.IsChecked == true;
			lastRegex = checkBoxRegex.IsChecked == true;
			lastWrapSearch = checkBoxWrapSearch.IsChecked == true;
			lastSearchUp = checkBoxSearchUp.IsChecked == true;
			lastScope = Scope;
		}


		private ContentScript ActiveScript {
			get { return DesignerControl.GetActiveContentFile() as ContentScript; }
		}
		
		private ContentScript FindNextScript(ContentFile active = null, ContentFile startFile = null, bool replaceAll = false) {
			if (active == null) {
				active = DesignerControl.GetActiveContentFile();
			}
			if (Scope != Scopes.AllOpenDocuments && Scope != Scopes.EntireProject)
				return active as ContentScript;
			ContentScript result = null;
			if (active == null) {
				if (DesignerControl.Project.LocalFileCount > 0)
					result = FindNextScriptSearchUp(DesignerControl.Project, startFile ?? null);
			}
			else {
				result = FindNextScriptSearchDown(active.Parent, active, startFile ?? active);
				// Search from the opposite end back to the active file if we found nothing
				if (result == null && (checkBoxWrapSearch.IsChecked == true || replaceAll))
					result = FindNextScriptSearchUp(DesignerControl.Project, startFile ?? active);
			}
			return result;
		}

		private ContentScript FindNextScriptSearchDown(ContentFolder folder, ContentFile startChild, ContentFile startFile) {
			bool searchUp = checkBoxSearchUp.IsChecked == true;
			int startIndex = folder.IndexOfFile(startChild);
			startIndex += (searchUp ? -1 : 1);
			ContentScript result = null;
			if (startIndex >= 0 && startIndex < folder.LocalFileCount)
				result = FindNextScriptSearchUp(folder, startFile, startIndex);
			if (result == null && folder.Parent != null) {
				result = FindNextScriptSearchDown(folder.Parent, folder, startFile);
			}
			return result;
		}

		private ContentScript FindNextScriptSearchUp(ContentFolder folder, ContentFile startFile, int startIndex = -1) {
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
						if (file.IsOpen || Scope == Scopes.EntireProject)
							return (ContentScript) file;
					}
					else if (file.IsFolder)
						result = FindNextScriptSearchUp((ContentFolder) file, startFile);
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
						if (file.IsOpen || Scope == Scopes.EntireProject)
							return (ContentScript) file;
					}
					else if (file.IsFolder)
						result = FindNextScriptSearchUp((ContentFolder) file, startFile);
				}
			}
			return result;
		}

		private void OnFindNext(object sender = null, RoutedEventArgs e = null) {
			string find = textBoxFind.Text;
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

		private void OnReplaceFindNext(object sender = null, RoutedEventArgs e = null) {
			string find = textBoxReplaceFind.Text;
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

		private void NavigateToSelection(ContentScript script, Selection selection) {
			if (!script.IsOpen) {
				if (!script.Open(false))
					return;
			}
			//TextEditor editor = script.TextEditor;
			//editor.SelectionStart = selection.Start;
			//editor.SelectionLength = selection.Length;
			script.SelectText(selection.Start, selection.Length, false);
		}

		private void OnReplace(object sender = null, RoutedEventArgs e = null) {
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
				startSelection, replace: true))
			{
				NavigateToSelection(script, selection);
			}
			else {
				SystemSounds.Asterisk.Play();
			}
		}

		private void OnReplaceAll(object sender = null, RoutedEventArgs e = null) {
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
				currentFile, currentSelection, true))
			{
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

		private bool FindNext(string searchText, out ContentScript script, out Selection selection, ContentFile startFile = null, Selection? startSelection = null, ContentFile currentFile = null, Selection? currentSelection = null, bool replaceAll = false, bool replace = false) {
			selection = Selection.Empty;
			script = null;

			Match match = Match.Empty;
			Regex regex = GetRegex(searchText);
			bool searchUp = regex.Options.HasFlag(RegexOptions.RightToLeft);

			if (replaceAll) {
				replace = true;
				searchUp = false;
			}

			if (!currentSelection.HasValue)
				currentSelection = Selection.Invalid;
			script = startFile as ContentScript;
			if (currentFile != null)
				script = currentFile as ContentScript;
			if (currentFile == null) {
				script = ActiveScript;
				currentFile = script;
				if (script != null) {
					if (!replaceAll)
						currentSelection = Selection.FromTextEditor(script.TextEditor);
				}
				else {
					script = FindNextScript(replaceAll: replaceAll);
					currentFile = script;
				}
			}
			else if (script != null) {
				if (script.IsOpen && !replaceAll) {
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
				if (currentSelection.Value.IsInvalid) {
					if (searchUp)
						currentSelection = Selection.FromStart(script.LoadedText.Length);
					else
						currentSelection = Selection.FromStart(0);
				}
				int start = (searchUp == replace) ? currentSelection.Value.Start : currentSelection.Value.End;
				currentEnd = start;
				if (!replace) {
					if (searchUp)
						start = Math.Max(0, start - 1);
					else
						start = Math.Min(script.LoadedText.Length, start + 1);
				}
				if (startSelection.HasValue)
					startEnd = (searchUp == replace) ? startSelection.Value.Start : startSelection.Value.End;
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

				currentEnd = searchUp ? currentSelection.Value.End : currentSelection.Value.End;
				if (startSelection.HasValue)
					startEnd = (searchUp == replace) ? startSelection.Value.End : startSelection.Value.Start;
			}
			script = FindNextScript(currentFile, startFile, replaceAll);
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
				script = FindNextScript(script, startFile, replaceAll);
			}

			return false;
		}

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

		public static FindAndReplaceWindow Show(Window owner, bool replace, EventHandler onClose) {
			FindAndReplaceWindow window = new FindAndReplaceWindow(replace);
			window.Owner = owner;
			window.Show();
			window.Closed += onClose;
			return window;
		}

		public void FindMode() {
			tabControl.SelectedIndex = 0;
			textBoxFind.Focus();
			//textBoxFind.SelectAll();
		}

		public void ReplaceMode() {
			tabControl.SelectedIndex = 1;
			textBoxReplaceFind.Focus();
			//textBoxReplaceFind.SelectAll();
		}

		public void FindNext() {
			if (tabControl.SelectedIndex == 0)
				OnFindNext();
			else
				OnReplaceFindNext();
		}

		public void ReplaceNext() {
			OnReplace();
		}

		public void ReplaceAll() {
			OnReplaceAll();
		}

		private void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}

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

		private Scopes Scope {
			get { return (Scopes) ((ComboBoxItem) comboBoxScope.SelectedItem).Tag; }
		}
	}
}
