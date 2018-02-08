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
using ZeldaOracle.Common.Graphics;

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
			if (!IsSilent)
				RoomControl.SpawnEntity(new EffectFallingObject(DepthLayer.EffectCrackedFloorCrumble), Center);

			// Play crumble sound
			if (!IsSilent)
				AudioSystem.PlaySound(GameData.SOUND_FLOOR_CRUMBLE);

			// Create a pit tile if this tile is on the bottom layer.
			if (Layer == 0 && TileBelow == null) {
				TileData holeTileData = Resources.GetResource<TileData>("hole");
				Tile holeTile = Tile.CreateTile(holeTileData);
				RoomControl.PlaceTile(holeTile, Location, Layer);
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsSilent {
			get { return Properties.GetBoolean("silent", false); }
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Draws the tile data to display in the editor.</summary>
		public new static void DrawTileData(Graphics2D g, TileDataDrawArgs args) {
			Tile.DrawTileData(g, args);
		}
	}
}
