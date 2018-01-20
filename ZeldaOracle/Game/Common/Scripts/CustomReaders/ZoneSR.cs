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

		private enum Modes {
			Root,
			Zone
		}

		private Zone zone;
		private string zoneName;

		private bool postTileData;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ZoneSR(bool postTileData) {
			this.postTileData = postTileData;
			
			//=====================================================================================
			// SETUP
			//=====================================================================================
			AddCommand("ZONE", (int) Modes.Root,
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
				Mode = Modes.Zone;
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Zone,
				"",
			delegate (CommandParam parameters) {
				zoneName = "";
				zone = null;
				Mode = Modes.Root;
			});
			//=====================================================================================
			// BUILDING
			//=====================================================================================
			AddCommand("CLONE", (int) Modes.Zone,
				"string zoneID",
			delegate (CommandParam parameters) {
				string name = parameters.GetString(0);
				Zone cloneZone = GetResource<Zone>(name);
				zone = new Zone(cloneZone);
				zone.ID = zoneName;
				SetResource<Zone>(zoneName, zone);
			});
			//=====================================================================================
			AddCommand("NAME", (int) Modes.Zone,
				"string name",
			delegate (CommandParam parameters) {
				zone.Name = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("SIDESCROLLING", (int) Modes.Zone,
				"bool sidescrolling",
			delegate (CommandParam parameters) {
				zone.IsSideScrolling = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("UNDERWATER", (int) Modes.Zone,
				"bool underwater",
			delegate (CommandParam parameters) {
				zone.IsUnderwater = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("DEFAULTTILE", (int) Modes.Zone,
				"string tileName",
			delegate (CommandParam parameters) {
				if (postTileData) {
					string name = parameters.GetString(0);
					zone.DefaultTileData = GetResource<TileData>(name);
				}
			});
			//=====================================================================================
			AddCommand("IMAGEVARIANT", (int) Modes.Zone,
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
			AddCommand("PALETTE", (int) Modes.Zone,
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
			AddCommand("SETSTYLE", (int) Modes.Zone,
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


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>The mode of the Zone script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
