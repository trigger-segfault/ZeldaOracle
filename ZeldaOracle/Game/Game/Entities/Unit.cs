using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;

namespace ZeldaOracle.Game.Entities {

	// NOTE: These will probably be changed
	[Flags]
	public enum UnitFlags {
		Hurt,
		Bumpable,
		InGrass,
		Passable,
		FallInHoles,
	}

	
	public class Unit : Entity {
		
		// The direction the unit is facing.
		protected int direction;

		protected int health;
		protected int healthMax;

		protected int		knockbackDuration;
		protected int		hurtInvincibleDuration;
		protected int		hurtFlickerDuration;

		private int			knockbackTimer;
		private int			invincibleTimer;
		private int			hurtFlickerTimer;
		private Vector2F	knockbackVelocity;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Unit() {
			EnablePhysics();
			
			knockbackDuration		= GameSettings.UNIT_KNOCKBACK_DURATION;
			hurtInvincibleDuration	= GameSettings.UNIT_HURT_INVINCIBLE_DURATION;
			hurtFlickerDuration		= GameSettings.UNIT_HURT_FLICKER_DURATION;

			knockbackTimer			= 0;
			hurtFlickerTimer		= 0;
			invincibleTimer			= 0;
			knockbackVelocity		= Vector2F.Zero;

			health			= 1;
			healthMax		= 1;
			direction		= Directions.Right;
			centerOffset	= new Point2I(8, 8);
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------
		
		public void Hurt(int damage) {
			Hurt(new DamageInfo(damage));
		}
		
		public void Hurt(int damage, Vector2F sourcePosition) {
			Hurt(new DamageInfo(damage, sourcePosition));
		}

		public void Hurt(DamageInfo damage) {
			if (IsInvincible)
				return;

			// Knockback.
			if (damage.ApplyKnockBack) {
				Vector2F knockbackVelocity = Vector2F.Zero;
				if (damage.HasSource) {
					knockbackVelocity = (Center - damage.SourcePosition).Normalized;
					knockbackVelocity *= GameSettings.UNIT_KNOCKBACK_SPEED;
				
					// Snap velocity direction.
					float snapInterval = ((float) GMath.Pi * 2.0f) / GameSettings.UNIT_KNOCKBACK_ANGLE_SNAP_COUNT;
					float theta = (float) Math.Atan2(-knockbackVelocity.Y, knockbackVelocity.X);
					if (theta < 0)
						theta += (float) Math.PI * 2.0f;
					int angle = (int) ((theta / snapInterval) + 0.5f);
					knockbackVelocity = new Vector2F(
						(float) Math.Cos(angle * snapInterval) * knockbackVelocity.Length,
						(float) -Math.Sin(angle * snapInterval) * knockbackVelocity.Length);
				}
				Knockback(knockbackVelocity, knockbackDuration);
			}

			// Damage.
			health				= GMath.Max(0, health - damage.Amount);
			invincibleTimer		= hurtInvincibleDuration;
			hurtFlickerTimer	= hurtFlickerDuration;
			graphics.IsHurting	= true;
		}

		public void Knockback(Vector2F velocity, int duration) {
			knockbackVelocity	= velocity;
			knockbackDuration	= duration;
			knockbackTimer		= duration;
		}

		public virtual void RespawnDeath() {
			
		}

		public virtual void Death() {

		}

		public virtual void Die() {
			Destroy();
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
		}

		public override void Update() {
			
			// Update knockback.
			if (knockbackTimer > 0)
				knockbackTimer--;
			if (IsBeingKnockedBack)
				Physics.Velocity += knockbackVelocity;
			
			// Update hurt flickering.
			if (hurtFlickerTimer > 0)
				hurtFlickerTimer--;
			Graphics.IsHurting = IsHurtFlickering;
			
			// Update invinciblity timer.
			if (invincibleTimer > 0) {
				invincibleTimer--;
				if (invincibleTimer == 0) {
					if (health == 0) {
						Die();
					}
				}
			}

			base.Update();
		}

		public override void Draw(Graphics2D g) {
			base.Draw(g);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Health {
			get { return health; }
			set { health = GMath.Clamp(value, 0, healthMax); }
		}
		
		public int MaxHealth {
			get { return healthMax; }
			set { healthMax = GMath.Max(value, 0); }
		}

		public bool IsBeingKnockedBack {
			get { return (knockbackTimer > 0); }
		}
		
		public bool IsInvincible {
			get { return (invincibleTimer > 0); }
		}
		
		public bool IsHurtFlickering {
			get { return (hurtFlickerTimer > 0); }
		}
	}
}
