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
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Common.Scripts.CustomReaders {


	public class ZoneSR : ScriptReader {

		private Zone zone;
		private string zoneName;

		private bool postTileData;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ZoneSR(bool postTileData) {
			this.postTileData = postTileData;
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
			// SETUP
			//=====================================================================================
			AddCommand("ZONE", 0,
				"string id",
			delegate (CommandParam parameters) {
				zoneName = parameters.GetString(0);
				if (!Resources.ContainsResource<Zone>(zoneName)) {
					zone = new Zone();
					zone.ID = zoneName;
					AddResource<Zone>(zoneName, zone);
				}
				else {
					zone = GetResource<Zone>(zoneName, true);
				}
				Mode = 1;
			});
			//=====================================================================================
			AddCommand("END", 1,
				"",
			delegate (CommandParam parameters) {
				zoneName = "";
				zone = null;
				Mode = 0;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("CLONE", 1,
				"string zoneID",
			delegate (CommandParam parameters) {
				string name = parameters.GetString(0);
				Zone cloneZone = GetResource<Zone>(name);
				zone = new Zone(cloneZone);
				zone.ID = zoneName;
				SetResource<Zone>(zoneName, zone);
			});
			//=====================================================================================
			AddCommand("NAME", 1,
				"string name",
			delegate (CommandParam parameters) {
				zone.Name = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("SIDESCROLLING", 1,
				"bool sidescrolling",
			delegate (CommandParam parameters) {
				zone.IsSideScrolling = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("UNDERWATER", 1,
				"bool underwater",
			delegate (CommandParam parameters) {
				zone.IsUnderwater = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("DEFAULTTILE", 1,
				"string tileName",
			delegate (CommandParam parameters) {
				if (postTileData) {
					string name = parameters.GetString(0);
					zone.DefaultTileData = GetResource<TileData>(name);
				}
			});
			//=====================================================================================
			AddCommand("IMAGEVARIANT", 1,
				"string variantName",
			delegate (CommandParam parameters) {
				string name = parameters.GetString(0);
				foreach (var field in typeof(GameData).GetFields(BindingFlags.Static | BindingFlags.Public)) {
					if (field.Name == ("VARIANT_" + name.ToUpper())) {
						zone.ImageVariantID = (int) field.GetValue(null);
						return;
					}
				}
				ThrowCommandParseError("No image variant with the name '" + name + "'!");
			});
			//=====================================================================================
			AddCommand("PALETTE", 1,
				"string tilePaletteName",
			delegate (CommandParam parameters) {
				string name = parameters.GetString(0);
				Palette palette = GetResource<Palette>(name);
				if (palette.PaletteType != PaletteTypes.Tile) {
					ThrowCommandParseError("Zone palette must be a 'tile' palette!");
				}
				zone.PaletteID = name;
			});
			//=====================================================================================
			AddCommand("SETSTYLE", 1,
				"string styleGroup, string style",
			delegate (CommandParam parameters) {
				string styleGroup = parameters.GetString(0);
				string style = parameters.GetString(1);
				if (postTileData) {
					if (!Resources.ContainsStyleGroup(styleGroup))
						ThrowCommandParseError("Unknown style group '" + styleGroup + "'!");
					else if (!Resources.ContainsStyle(styleGroup, style))
						ThrowCommandParseError("Unknown style '" + style + "' in style group '" + styleGroup + "'!");
				}
				zone.StyleDefinitions.Set(styleGroup, style);
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Script Commands
		//-----------------------------------------------------------------------------
		


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			//loadingMode	= LoadingModes.Tilesets;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {

		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new ZoneSR(postTileData);
		}
		
	}
}
