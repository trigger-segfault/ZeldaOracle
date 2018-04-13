using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Indentation;
using ConscriptDesigner.Content;
using ConscriptDesigner.Themes;
using ZeldaWpf.Controls.TextEditing;
using ZeldaWpf.Util;

namespace ConscriptDesigner.Anchorables {
	public class ConscriptEditor : ContentFileDocument, ICommandAnchorable {

		/// <summary>The text editor for the conscript.</summary>
		private TextEditor editor;

		/// <summary>The timer to update if the file is modified.
		/// Needed because it doesn't always change right after an action.</summary>
		private ScheduledEvent modifiedTimer;
		/// <summary>Navigate after a short pause to ensure focus.</summary>
		private ScheduledEvent navigateTimer;
		/// <summary>True if the title is currently displayed as modified.</summary>
		private bool isTitleModified;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		/// <summary>Constructs the conscript editor for serialization.</summary>
		public ConscriptEditor() {
			Border border = CreateBorder();
			editor = new TextEditor();
			editor.TextArea.TextView.LineSpacing = 1.24;
			editor.TextChanged += OnTextChanged;
			isTitleModified = false;

			editor.PreviewMouseDown += OnPreviewMouseDown;
			editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;
			editor.TextArea.SelectionCornerRadius = 0;
			editor.TextArea.SelectionBorder = null;
			editor.FontFamily = new FontFamily("Consolas");
			editor.FontSize = 12;
			editor.TextChanged += OnTextChanged;
			editor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
			editor.ShowLineNumbers = true;
			editor.SyntaxHighlighting = Highlighting.Conscript;
			//this.editor.TextArea.TextView.Margin = new Thickness(10, 0, 0, 0);
			editor.TextArea.TextView.BackgroundRenderers.Add(
				new CurrentLineHighlighter(editor));
			//this.editor.TextArea.Options.HighlightCurrentLine = true;
			TextOptions.SetTextFormattingMode(this.editor, TextFormattingMode.Display);
			border.Child = this.editor;

			modifiedTimer = ScheduledEvents.New(0.05, TimerPriority.Low,
				CheckModified);

			Content = border;
		}

		/// <summary>Constructs the conscript editor.</summary>
		public ConscriptEditor(ContentScript file) :
			this()
		{
			LoadFile(file);
		}


		//-----------------------------------------------------------------------------
		// Overrides Methods
		//-----------------------------------------------------------------------------

		/// <summary>Completes setup after loading the content file.</summary>
		protected override void OnLoadFile(ContentFile loadFile) {
			ContentScript file = loadFile as ContentScript;
			if (file == null)
				Close();

			editor.Load(file.FilePath);
			Title = file.Name;
		}

		/// <summary>Focuses on the anchorable's content.</summary>
		public override void Focus() {
			editor.Focus();
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called every so often to check if the file is modified.</summary>
		private void CheckModified() {
			if ((editor.IsModified || File.IsModifiedOverride) != isTitleModified)
				UpdateTitle();
			CommandManager.InvalidateRequerySuggested();
		}

		/// <summary>Called when the text is changed in order to update the title.</summary>
		private void OnTextChanged(object sender, EventArgs e) {
			modifiedTimer.Restart();
		}

		/// <summary>HACK: Fix AvalonDock's broken tab focusing where programmatically
		/// changing the current document may cause it to change back.</summary>
		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			File.Document.IsActive = true;
		}


		//-----------------------------------------------------------------------------
		// Title
		//-----------------------------------------------------------------------------

		/// <summary>Updates the title and appends an asterisk if it's modified.</summary>
		public void UpdateTitle() {
			isTitleModified = (editor.IsModified || File.IsModifiedOverride);
			Title = File.Name + (isTitleModified ? "*" : "");
		}


		//-----------------------------------------------------------------------------
		// Script Navigatiuon
		//-----------------------------------------------------------------------------

		/// <summary>Navigates to the location in the file.</summary>
		public void GotoLocation(int line, int column) {
			StartNavigateTimer(() => {
				editor.Focus();
				editor.TextArea.Caret.Line = line;
				editor.TextArea.Caret.Column = column;
				editor.ScrollTo(line, column);
			});
		}

		/// <summary>Selects text in the editor.</summary>
		public void SelectText(int start, int length, bool focus) {
			StartNavigateTimer(() => {
				if (focus)
					editor.Focus();
				editor.TextArea.Caret.Offset = start;
				editor.Select(start, length);
				TextLocation loc = editor.Document.GetLocation(start);
				editor.ScrollTo(loc.Line, loc.Column);
			});
		}


		//-----------------------------------------------------------------------------
		// Actions
		//-----------------------------------------------------------------------------

		/// <summary>Saves the current text in the editor to the file.</summary>
		public void Save() {
			editor.Save(File.FilePath);
			UpdateTitle();
			CommandManager.InvalidateRequerySuggested();
		}

		/// <summary>Reloads the text in the editor from the file.</summary>
		public void Reload() {
			editor.Load(File.FilePath);
			UpdateTitle();
		}

		/// <summary>Performs the cut command.</summary>
		public void Cut() {
			// Handled by text editor
		}

		/// <summary>Performs the copy command.</summary>
		public void Copy() {
			// Handled by text editor
		}

		/// <summary>Performs the paste command.</summary>
		public void Paste() {
			// Handled by text editor
		}

		/// <summary>Performs the delete command.</summary>
		public void Delete() {
			// Handled by text editor
		}

		/// <summary>Undoes the last action in the editor.</summary>
		public void Undo() {
			editor.Undo();
			modifiedTimer.Restart();
		}

		/// <summary>Redoes the last action in the editor.</summary>
		public void Redo() {
			editor.Redo();
			modifiedTimer.Restart();
		}

		/// <summary>Focuses on the editor.</summary>
		public void FocusEditor() {
			editor.Focus();
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Calls a navigation action after a short pause to ensure focus.</summary>
		private void StartNavigateTimer(Action action) {
			if (navigateTimer != null)
				navigateTimer.Cancel();
			navigateTimer = ScheduledEvents.Start(0.05, TimerPriority.Normal, action);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the editor considers the text to be modified.</summary>
		public bool IsModified {
			get { return editor.IsModified; }
		}

		/// <summary>Returns true if the editor can cut.</summary>
		public bool CanCut {
			// Handled by text editor
			get { return false; }
		}

		/// <summary>Returns true if the editor can copy.</summary>
		public bool CanCopy {
			// Handled by text editor
			get { return false; }
		}

		/// <summary>Returns true if the editor can paste.</summary>
		public bool CanPaste {
			// Handled by text editor
			get { return false; }
		}

		/// <summary>Returns true if the editor can delete.</summary>
		public bool CanDelete {
			// Handled by text editor
			get { return false; }
		}

		/// <summary>Returns true if the editor can undo.</summary>
		public bool CanUndo {
			get { return editor.CanUndo; }
		}

		/// <summary>Returns true if the editor can redo.</summary>
		public bool CanRedo {
			get { return editor.CanRedo; }
		}

		/// <summary>Gets the text from the editor.</summary>
		public string Text {
			get { return editor.Text; }
		}

		/// <summary>Gets the text selected in the editor.</summary>
		public string SelectedText {
			get { return editor.TextArea.Selection.GetText(); }
		}

		/// <summary>Gets the start of the selection.</summary>
		public int SelectedStart {
			get { return editor.SelectionStart; }
		}

		/// <summary>Gets the end of the selection.</summary>
		public int SelectedEnd {
			get { return editor.SelectionStart + editor.SelectionLength; }
		}

		/// <summary>Gets the text editor control.</summary>
		public TextEditor TextEditor {
			get { return editor; }
		}
	}
}
