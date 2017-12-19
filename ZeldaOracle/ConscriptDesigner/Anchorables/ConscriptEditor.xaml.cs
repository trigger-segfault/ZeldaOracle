using System;
using System.Collections.Generic;
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
using System.Xml;
using ConscriptDesigner.Content;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Indentation;
using Xceed.Wpf.AvalonDock.Layout;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for ConscriptEditor.xaml
	/// </summary>
	public partial class ConscriptEditor : UserControl, IContentAnchorable {

		private RequestCloseDocument anchorable;
		private ContentScript file;
		private static IHighlightingDefinition highlightingDefinition;

		private DispatcherTimer modifiedTimer;
		private bool isTitleModified;

		static ConscriptEditor() {
			var fullName = "ConscriptDesigner.Themes.ConscriptHighlighting.xshd";
			using (var stream = typeof(ConscriptEditor).Assembly.GetManifestResourceStream(fullName))
			using (var reader = new XmlTextReader(stream))
				highlightingDefinition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
		}

		public ConscriptEditor(ContentScript file, RequestCloseDocument anchorable) {
			InitializeComponent();
			this.file = file;
			this.anchorable = anchorable;
			this.editor.Load(file.FilePath);
			this.editor.TextChanged += OnTextChanged;
			this.isTitleModified = false;

			editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;

			// Selection Style
			editor.TextArea.SelectionCornerRadius = 0;
			editor.TextArea.SelectionBorder = null;
			editor.FontFamily = new FontFamily("Lucida Console");
			editor.FontSize = 12;
			editor.TextChanged += OnTextChanged;
			TextOptions.SetTextFormattingMode(editor, TextFormattingMode.Display);
			editor.IsModified = false;
			editor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
			editor.ShowLineNumbers = true;
			editor.SyntaxHighlighting = highlightingDefinition;

			modifiedTimer = new DispatcherTimer(TimeSpan.FromSeconds(0.05), DispatcherPriority.ApplicationIdle,
				delegate { CheckModified(); }, Dispatcher);
			modifiedTimer.Stop();
		}

		private void CheckModified() {
			modifiedTimer.Stop();
			if (IsModified != isTitleModified)
				UpdateTitle();
			CommandManager.InvalidateRequerySuggested();
		}

		private void OnTextChanged(object sender, EventArgs e) {
			modifiedTimer.Start();
		}

		public void UpdateTitle() {
			isTitleModified = IsModified;
			anchorable.Title = file.Name + (editor.IsModified ? "*" : "");
		}

		public void OnRename() {
			UpdateTitle();
		}

		public void OnClose() {

		}

		public void Save() {
			editor.Save(file.FilePath);
			UpdateTitle();
			CommandManager.InvalidateRequerySuggested();
		}

		public void Undo() {
			editor.Undo();
			modifiedTimer.Start();
		}
		public void Redo() {
			editor.Redo();
			modifiedTimer.Start();
		}

		public bool IsModified {
			get { return editor.IsModified; }
		}

		public bool CanUndo {
			get { return editor.CanUndo; }
		}

		public bool CanRedo {
			get { return editor.CanRedo; }
		}

		public RequestCloseDocument Anchorable {
			get { return anchorable; }
		}

		public ContentFile ContentFile {
			get { return file; }
		}
	}
}
