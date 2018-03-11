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
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Tiles.Custom.Monsters;

namespace ZeldaOracle.Common.Scripts.CustomReaders {


	public class TileDataSR : ScriptReader {

		private enum Modes {
			Root,
			Tile,
			ActionTile,
			Model
		}

		private CollisionModel      model;
		private BaseTileData        baseTileData;
		private TileData            tileData;
		private ActionTileData       actionTileData;
		//private LoadingModes		loadingMode;
		private ISpriteSource source;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileDataSR() {
			
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
			AddType("Property",
				"(string type, string name, string value)",
				/*"(string type, string name, string value, string readableName, string editorType, " +
					"string category, string description)",
				"(string type, string name, string value, string readableName, (string editorType, " +
					"string editorSubtype), string category, string description)",*/
				"(string type, string name, string value, string readableName, string editorType, " +
					"string category, string description, bool browsable = true)",
				"(string type, string name, string value, string readableName, (string editorType, " +
					"string editorSubtype), string category, string description, bool browsable = true)"
			);
			//=====================================================================================
			// SOURCE
			//=====================================================================================
			AddCommand("SOURCE",
				"const none", "const null",
			delegate (CommandParam parameters) {
				source = null;
			});
			//=====================================================================================
			AddCommand("SOURCE",
				"string name",
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
			AddCommand("TILE", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				tileData = new TileData();
				tileData.Name = parameters.GetString(0);
				baseTileData = tileData;
				Mode = Modes.Tile;
			});
			//=====================================================================================
			AddCommand("ACTIONTILE", (int) Modes.Root,
				"string name",
			delegate (CommandParam parameters) {
				actionTileData = new ActionTileData();
				actionTileData.Name = parameters.GetString(0);
				baseTileData = actionTileData;
				Mode = Modes.ActionTile;
			});
			//=====================================================================================
			AddCommand("MONSTER", (int) Modes.Root,
				"string name, string sprite, string monsterType, string monsterColor, bool ignoreMonster = false",
			delegate (CommandParam parameters) {
				actionTileData = new ActionTileData();
				actionTileData.Name = parameters.GetString(0);
				baseTileData = actionTileData;

				actionTileData.Type = typeof(MonsterAction);
				actionTileData.Sprite = GetResource<ISprite>(parameters.GetString(1));
				actionTileData.EntityType = GameUtil.FindTypeWithBase
					<Monster>(parameters.GetString(2), false);
				actionTileData.Properties.Set("ignore_monster", parameters.GetBool(4));

				MonsterColor color;
				if (!Enum.TryParse<MonsterColor>(parameters.GetString(3), true, out color))
					ThrowParseError("Invalid monster color: '" + parameters.GetString(3) + "'!");
				actionTileData.Properties.SetEnum("color", color);
				Mode = Modes.ActionTile;
			});
			//=====================================================================================
			AddCommand("TILEMONSTER", (int) Modes.Root,
				"string name, string tileType, string monsterType, bool ignoreMonster = false",
			delegate (CommandParam parameters) {
				tileData = new TileData();
				tileData.Name = parameters.GetString(0);
				baseTileData = tileData;
				
				tileData.Type = GameUtil.FindTypeWithBase
					<TileMonster>(parameters.GetString(1), false);
				tileData.EntityType = GameUtil.FindTypeWithBase
					<Monster>(parameters.GetString(2), false);
				tileData.Properties.Set("ignore_monster", parameters.GetBool(3));
				
				Mode = Modes.Tile;
			});
			//=====================================================================================
			AddCommand("END", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"",
			delegate (CommandParam parameters) {
				if (tileData != null) {
					if (tileData.Tileset == null) {
						AddResource<BaseTileData>(tileData.Name, tileData);
						AddResource<TileData>(tileData.Name, tileData);
					}
					tileData = null;
					baseTileData = null;
				}
				else if (actionTileData != null) {
					if (actionTileData.Tileset == null) {
						AddResource<BaseTileData>(actionTileData.Name, actionTileData);
						AddResource<ActionTileData>(actionTileData.Name, actionTileData);
					}
					actionTileData = null;
					baseTileData = null;
				}
				Mode = Modes.Root;
			});
			//=====================================================================================
			// TILES 
			//=====================================================================================
			AddCommand("TYPE", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"string type",
			delegate (CommandParam parameters) {
				string typeName = parameters.GetString(0);
				if (tileData != null)
					baseTileData.Type = GameUtil.FindTypeWithBase<Tile>(typeName, true);
				else
					baseTileData.Type = GameUtil.FindTypeWithBase<ActionTile>(typeName, true);
			});
			//=====================================================================================
			AddCommand("ENTITYTYPE", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"string type",
			delegate (CommandParam parameters) {
				string typeName = parameters.GetString(0);
				baseTileData.EntityType = GameUtil.FindTypeWithBase<Entity>(typeName, true);
			});
			//=====================================================================================
			AddCommand("FLAGS", (int) Modes.Tile,
				"string flags...",
			delegate (CommandParam parameters) {
				for (int i = 0; i < parameters.ChildCount; i++) {
					TileFlags flags = TileFlags.None;
					if (Enum.TryParse<TileFlags>(parameters.GetString(i), true, out flags))
						tileData.Flags |= flags;
					else
						ThrowParseError("Invalid tile flag: \"" + parameters.GetString(i) + "\"!", parameters[i]);
				}
			});
			//=====================================================================================
			AddCommand("ENVTYPE", (int) Modes.Tile,
				"string envType",
			delegate (CommandParam parameters) {
				TileEnvironmentType envType = TileEnvironmentType.Normal;
				if (Enum.TryParse<TileEnvironmentType>(parameters.GetString(0), true, out envType))
					tileData.Properties.Set("environment_type", (int) envType);
				else
					ThrowParseError("Invalid tile environment type: \"" + parameters.GetString(0) + "\"!", parameters[0]);
			});
			//=====================================================================================
			AddCommand("RESETWHEN", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"string resetCondition",
			delegate (CommandParam parameters) {
				TileResetCondition resetCondition = TileResetCondition.LeaveRoom;
				if (Enum.TryParse(parameters.GetString(0), true, out resetCondition))
					baseTileData.ResetCondition = resetCondition;
				else
					ThrowParseError("Invalid tile reset condition: \"" + parameters.GetString(0) + "\"!", parameters[0]);
			});
			//=====================================================================================
			AddCommand("CONVEYOR", (int) Modes.Tile,
				"string angle, float speed",
			delegate (CommandParam parameters) {
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
			/*AddCommand("PROPERTIES", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"Property properties...",
				//"(string type, string name, var otherData...)...",
				CommandProperties);*/
			//=====================================================================================
			AddCommand("PROPERTY", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"(string name, string value)",
				CommandProperty);
			//=====================================================================================
			/*AddCommand("LOCKPROP", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"string name",
				CommandLockProperty);*/
			//=====================================================================================
			/*AddCommand("DOCUMENT", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"(string name, string readableName, string editorType, string category, " +
					"string description, bool browsable = true)",
				"(string name, string readableName, (string editorType, string editorSubtype), " +
					"string category, string description, bool browsable = true)",
				CommandDocumentation);*/
			//=====================================================================================
			/*AddCommand("EVENT", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"string name, string readableName, string category, string description",
				"string name, string readableName, string category, string description, (string params...)", // Params = (type1, name1, type2, name2...)
			delegate (CommandParam parameters) {
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
			});*/
			//=====================================================================================
			AddCommand("SPRITE", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"Sprite sprite, Point drawOffset = (0, 0)",
			delegate (CommandParam parameters) {
				baseTileData.Sprite = GetSpriteFromParams(parameters);
				Point2I drawOffset = parameters.GetPoint(1);
				if (!drawOffset.IsZero) {
					baseTileData.Sprite = new OffsetSprite(baseTileData.Sprite, drawOffset);
				}
			});
			//=====================================================================================
			AddCommand("SAMESPRITE", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"",
			delegate (CommandParam parameters) {
				if (baseTileData == null)
					ThrowCommandParseError("Cannot call SAMESPRITE without editing tile data");
				baseTileData.Sprite = GetResource<ISprite>("tile_" + baseTileData.Name);
			});
			//=====================================================================================
			AddCommand("SAMESPRITEABOVE", (int) Modes.Tile,
				"",
			delegate (CommandParam parameters) {
				if (tileData == null)
					ThrowCommandParseError("Cannot call SAMESPRITEABOVE without editing tile data");
				tileData.SpriteAbove = GetResource<ISprite>("tile_" + baseTileData.Name + "_above");
			});
			//=====================================================================================
			AddCommand("SAMESPRITEOBJ", (int) Modes.Tile,
				"",
			delegate (CommandParam parameters) {
				if (tileData == null)
					ThrowCommandParseError("Cannot call SAMESPRITEOBJ without editing tile data");
				tileData.SpriteAsObject = GetResource<ISprite>("tile_" + baseTileData.Name + "_asobject");
			});
			//=====================================================================================
			AddCommand("SIZE", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"Point size",
			delegate (CommandParam parameters) {
				baseTileData.Size = parameters.GetPoint(0);
			});
			//=====================================================================================
			AddCommand("SPRITEINDEX", (int) Modes.Tile,
				"int index, Sprite sprite, Point drawOffset = (0, 0)",
			delegate (CommandParam parameters) {
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

				tileData.SpriteList[index] = GetSpriteFromParams(parameters, 1);
				Point2I drawOffset = parameters.GetPoint(2);
				if (!drawOffset.IsZero) {
					tileData.SpriteList[index] = new OffsetSprite(tileData.Sprite, drawOffset);
				}
			});
			//=====================================================================================
			AddCommand("SPRITELIST", (int) Modes.Tile,
				"Sprite sprites...",
			delegate (CommandParam parameters) {
				ISprite[] spriteList = new ISprite[parameters.ChildCount];
				for (int i = 0; i < parameters.ChildCount; i++)
					spriteList[i] = GetSpriteFromParams(parameters, i);

				tileData.SpriteList = spriteList;
			});
			//=====================================================================================
			AddCommand("SPRITEABOVE", (int) Modes.Tile,
				"const none", "const null",
			delegate (CommandParam parameters) {
				tileData.SpriteAbove = null;
			});
			//=====================================================================================
			AddCommand("SPRITEOBJ", (int) Modes.Tile,
				"const none", "const null",
			delegate (CommandParam parameters) {
				tileData.SpriteAsObject = null;
			});
			//=====================================================================================
			AddCommand("SPRITEABOVE", (int) Modes.Tile,
				"Sprite sprite",
			delegate (CommandParam parameters) {
				tileData.SpriteAbove = GetSpriteFromParams(parameters);
			});
			//=====================================================================================
			AddCommand("SPRITEOBJ", (int) Modes.Tile,
				"Sprite sprite, Point drawOffset = (0, 0)",
			delegate (CommandParam parameters) {
				tileData.SpriteAsObject = GetSpriteFromParams(parameters);
				Point2I drawOffset = parameters.GetPoint(1);
				if (!drawOffset.IsZero) {
					tileData.SpriteAsObject = new OffsetSprite(tileData.Sprite, drawOffset);
				}
			});
			//=====================================================================================
			AddCommand("BREAKLAYER", (int) Modes.Tile,
				"string layerName",
			delegate (CommandParam parameters) {
				string depthStr = parameters.GetString(0);
				DepthLayer breakLayer;
				if (!Enum.TryParse(depthStr, true, out breakLayer))
					ThrowCommandParseError("Unknown DepthLayer '" + depthStr + "'!");
				tileData.BreakLayer = breakLayer;
			});
			//=====================================================================================
			AddCommand("BREAKANIM", (int) Modes.Tile,
				"string animationName",
			delegate (CommandParam parameters) {
				tileData.BreakAnimation = GetResource<ISprite>(parameters.GetString(0)) as Animation;
			});
			//=====================================================================================
			AddCommand("BREAKSOUND", (int) Modes.Tile,
				"string soundName",
			delegate (CommandParam parameters) {
				tileData.BreakSound = GetResource<Sound>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("MODEL", (int) Modes.Tile,
				"string collisionModelName",
			delegate (CommandParam parameters) {
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("SOLID", (int) Modes.Tile,
				"string collisionModelName",
			delegate (CommandParam parameters) {
				tileData.SolidType = TileSolidType.Solid;
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("HALFSOLID", (int) Modes.Tile,
				"string collisionModelName",
			delegate (CommandParam parameters) {
				tileData.SolidType = TileSolidType.HalfSolid;
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("LEDGE", (int) Modes.Tile,
				"string collisionModelName, string ledgeDirection",
			delegate (CommandParam parameters) {
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
			AddCommand("BASICLEDGE", (int) Modes.Tile,
				"string collisionModelName, string ledgeDirection",
			delegate (CommandParam parameters) {
				tileData.SolidType = TileSolidType.BasicLedge;
				tileData.CollisionModel = GetResource<CollisionModel>(parameters.GetString(0));
				string dirName = parameters.GetString(1);
				int direction;
				if (Directions.TryParse(dirName, true, out direction))
					tileData.LedgeDirection = direction;
				else
					ThrowParseError("Unknown value for ledge direction: " + dirName);
			});
			//=====================================================================================
			AddCommand("LEAPLEDGE", (int) Modes.Tile,
				"string ledgeDirection",
			delegate (CommandParam parameters) {
				tileData.SolidType = TileSolidType.LeapLedge;
				string dirName = parameters.GetString(0);
				int direction;
				if (Directions.TryParse(dirName, true, out direction))
					tileData.LedgeDirection = direction;
				else
					ThrowParseError("Unknown value for ledge direction: " + dirName);
			});
			//=====================================================================================
			AddCommand("HURT", (int) Modes.Tile,
				"int damage, Rectangle area",
			delegate (CommandParam parameters) {
				tileData.HurtDamage = parameters.GetInt(0);
				tileData.HurtArea = parameters.GetRectangle(1);
			});
			//=====================================================================================
			AddCommand("TILEBELOW", (int) Modes.Tile,
				"const none", "const null",
			delegate (CommandParam parameters) {
				tileData.TileBelow = null;
			});
			//=====================================================================================
			AddCommand("TILEBELOW", (int) Modes.Tile,
				"string tileData",
			delegate (CommandParam parameters) {
				tileData.TileBelow = GetResource<TileData>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("CLONE", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"string tileDataName",
			delegate (CommandParam parameters) {
				if (tileData != null)
					tileData.Clone(GetResource<TileData>(parameters.GetString(0)));
				else if (actionTileData != null)
					actionTileData.Clone(GetResource<ActionTileData>(parameters.GetString(0)));
			});
			//=====================================================================================
			AddCommand("PREVIEW", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"Sprite sprite",
			delegate (CommandParam parameters) {
				baseTileData.PreviewSprite = GetSpriteFromParams(parameters);
			});
			//=====================================================================================
			AddCommand("ENTITYDRAW", (int) Modes.Tile,
				"bool drawAsEntity",
			delegate (CommandParam parameters) {
				tileData.DrawAsEntity = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("SHARED", new int[] { (int) Modes.Tile, (int) Modes.ActionTile },
				"bool shared",
			delegate (CommandParam parameters) {
				baseTileData.IsShared = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("MODEL", (int) Modes.Root,
				"string modeName",
			delegate (CommandParam parameters) {
				string modelName = parameters.GetString(0);
				if (!modelName.StartsWith("temp_"))
					ThrowCommandParseError("Model names defined in Tile Data scripts must be temporary and prefixed with \"temp_\"!");
				model = new CollisionModel();
				AddResource(modelName, model);
				Mode = Modes.Model;
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Model,
				"",
			delegate (CommandParam parameters) {
				model = null;
				Mode = Modes.Root;
			});
			//=====================================================================================
			AddCommand("ADD", (int) Modes.Model,
				"Rectangle box",
			delegate (CommandParam parameters) {
				model.AddBox(parameters.GetRectangle(0));
			});
			//=====================================================================================
			AddCommand("COMBINE", (int) Modes.Model,
				"string modelName, Point offset = (0, 0)",
			delegate (CommandParam parameters) {
				model.Combine(
					GetResource<CollisionModel>(parameters.GetString(0)),
					parameters.GetPoint(1));
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Script Commands
		//-----------------------------------------------------------------------------

		private void CommandProperties(CommandParam param) {
			foreach (CommandParam child in param.GetChildren())
				ParseProperty(child);
		}

		private void CommandProperty(CommandParam param) {
			param = param.GetParam(0);
			string name = param.GetString(0);
			Property property = baseTileData.Properties.GetProperty(name, true);
			if (property == null)
				ThrowCommandParseError("Property with name '" + name + "' does not exist!");

			object value = ParsePropertyValue(param[1], property.Type);
			baseTileData.Properties.SetGeneric(name, value);
		}

		private void CommandLockProperty(CommandParam param) {
			string name = param.GetString(0);
			Property property = baseTileData.Properties.GetProperty(name, true);
			if (property == null)
				ThrowCommandParseError("Property with name '" + name + "' does not exist!");

			if (!property.HasDocumentation)
				property.Documentation = new PropertyDocumentation();

			property.Documentation.IsBrowsable = false;
			property.Documentation.IsReadOnly = true;
		}

		private void CommandDocumentation(CommandParam param) {
			param = param.GetParam(0);
			string name = param.GetString(0);
			Property property = baseTileData.Properties.GetProperty(name, true);
			if (property == null)
				ThrowCommandParseError("Property with name '" + name + "' does not exist!");

			ParsePropertyDocumentation(param, property, 1);
		}

		private void ParseProperty(CommandParam param) {
			string name = param.GetString(1);

			// Parse the property type and value.
			PropertyType type;
			if (!Enum.TryParse<PropertyType>(param.GetString(0), true, out type))
				ThrowParseError("Unknown property type '" + param.GetString(0) + "'");
			object value = ParsePropertyValue(param[2], type);

			// Set the property.
			Property property = baseTileData.Properties.SetGeneric(name, value);

			// Set the property's documentation.
			if (param.ChildCount > 3 && property != null) {
				ParsePropertyDocumentation(param, property, 3);
				/*string editorType = "";
				string editorSubType = "";
				if (param[4].Type == CommandParamType.Array) {
					editorType = param[4].GetString(0);
					editorSubType = param[4].GetString(1);
				}
				else {
					editorType = param.GetString(4);
				}

				property.SetDocumentation(
					readableName: param.GetString(3),
					editorType: editorType,
					editorSubType: editorSubType,
					category: param.GetString(5),
					description: param.GetString(6),
					isReadOnly: false,
					isBrowsable: param.GetBool(7, true)
				);*/
			}
		}

		private void ParsePropertyDocumentation(CommandParam param, Property property, int index) {
			string editorType = "";
			string editorSubType = "";
			if (param[index + 1].Type == CommandParamType.Array) {
				editorType = param[index + 1].GetString(0);
				editorSubType = param[index + 1].GetString(1);
			}
			else {
				editorType = param.GetString(index + 1);
			}

			property.SetDocumentation(
				readableName: param.GetString(index),
				editorType: editorType,
				editorSubType: editorSubType,
				category: param.GetString(index + 2),
				description: param.GetString(index + 3),
				isReadOnly: false,
				isBrowsable: param.GetBool(index + 4, true)
			);
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
				ThrowCommandParseError("Cannot get sprite from source with no sprite source!");
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
				ThrowCommandParseError("Cannot get sprite from source with no sprite source!");
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
			tileData    = null;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {

		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new TileDataSR();
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

		/// <summary>The mode of the TileData script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
