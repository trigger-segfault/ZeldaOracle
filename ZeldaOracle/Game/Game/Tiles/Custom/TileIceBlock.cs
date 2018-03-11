using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;

namespace ZeldaOracle.Game.Tiles {

	public class TileIceBlock : Tile {

		private int continuedMoveDirection;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileIceBlock() {
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			continuedMoveDirection = -1;
		}

		// Called when the player wants to push the tile.
		public override bool OnPush(int direction, float movementSpeed) {
			if (base.OnPush(direction, GameSettings.TILE_ICE_BLOCK_MOVEMENT_SPEED)) {
				continuedMoveDirection = MoveDirection;
				return true;
			}
			return false;
		}

		public override void OnCompleteMovement() {
			base.OnCompleteMovement();
			if (IsDestroyed)
				return;
			
			// Keep moving until we can move no more, then play a cling sound.
			if (!Move(continuedMoveDirection, 1, GameSettings.TILE_ICE_BLOCK_MOVEMENT_SPEED)) {
				AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
			}
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes the properties and events for the tile type.</summary>
		public static void InitializeTileData(TileData data) {
			data.Flags |= TileFlags.Movable;
		}
	}
}
