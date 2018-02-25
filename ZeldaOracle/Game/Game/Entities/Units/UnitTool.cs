using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;


namespace ZeldaOracle.Game.Entities.Units {

	public enum UnitToolType {
		Sword,
		Shield,
		Visual,
	}

	public class UnitTool {
		
		protected Unit			unit;
		protected bool			drawAboveUnit;
		protected bool			syncAnimationWithDirection;
		protected Rectangle2I	collisionBox;
		protected Point2I		drawOffset;
		protected UnitToolType	toolType;
		private AnimationPlayer	animationPlayer;
		private bool			isEquipped;
		private bool			isPhysicsEnabled;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public UnitTool() {
			unit				= null;
			drawAboveUnit		= false;
			animationPlayer		= new AnimationPlayer();
			collisionBox		= new Rectangle2I(-1, -1, 2, 2);
			toolType			= UnitToolType.Visual;
			isEquipped			= false;
			isPhysicsEnabled	= true;
			syncAnimationWithDirection	= true;
		}

		public void Initialize(Unit unit) {
			this.unit = unit;
			OnInitialize();
		}
		

		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------
		
		public virtual void OnParry(Unit other, Vector2F contactPoint) {
		}

		public virtual void OnHitMonster(Monster monster) {
		}
		
		public virtual void OnHitPlayer(Player player) {
		}
		
		public virtual void OnHitProjectile(Projectile projectile) {
		}

		public virtual void OnInitialize() {
		}

		public virtual void OnEquip() {
		}
		
		public virtual void OnUnequip() {
		}

		public virtual void Update() {
			animationPlayer.Update();
			if (syncAnimationWithDirection)
				animationPlayer.SubStripIndex = unit.Direction;
		}
		

		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------
		

		public void Enable() {
			isEquipped = true;
		}

		public void Disable() {
			isEquipped = false;
		}

		public void EnablePhysics() {
			isPhysicsEnabled = true;
		}

		public void EnablePhysics(Rectangle2I collisionBox) {
			this.collisionBox = collisionBox;
			isPhysicsEnabled = true;
		}

		public void DisablePhyscs() {
			isPhysicsEnabled = false;
		}


		//-----------------------------------------------------------------------------
		// Animation
		//-----------------------------------------------------------------------------

		public void PlayAnimation() {
			animationPlayer.Play();
		}

		public void PlayAnimation(Animation animation) {
			animationPlayer.Play(animation);
		}
		
		public void StopAnimation() {
			animationPlayer.Stop();
		}
		
		public void RemoveAnimation() {
			animationPlayer.Clear();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public AnimationPlayer AnimationPlayer {
			get { return animationPlayer; }
		}
		
		public bool DrawAboveUnit {
			get { return drawAboveUnit; }
			set { drawAboveUnit = value; }
		}

		public Rectangle2I CollisionBox {
			get { return collisionBox; }
			set { collisionBox = value; }
		}

		public Rectangle2F PositionedCollisionBox {
			get { return Rectangle2F.Translate(collisionBox, unit.Position); }
		}

		public Point2I DrawOffset {
			get { return drawOffset; }
			set { drawOffset = value; }
		}

		public UnitToolType ToolType {
			get { return toolType; }
			set { toolType = value; }
		}
		
		public bool IsEquipped {
			get { return isEquipped; }
			set { isEquipped = value; }
		}
		
		public Unit Unit {
			get { return unit; }
			set { unit = value; }
		}
		
		public bool IsPhysicsEnabled {
			get { return isPhysicsEnabled; }
			set { isPhysicsEnabled = value; }
		}
		
		public bool IsSword {
			get { return (toolType == UnitToolType.Sword); }
		}
		
		public bool IsShield {
			get { return (toolType == UnitToolType.Shield); }
		}

		public bool IsSwordOrShield {
			get { return (toolType == UnitToolType.Sword || toolType == UnitToolType.Shield); }
		}
	}
}
