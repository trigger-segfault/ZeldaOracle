using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaAPI {

	// Base interface for all tiles.
	public interface Tile {
		string Id { get; }
		bool IsMovable { get; set; }
	}
	
	public interface Lantern : Tile {
		void Light();
		void PutOut();
		void Light(bool stayLit);
		void PutOut(bool stayLit);
		bool IsLit { get; set; }
	}
	
	public interface ColorCubeSlot : Tile {
		Color Color { get; set; }
	}
	
	public interface ColorLantern : Tile {
		Color Color { get; set; }
	}
	
	public interface ColorJumpPad : Tile {
		Color Color { get; set; }
	}
	
	public interface ColorTile : Tile {
		Color Color { get; set; }
	}
	
	public interface ColorStatue : Tile {
		Color Color { get; }
	}

	public enum DoorState {
		Opened,
		Closed,
	}

	public interface Door : Tile {
		void Open(bool instantaneous = false, bool rememberState = false);
		void Close(bool instantaneous = false, bool rememberState = false);
		bool IsOpen { get; }
	}

	public interface Button : Tile {
		// Nothing yet...
	}

	public interface Lever : Tile {
		bool IsFacingLeft { get; }
		bool IsFacingRight { get; }
	}

	public interface ColorSwitch : Tile {
		// Nothing yet...
	}

	public interface SeedBouncer : Tile {
		void RotateClockwise(int amount = 1);
		void RotateCounterClockwise(int amount = 1);
	}

	public interface Bridge : Tile {
		void BuildBridge(bool instantaneous = false, bool rememberState = false);
		void DestroyBridge(bool instantaneous = false, bool rememberState = false);
		bool IsBridgeBuilt { get; }
	}

	public interface MinecartTrack : Tile {
		void SwitchTrackDirection();
	}

	public interface CrossingGate : Tile {
		void Raise();
		void Lower();
		bool IsRaised { get; }
	}
	
	public interface Reward : Tile {
		void SpawnReward();
		bool IsLooted { get; }
	}
}
