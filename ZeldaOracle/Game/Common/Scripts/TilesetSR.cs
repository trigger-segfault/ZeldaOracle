using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Collision;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Properties;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Common.Scripts {

	public class TilesetSR : NewScriptReader {

		private Tileset		tileset;
		private TileData	tileData;
		private string		tileDataName;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public TilesetSR() {

			// BEGIN/END.
			
			// Tileset <name> <sheet-name> <size(width, height)>
			AddCommand("Tileset", delegate(CommandParam parameters) {
				SpriteSheet sheet = Resources.GetSpriteSheet(parameters.GetString(1));
				tileset = new Tileset(parameters.GetString(0),
					sheet, parameters.GetPoint(2));
			});
			// With a tileset:
			//   Tile <sheet-location>
			// Without a tileset:
			//   Tile <name>
			AddCommand("Tile", delegate(CommandParam parameters) {
				if (tileset != null) {
					Point2I location = parameters.GetPoint(0);
					tileData = tileset.TileData[location.X, location.Y];
				}
				else {
					tileData = new TileData();
					tileDataName = parameters.GetString(0);
				}
			});
			AddCommand("End", delegate(CommandParam parameters) {
				if (tileData != null) {
					if (tileData.Tileset == null)
						Resources.AddResource<TileData>(tileDataName, tileData);
					tileData = null;
				}
				else if (tileset != null) {
					Resources.AddResource<Tileset>(tileset.ID, tileset);
					tileset = null;
				}
			});

			
			// TILESETS.

			// Default <default-tile-location(x, y)>
			AddCommand("Default", delegate(CommandParam parameters) {
				tileset.DefaultTile = parameters.GetPoint(0);
			});
			// Config: data to configure tiles with a single character per tile.
			AddCommand("Config", delegate(CommandParam parameters) {
				string line = NextLine();
				int y = 0;
				while (!line.StartsWith("END;", StringComparison.OrdinalIgnoreCase)) {
					if (y < tileset.Height) {
						for (int x = 0; x < line.Length && x < tileset.Width; x++)
							tileset.ConfigureTile(tileset.TileData[x, y], line[x]);
					}
					line = NextLine();
					y++;
				}
			});


			// TILES.
			
			// Type <type>
			AddCommand("Type", delegate(CommandParam parameters) {
				tileData.Type = Tile.GetType(parameters.GetString(0), true);
				
			});
			// Flags <flags[]...>
			AddCommand("Flags", delegate(CommandParam parameters) {
				for (int i = 0; i < parameters.Count; i++) {
					tileData.Flags |= (TileFlags) Enum.Parse(typeof(TileFlags), parameters.GetString(i), true);
				}
			});
			// Properties <(type, name, value, editor-type, category, description)...>
			AddCommand("Properties", delegate(CommandParam parameters) {
				// TODO: handle lists.
				for (int i = 0; i < parameters.Count; i++) {
					CommandParam param = parameters[i];
					
					Property property = null;
					string name = param.GetString(1);
					PropertyType type = (PropertyType) Enum.Parse(typeof(PropertyType), param.GetString(0), true);

					if (type == PropertyType.String)
						property = Property.CreateString(name, param.GetString(2));
					else if (type == PropertyType.Integer)
						property = Property.CreateInt(name, param.GetInt(2));
					else if (type == PropertyType.Float)
						property = Property.CreateFloat(name, param.GetFloat(2));
					else if (type == PropertyType.Boolean)
						property = Property.CreateBool(name, (param.GetString(2) == "true"));
					else
						ThrowParseError("Unsupported property type for " + name);

					if (param.Count > 3) {
						property.SetDocumentation(
							param.GetString(3),
							param.GetString(4),
							param.GetString(5),
							param.GetString(6));
					}

					if (property != null)
						tileData.Properties.Add(property);
				}
			});
			// Sprite <sprite>
			AddCommand("Sprite", delegate(CommandParam parameters) {
				tileData.Sprite = Resources.GetSpriteAnimation(parameters.GetString(0));
			});
			// SpriteList [sprite1] [sprite2]...
			AddCommand("SpriteList", delegate(CommandParam parameters) {
				SpriteAnimation[] spriteList = new SpriteAnimation[parameters.Count];
				for (int i = 0; i < parameters.Count; i++)
					spriteList[i] = Resources.GetSpriteAnimation(parameters.GetString(i));

				tileData.SpriteList = spriteList;
			});
			// SpriteObj <sprite-object>
			AddCommand("SpriteObj", delegate(CommandParam parameters) {
				tileData.SpriteAsObject = Resources.GetSpriteAnimation(parameters.GetString(0));
			});
			// BreakAnim <animation>
			AddCommand("BreakAnim", delegate(CommandParam parameters) {
				tileData.BreakAnimation = Resources.GetAnimation(parameters.GetString(0));
			});
			// Model <collision-model>
			AddCommand("Model", delegate(CommandParam parameters) {
				tileData.CollisionModel = Resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			// Solid <collision-model>
			AddCommand("Solid", delegate(CommandParam parameters) {
				tileData.Flags |= TileFlags.Solid;
				tileData.CollisionModel = Resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			// Ledge <collision-model> <direction>
			AddCommand("ledge", delegate(CommandParam parameters) {
				tileData.Flags |= TileFlags.Solid;
				tileData.CollisionModel = Resources.GetResource<CollisionModel>(parameters.GetString(0));
				string dirName = parameters.GetString(1);
				if (dirName == "right" || dirName == "east")
					tileData.Flags |= TileFlags.LedgeRight;
				else if (dirName == "left" || dirName == "west")
					tileData.Flags |= TileFlags.LedgeLeft;
				else if (dirName == "up" || dirName == "north")
					tileData.Flags |= TileFlags.LedgeUp;
				else if (dirName == "down" || dirName == "south")
					tileData.Flags |= TileFlags.LedgeDown;
			});
		}

		// Begins reading the script.
		protected override void BeginReading() {
			tileset  = null;
			tileData = null;
		}

		// Ends reading the script.
		protected override void EndReading() {

		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
} // end namespace
