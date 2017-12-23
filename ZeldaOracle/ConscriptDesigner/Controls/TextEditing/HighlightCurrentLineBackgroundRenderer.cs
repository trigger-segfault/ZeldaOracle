using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

namespace ConscriptDesigner.Controls.TextEditing {

	/// <summary>Highlights the line even when the text editor doesn't have focus.
	/// https://stackoverflow.com/questions/5072761/avalonedit-highlight-current-line-even-when-not-focused </summary>
	public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer {

		//-----------------------------------------------------------------------------
		// Static Members
		//-----------------------------------------------------------------------------

		/// <summary>The pen used for the rectangle.</summary>
		private static Pen pen;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The reference to the owning text editor.</summary>
		private TextEditor editor;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the line highlighter.</summary>
		static HighlightCurrentLineBackgroundRenderer() {
			pen = new Pen(new SolidColorBrush(Color.FromArgb(20, 40, 40, 42)), 2.0);
			pen.Freeze();
		}

		/// <summary>Constructs the line highlighter.</summary>
		public HighlightCurrentLineBackgroundRenderer(TextEditor editor) {
			this.editor = editor;
			this.editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
			this.editor.TextArea.TextView.SizeChanged += OnTextViewSizeChanged;
		}


		//-----------------------------------------------------------------------------
		// IBackgroundRenderer Overrides
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the layer on which this background renderer should draw.</summary>
		public KnownLayer Layer {
			get { return KnownLayer.Background; }
		}

		/// <summary>Causes the background renderer to draw.</summary>
		public void Draw(TextView textView, DrawingContext drawingContext) {
			if (editor.Document == null)
				return;
			
			textView.EnsureVisualLines();
			// Don't highlight the line when a selection exists
			if (editor.TextArea.TextView.ActualWidth > 0 && editor.TextArea.Selection.IsEmpty) {
				var currentLine = editor.Document.GetLineByOffset(editor.CaretOffset);
				foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine)) {
					Point point = new Point(Math.Round(rect.X + 2), Math.Round(rect.Y));
					Size size = new Size(
						Math.Round(editor.TextArea.TextView.ActualWidth - 5),
						Math.Round(rect.Height));
					drawingContext.DrawRectangle(null, pen, new Rect(point, size));
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Event Handlers
		//-----------------------------------------------------------------------------

		/// <summary>Called when the caret position is changed to highlight the current line.</summary>
		private void OnCaretPositionChanged(object sender, EventArgs e) {
			editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
		}

		/// <summary>Called when the text view changes size to update the current line rect width.</summary>
		private void OnTextViewSizeChanged(object sender, SizeChangedEventArgs e) {
			editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
		}
	}
}
