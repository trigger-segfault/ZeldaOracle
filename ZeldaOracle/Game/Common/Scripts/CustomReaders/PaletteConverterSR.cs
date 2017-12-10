using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripts.Commands;

using XnaColor = Microsoft.Xna.Framework.Color;
using Color = ZeldaOracle.Common.Graphics.Color;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	public class PaletteConverterSR : ScriptReader {

		private PaletteDictionary dictionary;
		private XnaColor[] colorData;
		private SpriteSheet sheet;
		private Image image;
		private Point2I location;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public PaletteConverterSR() {

			//=====================================================================================
			AddCommand("CONVERT", "string sheetName, string imageVariant, string dictionary",
			delegate (CommandParam parameters) {
				if (image != null)
					ThrowCommandParseError("Must end previous CONVERT!");

				sheet = Resources.GetSpriteSheet(parameters.GetString(0));
				image = sheet.Image.GetVariant(parameters.GetString(1));

				dictionary = Resources.GetPaletteDictionary(parameters.GetString(2));

				colorData = new XnaColor[image.Width * image.Height];
				image.Texture.GetData<XnaColor>(colorData);
			});
			//=====================================================================================
			AddCommand("END",
			delegate (CommandParam parameters) {
				if (image == null)
					ThrowCommandParseError("Must start a CONVERT before calling end!");

				image.Texture.SetData<XnaColor>(colorData);
				image = null;
				colorData = null;
			});
			//=====================================================================================
			AddCommand("TILE", "string name, (string subtypes...)",
				"string name1, (string subtypes...), " +
				"string name2, (string subtypes...), " +
				"string name3, (string subtypes...), " +
				"string name4, (string subtypes...)",
			delegate (CommandParam parameters) {
				if (image == null)
					ThrowCommandParseError("Must start a CONVERT before defining a tile!");
				if (parameters.ChildCount == 2) {
					string name = parameters.GetString(0);
					int index = dictionary.Get(name);
					List<Color> colors = new List<Color>();
					Point2I position = sheet.Offset + (sheet.CellSize + sheet.Spacing) * location;
					for (int x = 0; x < 16; x++) {
						for (int y = 0; y < 16; y++) {
							Color color = GetColor(position.X + x, position.Y + y);
							if (!colors.Contains(color)) {
								colors.Add(color);
								if (colors.Count == 4)
									break;
							}
						}
					}
					CommandParam param = parameters.GetParam(1);
					colors.Sort((a, b) => b.Total - a.Total);
					List<LookupSubtypes> subtypes = new List<LookupSubtypes>();
					for (int i = 0; i < param.ChildCount; i++) {
						subtypes.Add(ParseSubtype(param.GetString(i)));
					}
					for (int x = 0; x < 16; x++) {
						for (int y = 0; y < 16; y++) {
							int index2 = index;
							Color color = GetColor(position.X+x, position.Y+y);
							if (color == Color.Black)
								index2 += 3;
							else
								index2 += (int) subtypes[colors.IndexOf(color)];
							SetColor(position.X + x, position.Y + y, index2);
						}
					}
				}
				else {
					for (int j = 0; j < 4; j++) {
						string name = parameters.GetString(j * 2);
						int index = dictionary.Get(name);
						List<Color> colors = new List<Color>();
						Point2I position = sheet.Offset + (sheet.CellSize + sheet.Spacing) * location +
							new Point2I(j % 2, j / 2) * 8;
						for (int x = 0; x < 8; x++) {
							for (int y = 0; y < 8; y++) {
								Color color = GetColor(position.X + x, position.Y + y);
								if (!colors.Contains(color)) {
									colors.Add(color);
									if (colors.Count == 4)
										break;
								}
							}
						}
						CommandParam param = parameters.GetParam(j * 2 + 1);
						colors.Sort((a, b) => b.Total - a.Total);
						List<LookupSubtypes> subtypes = new List<LookupSubtypes>();
						for (int i = 0; i < param.ChildCount; i++) {
							subtypes.Add(ParseSubtype(param.GetString(i)));
						}
						for (int x = 0; x < 8; x++) {
							for (int y = 0; y < 8; y++) {
								int index2 = index;
								Color color = GetColor(position.X+x, position.Y+y);
								if (color == Color.Black)
									index2 += 3;
								else
									index2 += (int)subtypes[colors.IndexOf(color)];
								SetColor(position.X + x, position.Y + y, index2);
							}
						}
					}
				}
				NextTile();
			});
			//=====================================================================================
			AddCommand("SKIP",
			delegate (CommandParam parameters) {
				if (image == null)
					ThrowCommandParseError("Must start a CONVERT before calling skip!");

				NextTile();
			});
		}

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private Color GetColor(int x, int y) {
			return (Color) colorData[x + image.Width * y];
		}
		private void SetColor(int x, int y, int index) {
			colorData[x + image.Width * y] = new XnaColor(index & 0xFF, (index & 0xFF00) >> 8, 0, 254);
		}

		private void NextTile() {
			location.X++;
			Point2I pos = sheet.Offset + (sheet.CellSize + sheet.Spacing) * location;
			if (pos.X + sheet.CellSize.X > sheet.Image.Width) {
				location.X = 0;
				location.Y++;
			}
		}

		private LookupSubtypes ParseSubtype(string subtypeStr) {
			LookupSubtypes subtype;
			if (!Enum.TryParse(subtypeStr, true, out subtype))
				ThrowCommandParseError("Invalid subtype!");

			if (subtype == LookupSubtypes.All)
				ThrowCommandParseError("All not supported in converter!");
			else if (subtypeStr == "medium" && dictionary.PaletteType == PaletteTypes.Entity)
				ThrowCommandParseError("Medium color not available in entity color group!");
			else if (subtypeStr == "transparent" && dictionary.PaletteType == PaletteTypes.Tile)
				ThrowCommandParseError("Transparent color not available in tile color group!");

			return subtype;
		}

		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		// Begins reading the script.
		protected override void BeginReading() {
			dictionary      = null;
			image           = null;
			colorData       = null;
		}

		// Ends reading the script.
		protected override void EndReading() {

		}
	}
}
