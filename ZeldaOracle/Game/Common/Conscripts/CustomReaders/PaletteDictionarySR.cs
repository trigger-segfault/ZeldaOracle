using System;
using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Conscripts.CustomReaders {
	public class PaletteDictionarySR : ConscriptRunner {

		private enum Modes {
			Root,
			Dictionary,
		}

		private PaletteDictionary dictionary;
		private string assetName;
		private int paletteIndex;
		private PaletteTypes type;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PaletteDictionarySR() {

			//=====================================================================================
			// BEGIN/END
			//=====================================================================================
			AddCommand("PALETTEDICTIONARY", (int) Modes.Root,
				"string type, string name",
			delegate (CommandParam parameters) {
				if (dictionary != null)
					ThrowCommandParseError("Must end previous PALATTEDICTIONARY!");

				paletteIndex = 4;
				if (!Enum.TryParse(parameters.GetString(0), true, out type))
					ThrowCommandParseError("Invalid Palette Type");
				assetName = parameters.GetString(1);
				dictionary = new PaletteDictionary(type);
				AddResource<PaletteDictionary>(assetName, dictionary);
				Mode = Modes.Dictionary;
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Dictionary,
				"",
			delegate (CommandParam parameters) {
				dictionary = null;
				Mode = Modes.Root;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("TILE", (int) Modes.Dictionary,
				"string name",
			delegate (CommandParam parameters) {
				if (dictionary.PaletteType != PaletteTypes.Tile)
					ThrowCommandParseError("Palette dictionary is not a tile dictionary!");
				dictionary.Add(parameters.GetString(0), paletteIndex);
				paletteIndex += 4;
			});
			//=====================================================================================
			AddCommand("ENTITY", (int) Modes.Dictionary,
				"string name",
			delegate (CommandParam parameters) {
				if (dictionary.PaletteType != PaletteTypes.Entity)
					ThrowCommandParseError("Palette dictionary is not an entity dictionary!");
				dictionary.Add(parameters.GetString(0), paletteIndex);
				paletteIndex += 4;
			});
			//=====================================================================================
			AddCommand("COMBINE", (int) Modes.Dictionary,
				"string dictionaryName",
			delegate (CommandParam parameters) {
				PaletteDictionary combineDictionary = GetResource<PaletteDictionary>(parameters.GetString(0));
				if (dictionary.PaletteType != combineDictionary.PaletteType)
					ThrowCommandParseError("Palette dictionary type combine mismatch!");
				foreach (var key in combineDictionary.GetKeys()) {
					dictionary.Add(key, paletteIndex);
					paletteIndex += 4;
				}
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void OnBeginReading() {
			dictionary		= null;
			assetName		= "";
			paletteIndex    = 0;
			type            = PaletteTypes.Tile;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void OnEndReading() {
			OnBeginReading();
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>The mode of the Palette Dictionary script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
