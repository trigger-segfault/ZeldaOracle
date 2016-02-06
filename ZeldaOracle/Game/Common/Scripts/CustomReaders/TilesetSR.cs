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
using ZeldaOracle.Game;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.EventTiles;

namespace ZeldaOracle.Common.Scripts {

	enum LoadingModes {
		Tilesets,
		Animations,
		Sprites
	}

	public class TilesetSR : NewScriptReader {

		private Tileset			tileset;
		private EventTileset	eventTileset;
		private BaseTileData	baseTileData;
		private TileData		tileData;
		private EventTileData	eventTileData;
		//private string			tileDataName;
		//private AnimationSR		animationSR;
		//private SpritesSR		spritesSR;
		private LoadingModes	loadingMode;
		private TemporaryResources resources;

		private SpriteBuilder spriteBuilder;
		private Sprite sprite;
		private string spriteName;

		private AnimationBuilder animationBuilder;
		private Animation animation;
		private string animationName;

		private bool useTemporary;

		private List<ScriptCommand> tilesetCommands;
		private List<ScriptCommand> animationCommands;
		private List<ScriptCommand> spriteCommands;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public TilesetSR() {

			this.tilesetCommands	= new List<ScriptCommand>();
			this.animationCommands	= new List<ScriptCommand>();
			this.spriteCommands		= new List<ScriptCommand>();

			this.loadingMode	= LoadingModes.Tilesets;
			this.resources		= new TemporaryResources();
			this.useTemporary	= false;

			this.animationBuilder	= new AnimationBuilder();
			this.spriteBuilder	= new SpriteBuilder();

			// Tileset <name> <sheet-name> <size(width, height)>
			AddCommand("Load", delegate(CommandParam parameters) {
				string loadType = parameters.GetString(0).ToLower();
				if (loadType == "tilesets")
					loadingMode = LoadingModes.Tilesets;
				else if (loadType == "animations")
					loadingMode = LoadingModes.Animations;
				else if (loadType == "sprites")
					loadingMode = LoadingModes.Sprites;
				else
					ThrowParseError("Invalid Load type", true);
			});

			// BEGIN/END.
			
			// Tileset <name>, <sheet-name>, (<width>, <height>)
			AddCommand("Tileset",
				"string name, string sheetName, (int width, int height)",
			delegate(CommandParam parameters) {
				SpriteSheet sheet = Resources.GetSpriteSheet(parameters.GetString(1));
				tileset = new Tileset(parameters.GetString(0), sheet,
									  parameters.GetPoint(2));
			});
			// EventTileset <name>, (<width>, <height>)
			AddCommand("EventTileset",
				"string name, (int width, int height)",
			delegate(CommandParam parameters) {
				eventTileset = new EventTileset(parameters.GetString(0),
					null, parameters.GetPoint(1));
			});
			// Tile <name>
			// Tile (<sheetX>, <sheetY>)
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
			// TempTile <name>
			AddCommand("TempTile", "string name", delegate(CommandParam parameters) {
				useTemporary = true;
				tileData = new TileData();
				tileData.Name = parameters.GetString(0);
				baseTileData = tileData;
			});
			// EventTile <name>
			AddCommand("EventTile", "string name", delegate(CommandParam parameters) {
				useTemporary = false;
				eventTileData = new EventTileData();
				eventTileData.Name = parameters.GetString(0);
				baseTileData = eventTileData;
			});
			// Monster <name> <sprite> <monster-type> <monster-color>
			AddCommand("Monster",
				"string name, string sprite, string monsterType, string monsterColor",
			delegate(CommandParam parameters) {
				useTemporary = false;
				eventTileData = new EventTileData();
				eventTileData.Clone(Resources.GetResource<EventTileData>("monster"));
				eventTileData.Name = parameters.GetString(0);
				baseTileData = eventTileData;

				
				if (parameters.ChildCount > 1) {
					eventTileData.Sprite = resources.GetSpriteAnimation(parameters.GetString(1));
				}
				if (parameters.ChildCount > 2) {
					eventTileData.Properties.Set("monster_type", parameters.GetString(2));
				}
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
			// SetTile (<sheetX>, <sheetY>), <name>
			AddCommand("SetTile",
				"(int sheetX, int sheetY), string tileName",
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
			// End
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

			
			// TILESETS.

			// Default <default-tile-location(x, y)>
			AddCommand("Default",
				"(int defaultSheetX, int defaultSheetY)",
			delegate(CommandParam parameters) {
				tileset.DefaultTile = parameters.GetPoint(0);
			});

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


			// TILES.
			
			// Type <type>
			AddCommand("Type", "string type", delegate(CommandParam parameters) {
				baseTileData.Type = Tile.GetType(parameters.GetString(0), true);
			});
			// Flags <flags[]...>
			AddTilesetCommand("Flags", delegate(CommandParam parameters) {
				for (int i = 0; i < parameters.ChildCount; i++) {
					TileFlags flags = TileFlags.None;
					if (Enum.TryParse<TileFlags>(parameters.GetString(i), true, out flags))
						tileData.Flags |= flags;
					else
						ThrowParseError("Invalid tile flag: \"" + parameters.GetString(i) + "\"!");
				}
			});
			// EnvType <type>
			AddCommand("EnvType", "string envType", delegate(CommandParam parameters) {
				TileEnvironmentType envType = TileEnvironmentType.Normal;
				if (Enum.TryParse<TileEnvironmentType>(parameters.GetString(0), true, out envType))
					tileData.Properties.Set("environment_type", (int) envType);
				else
					ThrowParseError("Invalid tile environment type: \"" + parameters.GetString(0) + "\"!");
			});
			// CONVEYOR <angle>, <speed>
			AddCommand("Conveyor", "string angle, float speed", delegate(CommandParam parameters) {
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
			// Properties <(name, value>
			// Properties <(type, name, value, readable-name, editor-type, category, description)...>
			// Properties <(type, name, value, readable-name, (editor-type, editor-sub-type), category, description)...>
			// Properties <(hide, name)...>
			// Properties <(show, name)...>

			/*AddCommand("Properties", "(string type, string name, any value, string readableName='', string editorType='', string category='', string description='')...",
			delegate(CommandParam parameters) {

			});*/

			AddTilesetCommand("Properties", delegate(CommandParam parameters) {
				// TODO: handle lists.
				for (int i = 0; i < parameters.ChildCount; i++) {
					CommandParam param = parameters[i];
					string name = param.GetString(1);

					if (String.Compare(param.GetString(0), "hide", StringComparison.OrdinalIgnoreCase) == 0) {
						baseTileData.Properties.GetProperty(name, false).Documentation.IsHidden = true;
					}
					else if (String.Compare(param.GetString(0), "show", StringComparison.OrdinalIgnoreCase) == 0) {
						baseTileData.Properties.GetProperty(name, false).Documentation.IsHidden = false;
					}
					else {
						// Parse the property type.
						PropertyType type;
						if (!Enum.TryParse<PropertyType>(param.GetString(0), true, out type))
							ThrowParseError("Unknown property type " + name);

						// Parse the property value.
						object value = null;
						if (type == PropertyType.String)
							value = param.GetString(2);
						else if (type == PropertyType.Integer)
							value = param.GetInt(2);
						else if (type == PropertyType.Float)
							value = param.GetFloat(2);
						else if (type == PropertyType.Boolean)
							value = param.GetString(2).Equals("true", StringComparison.OrdinalIgnoreCase);
						else
							ThrowParseError("Unsupported property type for " + name);
						
						// Set the property.
						Property property = null;
						if (value != null)
							property = baseTileData.Properties.SetGeneric(name, value);

						// Set the property's documentation.
						if (param.ChildCount > 3) {
							string editorType = "";
							string editorSubType = "";
							if (param[4].Type == CommandParamType.Array) {
								editorType = param[4].GetString(0);
								editorSubType = param[4].GetString(1);
							}
							else {
								editorType = param.GetString(4);
							}
							
							PropertyDocumentation documentation = new PropertyDocumentation() {
								ReadableName	= param.GetString(3),
								EditorType		= editorType,
								EditorSubType	= editorSubType,
								Category		= param.GetString(5),
								Description		= param.GetString(6),
								IsEditable		= true,
								IsHidden		= param.GetBool(7, false),
							};

							if (property != null)
								property.Documentation = documentation;
						}
					}
				}
			});
			// Event <name>, <readable-name>, <description>, (<param-type-1>, <param-name-1>, <param-type-2>, <param-name-2>...)
			AddTilesetCommand("Event", delegate(CommandParam parameters) {
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
			// Sprite <sprite-animation>
			// Sprite <spritesheet> <x-index> <y-index> <x-offset> <y-offset>
			AddCommand("Sprite",
				"string spriteOrAnimName",
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

			// Size (<width>, <height>)
			AddCommand("Size", "(int width, int height)", delegate(CommandParam parameters) {
				tileData.Size = parameters.GetPoint(0);
			});

			// SpriteIndex <index> <sprite-or-anim> <sprite-animation>
			// SpriteIndex <index> <sprite-animation> 
			// SpriteIndex <index> <spritesheet> (<x-index> <y-index>) (<x-offset> <y-offset>)
			AddCommand("SpriteIndex",
				"int index, string spriteOrAnim, string spriteAnimation",
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

			// SpriteList [sprite-animation-1] [sprite-animation-2]...
			AddTilesetCommand("SpriteList", delegate(CommandParam parameters) {
				SpriteAnimation[] spriteList = new SpriteAnimation[parameters.ChildCount];
				for (int i = 0; i < parameters.ChildCount; i++)
					spriteList[i] = resources.GetSpriteAnimation(parameters.GetString(i));

				tileData.SpriteList = spriteList;
			});

			// SpriteObj <sprite-animation>
			// SpriteObj <spritesheet> <x-index> <y-index> <x-offset> <y-offset>
			AddCommand("SpriteObj",
				"string spriteOrAnimName",
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

			// BreakAnim <animation>
			AddCommand("BreakAnim", "string animationName", delegate(CommandParam parameters) {
				tileData.BreakAnimation = resources.GetResource<Animation>(parameters.GetString(0));
			});

			// BreakSound <sound>
			AddCommand("BreakSound", "string soundName", delegate(CommandParam parameters) {
				tileData.BreakSound = Resources.GetResource<Sound>(parameters.GetString(0));
			});
			// Model <collision-model>
			AddCommand("Model", "string collisionModelName", delegate(CommandParam parameters) {
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			// Solid <collision-model>
			AddCommand("Solid", "string collisionModelName", delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.Solid;
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			// HalfSolid <collision-model>
			AddCommand("HalfSolid", "string collisionModelName", delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.HalfSolid;
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			// Ledge <collision-model> <direction>
			AddCommand("Ledge", "string collisionModelName, string ledgeDirection", delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.Ledge;
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
				string dirName = parameters.GetString(1);
				int direction;
				if (Directions.TryParse(dirName, true, out direction))
					tileData.LedgeDirection = direction;
				else
					ThrowParseError("Unknown value for ledge direction: " + dirName);
			});
			// Hurt <damage> <(area-x, area-y, area-width, area-height)>
			AddCommand("Hurt",
				"int damage, (int areaX, int areaY, int areaWidth, int areaHeight)",
			delegate(CommandParam parameters) {
				tileData.HurtDamage = parameters.GetInt(0);
				tileData.HurtArea = new Rectangle2I(
					parameters[1].GetInt(0),
					parameters[1].GetInt(1),
					parameters[1].GetInt(2),
					parameters[1].GetInt(3));
			});
			// Clone <tiledata>
			AddCommand("Clone", "string tileDataName", delegate(CommandParam parameters) {
				if (tileData != null)
					tileData.Clone(resources.GetResource<TileData>(parameters.GetString(0)));
				else if (eventTileData != null)
					eventTileData.Clone(resources.GetResource<EventTileData>(parameters.GetString(0)));
			});

			// SPRITE SHEET.

			AddAnimationCommand("SpriteSheet", delegate(CommandParam parameters) {
				SpriteSheet sheet = Resources.GetResource<SpriteSheet>(parameters.GetString(0));
				animationBuilder.SpriteSheet = sheet;
			});


			// BEGIN/END.

			AddAnimationCommand("Anim", delegate(CommandParam parameters) {
				animationName = parameters.GetString(0);
				animationBuilder.BeginNull();
				animation = null;
				useTemporary = false;
			});
			AddAnimationCommand("TempAnim", delegate(CommandParam parameters) {
				animationName = parameters.GetString(0);
				animationBuilder.BeginNull();
				animation = null;
				useTemporary = true;
			});
			AddAnimationCommand("End", delegate(CommandParam parameters) {
				if (animation != null) {
					animationBuilder.End();
					if (useTemporary)
						resources.AddResource<Animation>(animationName, animation);
					else
						Resources.AddResource<Animation>(animationName, animation);
				}
			});
			AddAnimationCommand("SubStrip", delegate(CommandParam parameters) {
				LoopMode loopMode = LoopMode.Repeat;
				if (parameters.GetString(0) == "reset")
					loopMode = LoopMode.Reset;
				else if (parameters.GetString(0) == "repeat" || parameters.GetString(0) == "loop")
					loopMode = LoopMode.Repeat;
				else if (parameters.GetString(0) == "clamp")
					loopMode = LoopMode.Clamp;
				else
					ThrowParseError("Unknown loop mode '" + parameters.GetString(0) + "' for animation");

				animationBuilder.CreateSubStrip();
				animationBuilder.SetLoopMode(loopMode);
				if (animation == null)
					animation = animationBuilder.Animation;
			});
			AddAnimationCommand("Clone", delegate(CommandParam parameters) {
				if (resources.ExistsResource<Animation>(parameters.GetString(0))) {
					animationBuilder.CreateClone(resources.GetResource<Animation>(parameters.GetString(0)));
					animation = animationBuilder.Animation;
				}
				else {
					// ERROR: can't clone nonexistant animation.
				}
			});


			// FRAME BUILDING.

			AddAnimationCommand("Add", delegate(CommandParam parameters) {
				if (parameters.GetString(0) == "strip") {
					animationBuilder.AddFrameStrip(
						parameters.GetInt(1),
						parameters.GetPoint(3).X,
						parameters.GetPoint(3).Y,
						parameters.GetInt(2),
						parameters.GetPoint(4, Point2I.Zero).X,
						parameters.GetPoint(4, Point2I.Zero).Y,
						parameters.GetPoint(5, new Point2I(1, 0)).X,
						parameters.GetPoint(5, new Point2I(1, 0)).Y);
				}
				else if (parameters.GetString(0) == "frame") {
					animationBuilder.AddFrame(
						parameters.GetInt(1),
						parameters.GetPoint(2).X,
						parameters.GetPoint(2).Y,
						parameters.GetPoint(3, Point2I.Zero).X,
						parameters.GetPoint(3, Point2I.Zero).Y);
				}
				else if (parameters.GetString(0) == "part") {
					animationBuilder.AddPart(
						parameters.GetInt(1),
						parameters.GetPoint(2).X,
						parameters.GetPoint(2).Y,
						parameters.GetPoint(3, Point2I.Zero).X,
						parameters.GetPoint(3, Point2I.Zero).Y);
				}
				else
					ThrowParseError("Unknown add type '" + parameters.GetString(0) + "' for animation");
			});
			AddAnimationCommand("Insert", delegate(CommandParam parameters) {
				if (parameters.GetString(0) == "strip") {
					animationBuilder.InsertFrameStrip(
						parameters.GetInt(1),
						parameters.GetInt(2),
						parameters.GetPoint(4).X,
						parameters.GetPoint(4).Y,
						parameters.GetInt(3),
						parameters.GetPoint(5, Point2I.Zero).X,
						parameters.GetPoint(5, Point2I.Zero).Y,
						parameters.GetPoint(6, new Point2I(1, 0)).X,
						parameters.GetPoint(6, new Point2I(1, 0)).Y);
				}
				else if (parameters.GetString(0) == "frame") {
					animationBuilder.InsertFrame(
						parameters.GetInt(1),
						parameters.GetInt(2),
						parameters.GetPoint(3).X,
						parameters.GetPoint(3).Y,
						parameters.GetPoint(4, Point2I.Zero).X,
						parameters.GetPoint(4, Point2I.Zero).Y);
				}
				else
					ThrowParseError("Unknown insert type '" + parameters.GetString(0) + "' for animation");
			});


			// MODIFICATIONS.

			AddAnimationCommand("MakeQuad", delegate(CommandParam parameters) {
				animationBuilder.MakeQuad();
			});
			AddAnimationCommand("MakeDynamic", delegate(CommandParam parameters) {
				animationBuilder.MakeDynamic(
					parameters.GetInt(0),
					parameters.GetPoint(1).X,
					parameters.GetPoint(1).Y);
			});
			AddAnimationCommand("Offset", delegate(CommandParam parameters) {
				animationBuilder.Offset(
					parameters.GetPoint(0).X,
					parameters.GetPoint(0).Y);
			});
			AddAnimationCommand("Flicker", delegate(CommandParam parameters) {
				// FLICKER <alternateDelay> <on/off>

				bool startOn = true;
				if (parameters.GetString(1) == "on")
					startOn = true;
				else if (parameters.GetString(1) == "off")
					startOn = false;
				else
					ThrowParseError("Must be either on or off for flicker start state");

				animationBuilder.MakeFlicker(parameters.GetInt(0), startOn);
			});


			// SPRITE SHEET.
			
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

			// BEGIN/END.

			// Sprite <name> <grid-location> <draw-offset = (0, 0)>
			AddSpriteCommand("Sprite", delegate(CommandParam parameters) {
				spriteName = parameters.GetString(0);
				sprite = new Sprite(
					spriteBuilder.SpriteSheet,
					parameters.GetPoint(1),
					parameters.GetPoint(2, Point2I.Zero));
				spriteBuilder.Begin(sprite);
				useTemporary = false;
			});
			// TempSprite <name> <grid-location> <draw-offset = (0, 0)>
			AddSpriteCommand("TempSprite", delegate(CommandParam parameters) {
				spriteName = parameters.GetString(0);
				sprite = new Sprite(
					spriteBuilder.SpriteSheet,
					parameters.GetPoint(1),
					parameters.GetPoint(2, Point2I.Zero));
				spriteBuilder.Begin(sprite);
				useTemporary = true;
			});
			AddSpriteCommand("End", delegate(CommandParam parameters) {
				if (sprite != null) {
					spriteBuilder.End();
					if (useTemporary)
						resources.AddResource<Sprite>(spriteName, sprite);
					else
						Resources.AddResource<Sprite>(spriteName, sprite);
					sprite = null;
				}
			});

			// BUILDING.

			// Add <grid-location> <draw-offset = (0, 0)>
			AddSpriteCommand("Add", delegate(CommandParam parameters) {
				spriteBuilder.AddPart(
					parameters.GetPoint(0).X,
					parameters.GetPoint(0).Y,
					parameters.GetPoint(1, Point2I.Zero).X,
					parameters.GetPoint(1, Point2I.Zero).Y);
			});
			// Size <size>
			AddSpriteCommand("Size", delegate(CommandParam parameters) {
				spriteBuilder.SetSize(
					parameters.GetPoint(0).X,
					parameters.GetPoint(0).Y);
			});

			// Add each command type as a command.
			/*
			for (int i = 0; i < tilesetCommands.Count; i++) {
				string command = tilesetCommands[i].Name;
				AddCommand(tilesetCommands[i].Name, delegate(CommandParam parameters) {
					ReadSpecialCommand(command, parameters);
				});
			}
			for (int i = 0; i < animationCommands.Count; i++) {
				string command = animationCommands[i].Name;
				AddCommand(animationCommands[i].Name, delegate(CommandParam parameters) {
					ReadSpecialCommand(command, parameters);
				});
			}
			for (int i = 0; i < spriteCommands.Count; i++) {
				string command = spriteCommands[i].Name;
				AddCommand(spriteCommands[i].Name, delegate(CommandParam parameters) {
					ReadSpecialCommand(command, parameters);
				});
			}*/
		}

		// Begins reading the script.
		protected override void BeginReading() {
			tileset  = null;
			tileData = null;

			animation = null;
			animationName = "";
			animationBuilder.SpriteSheet = null;

			sprite = null;
			spriteName = "";
			spriteBuilder.SpriteSheet = null;
		}

		// Ends reading the script.
		protected override void EndReading() {
			animation = null;

			sprite = null;
		}

		private void AddTilesetCommand(string name, Action<CommandParam> action) {
			AddCommand(name, action);
			//ScriptCommand command = new ScriptCommand(name, action);
			//tilesetCommands.Add(command);
		}

		private void AddAnimationCommand(string name, Action<CommandParam> action) {
			ScriptCommand command = new ScriptCommand(name, action);
			animationCommands.Add(command);
		}

		private void AddSpriteCommand(string name, Action<CommandParam> action) {
			ScriptCommand command = new ScriptCommand(name, action);
			spriteCommands.Add(command);
		}

		private void ReadSpecialCommand(string commandName, CommandParam parameters) {
			bool exists = false;
			switch (loadingMode) {
			case LoadingModes.Tilesets:
				for (int i = 0; i < tilesetCommands.Count; i++) {
					if (String.Compare(tilesetCommands[i].Name, commandName,
						StringComparison.CurrentCultureIgnoreCase) == 0) {
							tilesetCommands[i].Action(parameters);
						exists = true;
						break;
					}
				}
				if (!exists)
					ThrowParseError(commandName + " is not a valid command while loading tilesets", false);
				break;
			case LoadingModes.Animations:
				for (int i = 0; i < animationCommands.Count; i++) {
					if (String.Compare(animationCommands[i].Name, commandName,
						StringComparison.CurrentCultureIgnoreCase) == 0) {
							animationCommands[i].Action(parameters);
						exists = true;
						break;
					}
				}
				if (!exists)
					ThrowParseError(commandName + " is not a valid command while loading animations", false);
				break;
			case LoadingModes.Sprites:
				for (int i = 0; i < spriteCommands.Count; i++) {
					if (String.Compare(spriteCommands[i].Name, commandName,
						StringComparison.CurrentCultureIgnoreCase) == 0) {
							spriteCommands[i].Action(parameters);
						exists = true;
						break;
					}
				}
				if (!exists)
					ThrowParseError(commandName + " is not a valid command while loading sprites", false);
				break;
			}
		}

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
} // end namespace
