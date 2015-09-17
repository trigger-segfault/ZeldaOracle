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
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players {
	public class PlayerLadderState : PlayerMovableState {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerLadderState() {
			moveSpeedScale = 0.5f;
		}
		
		
		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		public void CheckTiles() {
			bool onLadder = false;

			Point2I origin = (Point2I) player.Position - new Point2I(0, 2);
			Point2I location = origin / new Point2I(GameSettings.TILE_SIZE, GameSettings.TILE_SIZE);
			if (!player.RoomControl.IsTileInBounds(location))
				return;

			for (int i = 0; i < player.RoomControl.Room.LayerCount; i++) {
				Tile tile = player.RoomControl.GetTile(location, i);
				if (tile != null) {
					
					if (tile.Flags.HasFlag(TileFlags.Ladder)) {
						onLadder = true;
					}
				}
			}
			if (!onLadder) {
				player.BeginState(Player.NormalState);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			// Movement settings.
			allowMovementControl	= true;
			moveSpeed				= 0.5f;
			moveSpeedScale			= 1.0f;
			isSlippery				= false;
			acceleration			= 0.1f;
			deceleration			= 0.05f;
			minSpeed				= 0.05f;
			autoAccelerate			= false;
			directionSnapCount		= 16;
			strafing				= true;

			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
		}
		
		public override void OnEnd() {
			Player.Graphics.StopAnimation();
			strafing				= false;
			base.OnEnd();
		}

		public override void Update() {
			player.Direction = Directions.Up;
			player.Angle = Angles.Up;
			CheckTiles();
			if (!IsActive)
				return;

			base.Update();

			// Update animations
			if (isMoving && !Player.Graphics.IsAnimationPlaying)
				Player.Graphics.PlayAnimation();
			if (!isMoving && Player.Graphics.IsAnimationPlaying)
				Player.Graphics.StopAnimation();
			
			// Update pushing.
			CollisionInfo collisionInfo = player.Physics.CollisionInfo[player.Direction];

			if (isMoving && collisionInfo.Type == CollisionType.Tile && !collisionInfo.Tile.IsMoving) {
				Tile tile = collisionInfo.Tile;
				
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
			else {
				player.Graphics.AnimationPlayer.Animation = GameData.ANIM_PLAYER_DEFAULT;
			}
			
			// Update items.
			//Player.UpdateEquippedItems();
			// TODO: Handle holding sheild on ladder
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
