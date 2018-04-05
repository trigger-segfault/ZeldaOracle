using System;
using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Conscripts.CustomReaders {
	public class PaletteSR : ConscriptRunner {

		private enum Modes {
			Root,
			Palette,
		}

		private Palette palette;
		private string paletteName;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PaletteSR() {

			//=====================================================================================
			// BEGIN/END
			//=====================================================================================
			AddCommand("PALETTE", (int) Modes.Root,
				"string dictionary, string name",
			delegate (CommandParam parameters) {
				PaletteDictionary dictionary =
					Resources.Get<PaletteDictionary>(parameters.GetString(0));
				palette = new Palette(dictionary);
				paletteName = parameters.GetString(1);
				AddResource<Palette>(paletteName, palette);
				Mode = Modes.Palette;
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Palette,
			delegate (CommandParam parameters) {
				palette.UpdatePalette();
				palette = null;
				Mode = Modes.Root;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("COLOR", (int) Modes.Palette,
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
			AddCommand("LOOKUP", (int) Modes.Palette,
				"string name, string subtype, string lookupName, string lookupSubtype",
			delegate (CommandParam parameters) {
				LookupSubtypes subtype = ParseSubtype(parameters.GetString(1));
				LookupSubtypes lookupSubtype = ParseSubtype(parameters.GetString(3));
				
				palette.SetLookup(
					parameters.GetString(0), subtype,
					parameters.GetString(2), lookupSubtype);
			});
			//=====================================================================================
			AddCommand("COPY", (int) Modes.Palette,
				"string name, string subtype, string lookupName, string lookupSubtype",
			delegate (CommandParam parameters) {
				LookupSubtypes subtype = ParseSubtype(parameters.GetString(1));
				LookupSubtypes lookupSubtype = ParseSubtype(parameters.GetString(3));

				palette.CopyLookup(
					parameters.GetString(0), subtype,
					parameters.GetString(2), lookupSubtype);
			});
			//=====================================================================================
			AddCommand("RESET", (int) Modes.Palette,
				"string name, string subtype",
			delegate (CommandParam parameters) {
				LookupSubtypes subtype = ParseSubtype(parameters.GetString(1));

				palette.Reset(parameters.GetString(0), subtype);
			});
			//=====================================================================================
			AddCommand("CONST", (int) Modes.Palette,
				"string name",
			delegate (CommandParam parameters) {
				palette.AddConst(parameters.GetString(0));
			});
			//=====================================================================================
			// CLONING
			//=====================================================================================
			AddCommand("CLONE", (int) Modes.Palette,
				"string paletteName",
			delegate (CommandParam parameters) {
				palette = new Palette(GetResource<Palette>(parameters.GetString(0)));
				SetResource<Palette>(paletteName, palette);
			});
			//=====================================================================================
			AddCommand("COMBINE", (int) Modes.Palette,
				"string paletteName",
			delegate (CommandParam parameters) {
				Palette combinePalette = GetResource<Palette>(parameters.GetString(0));
				if (combinePalette.PaletteType != palette.PaletteType)
					ThrowCommandParseError("Palette type combine mismatch!");
				foreach (var pair in palette.GetDefinedConsts()) {
					for (int i = 0; i < Palette.ColorGroupSize; i++) {
						if (pair.Value[i].IsUndefined)
							continue;
						palette.SetColor(pair.Key, (LookupSubtypes) i, pair.Value[i].Color);
					}
				}
				foreach (var pair in palette.GetDefinedColors()) {
					for (int i = 0; i < Palette.ColorGroupSize; i++) {
						if (pair.Value[i].IsUndefined)
							continue;
						palette.SetColor(pair.Key, (LookupSubtypes) i, pair.Value[i].Color);
					}
				}
				foreach (var pair in palette.GetDefinedLookups()) {
					for (int i = 0; i < Palette.ColorGroupSize; i++) {
						if (pair.Value[i].IsUndefined)
							continue;
						palette.SetLookup(pair.Key, (LookupSubtypes) i, pair.Value[i].Name, pair.Value[i].Subtype);
					}
				}
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
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void OnBeginReading() {
			palette			= null;
			paletteName		= "";
		}

		/// <summary>Ends reading the script.</summary>
		protected override void OnEndReading() {
			OnBeginReading();
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>The mode of the Palette script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
