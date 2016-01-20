using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Tiles {

	public class TileCrackedFloor : Tile {

		private float crumbleTimer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileCrackedFloor() {
		}


		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		private void Crumble() {
			RoomControl.RemoveTile(this);
			RoomControl.SpawnEntity(new EffectFallingObject(DepthLayer.EffectCrackedFloorCrumble), Center);

			// Play crumble sound
			AudioSystem.PlaySound(GameData.SOUND_FLOOR_CRUMBLE);

			// Create a pit tile if this tile is on the bottom layer.
			if (Layer == 0) {
				TileData pitTileData = Resources.GetResource<TileData>("pit");
				Tile pitTile = Tile.CreateTile(pitTileData);
				RoomControl.PlaceTile(pitTile, Location, Layer);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnInitialize() {
			crumbleTimer = 0;
		}

		public override void Update() {
			base.Update();

			// Check if the player is on top of this tile.
			Player player = RoomControl.Player;
			if (RoomControl.GetTileLocation(player.Position) == Location) {
				crumbleTimer++;
				if (crumbleTimer > GameSettings.TILE_CRACKED_FLOOR_CRUMBLE_DELAY) {
					Crumble();
				}
			}
			else {
				crumbleTimer = 0;
			}
		}
	}
}
