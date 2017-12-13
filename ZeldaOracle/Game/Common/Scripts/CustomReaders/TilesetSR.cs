using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Scripts.Commands;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaOracle.Common.Scripts.CustomReaders {


	public class TilesetSR : ScriptReader {

		private enum LoadingModes {
			Tilesets,
			Animations,
			Sprites
		}

		private Tileset				tileset;
		private EventTileset		eventTileset;
		private BaseTileData		baseTileData;
		private TileData			tileData;
		private EventTileData		eventTileData;
		//private LoadingModes		loadingMode;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilesetSR() {

			//this.loadingMode	= LoadingModes.Tilesets;

			//=====================================================================================
			// LOADING MODE 
			//=====================================================================================
			//AddCommand("Load", "string resourceType",
			//delegate(CommandParam parameters) {
				/*string loadType = parameters.GetString(0).ToLower();
				if (loadType == "tilesets")
					loadingMode = LoadingModes.Tilesets;
				else if (loadType == "animations")
					loadingMode = LoadingModes.Animations;
				else if (loadType == "sprites")
					loadingMode = LoadingModes.Sprites;
				else
					ThrowParseError("Invalid Load type", true);*/
			//});
			//=====================================================================================
			// TILE/TILESET BEGIN/END 
			//=====================================================================================
			AddCommand("Tileset", "string name, string sheetName, (int width, int height)",
			delegate(CommandParam parameters) {
				SpriteSheet sheet = GetResource<ISpriteSource>(parameters.GetString(1)) as SpriteSheet;
				tileset = new Tileset(parameters.GetString(0), sheet,
									  parameters.GetPoint(2));
			});
			//=====================================================================================
			AddCommand("EventTileset", "string name, (int width, int height)",
			delegate(CommandParam parameters) {
				eventTileset = new EventTileset(parameters.GetString(0),
					null, parameters.GetPoint(1));
			});
			//=====================================================================================
			AddCommand("Tile",
				"string name",
				"(int sheetX, int sheetY)",
			delegate(CommandParam parameters) {
				if (parameters.GetParam(0).Type == CommandParamType.Array) {
					Point2I location = parameters.GetPoint(0);
					tileData = tileset.TileData[location.X, location.Y];
				}
				else {
					tileData = new TileData();
					tileData.Name = parameters.GetString(0);
				}
				baseTileData = tileData;
			});
			//=====================================================================================
			AddCommand("EventTile", "string name",
			delegate(CommandParam parameters) {
				eventTileData = new EventTileData();
				eventTileData.Name = parameters.GetString(0);
				baseTileData = eventTileData;
			});
			//=====================================================================================
			AddCommand("Monster", "string name, string sprite, string monsterType, string monsterColor",
			delegate(CommandParam parameters) {
				eventTileData = new EventTileData();
				eventTileData.Clone(GetResource<EventTileData>("monster"));
				eventTileData.Name = parameters.GetString(0);
				baseTileData = eventTileData;
								
				if (parameters.ChildCount > 1)
					eventTileData.Sprite = GetResource<ISprite>(parameters.GetString(1));
				if (parameters.ChildCount > 2)
					eventTileData.Properties.Set("monster_type", parameters.GetString(2));
				if (parameters.ChildCount > 3) {
					MonsterColor color;
					if (!Enum.TryParse<MonsterColor>(parameters.GetString(3), true, out color))
						ThrowParseError("Invalid monster color: \"" + parameters.GetString(3) + "\"!");
					eventTileData.Properties.Set("color", (int) color);
					int imageVariantID = GameData.VARIANT_RED;
					if (color == MonsterColor.Red)
						imageVariantID = GameData.VARIANT_RED;
					else if (color == MonsterColor.Blue)
						imageVariantID = GameData.VARIANT_BLUE;
					else if (color == MonsterColor.Green)
						imageVariantID = GameData.VARIANT_GREEN;
					else if (color == MonsterColor.Orange)
						imageVariantID = GameData.VARIANT_ORANGE;
					eventTileData.Properties.Set("image_variant", imageVariantID);
				}
			});
			//=====================================================================================
			AddCommand("End", "", delegate(CommandParam parameters) {
				if (tileData != null) {
					if (tileData.Tileset == null) {
						AddResource<TileData>(tileData.Name, tileData);
					}
					tileData = null;
					baseTileData = null;
				}
				else if (eventTileData != null) {
					if (eventTileData.Tileset == null) {
						AddResource<EventTileData>(eventTileData.Name, eventTileData);
					}
					eventTileData = null;
					baseTileData = null;
				}
				else if (tileset != null) {
					AddResource<Tileset>(tileset.ID, tileset);
					tileset = null;
				}
				else if (eventTileset != null) {
					AddResource<EventTileset>(eventTileset.ID, eventTileset);
					eventTileset = null;
				}
			});
			//=====================================================================================
			// TILESET SETUP
			//=====================================================================================
			AddCommand("Default", "(int defaultSheetX, int defaultSheetY)",
			delegate(CommandParam parameters) {
				tileset.DefaultTile = parameters.GetPoint(0);
			});
			//=====================================================================================
			// Config: data to configure tiles with a single character per tile.
			AddCommand("Config", "", delegate(CommandParam parameters) {
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
			//=====================================================================================
			AddCommand("SetTile", "(int sheetX, int sheetY), string tileName",
			delegate(CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);

				if (tileset != null) {
					tileset.TileData[location.X, location.Y] = 
						GetResource<TileData>(parameters.GetString(1));
				}
				else if (eventTileset != null) {
					eventTileset.TileData[location.X, location.Y] = 
						GetResource<EventTileData>(parameters.GetString(1));
				}
			});
			//=====================================================================================
			// TILES 
			//=====================================================================================
			AddCommand("Type", "string type",
			delegate(CommandParam parameters) {
				baseTileData.Type = Tile.GetType(parameters.GetString(0), true);
			});
			//=====================================================================================
			AddCommand("Flags", "string flags...",
			delegate(CommandParam parameters) {
				for (int i = 0; i < parameters.ChildCount; i++) {
					TileFlags flags = TileFlags.None;
					if (Enum.TryParse<TileFlags>(parameters.GetString(i), true, out flags))
						tileData.Flags |= flags;
					else
						ThrowParseError("Invalid tile flag: \"" + parameters.GetString(i) + "\"!", parameters[i]);
				}
			});
			//=====================================================================================
			AddCommand("EnvType", "string envType",
			delegate(CommandParam parameters) {
				TileEnvironmentType envType = TileEnvironmentType.Normal;
				if (Enum.TryParse<TileEnvironmentType>(parameters.GetString(0), true, out envType))
					tileData.Properties.Set("environment_type", (int) envType);
				else
					ThrowParseError("Invalid tile environment type: \"" + parameters.GetString(0) + "\"!", parameters[0]);
			});
			//=====================================================================================
			AddCommand("ResetWhen", "string resetCondition",
			delegate(CommandParam parameters) {
				TileResetCondition envType = TileResetCondition.LeaveRoom;
				if (Enum.TryParse<TileResetCondition>(parameters.GetString(0), true, out envType))
					tileData.ResetCondition = envType;
				else
					ThrowParseError("Invalid tile reset condition: \"" + parameters.GetString(0) + "\"!", parameters[0]);
			});
			//=====================================================================================
			AddCommand("Conveyor", "string angle, float speed",
			delegate(CommandParam parameters) {
				string str = parameters.GetString(0).ToLower();
				int angle = -1;
				if (Angles.TryParse(str, true, out angle))
					tileData.ConveyorAngle = angle;
				else if (parameters[0].Type == CommandParamType.Integer)
					tileData.ConveyorAngle = parameters.GetInt(0);
				else
					ThrowParseError("Unknown value for conveyor angle: " + str, parameters[0]);

				tileData.ConveyorSpeed = parameters.GetFloat(1);
			});
			//=====================================================================================
			// (string type, string name, var value)...
			// (string type, string name, var value, string readableName, string editorType, string category, string description)...
			// (string type, string name, var value, string readableName, (string editorType, string editorSubType), string category, string description, bool isHidden = false)...
			AddCommand("Properties",
				"(string type, string name, var otherData...)...",
				CommandProperties);
			//=====================================================================================
			AddCommand("Event",
				"string name, string readableName, string category, string description",
				"string name, string readableName, string category, string description, (string params...)", // Params = (type1, name1, type2, name2...)
			delegate(CommandParam parameters) {
				Property property = Property.CreateString(parameters.GetString(0), "");
				//property.SetDocumentation(parameters.GetString(1), "script", "", "Events", parameters.GetString(2), true, false);
				//baseTileData.Properties.Set(property.Name, property)
				//	.SetDocumentation(parameters.GetString(1), "script", "", "Events", parameters.GetString(2), true, false);
				
				// Create the event's script parameter list.
				ScriptParameter[] scriptParams;
				if (parameters.ChildCount > 4) {
					CommandParam paramList = parameters[4];
					scriptParams = new ScriptParameter[paramList.ChildCount / 2];
					for (int i = 0; i < scriptParams.Length; i++) {
						scriptParams[i] = new ScriptParameter() {
							Type = paramList.GetString(i * 2),
							Name = paramList.GetString((i * 2) + 1)
						};
					}
				}
				else
					scriptParams = new ScriptParameter[0];

				// Add the event to the tile-data.
				baseTileData.Events.AddEvent(
						parameters.GetString(0), // Name
						parameters.GetString(1), // Readable name
						parameters.GetString(2), // Category
						parameters.GetString(3), // Description
						scriptParams);
			});
			//=====================================================================================
			AddCommand("Sprite",
				"string spriteOrAnimationName",
				"string spriteSheetName, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
			delegate(CommandParam parameters) {
				if (parameters.ChildCount >= 2) {
					baseTileData.Sprite = GetResource<ISpriteSource>(parameters.GetString(0))
						.GetSprite(parameters.GetPoint(1));
					Point2I drawOffset = parameters.GetPoint(2);
					if (!drawOffset.IsZero) {
						baseTileData.Sprite = new OffsetSprite(baseTileData.Sprite, drawOffset);
					}
				}
				else {
					baseTileData.Sprite = GetResource<ISprite>(parameters.GetString(0));
				}
			});
			//=====================================================================================
			AddCommand("SameSprite",
				"",
			delegate (CommandParam parameters) {
				if (baseTileData == null)
					ThrowCommandParseError("Cannot call SpriteSame without editing tile data");
				baseTileData.Sprite = GetResource<ISprite>("tile_" + baseTileData.Name);
			});
			//=====================================================================================
			AddCommand("Size", "(int width, int height)", delegate(CommandParam parameters) {
				tileData.Size = parameters.GetPoint(0);
			});
			//=====================================================================================
			AddCommand("SpriteIndex",
				"int index, string spriteOrAnim, string spriteAnimationName",
				"int index, string spriteAnimationName",
				"int index, string spriteSheetName, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
			delegate(CommandParam parameters) {
				int index = parameters.GetInt(0);
				if (tileData.SpriteList.Length <= index) {
					ISprite[] spriteList = new ISprite[index + 1];
					for (int i = 0; i < spriteList.Length; i++) {
						if (i < tileData.SpriteList.Length)
							spriteList[i] = tileData.SpriteList[i];
						else
							spriteList[i] = null;
					}
					tileData.SpriteList = spriteList;
				}
				if (parameters.ChildCount > 2 && parameters[2].Type == CommandParamType.Array) {
					ISprite sprite = GetResource<ISpriteSource>(parameters.GetString(1))
						.GetSprite(parameters.GetPoint(2));
					Point2I drawOffset = parameters.GetPoint(3);
					if (!drawOffset.IsZero) {
						sprite = new OffsetSprite(sprite, drawOffset);
					}
					tileData.SpriteList[index] = sprite;
				}
				else {
					if (parameters.ChildCount == 3) {
						string typeName = parameters.GetString(1);
						if (typeName == "sprite")
							tileData.SpriteList[index] = GetResource<ISprite>(parameters.GetString(2));
						else if (typeName == "animation")
							tileData.SpriteList[index] = GetResource<Animation>(parameters.GetString(2));
						else
							ThrowParseError("Unknown sprite/animation type '" + typeName + "' (expected \"sprite\" or \"animation\")");
					}
					else {
						tileData.SpriteList[index] = GetResource<ISprite>(parameters.GetString(1));
					}
				}
			});
			//=====================================================================================
			AddCommand("SpriteList", "string spriteAnimationNames...", delegate(CommandParam parameters) {
				ISprite[] spriteList = new ISprite[parameters.ChildCount];
				for (int i = 0; i < parameters.ChildCount; i++)
					spriteList[i] = GetResource<ISprite>(parameters.GetString(i));

				tileData.SpriteList = spriteList;
			});
			//=====================================================================================
			AddCommand("SpriteObj",
				"string spriteAnimationName",
				"string spriteSheetName, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
			delegate(CommandParam parameters) {
				if (parameters.ChildCount >= 2) {
					baseTileData.Sprite = GetResource<ISpriteSource>(parameters.GetString(0))
						.GetSprite(parameters.GetPoint(1));
					Point2I drawOffset = parameters.GetPoint(2);
					if (!drawOffset.IsZero) {
						baseTileData.Sprite = new OffsetSprite(baseTileData.Sprite, drawOffset);
					}
				}
				else {
					tileData.SpriteAsObject = GetResource<ISprite>(parameters.GetString(0));
				}
			});
			//=====================================================================================
			AddCommand("BreakAnim", "string animationName",
			delegate(CommandParam parameters) {
				tileData.BreakAnimation = GetResource<ISprite>(parameters.GetString(0)) as Animation;
			});
			//=====================================================================================
			AddCommand("BreakSound", "string soundName",
			delegate(CommandParam parameters) {
				tileData.BreakSound = GetResource<Sound>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("Model", "string collisionModelName",
			delegate(CommandParam parameters) {
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("Solid", "string collisionModelName",
			delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.Solid;
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("HalfSolid", "string collisionModelName",
			delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.HalfSolid;
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("Ledge", "string collisionModelName, string ledgeDirection",
			delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.Ledge;
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
				string dirName = parameters.GetString(1);
				int direction;
				if (Directions.TryParse(dirName, true, out direction))
					tileData.LedgeDirection = direction;
				else
					ThrowParseError("Unknown value for ledge direction: " + dirName);
			});
			//=====================================================================================
			AddCommand("Hurt", "int damage, (int areaX, int areaY, int areaWidth, int areaHeight)",
			delegate(CommandParam parameters) {
				tileData.HurtDamage = parameters.GetInt(0);
				tileData.HurtArea = new Rectangle2I(
					parameters[1].GetInt(0),
					parameters[1].GetInt(1),
					parameters[1].GetInt(2),
					parameters[1].GetInt(3));
			});

			AddCommand("Clone", "string tileDataName",
			delegate(CommandParam parameters) {
				if (tileData != null)
					tileData.Clone(GetResource<TileData>(parameters.GetString(0)));
				else if (eventTileData != null)
					eventTileData.Clone(GetResource<EventTileData>(parameters.GetString(0)));
			});

			// SPRITE SHEET ---------------------------------------------------------------

			AddCommand("SpriteSheet",
				"string path, (int cellWidth, int cellHeight), (int spacingX, int spacingY), (int offsetX, int offsetY)",
				"string name, string path, (int cellWidth, int cellHeight), (int spacingX, int spacingY), (int offsetX, int offsetY)",
			delegate(CommandParam parameters) {
			//AddSpriteCommand("SpriteSheet", delegate(CommandParam parameters) {
				if (parameters.ChildCount == 1) {
					// Start using the given sprite sheet.
					SpriteSheet sheet = GetResource<ISpriteSource>(parameters.GetString(0)) as SpriteSheet;
				}
				else {
					int i = 1;
					// Create a new sprite sheet.
					Image image = null;
					string imagePath = parameters.GetString(0);
					string sheetName = imagePath;

					if (parameters.ChildCount == 5) {
						imagePath = parameters.GetString(1);
						i = 2;
					}

					if (Resources.ContainsImage(imagePath))
						image = Resources.GetResource<Image>(imagePath);
					else
						image = Resources.LoadImage(Resources.ImageDirectory + imagePath);

					if (sheetName.IndexOf('/') >= 0)
						sheetName = sheetName.Substring(sheetName.LastIndexOf('/') + 1);

					SpriteSheet sheet = new SpriteSheet(image,
							parameters.GetPoint(i + 0),
							parameters.GetPoint(i + 2),
							parameters.GetPoint(i + 1));
					AddResource<ISpriteSource>(sheetName, sheet);
				}
			});

		}


		//-----------------------------------------------------------------------------
		// Script Commands
		//-----------------------------------------------------------------------------

		private void CommandProperties(CommandParam parameters) {
			foreach (CommandParam child in parameters.GetChildren())
				ParseProperty(child);
		}

		private void ParseProperty(CommandParam param) {
			string name = param.GetString(1);

			// Parse the property type and value.
			PropertyType type;
			if (!Enum.TryParse<PropertyType>(param.GetString(0), true, out type))
				ThrowParseError("Unknown property type " + name);
			object value = ParsePropertyValue(param[2], type);
						
			// Set the property.
			Property property = baseTileData.Properties.SetGeneric(name, value);

			// Set the property's documentation.
			if (param.ChildCount > 3 && property != null) {
				string editorType = "";
				string editorSubType = "";
				if (param[4].Type == CommandParamType.Array) {
					editorType = param[4].GetString(0);
					editorSubType = param[4].GetString(1);
				}
				else {
					editorType = param.GetString(4);
				}
				
				property.SetDocumentation(
					readableName:	param.GetString(3),
					editorType:		editorType,
					editorSubType:	editorSubType,
					category:		param.GetString(5),
					description:	param.GetString(6),
					isReadOnly:		false,
					isBrowsable:	param.GetBool(7, false)
				);
			}
		}

		private object ParsePropertyValue(CommandParam param, PropertyType type) {
			if (type == PropertyType.String) {
				if (param.IsValidType(CommandParamType.String))
					return param.StringValue;
			}
			else if (type == PropertyType.Integer) {
				if (param.IsValidType(CommandParamType.Integer))
					return param.IntValue;
			}
			else if (type == PropertyType.Float) {
				if (param.IsValidType(CommandParamType.Float))
					return param.FloatValue;
			}
			else if (type == PropertyType.Boolean) {
				if (param.IsValidType(CommandParamType.Boolean))
					return param.BoolValue;
			}
			else if (type == PropertyType.List)
				ThrowParseError("Lists are unsupported as a property type");
			ThrowParseError("The property value '" + param.StringValue + "' is not of type " + type.ToString());
			return null;
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			//loadingMode	= LoadingModes.Tilesets;
			tileset		= null;
			tileData	= null;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {

		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new TilesetSR();
		}
	}
}
