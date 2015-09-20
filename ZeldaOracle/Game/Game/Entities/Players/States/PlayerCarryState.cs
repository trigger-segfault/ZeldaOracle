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
		private bool isPickingUp; // Is the pickup animation playing?
		private int pickupTimer;
		private Point2I objectDrawOffset;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerCarryState(Tile carryTile) {
			this.carryTile = carryTile;
		}

		
		//-----------------------------------------------------------------------------
		// Internal methods.
		//-----------------------------------------------------------------------------
		
		public Entity DropObject() {
			Projectile projectile = new Projectile();
			
			Vector2F pos = player.Position - new Point2I(8, 32 - 3);

			// General
			projectile.Owner		= Player;
			projectile.OriginOffset = new Point2I(8, 14);
			projectile.Origin		= Player.Origin + new Vector2F(0, 1);
			projectile.ZPosition	= player.ZPosition + 12;
			projectile.Angle		= Directions.ToAngle(player.MoveDirection);

			player.Direction = player.MoveDirection;

			// Graphics.
			projectile.Graphics.PlaySprite(carryTile.SpriteAsObject);
			//projectile.Graphics.DrawOffset = new Point2I(-8, -14);
			projectile.Graphics.ShadowDrawOffset = projectile.OriginOffset;

			// Physics.
			projectile.Physics.HasGravity		= true;
			projectile.Physics.CollisionBox		= new Rectangle2F(-2, -2, 4, 4) + new Vector2F(8, 8);
			projectile.Physics.SoftCollisionBox	= new Rectangle2F(-2, -2, 4, 4) + new Vector2F(8, 8);
			projectile.EnablePhysics(PhysicsFlags.CollideWorld | PhysicsFlags.LedgePassable |
								PhysicsFlags.HalfSolidPassable | PhysicsFlags.DestroyedOutsideRoom);

			// Crash event.
			Vector2F v = projectile.Physics.Velocity;
			Action landAction = delegate() {
				// Create crash effect.
				Effect effect = new Effect(GameData.ANIM_EFFECT_SIGN_BREAK);
				effect.Position = projectile.Position;
				projectile.RoomControl.SpawnEntity(effect);
				projectile.Destroy();
			};
			projectile.EventCollision += landAction;
			projectile.EventLand += landAction;

			player.RoomControl.SpawnEntity(projectile);
				

			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
			player.BeginState(new PlayerBusyState(4));

			return projectile;
		}

		public Entity ThrowObject() {
			Entity obj = DropObject();
			obj.Physics.ZVelocity = 1.0f;
			obj.Physics.Velocity = Directions.ToVector(Player.MoveDirection) * 1.5f;
			return obj;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			base.OnBegin();
			
			objectDrawOffset = new Point2I(-8, -16);
			objectDrawOffset += Directions.ToPoint(player.Direction) * 8;
			pickupTimer = 0;
			isPickingUp = true;
			player.Movement.AllowMovementControl = false;
			Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
		}
		
		public override void OnEnd() {
			Player.Graphics.StopAnimation();

			base.OnEnd();
		}
		
		public override bool RequestStateChange(PlayerState newState) {
			if (newState is PlayerSwimState || newState is PlayerLadderState) {
				DropObject();
				return true;
			}
			return base.RequestStateChange(newState);
		}

		public override void Update() {
			base.Update();

			if (isPickingUp) {
				pickupTimer++;
				
				// Handle the 2 frames of picking up.
				if (pickupTimer > 10) {
					objectDrawOffset = new Point2I(-8, -29);
					isPickingUp = false;
					player.Movement.AllowMovementControl = true;
					Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_CARRY);
				}
				else if (pickupTimer > 6) {
					objectDrawOffset = new Point2I(-8, -24);
					objectDrawOffset += Directions.ToPoint(player.Direction) * 2;
					Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_GRAB);
				}
				else {
					objectDrawOffset = new Point2I(-8, -16);
					objectDrawOffset += Directions.ToPoint(player.Direction) * 8;
					Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_PULL);
				}
			}
			else {
				// Update animations
				if (player.Movement.IsMoving && !Player.Graphics.IsAnimationPlaying)
					Player.Graphics.PlayAnimation();
				if (!player.Movement.IsMoving && Player.Graphics.IsAnimationPlaying)
					Player.Graphics.StopAnimation();
			
				// Check for button press to throw/drop.
				if (Controls.A.IsPressed() || Controls.B.IsPressed()) {
					if (player.Movement.IsMoving)
						ThrowObject();
					else
						DropObject();
				}
			}
		}
		
		public override void DrawOver(Graphics2D g) {
			// Draw the object.
			if (carryTile.SpriteAsObject != null) {
				Vector2F pos = player.Position + objectDrawOffset;

				// Handle head bobbing when the player is moving horizontally.
				if (!isPickingUp &&
					(player.Direction == Directions.Left || player.Direction == Directions.Right)
						&& player.Graphics.AnimationPlayer.PlaybackTime < 6)
				{
					pos.Y -= 1;
				}

				g.DrawSprite(carryTile.SpriteAsObject, pos);
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

	}
}
