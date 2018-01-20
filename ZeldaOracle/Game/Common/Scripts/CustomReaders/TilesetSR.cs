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

		private enum Modes {
			Root,
			Tileset
		}

		private enum LoadingModes {
			Tilesets,
			Animations,
			Sprites
		}

		//private TilesetOld				tileset;
		private Tileset			newTileset;
		//private EventTileset		eventTileset;
		private BaseTileData		baseTileData;
		private TileData			tileData;
		private EventTileData		eventTileData;
		//private LoadingModes		loadingMode;
		private ISpriteSource		source;
		private string				tilesetName;


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
			// Type Definitions
			//=====================================================================================
			AddType("Sprite",
				"string spriteName",
				// Int needs to go before string as int/float defaults to string.
				"Point sourceIndex",
				"(string animationName, int substrip)",
				"(string spriteName, string definition)",
				"(Point sourceIndex, string definition)",
				"(string sourceName, Point sourceIndex)",
				"(string sourceName, Point sourceIndex, string definition)"
			);
			//=====================================================================================
			// SOURCE
			//=====================================================================================
			AddCommand("SOURCE", "string name",
			delegate (CommandParam parameters) {
				string name = parameters.GetString(0);
				if (name.ToLower() == "none") {
					source = null;
					return;
				}
				if (!ContainsResource<ISpriteSource>(name)) {
					ThrowCommandParseError("No sprite source with the name '" + name + "' exists in resources!");
				}
				source = Resources.GetResource<ISpriteSource>(name);
			});
			//=====================================================================================
			// TILE/TILESET BEGIN/END 
			//=====================================================================================
			/*AddCommand("TILESET",
				"string name, string sheetName, Point size",
			delegate(CommandParam parameters) {
				tilesetName = parameters.GetString(0);
				SpriteSheet sheet = GetResource<ISpriteSource>(parameters.GetString(1)) as SpriteSheet;
				tileset = new TilesetOld(tilesetName, sheet,
					parameters.GetPoint(2));
				AddResource<Tileset>(tileset.ID, tileset);
			});*/
			//=====================================================================================
			AddCommand("TILESET", (int) Modes.Root,
				"string name, Point size, bool usePreviewSprites = true",
			delegate (CommandParam parameters) {
				tilesetName = parameters.GetString(0);
				newTileset = new Tileset(tilesetName,
					parameters.GetPoint(1), parameters.GetBool(2));
				AddResource<Tileset>(newTileset.ID, newTileset);
				Mode = Modes.Tileset;
			});
			//=====================================================================================
			AddCommand("CONTINUE TILESET", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				tilesetName = parameters.GetString(0);
				newTileset = GetResource<Tileset>(tilesetName);
				Mode = Modes.Tileset;
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Tileset,
				"",
			delegate (CommandParam parameters) {
				newTileset = null;
				tilesetName = "";
				Mode = Modes.Root;
			});
			//=====================================================================================
			// TILESET SETUP
			//=====================================================================================
			AddCommand("CLONE TILESET", (int) Modes.Tileset,
				"string tileset",
			delegate (CommandParam parameters) {
				Tileset cloneTileset = GetResource<Tileset>(parameters.GetString(0));
				newTileset = new Tileset(cloneTileset);
				newTileset.ID = tilesetName;
				SetResource<Tileset>(tilesetName, newTileset);
			});
			//=====================================================================================
			AddCommand("RESIZE", (int) Modes.Tileset,
				"Point newDimensions",
			delegate (CommandParam parameters) {
				newTileset.Resize(parameters.GetPoint(0));
			});
			//=====================================================================================
			// TILESET BUILDING
			//=====================================================================================
			AddCommand("ADDTILE", (int) Modes.Tileset,
				"Point sourceIndex, string tileName",
			delegate (CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);
				newTileset.AddTileData(location,
					GetResource<BaseTileData>(parameters.GetString(1)));
			});
			//=====================================================================================
			AddCommand("SETTILE", (int) Modes.Tileset,
				"Point sourceIndex, string tileName",
			delegate (CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);
				newTileset.SetTileData(location,
					GetResource<BaseTileData>(parameters.GetString(1)));
			});
			//=====================================================================================
			AddCommand("REMOVETILE", (int) Modes.Tileset,
				"Point sourceIndex",
			delegate (CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);
				newTileset.RemoveTileData(location);
			});
			//=====================================================================================
			/*AddCommand("ACTIONTILESET", "string name, Point size",
			delegate(CommandParam parameters) {
				eventTileset = new EventTileset(parameters.GetString(0),
					null, parameters.GetPoint(1));
			});*/
			//=====================================================================================
			/*AddCommand("TILE",
				"Point sourceIndex",
			delegate(CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);
				tileData = tileset.TileData[location.X, location.Y];
				baseTileData = tileData;
			});*/
			//=====================================================================================
			/*AddCommand("ACTIONTILE", "string name",
			delegate (CommandParam parameters) {
				eventTileData = new EventTileData();
				eventTileData.Name = parameters.GetString(0);
				baseTileData = eventTileData;
			});
			//=====================================================================================
			AddCommand("MONSTER", "string name, string sprite, string monsterType, string monsterColor",
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
			});*/
			//=====================================================================================
			// TILESET SETUP
			//=====================================================================================
			/*AddCommand("DEFAULT", "Point defaultIndex",
			delegate(CommandParam parameters) {
				tileset.DefaultTile = parameters.GetPoint(0);
			});*/
			//=====================================================================================

			//=====================================================================================
			// Config: data to configure tiles with a single character per tile.
			/*AddCommand("CONFIG", "", delegate(CommandParam parameters) {
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
			});*/
			//=====================================================================================
			// TILES 
			//=====================================================================================
			/*AddCommand("TYPE", "string type",
			delegate(CommandParam parameters) {
				baseTileData.Type = Tile.GetType(parameters.GetString(0), true);
			});
			//=====================================================================================
			AddCommand("FLAGS", "string flags...",
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
			AddCommand("ENVTYPE", "string envType",
			delegate(CommandParam parameters) {
				TileEnvironmentType envType = TileEnvironmentType.Normal;
				if (Enum.TryParse<TileEnvironmentType>(parameters.GetString(0), true, out envType))
					tileData.Properties.Set("environment_type", (int) envType);
				else
					ThrowParseError("Invalid tile environment type: \"" + parameters.GetString(0) + "\"!", parameters[0]);
			});
			//=====================================================================================
			AddCommand("RESETWHEN", "string resetCondition",
			delegate(CommandParam parameters) {
				TileResetCondition envType = TileResetCondition.LeaveRoom;
				if (Enum.TryParse<TileResetCondition>(parameters.GetString(0), true, out envType))
					tileData.ResetCondition = envType;
				else
					ThrowParseError("Invalid tile reset condition: \"" + parameters.GetString(0) + "\"!", parameters[0]);
			});
			//=====================================================================================
			AddCommand("CONVEYOR", "string angle, float speed",
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
			AddCommand("PROPERTIES",
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
			AddCommand("SPRITE",
				"string spriteOrAnimationName",
				"string spriteSheetName, Point sourceIndex, Point drawOffset = (0, 0)",
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
			AddCommand("SAMESPRITE",
				"",
			delegate (CommandParam parameters) {
				if (baseTileData == null)
					ThrowCommandParseError("Cannot call SAMESPRITE without editing tile data");
				baseTileData.Sprite = GetResource<ISprite>("tile_" + baseTileData.Name);
			});
			//=====================================================================================
			AddCommand("SAMESPRITEABOVE",
				"",
			delegate (CommandParam parameters) {
				if (tileData == null)
					ThrowCommandParseError("Cannot call SAMESPRITEABOVE without editing tile data");
				tileData.SpriteAbove = GetResource<ISprite>("tile_" + baseTileData.Name + "_above");
			});
			//=====================================================================================
			AddCommand("SAMESPRITEOBJ",
				"",
			delegate (CommandParam parameters) {
				if (tileData == null)
					ThrowCommandParseError("Cannot call SAMESPRITEOBJ without editing tile data");
				tileData.SpriteAsObject = GetResource<ISprite>("tile_" + baseTileData.Name + "_asobject");
			});
			//=====================================================================================
			AddCommand("SIZE", "Point size", delegate(CommandParam parameters) {
				baseTileData.Size = parameters.GetPoint(0);
			});
			//=====================================================================================
			AddCommand("SPRITEINDEX",
				"int index, string spriteOrAnim, string spriteAnimationName",
				"int index, string spriteAnimationName",
				"int index, string spriteSheetName, Point sourceIndex, Point drawOffset = (0, 0)",
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
			AddCommand("SPRITELIST", "string spriteAnimationNames...", delegate(CommandParam parameters) {
				ISprite[] spriteList = new ISprite[parameters.ChildCount];
				for (int i = 0; i < parameters.ChildCount; i++)
					spriteList[i] = GetResource<ISprite>(parameters.GetString(i));

				tileData.SpriteList = spriteList;
			});
			//=====================================================================================
			AddCommand("SPRITEABOVE", "const none", "const null",
			delegate (CommandParam parameters) {
				if (tileData == null)
					ThrowCommandParseError("Cannot call SPRITEABOVE without editing tile data");
				tileData.SpriteAsObject = null;
			});
			//=====================================================================================
			AddCommand("SPRITEABOVE",
				"Sprite sprite",
			delegate (CommandParam parameters) {
				if (tileData == null)
					ThrowCommandParseError("Cannot call SPRITEABOVE without editing tile data");
				tileData.SpriteAsObject = GetSpriteFromParams(parameters);
			});
			//=====================================================================================
			AddCommand("SPRITEOBJ",
				"string spriteAnimationName",
				"string spriteSheetName, Point sourceIndex, Point drawOffset = (0, 0)",
			delegate(CommandParam parameters) {
				if (tileData == null)
					ThrowCommandParseError("Cannot call SPRITEOBJ without editing tile data");
				if (parameters.ChildCount >= 2) {
					tileData.SpriteAsObject = GetResource<ISpriteSource>(parameters.GetString(0))
						.GetSprite(parameters.GetPoint(1));
					Point2I drawOffset = parameters.GetPoint(2);
					if (!drawOffset.IsZero) {
						tileData.SpriteAsObject = new OffsetSprite(baseTileData.Sprite, drawOffset);
					}
				}
				else {
					tileData.SpriteAsObject = GetResource<ISprite>(parameters.GetString(0));
				}
			});
			//=====================================================================================
			AddCommand("BREAKANIM", "string animationName",
			delegate(CommandParam parameters) {
				tileData.BreakAnimation = GetResource<ISprite>(parameters.GetString(0)) as Animation;
			});
			//=====================================================================================
			AddCommand("BREAKSOUND", "string soundName",
			delegate(CommandParam parameters) {
				tileData.BreakSound = GetResource<Sound>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("MODEL", "string collisionModelName",
			delegate(CommandParam parameters) {
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("SOLID", "string collisionModelName",
			delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.Solid;
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("HALFSOLID", "string collisionModelName",
			delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.HalfSolid;
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("LEDGE", "string collisionModelName, string ledgeDirection",
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
			AddCommand("HURT", "int damage, Rectangle area",
			delegate(CommandParam parameters) {
				tileData.HurtDamage = parameters.GetInt(0);
				tileData.HurtArea = parameters.GetRectangle(1);
			});
			//=====================================================================================
			AddCommand("CLONE", "string tileDataName",
			delegate(CommandParam parameters) {
				if (tileData != null)
					tileData.Clone(GetResource<TileData>(parameters.GetString(0)));
				else if (eventTileData != null)
					eventTileData.Clone(GetResource<EventTileData>(parameters.GetString(0)));
			});
			//=====================================================================================
			AddCommand("PREVIEW",
				"Sprite sprite",
			delegate (CommandParam parameters) {
				baseTileData.PreviewSprite = GetSpriteFromParams(parameters);
			});

			// SPRITE SHEET ---------------------------------------------------------------

			AddCommand("SPRITESHEET",
				"string path, Point cellSize, Point spacing, Point offset",
				"string name, string path, Point cellSize, Point spacing, Point offset",
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
			});*/

		}


		//-----------------------------------------------------------------------------
		// Script Commands
		//-----------------------------------------------------------------------------

		/*private void CommandProperties(CommandParam parameters) {
			foreach (CommandParam child in parameters.GetChildren())
				ParseProperty(child);
		}*/

		/*private void ParseProperty(CommandParam param) {
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
					isBrowsable:	param.GetBool(7, true)
				);
			}
		}*/

		/*private object ParsePropertyValue(CommandParam param, PropertyType type) {
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
		}*/


		/// <summary>Gets a sprite.</summary>
		private ISprite GetSprite(string name) {
			ISprite sprite = GetResource<ISprite>(name);
			if (sprite == null) {
				ThrowCommandParseError("Sprite with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite.</summary>
		private ISprite GetSprite(ISpriteSource source, Point2I index) {
			if (source == null)
				ThrowCommandParseError("Cannot get sprite from source with no sprite sheet source!");
			ISprite sprite = source.GetSprite(index);
			if (sprite == null) {
				ThrowCommandParseError("Sprite at source index '" + index + "' does not exist!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite and confirms its type.</summary>
		private T GetSprite<T>(string name) where T : class, ISprite {
			T sprite = GetResource<ISprite>(name) as T;
			if (sprite == null) {
				ThrowCommandParseError(typeof(T).Name + " with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite and confirms its type.</summary>
		private T GetSprite<T>(ISpriteSource source, Point2I index) where T : class, ISprite {
			if (source == null)
				ThrowCommandParseError("Cannot get sprite from source with no sprite sheet source!");
			T sprite = source.GetSprite(index) as T;
			if (sprite == null) {
				ThrowCommandParseError(typeof(T).Name + " at source index '" + index + "' does not exist!");
			}
			return sprite;
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(string name, string definition) {
			return GetDefinedSprite(GetSprite<DefinitionSprite>(name), definition);
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(ISpriteSource source, Point2I index, string definition) {
			return GetDefinedSprite(GetSprite<DefinitionSprite>(source, index), definition);
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(DefinitionSprite sprite, string definition) {
			ISprite defSprite = sprite.Get(definition);
			if (defSprite == null)
				ThrowCommandParseError("Defined sprite with definition '" + definition + "' does not exist!");
			return defSprite;
		}

		/// <summary>Gets the sprite from one of the many parameter overloads.</summary>
		private ISprite GetSpriteFromParams(CommandParam param, int startIndex = 0) {
			ISpriteSource source;
			Point2I index;
			string definition;
			return GetSpriteFromParams(param, startIndex, out source, out index, out definition);
		}

		/// <summary>Gets the sprite from one of the many parameter overloads and returns the source.</summary>
		private ISprite GetSpriteFromParams(CommandParam param, int startIndex, out ISpriteSource source, out Point2I index, out string definition) {
			// 1: string spriteName
			// 2: (int indexX, int indexY)
			// 3: (string animationName, int substrip)
			// 4: (string spriteName, string definition)
			// 5: ((int indexX, int indexY), string definition)
			// 6: (string sourceName, (int indexX, int indexY))
			// 7: (string sourceName, (int indexX, int indexY), string definition)
			source = null;
			index = Point2I.Zero;
			definition = null;

			var param0 = param.GetParam(startIndex);
			if (param0.Type == CommandParamType.String) {
				// Overload 1:
				return GetResource<ISprite>(param.GetString(startIndex));
			}
			else if (param0.GetParam(0).IsValidType(CommandParamType.Integer)) {
				// Overload 2:
				source = this.source;
				index = param.GetPoint(startIndex);
				return GetSprite(source, index);
			}
			else if (param0.GetParam(0).IsValidType(CommandParamType.String)) {
				if (param0.GetParam(1).IsValidType(CommandParamType.Integer)) {
					// Overload 3:
					return GetSprite<Animation>(param0.GetString(0)).GetSubstrip(param0.GetInt(1));
				}
				else if (param0.GetParam(1).IsValidType(CommandParamType.String)) {
					// Overload 4:
					return GetDefinedSprite(param0.GetString(0), param0.GetString(1));
				}
				else if (param0.ChildCount == 2) {
					// Overload 6:
					source = GetResource<ISpriteSource>(param0.GetString(0));
					index = param0.GetPoint(1);
					return GetSprite(source, index);
				}
				else {
					// Overload 7:
					source = GetResource<ISpriteSource>(param0.GetString(0));
					index = param0.GetPoint(1);
					definition = param0.GetString(2);
					return GetDefinedSprite(source, index, definition);
				}
			}
			else {
				// Overload 5:
				source = this.source;
				index = param0.GetPoint(0);
				definition = param0.GetString(1);
				return GetDefinedSprite(source, index, definition);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			//loadingMode	= LoadingModes.Tilesets;
			//tileset			= null;
			newTileset		= null;
			tileData		= null;
			eventTileData	= null;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {

		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new TilesetSR();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Gets the source as a sprite sheet.</summary>
		private SpriteSheet SpriteSheet {
			get { return source as SpriteSheet; }
		}

		/// <summary>Gets the source as a sprite set.</summary>
		private SpriteSet SpriteSet {
			get { return source as SpriteSet; }
		}

		/// <summary>The mode of the Tileset script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
