using System;
using System.IO;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ZeldaOracle.Common.Util;

namespace ConscriptDesigner.Themes {
	/// <summary>A static class for storing and loading all highlighting definitions.</summary>
	public static class Highlighting {

		//-----------------------------------------------------------------------------
		// Definitions
		//-----------------------------------------------------------------------------

		/// <summary>The highlight definition for the conscript editor.</summary>
		private static IHighlightingDefinition conscriptDefinition;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Loads all highlighting definitions.</summary>
		static Highlighting() {
			conscriptDefinition		= Load("ConscriptHighlighting");
		}


		//-----------------------------------------------------------------------------
		// Loading
		//-----------------------------------------------------------------------------

		/// <summary>Loads the Embedded .xshd file from the the Themes
		/// namespace/folder.</summary>
		public static IHighlightingDefinition Load(string name) {
			string fullPath = typeof(Highlighting).Namespace + "." + name + ".xshd";
			using (Stream stream = Embedding.Get(fullPath)) {
				if (stream == null)
					throw new ArgumentException("Failed to find embedded resource '" +
						fullPath + "', is its build action set as Embedded Resource?");
				return HighlightingLoader.Load(stream);
			}
		}


		//-----------------------------------------------------------------------------
		// Casting Extensions
		//-----------------------------------------------------------------------------

		/// <summary>Creates a highlighting colorizer from the definition.</summary>
		public static HighlightingColorizer ToColorizer(
			this IHighlightingDefinition highlightingDefinition)
		{
			return new HighlightingColorizer(highlightingDefinition);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the highlight definition for the conscript editor.</summary>
		public static IHighlightingDefinition Conscript {
			get { return conscriptDefinition; }
		}
	}
}
