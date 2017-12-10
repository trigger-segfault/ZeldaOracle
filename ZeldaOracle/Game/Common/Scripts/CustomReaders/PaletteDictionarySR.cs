using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripts.Commands;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	public class PaletteDictionarySR : ScriptReader {
		
		private PaletteDictionary dictionary;
		private string assetName;
		private int paletteIndex;
		private PaletteTypes type;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PaletteDictionarySR() {

			//=====================================================================================
			AddCommand("PALETTEDICTIONARY", "string type, string name",
			delegate (CommandParam parameters) {
				if (dictionary != null)
					ThrowCommandParseError("Must end previous PALATTEDICTIONARY!");

				paletteIndex = 4;
				if (!Enum.TryParse(parameters.GetString(0), true, out type))
					ThrowCommandParseError("Invalid Palette Type");
				dictionary = new PaletteDictionary(type);
				assetName = parameters.GetString(1);
			});
			//=====================================================================================
			AddCommand("END",
			delegate (CommandParam parameters) {
				if (dictionary == null)
					ThrowCommandParseError("Must start a PALETTEDICTIONARY before calling end!");

				Resources.AddPaletteDictionary(assetName, dictionary);
				dictionary = null;
			});
			//=====================================================================================
			AddCommand("TILE", "string name",
			delegate (CommandParam parameters) {
				if (dictionary == null)
					ThrowCommandParseError("Must start a PALETTEDICTIONARY before defining a tile!");
				dictionary.Add(parameters.GetString(0), paletteIndex);
				paletteIndex += 4;
			});
			//=====================================================================================
			AddCommand("ENTITY", "string name",
			delegate (CommandParam parameters) {
				if (dictionary == null)
					ThrowCommandParseError("Must start a PALETTEDICTIONARY before defining a tile!");
				dictionary.Add(parameters.GetString(0), paletteIndex);
				paletteIndex += 4;
			});
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		// Begins reading the script.
		protected override void BeginReading() {
			dictionary		= null;
			assetName		= "";
			paletteIndex    = 0;
			type            = PaletteTypes.Tile;
		}

		// Ends reading the script.
		protected override void EndReading() {

		}
	}
}
