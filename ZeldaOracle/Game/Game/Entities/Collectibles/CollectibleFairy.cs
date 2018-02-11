using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.Entities {
	public class CollectibleFairy : Collectible {

		private float directionSpeed;
		private float direction;
		private float moveSpeed;
		private float maxMoveSpeed;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CollectibleFairy() {
			maxMoveSpeed	= 0.8f;
			aliveDuration	= GameSettings.COLLECTIBLE_FAIRY_ALIVE_DURATION;
			fadeDelay		= GameSettings.COLLECTIBLE_FAIRY_FADE_DELAY;

			// Physics.
			Physics.CollisionBox		= new Rectangle2I(-4, -9, 8, 8);
			Physics.SoftCollisionBox	= new Rectangle2I(-5, -9, 9, 8);
			EnablePhysics(PhysicsFlags.ReboundRoomEdge);

			// Graphics.
			Graphics.DepthLayer	= DepthLayer.InAirCollectibles;
			Graphics.DrawOffset	= new Point2I(0, -5);
			centerOffset		= new Point2I(0, -5);
		}


		//-----------------------------------------------------------------------------
		// Collection
		//-----------------------------------------------------------------------------

		public override void Collect() {
			if (GameControl.HUD.DynamicHealth >= GameControl.Player.MaxHealth)
				AudioSystem.PlaySound(GameData.SOUND_GET_HEART);
			RoomControl.Player.Health += 6 * 4;
			base.Collect();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			
			directionSpeed = 0.0f;

			Graphics.PlayAnimation(GameData.ANIM_COLLECTIBLE_FAIRY);
		}

		public override void Update() {
			// Adjust Z-position and velocity to hover at a certain height.
			if (Math.Abs(zPosition - GameSettings.COLLECTIBLE_FAIRY_HOVER_HEIGHT) > 2.0f) {
				if (zPosition < GameSettings.COLLECTIBLE_FAIRY_HOVER_HEIGHT)
					Physics.ZVelocity = Math.Min(0.5f, Physics.ZVelocity + 0.05f);
				if (zPosition > GameSettings.COLLECTIBLE_FAIRY_HOVER_HEIGHT)
					Physics.ZVelocity = Math.Max(-0.5f, Physics.ZVelocity - GameSettings.DEFAULT_GRAVITY);
			}
			else {
				Physics.ZVelocity = 0.0f;
				zPosition = GameSettings.COLLECTIBLE_FAIRY_HOVER_HEIGHT;
			}
			
			// Adjust move direction.
			moveSpeed = physics.Velocity.Length;
			if (moveSpeed == 0.0f)
				direction = GRandom.NextFloat(GMath.FullAngle);
			else 
				direction = Physics.Velocity.Direction;

			// Adjust velocity.
			if (Math.Abs(moveSpeed - maxMoveSpeed) > 0.01f) {
				moveSpeed += 0.04f * Math.Sign(maxMoveSpeed - moveSpeed);
			}
			else
				moveSpeed = maxMoveSpeed;

			// Update random motion.
			directionSpeed += 0.05f * GRandom.NextFloat(-30.0f, 30.0f);
			directionSpeed = GMath.Clamp(directionSpeed, -6.0f, 6.0f);
			direction += directionSpeed;
			physics.Velocity = Vector2F.FromPolar(moveSpeed, direction);

			// Syncronize facing direction with velocity.
			if (Physics.Velocity.X > 0.0f)
				Graphics.SubStripIndex = 0;
			else if (Physics.Velocity.X < 0.0f)
				Graphics.SubStripIndex = 1;
			
			base.Update();
		}
	}
}
