using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {

	public class PlayerPushState : PlayerState {

		private int pushTimer;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerPushState() {
			StateParameters.PlayerAnimations.Default = GameData.ANIM_PLAYER_PUSH;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void OnBegin(PlayerState previousState) {
			pushTimer = 0;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PUSH);
		}
		
		public override void OnEnd(PlayerState newState) {
		}

		public override void Update() {
			base.Update();
			
			Tile actionTile = player.Physics.GetFacingSolidTile(player.Direction);
			Collision collision = player.Physics.GetCollisionInDirection(player.Direction);
			bool isColliding = (collision != null && collision.IsTile && 
				!collision.Tile.IsMoving && !collision.Tile.IsNotPushable);
			
			if (!player.IsInAir &&
				!player.StateParameters.ProhibitPushing &&
				actionTile != null &&
				player.Movement.IsMoving &&
				isColliding)
			{
				pushTimer++;

				if (pushTimer > actionTile.PushDelay) {
					if (actionTile.OnPush(player.Direction, player.PushSpeed))
						pushTimer = 0;
					else
						actionTile.OnPushing(player.Direction);
				}
				else {
					actionTile.OnPushing(player.Direction);
				}
			}
			else
				End();
		}
	}
}
