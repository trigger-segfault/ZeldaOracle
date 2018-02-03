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
using ZeldaOracle.Game.Tiles.ActionTiles;

namespace ZeldaOracle.Common.Scripts.CustomReaders {


	public class TilesetSR : ScriptReader {

		private enum Modes {
			Root,
			Tileset
		}
		
		private Tileset				tileset;
		private string				tilesetName;
		private bool                tilesetEnded;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TilesetSR() {

			tilesetEnded = false;
			
			//=====================================================================================
			// TILE/TILESET BEGIN/END 
			//=====================================================================================
			AddCommand("TILESET", (int) Modes.Root,
				"string name, Point dimensions, bool usePreviewSprites = true",
			delegate (CommandParam parameters) {
				if (tilesetEnded)
					ThrowCommandParseError("Can only make one tileset per .conscript file!");
				tilesetName = parameters.GetString(0);
				tileset = new Tileset(tilesetName,
					parameters.GetPoint(1), parameters.GetBool(2));
				tileset.ConscriptPath = FileName;
				AddResource<Tileset>(tileset.ID, tileset);
				Mode = Modes.Tileset;
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Tileset,
				"",
			delegate (CommandParam parameters) {
				tileset = null;
				tilesetName = "";
				Mode = Modes.Root;
				tilesetEnded = true;
			});
			//=====================================================================================
			// TILESET SETUP
			//=====================================================================================
			AddCommand("CLONE TILESET", (int) Modes.Tileset,
				"string tileset",
			delegate (CommandParam parameters) {
				Tileset cloneTileset = GetResource<Tileset>(parameters.GetString(0));
				tileset = new Tileset(cloneTileset);
				tileset.ID = tilesetName;
				tileset.ConscriptPath = FileName;
				SetResource<Tileset>(tilesetName, tileset);
			});
			//=====================================================================================
			AddCommand("RESIZE", (int) Modes.Tileset,
				"Point newDimensions",
			delegate (CommandParam parameters) {
				tileset.Resize(parameters.GetPoint(0));
			});
			//=====================================================================================
			// TILESET BUILDING
			//=====================================================================================
			AddCommand("ADDTILE", (int) Modes.Tileset,
				"Point tilesetIndex, string tileName",
			delegate (CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);
				tileset.AddTileData(location,
					GetResource<BaseTileData>(parameters.GetString(1)));
			});
			//=====================================================================================
			AddCommand("SETTILE", (int) Modes.Tileset,
				"Point tilesetIndex, string tileName",
			delegate (CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);
				tileset.SetTileData(location,
					GetResource<BaseTileData>(parameters.GetString(1)));
			});
			//=====================================================================================
			AddCommand("REMOVETILE", (int) Modes.Tileset,
				"Point tilesetIndex",
			delegate (CommandParam parameters) {
				Point2I location = parameters.GetPoint(0);
				tileset.RemoveTileData(location);
			});
			//=====================================================================================

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
		/*private ISprite GetSprite(string name) {
			ISprite sprite = GetResource<ISprite>(name);
			if (sprite == null) {
				ThrowCommandParseError("Sprite with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}*/

		/// <summary>Gets a sprite.</summary>
		/*private ISprite GetSprite(ISpriteSource source, Point2I index) {
			if (source == null)
				ThrowCommandParseError("Cannot get sprite from source with no sprite sheet source!");
			ISprite sprite = source.GetSprite(index);
			if (sprite == null) {
				ThrowCommandParseError("Sprite at source index '" + index + "' does not exist!");
			}
			return sprite;
		}*/

		/// <summary>Gets a sprite and confirms its type.</summary>
		/*private T GetSprite<T>(string name) where T : class, ISprite {
			T sprite = GetResource<ISprite>(name) as T;
			if (sprite == null) {
				ThrowCommandParseError(typeof(T).Name + " with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}*/

		/// <summary>Gets a sprite and confirms its type.</summary>
		/*private T GetSprite<T>(ISpriteSource source, Point2I index) where T : class, ISprite {
			if (source == null)
				ThrowCommandParseError("Cannot get sprite from source with no sprite sheet source!");
			T sprite = source.GetSprite(index) as T;
			if (sprite == null) {
				ThrowCommandParseError(typeof(T).Name + " at source index '" + index + "' does not exist!");
			}
			return sprite;
		}*/

		/// <summary>Gets the sprite of a definition sprite.</summary>
		/*private ISprite GetDefinedSprite(string name, string definition) {
			return GetDefinedSprite(GetSprite<DefinitionSprite>(name), definition);
		}*/

		/// <summary>Gets the sprite of a definition sprite.</summary>
		/*private ISprite GetDefinedSprite(ISpriteSource source, Point2I index, string definition) {
			return GetDefinedSprite(GetSprite<DefinitionSprite>(source, index), definition);
		}*/

		/// <summary>Gets the sprite of a definition sprite.</summary>
		/*private ISprite GetDefinedSprite(DefinitionSprite sprite, string definition) {
			ISprite defSprite = sprite.Get(definition);
			if (defSprite == null)
				ThrowCommandParseError("Defined sprite with definition '" + definition + "' does not exist!");
			return defSprite;
		}*/

		/// <summary>Gets the sprite from one of the many parameter overloads.</summary>
		/*private ISprite GetSpriteFromParams(CommandParam param, int startIndex = 0) {
			ISpriteSource source;
			Point2I index;
			string definition;
			return GetSpriteFromParams(param, startIndex, out source, out index, out definition);
		}*/

		/// <summary>Gets the sprite from one of the many parameter overloads and returns the source.</summary>
		/*private ISprite GetSpriteFromParams(CommandParam param, int startIndex, out ISpriteSource source, out Point2I index, out string definition) {
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
		}*/


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			//loadingMode	= LoadingModes.Tilesets;
			//tileset			= null;
			tileset		= null;
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
		/*private SpriteSheet SpriteSheet {
			get { return source as SpriteSheet; }
		}*/

		/// <summary>Gets the source as a sprite set.</summary>
		/*private SpriteSet SpriteSet {
			get { return source as SpriteSet; }
		}*/

		/// <summary>The mode of the Tileset script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
