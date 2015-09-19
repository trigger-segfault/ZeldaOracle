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
	public class PlayerNormalState : PlayerState {
		
		private int pushTimer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerNormalState() {
			isNaturalState	= true;
			pushTimer		= 0;
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


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			pushTimer = 0;
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_DEFAULT);
		}
		
		public override void OnEnd() {
			pushTimer = 0;
			Player.Graphics.StopAnimation();
			base.OnEnd();
		}

		public override void Update() {
			base.Update();

			// Update animations
			if (player.Movement.IsMoving && !Player.Graphics.IsAnimationPlaying)
				Player.Graphics.PlayAnimation();
			if (!player.Movement.IsMoving && Player.Graphics.IsAnimationPlaying)
				Player.Graphics.StopAnimation();
			
			// Update pushing.
			CollisionInfo collisionInfo = player.Physics.CollisionInfo[player.Direction];

			if (player.Movement.IsMoving && collisionInfo.Type == CollisionType.Tile && !collisionInfo.Tile.IsMoving) {
				Tile tile = collisionInfo.Tile;
				player.Graphics.AnimationPlayer.Animation = GameData.ANIM_PLAYER_PUSH;
				pushTimer++;

				if (pushTimer > 20 && tile.Flags.HasFlag(TileFlags.Movable)) {
					tile.Push(player.Direction, 1.0f);
					//Message message = new Message("Oof! It's heavy!");
					//player.RoomControl.GameManager.PushGameState(new StateTextReader(message));
					pushTimer = 0;
				}
			}
			else {
				pushTimer = 0;
				player.Graphics.AnimationPlayer.Animation = GameData.ANIM_PLAYER_DEFAULT;
			}
			
			
			// Check for tile interactions (like signs).
			//player.Physics.
			//player.Physics.IsPlaceMeetingSolid

			// Update items.
			Player.UpdateEquippedItems();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
