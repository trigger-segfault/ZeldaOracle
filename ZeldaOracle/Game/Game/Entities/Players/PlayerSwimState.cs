using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerSwimState : PlayerMovableState {

		private bool	isSubmerged;
		private int		submergedTimer;
		private int		submergedDuration;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwimState() {
			submergedDuration	= 128;
			isSubmerged			= false;
			submergedTimer		= 0;
			
			// Movement settings.
			allowMovementControl	= true;
			moveSpeed				= 0.5f;
			moveSpeedScale			= 1.0f;
			isSlippery				= true;
			acceleration			= 0.08f;
			deceleration			= 0.05f;
			minSpeed				= 0.05f;
			autoAccelerate			= false;
			directionSnapCount		= 8;
		}
		
		
		//-----------------------------------------------------------------------------
		// Internal methods
		//-----------------------------------------------------------------------------

		public void CheckTiles() {
			Point2I origin = (Point2I) player.Position - new Point2I(0, 2);
			Point2I location = origin / new Point2I(GameSettings.TILE_SIZE, GameSettings.TILE_SIZE);
			if (!player.RoomControl.IsTileInBounds(location))
				return;

			for (int i = 0; i < player.RoomControl.Room.LayerCount; i++) {
				Tile tile = player.RoomControl.GetTile(location, i);
				if (tile != null) {
					if (!tile.Flags.HasFlag(TileFlags.Water)) {
						player.BeginState(player.NormalState);
						return;
					}
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			isSubmerged		= false;
			moveSpeedScale	= 1.0f;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SWIM);
		}
		
		public override void OnEnd() {
			isSubmerged = false;
			base.OnEnd();
		}

		public override void Update() {
			// Slow down movement over time from strokes
			if (moveSpeedScale > 1.0f)
				moveSpeedScale -= 0.025f;
			
			// Stroking scales the movement speed.
			if (moveSpeedScale <= 1.4f && Controls.A.IsPressed()) {
				//Sounds.PLAYER_SWIM.play();
				moveSpeedScale = 2.0f;
			}

			// Auto accelerate during the beginning of a stroke.
			autoAccelerate = IsStroking;

			// Update the submerge state.
			if (isSubmerged) {
				submergedTimer--;
				if (submergedTimer <= 0 || Controls.B.IsPressed()) {
					isSubmerged = false;
					player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SWIM);
				}
			}
			else if (Controls.B.IsPressed()) {
				isSubmerged = true;
				submergedTimer = submergedDuration;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DIVE);

				// Create a splash effect.
				Effect splash = new Effect(GameData.ANIM_EFFECT_WATER_SPLASH);
				splash.Position = player.Position - new Vector2F(0, 4);
				player.RoomControl.SpawnEntity(splash);

				//Sounds.PLAYER_WADE.play();

				// TODO: Change player depth to lowest.
			}

			base.Update();
			
			// Check for ledge jumping (ledges/waterfalls)
			CollisionInfo collisionInfo = player.Physics.CollisionInfo[player.Direction];
			if (isMoving && collisionInfo.Type == CollisionType.Tile && !collisionInfo.Tile.IsMoving) {
				Tile tile = collisionInfo.Tile;
				
				// TODO: Code duplication. (here and in normal state)
				if (tile.Flags.HasFlag(TileFlags.Ledge) &&
					player.Direction == tile.LedgeDirection &&
					collisionInfo.Direction == tile.LedgeDirection)
				{
					// Ledge jump!
					player.LedgeJumpState.LedgeBeginTile = tile;
					player.BeginState(player.LedgeJumpState);
					return;
				}
			}

			CheckTiles();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		// This is the threshhold of movement speed scale to be considered stroking.
		public bool IsStroking {
			get { return (moveSpeedScale > 1.3f); }
		}

	}
}
