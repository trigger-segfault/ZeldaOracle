using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Projectiles {
	public class Boomerang : Projectile {

		private bool isReturning;
		private int timer;
		protected float speed;
		protected int returnDelay;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Boomerang() {
			speed		= 1.5f;
			returnDelay	= 40;

			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 2);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 2);
			Physics.Enable(
				PhysicsFlags.CollideWorld |
				PhysicsFlags.LedgePassable |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.CollideRoomEdge);
			
			// Graphics.
			Graphics.DepthLayer			= DepthLayer.ProjectileBoomerang;
			Graphics.IsShadowVisible	= false;
		}

		
		//-----------------------------------------------------------------------------
		// Boomerang Methods
		//-----------------------------------------------------------------------------
		
		// Occurs when the boomerang has returned to its owner.
		protected virtual void OnReturnedToOwner() { }

		public void BeginReturn() {
			if (!isReturning) {
				isReturning					= true;
				physics.CollideWithWorld	= false;
				physics.CollideWithRoomEdge	= false;
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			isReturning			= false;
			timer				= 0;
			physics.Velocity	= Angles.ToVector(angle) * speed;
		}

		public override void Intercept() {
			BeginReturn();
		}

		public override void OnCollideRoomEdge() {
			BeginReturn();
		}

		public override void OnCollideTile(Tile tile, bool isInitialCollision) {
			// Create cling effect.
			RoomControl.SpawnEntity(new EffectCling(), position, zPosition);
			AudioSystem.PlaySound(GameData.SOUND_EFFECT_CLING);
			BeginReturn();
		}

		public override void Update() {
			AudioSystem.LoopSoundWhileActive(GameData.SOUND_BOOMERANG_LOOP);

			if (isReturning) {
				// Return to owner.
				Vector2F trajectory = owner.Center - Center;
				if (trajectory.Length <= speed) {
					OnReturnedToOwner();
					Destroy();
				}
				else {
					physics.Velocity = trajectory.Normalized * speed;
				}
			}
			else {
				// Update return timer.
				timer++;
				if (timer > returnDelay)
					BeginReturn();
			}

			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsReturning {
			get { return isReturning; }
		}

		public float Speed {
			get { return speed; }
		}
	}
}
