using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Units {

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
		
		/// <summary>List of tools that the unit is carrying/holding.</summary>
		private HashSet<UnitTool> tools;
		
		/// <summary>The direction the unit is facing.</summary>
		protected Direction	direction;

		protected bool		syncAnimationWithDirection;
		protected int		health;
		protected int		healthMax;
		protected bool		isKnockbackable; // Can the unit be knocked back?
		protected bool		isDamageable;
		protected bool		isPassable; // True if cannot touch other entities like projectiles or monsters/player
		protected float		knockbackSpeed;
		protected int		hurtInvincibleDuration;
		protected int		hurtFlickerDuration;
		
		// Internal state
		protected int		knockbackDuration;
		protected int		hurtKnockbackDuration;
		protected int		bumpKnockbackDuration;
		private int			knockbackTimer;
		private int			invincibleTimer;
		private int			hurtFlickerTimer;
		private Vector2F	knockbackVelocity;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public Unit() {
			Physics.Enable();

			isKnockbackable			= true;
			isDamageable			= true;
			isPassable				= false;

			knockbackSpeed			= GameSettings.UNIT_KNOCKBACK_SPEED;
			knockbackDuration		= GameSettings.UNIT_KNOCKBACK_DURATION;
			hurtInvincibleDuration	= GameSettings.UNIT_HURT_INVINCIBLE_DURATION;
			hurtFlickerDuration		= GameSettings.UNIT_HURT_FLICKER_DURATION;

			knockbackTimer			= 0;
			hurtFlickerTimer		= 0;
			invincibleTimer			= 0;
			knockbackVelocity		= Vector2F.Zero;
			tools					= new HashSet<UnitTool>();
			
			direction					= Direction.Right;
			syncAnimationWithDirection	= false;

			health			= 1;
			healthMax		= 1;
			direction		= Direction.Right;
			centerOffset	= new Point2I(8, 8);
			
			Graphics.IsShadowVisible		= true;
			Graphics.IsGrassEffectVisible	= true;
			Graphics.IsRipplesEffectVisible	= true;

		}


		//-----------------------------------------------------------------------------
		// Tools
		//-----------------------------------------------------------------------------

		public bool IsToolEquipped(UnitTool tool) {
			return tools.Contains(tool);
		}

		public void EquipTool(UnitTool tool) {
			if (tools.Add(tool)) {
				tool.Unit = this;
				tool.IsEquipped = true;
				AttachEntity(tool);
				tool.OnEquip();
			}
		}
		
		public void UnequipTool(UnitTool tool) {
			if (tools.Remove(tool)) {
				tool.IsEquipped = false;
				tool.OnUnequip();
				DetachEntity(tool);
				tool.Destroy();
			}
		}


		//-----------------------------------------------------------------------------
		// Projectiles
		//-----------------------------------------------------------------------------
		
		public Projectile ShootFromDirection(
			Projectile projectile, int direction, float speed)
		{
			return ShootFromDirection(projectile, direction, speed, Vector2F.Zero, 0);
		}

		public Projectile ShootFromAngle(
			Projectile projectile, int angle,float speed)
		{
			return ShootFromAngle(projectile, angle, speed, Vector2F.Zero, 0);
		}

		public Projectile ShootProjectile(Projectile projectile, Vector2F velocity) {
			return ShootProjectile(projectile, velocity, Vector2F.Zero, 0);
		}
		
		public Projectile ShootFromDirection(Projectile projectile,
			Direction direction, float speed, Vector2F positionOffset,
			int zPositionOffset = 0)
		{
			projectile.Owner		= this;
			projectile.Direction	= direction;
			projectile.Physics.Velocity	= direction.ToVector(speed);
			RoomControl.SpawnEntity(projectile,
				Center + positionOffset, zPosition + zPositionOffset);
			return projectile;
		}
		
		public Projectile ShootFromAngle(Projectile projectile, Angle angle,
			float speed, Vector2F positionOffset, int zPositionOffset = 0)
		{
			projectile.Owner	= this;
			projectile.Angle	= angle;
			projectile.Physics.Velocity	= angle.ToVector(speed);
			RoomControl.SpawnEntity(projectile,
				Center + positionOffset, zPosition + zPositionOffset);
			return projectile;
		}

		public Projectile ShootProjectile(Projectile projectile, Vector2F velocity,
			Vector2F positionOffset, int zPositionOffset = 0)
		{
			projectile.Owner		= this;
			projectile.Direction	= direction;
			projectile.Physics.Velocity	= velocity;
			RoomControl.SpawnEntity(projectile,
				Center + positionOffset, zPosition + zPositionOffset);
			return projectile;
		}


		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------
		
		public virtual void OnDie() {}

		public virtual void OnHurt(DamageInfo damage) { }
		
		public void Kill() {
			if (!IsInvincible)
				Die();
		}

		public virtual void Knockback(int duration, float speed, Vector2F sourcePosition) {
			if (isKnockbackable) {
				knockbackDuration	= duration;
				knockbackTimer		= duration;
				knockbackVelocity	= (Center - sourcePosition).Normalized;
				knockbackVelocity  *= speed;
				knockbackVelocity	= Vector2F.SnapDirectionByCount(
					knockbackVelocity, GameSettings.UNIT_KNOCKBACK_ANGLE_SNAP_COUNT);
			}
		}
		
		public void Bump(Vector2F sourcePosition) {
			Knockback(bumpKnockbackDuration, knockbackSpeed, sourcePosition);
		}

		public void Hurt(int damage) {
			Hurt(new DamageInfo(damage));
		}
		
		public void Hurt(int damage, Vector2F sourcePosition) {
			Hurt(new DamageInfo(damage, sourcePosition));
		}

		public void Hurt(DamageInfo damage) {
			if (IsInvincible || !IsDamageable || IsBeingKnockedBack)
				return;

			// Apply knockback
			if (damage.ApplyKnockback) {
				Vector2F damageSourcePos = Center;
				int duration = hurtKnockbackDuration;
				if (damage.HasSource)
					damageSourcePos = damage.SourcePosition;
				if (damage.KnockbackDuration >= 0)
					duration = damage.KnockbackDuration;
				Knockback(duration, knockbackSpeed, damageSourcePos);
			}

			// Apply damage
			if (damage.Amount > 0) {
				health				= GMath.Max(0, health - damage.Amount);
				invincibleTimer		= hurtInvincibleDuration;
				if (damage.Flicker) {
					hurtFlickerTimer = hurtFlickerDuration;
					graphics.IsHurting = true;
				}
			}

			if (damage.InvincibleDuration > 0)
				invincibleTimer = damage.InvincibleDuration;

			OnHurt(damage);
		}

		public void GiveInvincibility(int duration) {
			invincibleTimer = duration;
		}

		public virtual void Die() {
			OnDie();
			Destroy();
		}

		public virtual void OnKnockbackEnd() {
			if (IsOnGround)
				physics.Velocity = Vector2F.Zero;
		}

		public virtual void UpdateSubStripIndex() {
			if (syncAnimationWithDirection)
				Graphics.SubStripIndex = direction;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
		}

		public override void Update() {
			
			// Update knockback
			if (knockbackTimer > 0) {
				knockbackTimer--;
				if (knockbackTimer == 0)
					OnKnockbackEnd();
			}
			if (IsBeingKnockedBack)
				Physics.Velocity = knockbackVelocity; // TODO: player can move while being knocked back.
			
			// Update hurt flickering
			if (hurtFlickerTimer > 0)
				hurtFlickerTimer--;
			Graphics.IsHurting = IsHurtFlickering;
			
			// Update invinciblity timer
			if (invincibleTimer > 0) {
				invincibleTimer--;
				if (invincibleTimer == 0) {
					if (health == 0) {
						Die();
					}
				}
			}

			// Update tools
			//foreach (UnitTool tool in tools)
				//tool.Update();
			
			UpdateSubStripIndex();

			// TODO: Remove Is-Passable
			Interactions.IsEnabled = !isPassable;

			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			DepthLayer depthLayer = Graphics.CurrentDepthLayer;

			// Draw tools under.
			//foreach (UnitTool tool in tools) {
			//	if (!tool.DrawAboveUnit) {
			//		Vector2F drawPosition = DrawPosition - new Vector2F(0, zPosition) + Graphics.DrawOffset + tool.DrawOffset;
			//		g.DrawAnimationPlayer(tool.AnimationPlayer, drawPosition, depthLayer, position);
			//	}
			//}

			// Draw entity.
			base.Draw(g);

			// Draw tools over.
			//foreach (UnitTool tool in tools) {
			//	if (tool.DrawAboveUnit) {
			//		Vector2F drawPosition = DrawPosition - new Vector2F(0, zPosition) + Graphics.DrawOffset + tool.DrawOffset;
			//		g.DrawAnimationPlayer(tool.AnimationPlayer, drawPosition, depthLayer, position);
			//	}
			//}
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

		public Vector2F KnockbackVelocity {
			get { return knockbackVelocity; }
			set { knockbackVelocity = value; }
		}

		public bool IsKnockbackable {
			get { return isKnockbackable; }
			set { isKnockbackable = value; }
		}

		public bool IsPassable {
			get { return isPassable; }
			set { isPassable = value; }
		}

		public bool IsBeingKnockedBack {
			get { return (knockbackTimer > 0); }
		}
		
		public bool IsDamageable {
			get { return isDamageable; }
			set { isDamageable = value; }
		}

		public bool IsInvincible {
			get { return (invincibleTimer > 0); }
		}
		
		public bool IsHurtFlickering {
			get { return (hurtFlickerTimer > 0); }
		}
		
		public Direction Direction {
			get { return direction; }
			set {
				direction = value;
				if (syncAnimationWithDirection)
					graphics.SubStripIndex = direction;
			}
		}
		
		public bool SyncAnimationWithDirection {
			get { return syncAnimationWithDirection; }
			set { syncAnimationWithDirection = value; }
		}
		
		public HashSet<UnitTool> EquippedTools {
			get { return tools; }
		}
	}
}
