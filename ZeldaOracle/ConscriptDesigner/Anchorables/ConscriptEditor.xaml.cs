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
using ConscriptDesigner.Content;
using ICSharpCode.AvalonEdit.Indentation;
using Xceed.Wpf.AvalonDock.Layout;

namespace ConscriptDesigner.Anchorables {
	/// <summary>
	/// Interaction logic for ConscriptEditor.xaml
	/// </summary>
	public partial class ConscriptEditor : UserControl, IContentAnchorable {

		private RequestCloseAnchorable anchorable;
		private ContentScript file;

		public ConscriptEditor(ContentScript file, RequestCloseAnchorable anchorable) {
			InitializeComponent();
			this.file = file;
			this.anchorable = anchorable;
			this.editor.Load(file.FilePath);
			this.editor.TextChanged += OnTextChanged;

			editor.TextArea.Margin = new Thickness(4, 4, 0, 4);
			editor.TextArea.TextView.Options.AllowScrollBelowDocument = true;

			// Selection Style
			editor.TextArea.SelectionCornerRadius = 0;
			editor.TextArea.SelectionBorder = null;
			editor.FontFamily = new FontFamily("Lucida Console");
			editor.FontSize = 12.667;
			editor.TextChanged += OnTextChanged;
			TextOptions.SetTextFormattingMode(editor, TextFormattingMode.Display);
			editor.IsModified = false;
			editor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
			editor.ShowLineNumbers = true;
		}

		private void OnTextChanged(object sender, EventArgs e) {
			UpdateTitle();
			CommandManager.InvalidateRequerySuggested();
		}

		public void UpdateTitle() {
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
			editor.IsModified = true;
			UpdateTitle();
			CommandManager.InvalidateRequerySuggested();
		}
		public void Redo() {
			editor.Redo();
			editor.IsModified = true;
			UpdateTitle();
			CommandManager.InvalidateRequerySuggested();
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

		public RequestCloseAnchorable Anchorable {
			get { return anchorable; }
		}

		public ContentFile ContentFile {
			get { return file; }
		}
	}
}
