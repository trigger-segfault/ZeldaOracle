using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaAPI {

	public interface Tile {
	}
	
	public interface Lantern {
		void Light();
		void PutOut();
		void Light(bool stayLit);
		void PutOut(bool stayLit);
		bool IsLit { get; set; }
	}
	
	public interface ColorCubeSlot {
		Color Color { get; set; }
	}
	
	public interface ColorLantern {
		Color Color { get; set; }
	}

	public interface Door {
		void Open(bool instantaneous = false, bool rememberState = false);
		void Close(bool instantaneous = false, bool rememberState = false);
		bool IsOpen { get; }
	}

	public interface Button {
	}
	
	public interface Reward {
		void SpawnReward();
		bool IsLooted { get; }
	}

	public enum DoorState {
		Opened,
		Closed,
	}
}
