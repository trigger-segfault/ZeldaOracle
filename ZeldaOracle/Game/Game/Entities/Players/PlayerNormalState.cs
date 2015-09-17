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
	public class PlayerNormalState : PlayerMovableState {
		
		private int		pushTimer;
		private bool	isOnIce;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerNormalState() {
			pushTimer	= 0;
			isOnIce		= false;
		}
		
		
		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------

		public void Jump() {
			if (player.IsOnGround) {
				player.Physics.ZVelocity = GameSettings.PLAYER_JUMP_SPEED;
				player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_JUMP);
				player.BeginState(new PlayerJumpState());
			}
		}

		public void CheckTiles() {
			isOnIce = false;
			moveSpeedScale = 1.0f;

			Point2I origin = (Point2I) player.Position - new Point2I(0, 2);
			Point2I location = origin / new Point2I(GameSettings.TILE_SIZE, GameSettings.TILE_SIZE);
			if (!player.RoomControl.IsTileInBounds(location))
				return;

			for (int i = 0; i < player.RoomControl.Room.LayerCount; i++) {
				Tile tile = player.RoomControl.GetTile(location, i);
				if (tile != null) {
					
					if (tile.Flags.HasFlag(TileFlags.Stairs)) {
						moveSpeedScale = 0.5f;
					}
					if (tile.Flags.HasFlag(TileFlags.Ice)) {
						isOnIce = true;
					}
					if (tile.Flags.HasFlag(TileFlags.Ladder)) {
						player.BeginState(player.LadderState);
					}
				}
			}

			isSlippery = isOnIce;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			pushTimer	= 0;
			isOnIce		= false;
			
			// Movement settings.
			allowMovementControl	= true;
			moveSpeed				= 1.0f;
			moveSpeedScale			= 1.0f;
			isSlippery				= false;
			acceleration			= 0.1f;
			deceleration			= 0.05f;
			minSpeed				= 0.05f;
			autoAccelerate			= false;
			directionSnapCount		= 32;

			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
		}
		
		public override void OnEnd() {
			pushTimer		= 0;
			Player.Graphics.StopAnimation();
			base.OnEnd();
		}

		public override void Update() {
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
				else {
					player.Graphics.AnimationPlayer.Animation = GameData.ANIM_PLAYER_PUSH;
					pushTimer++;

					if (pushTimer > 20 && tile.Flags.HasFlag(TileFlags.Movable)) {
						tile.Push(player.Direction, 1.0f);
						//Message message = new Message("Oof! It's heavy!");
						//player.RoomControl.GameManager.PushGameState(new StateTextReader(message));
						pushTimer = 0;
					}
				}
			}
			else {
				pushTimer = 0;
				player.Graphics.AnimationPlayer.Animation = GameData.ANIM_PLAYER_DEFAULT;
			}
			
			// Update items.
			Player.UpdateEquippedItems();

			//player.Physics.SetFlags(PhysicsFlags.CollideRoomEdge, !isMoving || player.IsInAir);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
