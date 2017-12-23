using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace ConscriptDesigner.Controls.TextEditing {
	/// <summary>A search result storing a match and text segment.</summary>
	public class SearchResult : TextSegment {
		/// <summary>The regex match for the search result.</summary>
		public Match Match { get; }

		/// <summary>Constructs the search result from the match.</summary>
		public SearchResult(Match match) {
			this.StartOffset = match.Index;
			this.Length = match.Length;
			this.Match = match;
		}
	}

	/// <summary>Colorizes search results behind the selection.</summary>
	public class ColorizeSearchResultsBackgroundRenderer : IBackgroundRenderer {

		/// <summary>The search results to be modified.</summary>
		TextSegmentCollection<SearchResult> currentResults = new TextSegmentCollection<SearchResult>();

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the search result colorizer.</summary>
		public ColorizeSearchResultsBackgroundRenderer() {
			Background = new SolidColorBrush(Color.FromRgb(246, 185, 77));
			Background.Freeze();
		}


		//-----------------------------------------------------------------------------
		// IBackgroundRenderer Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the layer on which this background renderer should draw.</summary>
		public KnownLayer Layer {
			get {
				// draw behind selection
				return KnownLayer.Selection;
			}
		}
		
		/// <summary>Causes the background renderer to draw.</summary>
		public void Draw(TextView textView, DrawingContext drawingContext) {
			if (textView == null)
				throw new ArgumentNullException("textView");
			if (drawingContext == null)
				throw new ArgumentNullException("drawingContext");

			if (currentResults == null || !textView.VisualLinesValid)
				return;

			var visualLines = textView.VisualLines;
			if (visualLines.Count == 0)
				return;

			int viewStart = visualLines.First().FirstDocumentLine.Offset;
			int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;

			foreach (SearchResult result in currentResults.FindOverlappingSegments(viewStart, viewEnd - viewStart)) {
				BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
				geoBuilder.AlignToWholePixels = true;
				geoBuilder.BorderThickness = 0;
				geoBuilder.CornerRadius = 0;
				geoBuilder.AddSegment(textView, result);
				Geometry geometry = geoBuilder.CreateGeometry();
				if (geometry != null) {
					drawingContext.DrawGeometry(Background, null, geometry);
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the search results for modification.</summary>
		public TextSegmentCollection<SearchResult> CurrentResults {
			get { return currentResults; }
		}

		/// <summary>Gets or sets the background brush for the search results.</summary>
		public Brush Background { get; set; }
	}
}
