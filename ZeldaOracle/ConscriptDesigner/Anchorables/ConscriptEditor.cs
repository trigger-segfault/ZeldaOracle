using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ConscriptDesigner.Content;
using ConscriptDesigner.Controls;
using ConscriptDesigner.Controls.TextEditing;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.NRefactory;

namespace ConscriptDesigner.Anchorables {
	public class ConscriptEditor : ContentFileDocument, ICommandAnchorable {

		//-----------------------------------------------------------------------------
		// Static Members
		//-----------------------------------------------------------------------------

		/// <summary>The highlighting definition for all conscripts. May be split up later.</summary>
		private static IHighlightingDefinition highlightingDefinition;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The text editor for the conscript.</summary>
		private TextEditor editor;

		/// <summary>The timer to update if the file is modified.
		/// Needed because it doesn't always change right after an action.</summary>
		private DispatcherTimer modifiedTimer;
		/// <summary>Navigate after a short pause to ensure focus.</summary>
		private DispatcherTimer navigateTimer;
		/// <summary>True if the title is currently displayed as modified.</summary>
		private bool isTitleModified;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the conscript editor and loads the highlighting.</summary>
		static ConscriptEditor() {
			string fullName = "ConscriptDesigner.Themes.ConscriptHighlighting.xshd";
			using (var stream = typeof(ConscriptEditor).Assembly.GetManifestResourceStream(fullName))
			using (var reader = new XmlTextReader(stream))
				highlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
		}

		/// <summary>Constructs the conscript editor for serialization.</summary>
		public ConscriptEditor() {
			Border border = CreateBorder();
			this.editor = new TextEditor();
			this.editor.TextArea.TextView.LineSpacing = 1.24;
			this.editor.TextChanged += OnTextChanged;
			this.isTitleModified = false;

			this.editor.PreviewMouseDown += OnPreviewMouseDown;
			this.editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			this.editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;
			this.editor.TextArea.SelectionCornerRadius = 0;
			this.editor.TextArea.SelectionBorder = null;
			this.editor.FontFamily = new FontFamily("Consolas");
			this.editor.FontSize = 12;
			this.editor.TextChanged += OnTextChanged;
			this.editor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
			this.editor.ShowLineNumbers = true;
			this.editor.SyntaxHighlighting = highlightingDefinition;
			//this.editor.TextArea.TextView.Margin = new Thickness(10, 0, 0, 0);
			this.editor.TextArea.TextView.BackgroundRenderers.Add(
				new HighlightCurrentLineBackgroundRenderer(this.editor));
			//this.editor.TextArea.Options.HighlightCurrentLine = true;
			TextOptions.SetTextFormattingMode(this.editor, TextFormattingMode.Display);
			border.Child = this.editor;

			this.modifiedTimer = new DispatcherTimer(
				TimeSpan.FromSeconds(0.05),
				DispatcherPriority.ApplicationIdle,
				delegate { CheckModified(); }, Dispatcher);

			this.modifiedTimer.Stop();

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

			this.editor.Load(file.FilePath);
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
			modifiedTimer.Stop();
			if ((editor.IsModified || File.IsModifiedOverride) != isTitleModified)
				UpdateTitle();
			CommandManager.InvalidateRequerySuggested();
		}

		/// <summary>Called when the text is changed in order to update the title.</summary>
		private void OnTextChanged(object sender, EventArgs e) {
			modifiedTimer.Start();
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
			modifiedTimer.Start();
		}

		/// <summary>Redoes the last action in the editor.</summary>
		public void Redo() {
			editor.Redo();
			modifiedTimer.Start();
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
				navigateTimer.Stop();
			navigateTimer = new DispatcherTimer(
				TimeSpan.FromSeconds(0.05),
				DispatcherPriority.ApplicationIdle,
				delegate {
					action();
					navigateTimer.Stop();
					navigateTimer = null;
				}, Dispatcher);
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
