using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Graphics {
	/// <summary>Predefined palette references for mapped entity colors.</summary>
	public static class EntityColors {

		public static readonly PaletteReference Black		= new PaletteReference("default", LookupSubtypes.Black);
		public static readonly PaletteReference Tan			= new PaletteReference("red", LookupSubtypes.Light);

		public static readonly PaletteReference Red			= new PaletteReference("red", LookupSubtypes.Dark);
		public static readonly PaletteReference Green		= new PaletteReference("green", LookupSubtypes.Dark);
		public static readonly PaletteReference Blue		= new PaletteReference("blue", LookupSubtypes.Dark);
		public static readonly PaletteReference Orange		= new PaletteReference("orange", LookupSubtypes.Dark);

		public static readonly PaletteReference Yellow		= new PaletteReference("shaded_red", LookupSubtypes.Light);
		public static readonly PaletteReference DarkRed		= new PaletteReference("shaded_red", LookupSubtypes.Dark);
		public static readonly PaletteReference LightBlue	= new PaletteReference("shaded_blue", LookupSubtypes.Light);
		public static readonly PaletteReference DarkBlue	= new PaletteReference("shaded_blue", LookupSubtypes.Dark);

		public static readonly PaletteReference MenuWhite	= new PaletteReference("background", LookupSubtypes.Light, PaletteTypes.Tile);
		public static readonly PaletteReference MenuDark	= new PaletteReference("background", LookupSubtypes.Black, PaletteTypes.Tile);
	}

	/// <summary>Predefined palette references for mapped tile colors.</summary>
	public static class TileColors {
		public static readonly PaletteReference MenuWhite = new PaletteReference("background", LookupSubtypes.Light, PaletteTypes.Tile);
		public static readonly PaletteReference MenuDark = new PaletteReference("background", LookupSubtypes.Black, PaletteTypes.Tile);

		public static readonly PaletteReference DungeonMapFloorText = new PaletteReference("chest", LookupSubtypes.Black, PaletteTypes.Tile);
		public static readonly PaletteReference DungeonMapFloorTextShadow = new PaletteReference("chest", LookupSubtypes.Medium, PaletteTypes.Tile);

		public static readonly PaletteReference DungeonMapKeyText = new PaletteReference("background", LookupSubtypes.Black, PaletteTypes.Tile);
		public static readonly PaletteReference DungeonMapKeyTextShadow = new PaletteReference("background", LookupSubtypes.Medium, PaletteTypes.Tile);
	}
}
