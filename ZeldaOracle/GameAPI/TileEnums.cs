using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaAPI {
	
	/// <summary>The colors types for use with puzzles.</summary>
	public enum Color {
		/// <summary>No puzzle color.</summary>
		None = -1,
		/// <summary>Red puzzle color.</summary>
		Red,
		/// <summary>Yellow puzzle color.</summary>
		Yellow,
		/// <summary>Blue puzzle color.</summary>
		Blue
	}

	/// <summary>The orientations a minecart track can be in.</summary>
	public enum MinecartTrackOrientation {
		/// <summary>Track goes from left to right.</summary>
		Horizontal = 0,
		/// <summary>Track goes from top to bottom.</summary>
		Vertical,
		/// <summary>Track goes from top to right.</summary>
		UpRight,
		/// <summary>Track goes from top to left.</summary>
		UpLeft,
		/// <summary>Track goes from bottom to left.</summary>
		DownLeft,
		/// <summary>Track goes from bottom to right.</summary>
		DownRight
	}

	/// <summary>The states of a dungeon door.</summary>
	public enum DoorState {
		/// <summary>The door is opened.</summary>
		Opened = 0,
		/// <summary>The door is closed.</summary>
		Closed,
	}

	/// <summary>The color cube's sprite index has the value of one of these orientations.</summary>
	public enum ColorCubeOrientation {
		/// <summary>Blue on top with yellow on the side.</summary>
		BlueYellow = 0,
		/// <summary>Blue on top with red on the side.</summary>
		BlueRed,
		/// <summary>Yellow on top with red on the side.</summary>
		YellowRed,
		/// <summary>Yellow on top with blue on the side.</summary>
		YellowBlue,
		/// <summary>Red on top with blue on the side.</summary>
		RedBlue,
		/// <summary>Red on top with yellow on the side.</summary>
		RedYellow
	}
}
