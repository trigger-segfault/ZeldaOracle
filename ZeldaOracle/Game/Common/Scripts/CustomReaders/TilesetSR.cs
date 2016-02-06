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
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Scripts.Commands;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaOracle.Common.Scripts.CustomReaders {

	public enum LoadingModes {
		Tilesets,
		Animations,
		Sprites
	}

	public class TilesetSR : ScriptReader {

		private Tileset				tileset;
		private EventTileset		eventTileset;
		private BaseTileData		baseTileData;
		private TileData			tileData;
		private EventTileData		eventTileData;
		//private LoadingModes		loadingMode;
		private SpriteBuilder		spriteBuilder;
		private TemporaryResources	resources;
		private bool				useTemporary;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilesetSR() {

			//this.loadingMode	= LoadingModes.Tilesets;
			this.resources		= new TemporaryResources();
			this.useTemporary	= false;
			this.spriteBuilder	= new SpriteBuilder();

			//=====================================================================================
			// LOADING MODE 
			//=====================================================================================
			AddCommand("Load", "string resourceType",
			delegate(CommandParam parameters) {
				/*string loadType = parameters.GetString(0).ToLower();
				if (loadType == "tilesets")
					loadingMode = LoadingModes.Tilesets;
				else if (loadType == "animations")
					loadingMode = LoadingModes.Animations;
				else if (loadType == "sprites")
					loadingMode = LoadingModes.Sprites;
				else
					ThrowParseError("Invalid Load type", true);*/
			});
			//=====================================================================================
			// TILE/TILESET BEGIN/END 
			//=====================================================================================
			AddCommand("Tileset", "string name, string sheetName, (int width, int height)",
			delegate(CommandParam parameters) {
				SpriteSheet sheet = Resources.GetSpriteSheet(parameters.GetString(1));
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
				useTemporary = false;
				if (tileset != null) {
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
			AddCommand("TempTile", "string name",
			delegate(CommandParam parameters) {
				useTemporary = true;
				tileData = new TileData();
				tileData.Name = parameters.GetString(0);
				baseTileData = tileData;
			});
			//=====================================================================================
			AddCommand("EventTile", "string name",
			delegate(CommandParam parameters) {
				useTemporary = false;
				eventTileData = new EventTileData();
				eventTileData.Name = parameters.GetString(0);
				baseTileData = eventTileData;
			});
			//=====================================================================================
			AddCommand("Monster", "string name, string sprite, string monsterType, string monsterColor",
			delegate(CommandParam parameters) {
				useTemporary = false;
				eventTileData = new EventTileData();
				eventTileData.Clone(Resources.GetResource<EventTileData>("monster"));
				eventTileData.Name = parameters.GetString(0);
				baseTileData = eventTileData;
								
				if (parameters.ChildCount > 1)
					eventTileData.Sprite = resources.GetSpriteAnimation(parameters.GetString(1));
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
						if (useTemporary)
							resources.AddResource<TileData>(tileData.Name, tileData);
						else
							Resources.AddResource<TileData>(tileData.Name, tileData);
					}
					tileData = null;
					baseTileData = null;
				}
				else if (eventTileData != null) {
					if (eventTileData.Tileset == null) {
						if (useTemporary)
							resources.AddResource<EventTileData>(eventTileData.Name, eventTileData);
						else
							Resources.AddResource<EventTileData>(eventTileData.Name, eventTileData);
					}
					eventTileData = null;
					baseTileData = null;
				}
				else if (tileset != null) {
					Resources.AddResource<Tileset>(tileset.ID, tileset);
					tileset = null;
				}
				else if (eventTileset != null) {
					Resources.AddResource<EventTileset>(eventTileset.ID, eventTileset);
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
						resources.GetResource<TileData>(parameters.GetString(1));
				}
				else if (eventTileset != null) {
					eventTileset.TileData[location.X, location.Y] = 
						resources.GetResource<EventTileData>(parameters.GetString(1));
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
						ThrowParseError("Invalid tile flag: \"" + parameters.GetString(i) + "\"!");
				}
			});
			//=====================================================================================
			AddCommand("EnvType", "string envType",
			delegate(CommandParam parameters) {
				TileEnvironmentType envType = TileEnvironmentType.Normal;
				if (Enum.TryParse<TileEnvironmentType>(parameters.GetString(0), true, out envType))
					tileData.Properties.Set("environment_type", (int) envType);
				else
					ThrowParseError("Invalid tile environment type: \"" + parameters.GetString(0) + "\"!");
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
					ThrowParseError("Unknown value for conveyor angle: " + str);

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
				"string name, string readableName, string description",
				"string name, string readableName, string description, (string params...)", // Params = (type1, name1, type2, name2...)
			delegate(CommandParam parameters) {
				Property property = Property.CreateString(parameters.GetString(0), "");
				//property.SetDocumentation(parameters.GetString(1), "script", "", "Events", parameters.GetString(2), true, false);
				baseTileData.Properties.Set(property.Name, property)
					.SetDocumentation(parameters.GetString(1), "script", "", "Events", parameters.GetString(2), true, false);
				
				// Create the event's script parameter list.
				ScriptParameter[] scriptParams;
				if (parameters.ChildCount > 3) {
					CommandParam paramList = parameters[3];
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
				baseTileData.Events.AddEvent(new ObjectEvent(
						parameters.GetString(0), // Name
						parameters.GetString(1), // Readable name
						parameters.GetString(2), // Description
						scriptParams));
			});
			//=====================================================================================
			AddCommand("Sprite",
				"string spriteOrAnimationName",
				"string spriteSheetName, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
			delegate(CommandParam parameters) {
				if (parameters.ChildCount >= 2) {
					spriteBuilder.Begin(new Sprite(
						resources.GetResource<SpriteSheet>(parameters.GetString(0)),
						parameters.GetPoint(1),
						parameters.GetPoint(2, Point2I.Zero)
					));
					baseTileData.Sprite = spriteBuilder.End();
				}
				else {
					baseTileData.Sprite = resources.GetSpriteAnimation(parameters.GetString(0));
				}
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
					SpriteAnimation[] spriteList = new SpriteAnimation[index + 1];
					for (int i = 0; i < spriteList.Length; i++) {
						if (i < tileData.SpriteList.Length)
							spriteList[i] = tileData.SpriteList[i];
						else
							spriteList[i] = null;
					}
					tileData.SpriteList = spriteList;
				}
				if (parameters.ChildCount > 2 && parameters[2].Type == CommandParamType.Array) {
					spriteBuilder.Begin(new Sprite(
						resources.GetResource<SpriteSheet>(parameters.GetString(1)),
						parameters.GetPoint(2),
						parameters.GetPoint(3, Point2I.Zero)
					));
					tileData.SpriteList[index] = spriteBuilder.End();
				}
				else {
					if (parameters.ChildCount == 3) {
						string typeName = parameters.GetString(1);
						if (typeName == "sprite")
							tileData.SpriteList[index] = resources.GetResource<Sprite>(parameters.GetString(2));
						else if (typeName == "animation")
							tileData.SpriteList[index] = resources.GetResource<Animation>(parameters.GetString(2));
						else
							ThrowParseError("Unknown sprite/animation type '" + typeName + "' (expected \"sprite\" or \"animation\")");
					}
					else {
						tileData.SpriteList[index] = resources.GetSpriteAnimation(parameters.GetString(1));
					}
				}
			});
			//=====================================================================================
			AddCommand("SpriteList", "string spriteAnimationNames...", delegate(CommandParam parameters) {
				SpriteAnimation[] spriteList = new SpriteAnimation[parameters.ChildCount];
				for (int i = 0; i < parameters.ChildCount; i++)
					spriteList[i] = resources.GetSpriteAnimation(parameters.GetString(i));

				tileData.SpriteList = spriteList;
			});
			//=====================================================================================
			AddCommand("SpriteObj",
				"string spriteAnimationName",
				"string spriteSheetName, (int sourceX, int sourceY), (int offsetX, int offsetY) = (0, 0)",
			delegate(CommandParam parameters) {
				if (parameters.ChildCount >= 2) {
					spriteBuilder.Begin(new Sprite(
						resources.GetResource<SpriteSheet>(parameters.GetString(0)),
						parameters.GetPoint(1),
						parameters.GetPoint(2, Point2I.Zero)
					));
					tileData.SpriteAsObject = spriteBuilder.End();
				}
				else {
					tileData.SpriteAsObject = resources.GetSpriteAnimation(parameters.GetString(0));
				}
			});
			//=====================================================================================
			AddCommand("BreakAnim", "string animationName",
			delegate(CommandParam parameters) {
				tileData.BreakAnimation = resources.GetResource<Animation>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("BreakSound", "string soundName",
			delegate(CommandParam parameters) {
				tileData.BreakSound = Resources.GetResource<Sound>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("Model", "string collisionModelName",
			delegate(CommandParam parameters) {
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("Solid", "string collisionModelName",
			delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.Solid;
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("HalfSolid", "string collisionModelName",
			delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.HalfSolid;
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("Ledge", "string collisionModelName, string ledgeDirection",
			delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.Ledge;
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
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
					tileData.Clone(resources.GetResource<TileData>(parameters.GetString(0)));
				else if (eventTileData != null)
					eventTileData.Clone(resources.GetResource<EventTileData>(parameters.GetString(0)));
			});

			// SPRITE SHEET ---------------------------------------------------------------

			AddCommand("SpriteSheet",
				"string path, (int cellWidth, int cellHeight), (int spacingX, int spacingY), (int offsetX, int offsetY)",
				"string name, string path, (int cellWidth, int cellHeight), (int spacingX, int spacingY), (int offsetX, int offsetY)",
			delegate(CommandParam parameters) {
			//AddSpriteCommand("SpriteSheet", delegate(CommandParam parameters) {
				if (parameters.ChildCount == 1) {
					// Start using the given sprite sheet.
					SpriteSheet sheet = Resources.GetResource<SpriteSheet>(parameters.GetString(0));
					spriteBuilder.SpriteSheet = sheet;
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

					if (Resources.ImageExists(imagePath))
						image = Resources.GetResource<Image>(imagePath);
					else
						image = Resources.LoadImage(Resources.ImageDirectory + imagePath);

					if (sheetName.IndexOf('/') >= 0)
						sheetName = sheetName.Substring(sheetName.LastIndexOf('/') + 1);

					SpriteSheet sheet = new SpriteSheet(image,
							parameters.GetPoint(i + 0),
							parameters.GetPoint(i + 2),
							parameters.GetPoint(i + 1));
					if (useTemporary)
						resources.AddResource<SpriteSheet>(sheetName, sheet);
					else
						Resources.AddResource<SpriteSheet>(sheetName, sheet);
					spriteBuilder.SpriteSheet = sheet;
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
				
				property.Documentation = new PropertyDocumentation() {
					ReadableName	= param.GetString(3),
					EditorType		= editorType,
					EditorSubType	= editorSubType,
					Category		= param.GetString(5),
					Description		= param.GetString(6),
					IsEditable		= true,
					IsHidden		= param.GetBool(7, false),
				};
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

		// Begins reading the script.
		protected override void BeginReading() {
			//loadingMode	= LoadingModes.Tilesets;
			tileset		= null;
			tileData	= null;
			spriteBuilder.SpriteSheet = null;
		}

		// Ends reading the script.
		protected override void EndReading() {

		}
	}
}
