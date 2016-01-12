using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Tiles {

	public class TileButton : Tile, ZeldaAPI.Button {

		private bool isPressed;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileButton() {

		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			isPressed = false;
		}

		public override void Update() {
			base.Update();

			bool isDown = false;

			// Check if the player is on top of this button.
			Rectangle2F pressRect = new Rectangle2F(0, 5, 16, 16);
			pressRect.Point += Position;
			Player player = RoomControl.Player;
			if (pressRect.Contains(player.Position))
				isDown = true;

			// Check if a tile is on top of this button.
			for (int i = Layer; i < RoomControl.Room.LayerCount; i++) {
				Tile tile = RoomControl.GetTile(Location, i);

				if (tile != null && tile.IsSolid) {
					isDown = true;
				}
			}

			Properties.SetBase("sprite_index", (isDown ? 1 : 0));

			if (isPressed != isDown) {
				isPressed = isDown;

				if (isPressed) {
					GameControl.ExecuteScript(Properties.GetString("on_press", ""), this);
				}
				else {
					GameControl.ExecuteScript(Properties.GetString("on_release", ""), this);
				}
			}
		}
	}
}
