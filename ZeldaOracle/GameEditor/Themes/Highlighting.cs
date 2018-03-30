using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ZeldaOracle.Common.Util;

namespace ZeldaEditor.Themes {
	/// <summary>A static class for storing and loading all highlighting definitions.</summary>
	public static class Highlighting {

		//-----------------------------------------------------------------------------
		// Definitions
		//-----------------------------------------------------------------------------

		/// <summary>The highlight definition for the text message editor.</summary>
		private static IHighlightingDefinition textMessageDefinition;

		/// <summary>The highlight definition for the script editor's multiline
		/// commenting.</summary>
		private static IHighlightingDefinition scriptDefinition;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Loads all highlighting definitions.</summary>
		static Highlighting() {
			textMessageDefinition	= Load("TextMessageHighlighting");
			//scriptDefinition		= Load("CSharpMultilineHighlighting");
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
		
		/// <summary>Gets the highlight definition for the text message editor.</summary>
		public static IHighlightingDefinition TextMessage {
			get { return textMessageDefinition; }
		}

		/// <summary>Gets the highlight definition for the script editor's multiline
		/// commenting.</summary>
		public static IHighlightingDefinition Script {
			get { return scriptDefinition; }
		}

	}
}
