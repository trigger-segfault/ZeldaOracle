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
			AddCommand("PALETTE", "string dictionary, string name",
			delegate (CommandParam parameters) {
				if (palette != null)
					ThrowCommandParseError("Must end previous PALETTE!");

				PaletteDictionary dictionary = Resources.GetPaletteDictionary(parameters.GetString(0));
				palette = new Palette(Resources.GraphicsDevice, dictionary);
				assetName = parameters.GetString(1);
				AddResource<Palette>(assetName, palette);
			});
			//=====================================================================================
			AddCommand("END",
			delegate (CommandParam parameters) {
				if (palette == null)
					ThrowCommandParseError("Must start a PALETTE before calling end!");

				palette.UpdatePalette();
				palette = null;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("COLOR", "string name, string subtype, (int r, int g, int b)",
						"string name, string subtype, (int r, int g, int b, int a)",
			delegate (CommandParam parameters) {
				if (palette == null)
					ThrowCommandParseError("Must start a PALETTE before defining a color!");

				LookupSubtypes subtype = ParseSubtype(parameters.GetString(1));

				CommandParam colorParam = parameters.GetParam(2);
				int a = 255;
				if (colorParam.ChildCount == 4) {
					a = colorParam.GetInt(3);
					if (a != 255 && a != 0)
						ThrowCommandParseError("Color alpha must be either 0 or 255!");
				}
				Color color = new Color(
					colorParam.GetInt(0),
					colorParam.GetInt(1),
					colorParam.GetInt(2),
					a);

				palette.SetColor(parameters.GetString(0), subtype, color);
			});
			//=====================================================================================
			AddCommand("LOOKUP", "string name, string subtype, string lookupName, string lookupSubtype",
			delegate (CommandParam parameters) {
				if (palette == null)
					ThrowCommandParseError("Must start a PALETTE before defining a color!");

				LookupSubtypes subtype = ParseSubtype(parameters.GetString(1));
				LookupSubtypes lookupSubtype = ParseSubtype(parameters.GetString(3));
				
				palette.SetLookup(
					parameters.GetString(0), subtype,
					parameters.GetString(2), lookupSubtype);
			});
			//=====================================================================================
			// CLONING
			//=====================================================================================
			AddCommand("CLONE", "string paletteName",
			delegate (CommandParam parameters) {
				if (palette == null)
					ThrowCommandParseError("Must start a PALETTE before calling CLONE!");

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
