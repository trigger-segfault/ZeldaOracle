using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Players.States {
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
		// Interaction
		//-----------------------------------------------------------------------------

		public void StopPushing() {
			pushTimer = 0;
			player.Graphics.Animation = player.MoveAnimation;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override bool RequestStateChange(PlayerState newState) {
			return true;
		}

		public override void OnBegin(PlayerState previousState) {
			pushTimer = 0;
			Player.Graphics.PlayAnimation(player.MoveAnimation);
		}
		
		public override void OnEnd(PlayerState newState) {
			pushTimer = 0;
		}

		public override void Update() {
			base.Update();

			if (player.IsInAir) {
				// TODO: play jump animation at remembering the frame.
			}
			else {
				// Update pushing.
				Tile actionTile = player.Physics.GetMeetingSolidTile(player.Position, player.Direction);
				CollisionInfo collisionInfo = player.Physics.CollisionInfo[player.Direction];
				
				if (player.Movement.CanPush && actionTile != null && player.Movement.IsMoving &&
					collisionInfo.Type == CollisionType.Tile && !collisionInfo.Tile.IsMoving)
				{
					player.Graphics.Animation = GameData.ANIM_PLAYER_PUSH;
					pushTimer++;

					if (pushTimer > actionTile.PushDelay && actionTile.HasFlag(TileSpecialFlags.Movable)) {
						if (actionTile.OnPush(player.Direction, 1.0f))
							pushTimer = 0;
						else
							actionTile.OnPushing(player.Direction);
					}
					else {
						actionTile.OnPushing(player.Direction);
					}
				}
				else {
					pushTimer = 0;
					player.Graphics.Animation = player.MoveAnimation;
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
