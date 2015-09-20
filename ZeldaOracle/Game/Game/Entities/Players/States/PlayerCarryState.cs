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
	public class PlayerCarryState : PlayerState {
		
		private Tile carryTile;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerCarryState(Tile carryTile) {
			this.carryTile = carryTile;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();

			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_CARRY);
		}
		
		public override void OnEnd() {
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
			
			// Check for button press to throw/drop.
			if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
				
				Projectile projectile = new Projectile();
				
				// General
				projectile.Owner		= Player;
				projectile.Position		= Player.Position;
				projectile.ZPosition	= player.ZPosition + 12;
				projectile.Angle		= Directions.ToAngle(player.MoveDirection);
				if (player.Movement.IsMoving)
					projectile.Physics.Velocity = Directions.ToVector(Player.MoveDirection) * 1.5f;

				player.Direction = player.MoveDirection;

				// Graphics.
				projectile.Graphics.PlaySprite(carryTile.SpriteAsObject);
				projectile.Graphics.DrawOffset = new Point2I(-8, -14);

				// Physics.
				projectile.Physics.HasGravity	= true;
				projectile.Physics.CollisionBox	= new Rectangle2F(-2, -2, 4, 4);
				projectile.EnablePhysics(PhysicsFlags.CollideWorld | PhysicsFlags.LedgePassable |
									PhysicsFlags.HalfSolidPassable | PhysicsFlags.DestroyedOutsideRoom);

				/*
				// Crash event.
				Vector2F v = projectile.Physics.Velocity;
				projectile.EventCollision += delegate() {
					// Create crash effect.
					Effect effect = new Effect();
					effect.Position = projectile.Position;
					effect.CreateDestroyTimer(32);
					
					effect.Physics.Velocity		= -(v.Normalized) * 0.25f;
					effect.Physics.ZVelocity	= 1;
					effect.Physics.Gravity		= 0.07f;
					effect.EnablePhysics(PhysicsFlags.HasGravity);
					
					effect.Graphics.IsShadowVisible = false;
					effect.Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW_CRASH);

					RoomControl.SpawnEntity(effect);
					projectile.Destroy();
				};
				*/

				player.RoomControl.SpawnEntity(projectile);

				player.BeginNormalState();
			}
		}
		
		public override void DrawOver(Graphics2D g) {
			if (carryTile.SpriteAsObject != null) {
				g.DrawSprite(carryTile.SpriteAsObject, player.Position - new Point2I(8, 32));
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
