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
		
		protected float		knockbackSpeed;
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
			
			knockbackSpeed			= GameSettings.UNIT_KNOCKBACK_SPEED;
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
					knockbackVelocity *= knockbackSpeed;
					knockbackVelocity = Vector2F.SnapDirectionByCount(
						knockbackVelocity, GameSettings.UNIT_KNOCKBACK_ANGLE_SNAP_COUNT);
				}
				Knockback(knockbackVelocity, knockbackDuration);
			}

			// Damage.
			if (damage.Amount > 0) {
				health				= GMath.Max(0, health - damage.Amount);
				invincibleTimer		= hurtInvincibleDuration;
				hurtFlickerTimer	= hurtFlickerDuration;
				graphics.IsHurting	= true;
			}
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


		public virtual void OnKnockbackEnd() {
			physics.Velocity = Vector2F.Zero;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
		}

		public override void Update() {
			
			// Update knockback.
			if (knockbackTimer > 0) {
				knockbackTimer--;
				if (knockbackTimer == 0)
					OnKnockbackEnd();
			}
			if (IsBeingKnockedBack)
				Physics.Velocity = knockbackVelocity; // TODO: player can move while being knocked back.
			
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

		public bool IsAtFullHealth {
			get { return (health == healthMax); }
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
