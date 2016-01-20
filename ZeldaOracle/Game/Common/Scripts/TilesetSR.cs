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
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Common.Scripts {

	enum LoadingModes {
		Tilesets,
		Animations,
		Sprites
	}

	public class TilesetSR : NewScriptReader {

		private Tileset			tileset;
		private TileData		tileData;
		private string			tileDataName;
		private AnimationSR		animationSR;
		private SpritesSR		spritesSR;
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
			
			// Tileset <name> <sheet-name> <size(width, height)>
			AddTilesetCommand("Tileset", delegate(CommandParam parameters) {
				SpriteSheet sheet = Resources.GetSpriteSheet(parameters.GetString(1));
				tileset = new Tileset(parameters.GetString(0),
					sheet, parameters.GetPoint(2));
			});
			// With a tileset:
			//   Tile <sheet-location>
			// Without a tileset:
			//   Tile <name>
			AddTilesetCommand("Tile", delegate(CommandParam parameters) {
				useTemporary = false;
				if (tileset != null) {
					Point2I location = parameters.GetPoint(0);
					tileData = tileset.TileData[location.X, location.Y];
				}
				else {
					tileData = new TileData();
					tileDataName = parameters.GetString(0);
				}
			});
			// TempTile <name>
			AddTilesetCommand("TempTile", delegate(CommandParam parameters) {
				useTemporary = true;
				tileData = new TileData();
				tileDataName = parameters.GetString(0);
			});
			AddTilesetCommand("End", delegate(CommandParam parameters) {
				if (tileData != null) {
					if (tileData.Tileset == null) {
						if (useTemporary)
							resources.AddResource<TileData>(tileDataName, tileData);
						else
							Resources.AddResource<TileData>(tileDataName, tileData);
					}
					tileData = null;
				}
				else if (tileset != null) {
					Resources.AddResource<Tileset>(tileset.ID, tileset);
					tileset = null;
				}
			});

			
			// TILESETS.

			// Default <default-tile-location(x, y)>
			AddTilesetCommand("Default", delegate(CommandParam parameters) {
				tileset.DefaultTile = parameters.GetPoint(0);
			});
			// Config: data to configure tiles with a single character per tile.
			AddTilesetCommand("Config", delegate(CommandParam parameters) {
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
			AddTilesetCommand("Type", delegate(CommandParam parameters) {
				tileData.Type = Tile.GetType(parameters.GetString(0), true);
			});
			// Flags <flags[]...>
			AddTilesetCommand("Flags", delegate(CommandParam parameters) {
				for (int i = 0; i < parameters.Count; i++) {
					TileFlags flags = TileFlags.None;
					if (Enum.TryParse<TileFlags>(parameters.GetString(i), true, out flags))
						tileData.Flags |= flags;
					else
						throw new ArgumentException("Invalid tile flag: \"" + parameters.GetString(i) + "\"!");
				}
			});
			// EnvType <type>
			AddTilesetCommand("EnvType", delegate(CommandParam parameters) {
				TileEnvironmentType envType = TileEnvironmentType.Normal;
				if (Enum.TryParse<TileEnvironmentType>(parameters.GetString(0), true, out envType))
					tileData.Properties.Set("environment_type", (int) envType);
				else
					throw new ArgumentException("Invalid tile environment type: \"" + parameters.GetString(0) + "\"!");
			});
			// Properties <(name, value>
			// Properties <(type, name, value, readable-name, editor-type, category, description)...>
			// Properties <(type, name, value, readable-name, (editor-type, editor-sub-type), category, description)...>
			// Properties <(hide, name)...>
			// Properties <(show, name)...>
			AddTilesetCommand("Properties", delegate(CommandParam parameters) {
				// TODO: handle lists.
				for (int i = 0; i < parameters.Count; i++) {
					CommandParam param = parameters[i];
					string name = param.GetString(1);

					if (String.Compare(param.GetString(0), "hide", StringComparison.OrdinalIgnoreCase) == 0) {
						tileData.Properties.GetProperty(name, false).Documentation.IsHidden = true;
					}
					else if (String.Compare(param.GetString(0), "show", StringComparison.OrdinalIgnoreCase) == 0) {
						tileData.Properties.GetProperty(name, false).Documentation.IsHidden = false;
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
							property = tileData.Properties.SetGeneric(name, value);

						// Set the property's documentation.
						if (param.Count > 3) {
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
				tileData.Properties.Set(property.Name, property)
					.SetDocumentation(parameters.GetString(1), "script", "", "Events", parameters.GetString(2), true, false);
				
				// Create the event's script parameter list.
				ScriptParameter[] scriptParams;
				if (parameters.Count > 3) {
					CommandParam paramList = parameters[3];
					scriptParams = new ScriptParameter[paramList.Count / 2];
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
				tileData.Events.AddEvent(new ObjectEvent(
						parameters.GetString(0), // Name
						parameters.GetString(1), // Readable name
						parameters.GetString(2), // Description
						scriptParams));
			});
			// Sprite <sprite-animation>
			// Sprite <spritesheet> <x-index> <y-index> <x-offset> <y-offset>
			AddTilesetCommand("Sprite", delegate(CommandParam parameters) {
				if (parameters.Count >= 2) {
					spriteBuilder.Begin(new Sprite(
						resources.GetResource<SpriteSheet>(parameters.GetString(0)),
						parameters.GetPoint(1),
						parameters.GetPoint(2, Point2I.Zero)
					));
					tileData.Sprite = spriteBuilder.End();
				}
				else {
					tileData.Sprite = resources.GetSpriteAnimation(parameters.GetString(0));
				}
			});
			// SpriteIndex <index> <sprite-animation>
			// SpriteIndex <index> <spritesheet> <x-index> <y-index> <x-offset> <y-offset>
			AddTilesetCommand("SpriteIndex", delegate(CommandParam parameters) {
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
				if (parameters.Count >= 3) {
					spriteBuilder.Begin(new Sprite(
						resources.GetResource<SpriteSheet>(parameters.GetString(1)),
						parameters.GetPoint(2),
						parameters.GetPoint(3, Point2I.Zero)
					));
					tileData.SpriteList[index] = spriteBuilder.End();
				}
				else {
					tileData.SpriteList[index] = resources.GetSpriteAnimation(parameters.GetString(1));
				}
			});
			// SpriteList [sprite-animation-1] [sprite-animation-2]...
			AddTilesetCommand("SpriteList", delegate(CommandParam parameters) {
				SpriteAnimation[] spriteList = new SpriteAnimation[parameters.Count];
				for (int i = 0; i < parameters.Count; i++)
					spriteList[i] = resources.GetSpriteAnimation(parameters.GetString(i));

				tileData.SpriteList = spriteList;
			});
			// SpriteObj <sprite-animation>
			// SpriteObj <spritesheet> <x-index> <y-index> <x-offset> <y-offset>
			AddTilesetCommand("SpriteObj", delegate(CommandParam parameters) {
				if (parameters.Count >= 2) {
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
			AddTilesetCommand("BreakAnim", delegate(CommandParam parameters) {
				tileData.BreakAnimation = resources.GetResource<Animation>(parameters.GetString(0));
			});
			// BreakSound <sound>
			AddTilesetCommand("BreakSound", delegate(CommandParam parameters) {
				tileData.BreakSound = Resources.GetResource<Sound>(parameters.GetString(0));
			});
			// Model <collision-model>
			AddTilesetCommand("Model", delegate(CommandParam parameters) {
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			// Solid <collision-model>
			AddTilesetCommand("Solid", delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.Solid;
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			// HalfSolid <collision-model>
			AddTilesetCommand("HalfSolid", delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.HalfSolid;
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
			});
			// Ledge <collision-model> <direction>
			AddTilesetCommand("Ledge", delegate(CommandParam parameters) {
				tileData.SolidType = TileSolidType.Ledge;
				tileData.CollisionModel = resources.GetResource<CollisionModel>(parameters.GetString(0));
				string dirName = parameters.GetString(1);
				if (dirName == "right" || dirName == "east")
					tileData.LedgeDirection = Directions.Right;
				else if (dirName == "left" || dirName == "west")
					tileData.LedgeDirection = Directions.Left;
				else if (dirName == "up" || dirName == "north")
					tileData.LedgeDirection = Directions.Up;
				else if (dirName == "down" || dirName == "south")
					tileData.LedgeDirection = Directions.Down;
			});
			// Clone <tiledata>
			AddTilesetCommand("Clone", delegate(CommandParam parameters) {
				tileData.Clone(resources.GetResource<TileData>(parameters.GetString(0)));// = new TileData();
				//tileData.Tileset = tileset;
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

			AddSpriteCommand("SpriteSheet", delegate(CommandParam parameters) {
				if (parameters.Count == 1) {
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

					if (parameters.Count == 5) {
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
			}
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
			ScriptCommand command = new ScriptCommand(name, action);
			tilesetCommands.Add(command);
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
