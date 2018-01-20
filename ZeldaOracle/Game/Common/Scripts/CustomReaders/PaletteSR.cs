using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripts.Commands;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	public class PaletteSR : ScriptReader {
		
		private Palette palette;
		private string assetName;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PaletteSR() {

			//=====================================================================================
			// BEGIN/END
			//=====================================================================================
			AddCommand("PALETTE", 0,
				"string dictionary, string name",
			delegate (CommandParam parameters) {
				if (palette != null)
					ThrowCommandParseError("Must end previous PALETTE!");

				PaletteDictionary dictionary = Resources.GetPaletteDictionary(parameters.GetString(0));
				palette = new Palette(Resources.GraphicsDevice, dictionary);
				assetName = parameters.GetString(1);
				AddResource<Palette>(assetName, palette);
				Mode = 1;
			});
			//=====================================================================================
			AddCommand("END", 1,
			delegate (CommandParam parameters) {
				if (palette == null)
					ThrowCommandParseError("Must start a PALETTE before calling end!");

				palette.UpdatePalette();
				palette = null;
				Mode = 0;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("COLOR", 1,
				"string name, string subtype, Color color",
			delegate (CommandParam parameters) {
				LookupSubtypes subtype = ParseSubtype(parameters.GetString(1));

				CommandParam colorParam = parameters.GetParam(2);
				Color color = parameters.GetColor(2);
				if (color.A != 255 && color.A != 0)
					ThrowCommandParseError("Color alpha must be either 0 or 255!");
				
				palette.SetColor(parameters.GetString(0), subtype, color);
			});
			//=====================================================================================
			AddCommand("LOOKUP", 1,
				"string name, string subtype, string lookupName, string lookupSubtype",
			delegate (CommandParam parameters) {
				LookupSubtypes subtype = ParseSubtype(parameters.GetString(1));
				LookupSubtypes lookupSubtype = ParseSubtype(parameters.GetString(3));
				
				palette.SetLookup(
					parameters.GetString(0), subtype,
					parameters.GetString(2), lookupSubtype);
			});
			//=====================================================================================
			AddCommand("COPY", 1,
				"string name, string subtype, string lookupName, string lookupSubtype",
			delegate (CommandParam parameters) {
				LookupSubtypes subtype = ParseSubtype(parameters.GetString(1));
				LookupSubtypes lookupSubtype = ParseSubtype(parameters.GetString(3));

				palette.CopyLookup(
					parameters.GetString(0), subtype,
					parameters.GetString(2), lookupSubtype);
			});
			//=====================================================================================
			AddCommand("RESET", 1,
				"string name, string subtype",
			delegate (CommandParam parameters) {
				LookupSubtypes subtype = ParseSubtype(parameters.GetString(1));

				palette.Reset(parameters.GetString(0), subtype);
			});
			//=====================================================================================
			AddCommand("CONST", 1,
				"string name",
			delegate (CommandParam parameters) {
				palette.AddConst(parameters.GetString(0));
			});
			//=====================================================================================
			// CLONING
			//=====================================================================================
			AddCommand("CLONE", 1,
				"string paletteName",
			delegate (CommandParam parameters) {
				palette = new Palette(GetResource<Palette>(parameters.GetString(0)));
				SetResource<Palette>(assetName, palette);
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Parses the lookup subtype from a string with error handling.</summary>
		private LookupSubtypes ParseSubtype(string subtypeStr) {
			LookupSubtypes subtype;
			if (!Enum.TryParse(subtypeStr, true, out subtype))
				ThrowCommandParseError("Invalid subtype!");

			if (subtypeStr == "medium" && palette.PaletteType == PaletteTypes.Entity)
				ThrowCommandParseError("Medium color not available in entity color group!");
			else if (subtypeStr == "transparent" && palette.PaletteType == PaletteTypes.Tile)
				ThrowCommandParseError("Transparent color not available in tile color group!");

			return subtype;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			palette			= null;
			assetName		= "";
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {

		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new PaletteSR();
		}
	}
}
