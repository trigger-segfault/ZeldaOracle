using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Tiles {

	public class TileButton : Tile, ZeldaAPI.Button {

		private bool isPressed;
		private HashSet<Tile> tilesCovering; // List of tiles covering this button
		private bool isCovered;
		private int uncoverTimer;
		private int uncoverDelay;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileButton() {
			tilesCovering = new HashSet<Tile>();
			uncoverDelay = 27;
		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			isPressed = Properties.Get("pressed", false);
			//Properties.Set("sprite_index", (isPressed ? 1 : 0));

			/*if (isPressed)
				CustomSprite = GameData.SPR_TILE_BUTTON_DOWN;
			else
				CustomSprite = GameData.SPR_TILE_BUTTON_UP;*/
			SpriteIndex = (isPressed ? 1 : 0);

			isCovered = false;
			uncoverTimer = 0;
		}

		public override void OnCoverBegin(Tile tile) {
			tilesCovering.Add(tile);
		}

		public override void OnCoverComplete(Tile tile) {
			isCovered = true;
			tilesCovering.Add(tile);
		}

		public override void OnUncoverBegin(Tile tile) {
			isCovered = false;
			if (tile.IsMoving)
				uncoverTimer = uncoverDelay;
			else
				uncoverTimer = 0;
			tilesCovering.Remove(tile);
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

			pressRect = new Rectangle2F(6, 6, 4, 4);
			pressRect.Point += Position;

			if (isCovered) {
				isDown = true;
			}
			else {
				uncoverTimer--;
				if (uncoverTimer > 0)
					isDown = true;
				else {
					foreach (Tile tile in tilesCovering) {
						if (tile.Bounds.Contains(Center)) {
							isDown = true;
						}
					}
				}
			}

			/*
			pressRect = new Rectangle2F(6, 6, 4, 4);
			pressRect.Point += Position;

			// Check if a tile is on top of this button.
			foreach (Tile tile in RoomControl.TileManager.GetTilesAtPosition(Center)) {
				if (tile.IsSolid) {
					isDown = true;
					break;
				}
			}
			*/
			/*
			for (int i = Layer; i < RoomControl.Room.LayerCount; i++) {
				Tile tile = RoomControl.GetTile(Location, i);

				if (tile != null && tile.IsSolid) {
					isDown = true;
				}
			}*/

			// Check if the pressed state has changed.
			bool releasable = Properties.GetBoolean("releasable", true);
			if (isPressed != isDown && (isDown || releasable)) {
				isPressed = isDown;

				// Set the pressed state.
				if (Properties.Get("remember_state", false))
					Properties.SetBase("pressed", isPressed);
				else
					Properties.Set("pressed", isPressed);

				// Fire the event.
				if (isPressed) {
					//CustomSprite = GameData.SPR_TILE_BUTTON_DOWN;
					GameControl.FireEvent(this, "on_press", this);
				}
				else {
					//CustomSprite = GameData.SPR_TILE_BUTTON_UP;
					GameControl.FireEvent(this, "on_release", this);
				}
				SpriteIndex = (isPressed ? 1 : 0);

				AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
			}
		}
	}
}
