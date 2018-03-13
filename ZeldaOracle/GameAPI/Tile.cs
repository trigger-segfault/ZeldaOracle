using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Game;

namespace ZeldaAPI {

	/// <summary>Access to a base tile.</summary>
	public interface Tile {
		/// <summary>Overrides the default properties of the tile to its current properties.</summary>
		void OverrideDefaultState();
		/// <summary>Gets the ID of the tile.</summary>
		string ID { get; }
		/// <summary>Returns true if the tile is movable.</summary>
		bool IsMovable { get; set; }
	}

	/// <summary>Access to a lanturn tile.</summary>
	public interface Lantern : Tile {
		/// <summary>Lights the lanturn fire with the option of staying lit.</summary>
		/// <param name="stayLit">True if the lanturn stays lit even after leaving
		/// the room.</param>
		void Light(bool stayLit = false);
		/// <summary>Puts out the lanturn fire with the option of staying out.</summary>
		/// <param name="stayUnlit">True if the lanturn stays unlit even after leaving
		/// the room.</param>
		void PutOut(bool stayUnlit = false);
		/// <summary>Returns true if the lanturn is lit.</summary>
		bool IsLit { get; set; }
	}
	
	/// <summary>Access to a color cube slot tile.</summary>
	public interface ColorCubeSlot : Tile {
		/// <summary>The color of the tile based on the color cube on top of it.</summary>
		PuzzleColor Color { get; set; }
	}

	/// <summary>Access to a lanturn tile with a colored flame.</summary>
	public interface ColorLantern : Tile {
		/// <summary>The color of the lanturn's flame.</summary>
		PuzzleColor Color { get; set; }
	}

	/// <summary>Access to a tile that can change colors by jumping on it.</summary>
	public interface ColorJumpPad : Tile {
		/// <summary>The color of the jump pad.</summary>
		PuzzleColor Color { get; set; }
	}

	/// <summary>Access to a color tile.</summary>
	public interface ColorTile : Tile {
		/// <summary>Gets the color of the color tile.</summary>
		PuzzleColor Color { get; set; }
	}

	/// <summary>Access to a puzzle-colored statue tile.</summary>
	public interface ColorStatue : Tile {
		/// <summary>The color of the statue.</summary>
		PuzzleColor Color { get; }
	}

	/// <summary>Access to a color block tile.</summary>
	public interface ColorBlock : Tile {
		/// <summary>Gets the color of the color block.</summary>
		PuzzleColor Color { get; }
	}

	/// <summary>Access to a door tile.</summary>
	public interface Door : Tile {
		/// <summary>Opens the door.</summary>
		/// <param name="instantaneous">If true, the door animation does not play.</param>
		/// <param name="rememberState">True if the door stays in its new state.</param>
		void Open(bool instantaneous = false, bool rememberState = false);
		/// <summary>Closes the door.</summary>
		/// <param name="instantaneous">If true, the door animation does not play.</param>
		/// <param name="rememberState">True if the door stays in its new state.</param>
		void Close(bool instantaneous = false, bool rememberState = false);
		/// <summary>Returns true if the door is open.</summary>
		bool IsOpen { get; }
	}

	/// <summary>Access to a button switch tile.</summary>
	public interface Button : Tile {
		// Nothing yet...
	}

	/// <summary>Access to a level switch tile.</summary>
	public interface Lever : Tile {
		/// <summary>Returns true if the lever is facing left.</summary>
		bool IsFacingLeft { get; }
		/// <summary>Returns true if the lever is facing right.</summary>
		bool IsFacingRight { get; }
	}

	/// <summary>Access to a red/blue color switch tile.</summary>
	public interface ColorSwitch : Tile {
		/// <summary>Gets the color of the color switch tile. This can only be
		/// red or blue.</summary>
		PuzzleColor Color { get; }
	}

	/// <summary>Access to a rotatable plate for bouncing seeds off of.</summary>
	public interface SeedBouncer : Tile {
		/// <summary>Rotates the bouncer clockwise.</summary>
		/// <param name="amount">The amount of tiles to rotate the bouncer.</param>
		void RotateClockwise(int amount = 1);
		/// <summary>Rotates the bouncer counter-clockwise.</summary>
		/// <param name="amount">The amount of tiles to rotate the bouncer.</param>
		void RotateCounterClockwise(int amount = 1);
	}

	/// <summary>Access to a bridge that can appear and dissapear.</summary>
	public interface Bridge : Tile {
		/// <summary>Builds the bridge.</summary>
		/// <param name="instantaneous">If true, the build animation does not play.</param>
		/// <param name="rememberState">True if the bridge stays in its new state.</param>
		void BuildBridge(bool instantaneous = false, bool rememberState = false);
		/// <summary>Destroys the bridge.</summary>
		/// <param name="instantaneous">If true, the build animation does not play.</param>
		/// <param name="rememberState">True if the bridge stays in its new state.</param>
		void DestroyBridge(bool instantaneous = false, bool rememberState = false);
		/// <summary>Retuns true if the bridge is built.</summary>
		bool IsBridgeBuilt { get; }
	}

	/// <summary>Access to a minecart track tile.</summary>
	public interface MinecartTrack : Tile {
		/// <summary>Switches the track to the switched orientation or back to the
		/// regular orientation.</summary>
		void SwitchTrackDirection();
	}

	/// <summary>Access to a pull handle tile.</summary>
	public interface PullHandle : Tile {
		/// <summary>True if the pull handle has been extended away from the wall
		/// as far as possible.</summary>
		bool IsFullyExtended { get; }
		/// <summary>True if the pull handle has retracted all the way into the wall.</summary>
		bool IsFullyRetracted { get; }
		/// <summary>Gets the distance the pull handle is extended to in pixels.</summary>
		float ExtendDistance { get; }
	}

	/// <summary>Access to a minecart crossing gate tile.</summary>
	public interface CrossingGate : Tile {
		/// <summary>Raises the crossing gate.</summary>
		void Raise();
		/// <summary>Lowers the crossing gate.</summary>
		void Lower();
		/// <summary>Gets if the crossing gate is raised.</summary>
		bool IsRaised { get; }
	}

	/// <summary>Access to a standalone reward tile.</summary>
	public interface RewardTile : Tile {
		/// <summary>Spawns the reward.</summary>
		void SpawnReward();
		/// <summary>Returns true if the reward has already been taken.</summary>
		bool IsLooted { get; }
	}
}
