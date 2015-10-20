using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;

namespace ZeldaOracle.Game.Entities.Monsters {

	public enum InteractionType {
		None = -1,

		EmberSeed = 0,			// Hit by an ember seed.
		ScentSeed,				// Hit by a scent seed.
		PegasusSeed,			// Hit by a pegasus seed.
		GaleSeed,				// Hit by a gale seed.
		MysterySeed,			// Hit by a mystery seed.
		Fire,					// Touches fire.
		Arrow,					// Hit by an arrow.
		SwordBeam,				// Hit by a sword beam projectile.
		RodFire,				// Hit by a projectile from the fire-rod.
		Sword,					// Hit by a sword.
		BiggoronSword,			// Hit by a biggoron sword.
		Boomerang,				// Hit by a boomerang.
		BombExplosion,			// Hit by a bomb explosion.
		Shield,					// Hit by a shield.
		SwitchHook,				// Hit by the switch hook.
		Shovel,					// Hit by a shovel being used.
		Pickup,					// Attempt to use the bracelet to pickup.
		ButtonAction,			// The A button is pressed while colliding.
		SwordHitShield,			// Their sword hits my shield.
		BiggoronSwordHitShield,	// Their biggoron sword hits my shield.
		ShieldHitShield,		// Their shield hits my shield.
		ThrownObject,			// Hit by a thrown object (thrown tiles, not bombs).
		MineCart,				// Hit by a minecart.

		Count,
	};

	public class Monster : Unit {
		
		//-----------------------------------------------------------------------------
		// Interaction Handler Delegates
		//-----------------------------------------------------------------------------

		// Player & items
		public delegate void InteractionHandler_ButtonAction();
		public delegate void InteractionHandler_Sword(ItemSword itemSword);
		public delegate void InteractionHandler_BigSword(ItemBigSword itemBigSword);
		public delegate void InteractionHandler_Shield(ItemShield itemShield);
		public delegate void InteractionHandler_Shovel(ItemShovel itemShovel);
		public delegate void InteractionHandler_Bracelet(ItemBracelet itemBracelet);

		// Projectiles & Thrown objects
		public delegate void InteractionHandler_Seed(SeedEntity seed);
		public delegate void InteractionHandler_Arrow(Arrow arrow);
		public delegate void InteractionHandler_SwordBeam(SwordBeam swordBeam);
		public delegate void InteractionHandler_Boomerang(Boomerang boomerang);
		public delegate void InteractionHandler_RodFire(Projectile rodFire);
		public delegate void InteractionHandler_SwitchHook(Entity hook);
		public delegate void InteractionHandler_ThrownObject(CarriedTile thrownObject);

		// Effects
		public delegate void InteractionHandler_Fire(Fire fire);
		public delegate void InteractionHandler_BombExplosion(Effect explosion);
		

		//-----------------------------------------------------------------------------
		// Member Variables
		//-----------------------------------------------------------------------------

		private Properties properties;
		private int contactDamage;
		
		// Interaction handlers.
		// Player & items
		private InteractionHandler_ButtonAction		handlerButtonAction;
		private InteractionHandler_Sword			handlerSword;
		private InteractionHandler_BigSword			handlerBigSword;
		private InteractionHandler_Shield			handlerShield;
		private InteractionHandler_Shovel			handlerShovel;
		private InteractionHandler_Bracelet			handlerBracelet;
		// Projectiles & Thrown objects
		private InteractionHandler_Seed[]			handlerSeeds;
		private InteractionHandler_Arrow			handlerArrow;
		private InteractionHandler_Boomerang		handlerBoomerang;
		private InteractionHandler_SwordBeam		handlerSwordBeam;
		private InteractionHandler_RodFire			handlerRodFire;
		private InteractionHandler_SwitchHook		handlerSwitchHook;
		private InteractionHandler_ThrownObject		handlerThrownObject;
		// Effects
		private InteractionHandler_Fire				handlerFire;
		private InteractionHandler_BombExplosion	handlerBombExplosion;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Monster() {
			// Physics.
			Physics.CollisionBox		= new Rectangle2I(3 - 8, 5 - 14, 10, 10);
			Physics.SoftCollisionBox	= new Rectangle2I(2 - 8, 3 - 14, 12, 13);
			Physics.CollideWithWorld	= true;
			Physics.CollideWithRoomEdge	= true;
			Physics.HasGravity			= true;

			// Graphics.
			Graphics.DrawOffset = new Point2I(-8, -14);
			centerOffset		= new Point2I(0, -6);

			// Monster & unit settings.
			knockbackSpeed			= GameSettings.MONSTER_KNOCKBACK_SPEED;
			knockbackDuration		= GameSettings.MONSTER_KNOCKBACK_DURATION;
			hurtInvincibleDuration	= GameSettings.MONSTER_HURT_INVINCIBLE_DURATION;
			hurtFlickerDuration		= GameSettings.MONSTER_HURT_FLICKER_DURATION;
			contactDamage			= 1;

			// Interaction Handlers:
			// Player & items
			handlerButtonAction		= OnButtonAction;
			handlerSword			= OnSwordHit;
			handlerBigSword			= null;
			handlerShield			= OnShieldHit;
			handlerShovel			= null;
			handlerBracelet			= null;
			// Projectiles & Thrown objects
			handlerArrow			= OnArrowHit;
			handlerBoomerang		= OnBoomerangHit;
			handlerSwordBeam		= OnSwordBeamHit;
			handlerRodFire			= null;
			handlerSwitchHook		= null;
			handlerThrownObject		= OnThrownObjectHit;
			// Effects
			handlerFire				= OnFireHit;
			handlerBombExplosion	= OnBombExplosionHit;
			// Seeds
			handlerSeeds = new InteractionHandler_Seed[5];
			handlerSeeds[(int) SeedType.Ember]		= OnEmberSeedHit;
			handlerSeeds[(int) SeedType.Scent]		= OnScentSeedHit;
			handlerSeeds[(int) SeedType.Gale]		= OnGaleSeedHit;
			handlerSeeds[(int) SeedType.Pegasus]	= OnPegasusSeedHit;
			handlerSeeds[(int) SeedType.Mystery]	= OnMysterySeedHit;


			//SetInteraction(InteractionType.Fire,		ResponseBurn);
			//SetInteraction(InteractionType.EmberSeed,	ResponseBurn);
			//SetInteraction(InteractionType.RodFire,		ResponseBurn, DestroySource);

			// From Java game, translated a bit:
			/*
			SetInteraction(InteractionType.None,			null);
			SetInteraction(InteractionType.RodFire,			new BURN(1));
			SetInteraction(InteractionType.Fire,			new COMBO(new DESTROY_SOURCE(), new BURN(1)));
			SetInteraction(InteractionType.EmberSeed,		new BURN(1));
			SetInteraction(InteractionType.ScentSeed,		new SEED_REACTION(1, new DAMAGE(1)));
			SetInteraction(InteractionType.PegasusSeed,		new SEED_REACTION(2, new STUN()));
			SetInteraction(InteractionType.GaleSeed,		new GALE());
			SetInteraction(InteractionType.MysterySeed,		new MYSTERY_SEED());
			SetInteraction(InteractionType.Arrow,			new DAMAGE(1));
			SetInteraction(InteractionType.SwordBeam,		new DAMAGE(1));
			SetInteraction(InteractionType.Sword,			new DAMAGE(1, 2, 3));
			SetInteraction(InteractionType.BiggoronSword,	new DAMAGE(3));
			SetInteraction(InteractionType.BombExplosion,	new DAMAGE(1));
			SetInteraction(InteractionType.Boomerang,		new STUN());
			SetInteraction(InteractionType.Shield,			new BUMP(true));
			SetInteraction(InteractionType.Shovel,			new BUMP());
			SetInteraction(InteractionType.SwitchHook,		new SWITCH());
			SetInteraction(InteractionType.MineCart,		new DISSAPEAR_KILL()); // TODO
			SetInteraction(InteractionType.ThrownObject,	new DAMAGE(1)); // TODO
			SetInteraction(InteractionType.Pickup,			null);
			SetInteraction(InteractionType.ButtonAction,	null);
			SetInteraction(InteractionType.SwordHitShield,			new COMBO(new EFFECT_CLING(), new BUMP(true)));
			SetInteraction(InteractionType.BiggoronSwordHitShield,	new COMBO(new EFFECT_CLING(), new BUMP()));
			SetInteraction(InteractionType.ShieldHitShield,			new COMBO(new EFFECT_CLING(), new BUMP(true)));
			*/
			
		}

		
		//-----------------------------------------------------------------------------
		// Interactions
		//-----------------------------------------------------------------------------
		
		public void TriggerInteraction(Delegate action, params object[] args) {
			if (action != null)
				action.DynamicInvoke(args);
		}
		
		protected virtual void OnButtonAction() {
			
		}
		
		protected virtual void OnSwordHit(ItemSword itemSword) {
			Hurt(1, RoomControl.Player.Center);
		}
		
		protected virtual void OnShieldHit(ItemShield itemShield) {
			Hurt(0, RoomControl.Player.Center); // Knockback
		}
		
		protected virtual void OnEmberSeedHit(SeedEntity seed) {
			seed.DestroyWithEffect(SeedType.Ember, seed.Center);
			// Burn is handled by OnFireHit()
		}
		
		protected virtual void OnScentSeedHit(SeedEntity seed) {
			Hurt(1, seed.Center);
			seed.DestroyWithVisualEffect(SeedType.Scent, seed.Center);
		}
		
		protected virtual void OnGaleSeedHit(SeedEntity seed) {
			if (seed is SeedProjectile) {
				seed.DestroyWithVisualEffect(SeedType.Gale, Center);
			}
		}
		
		protected virtual void OnPegasusSeedHit(SeedEntity seed) {
			// Stun
			seed.DestroyWithVisualEffect(SeedType.Pegasus, seed.Center);
		}
		
		protected virtual void OnMysterySeedHit(SeedEntity seed) {
			// Random: burn, stun, damage, gale
			Random random = new Random();
			int rand = random.Next(4);
			if (rand == 0)
				TriggerInteraction(handlerSeeds[(int) SeedType.Ember], seed);
			else if (rand == 1)
				TriggerInteraction(handlerSeeds[(int) SeedType.Scent], seed);
			else if (rand == 2)
				TriggerInteraction(handlerSeeds[(int) SeedType.Gale], seed);
			else
				TriggerInteraction(handlerSeeds[(int) SeedType.Pegasus], seed);
		}
		
		protected virtual void OnArrowHit(Arrow arrow) {
			Hurt(1, arrow.Center);
			arrow.Destroy();
		}
		
		protected virtual void OnSwordBeamHit(SwordBeam swordBeam) {
			Hurt(1, swordBeam.Center);
			swordBeam.Destroy();
		}
		
		protected virtual void OnBoomerangHit(Boomerang boomerang) {
			// TODO: Stun
			Hurt(1, boomerang.Center);
			boomerang.BeginReturn();
		}
		
		protected virtual void OnThrownObjectHit(CarriedTile thrownObject) {
			// Damage
			Hurt(1, thrownObject.Center);
		}
		
		protected virtual void OnFireHit(Fire fire) {
			// Burn
			Hurt(1, fire.Center);
			fire.Destroy();
		}
		
		protected virtual void OnBombExplosionHit(Effect explosion) {
			Hurt(1, explosion.Center);
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			Graphics.PlayAnimation(GameData.ANIM_MONSTER_OCTOROK);
		}

		public override void Die() {
			Effect explosion = new Effect(GameData.ANIM_EFFECT_MONSTER_EXPLOSION);
			RoomControl.SpawnEntity(explosion, Center);
			base.Die();
		}

		public override void Update() {

			// Check collisions with player.
			Player player = RoomControl.Player;
			if (physics.IsSoftMeetingEntity(player)) {
				player.Hurt(contactDamage, Center);
			}

			base.Update();
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Properties Properties {
			get { return properties; }
			set { properties = value; }
		}

		public int ContactDamage {
			get { return contactDamage; }
			set { contactDamage = value; }
		}
		
		// Player & items
		public InteractionHandler_ButtonAction HandlerButtonAction {
			get { return handlerButtonAction; }
			set { handlerButtonAction = value; }
		}
		public InteractionHandler_Sword HandlerSword {
			get { return handlerSword; }
			set { handlerSword = value; }
		}
		public InteractionHandler_BigSword HandlerBigSword {
			get { return handlerBigSword; }
			set { handlerBigSword = value; }
		}
		public InteractionHandler_Shield HandlerShield {
			get { return handlerShield; }
			set { handlerShield = value; }
		}
		public InteractionHandler_Shovel HandlerShovel {
			get { return handlerShovel; }
			set { handlerShovel = value; }
		}
		public InteractionHandler_Bracelet HandlerBracelet {
			get { return handlerBracelet; }
			set { handlerBracelet = value; }
		}
		// Projectiles & Thrown objects
		public InteractionHandler_Seed[] HandlerSeeds {
			get { return handlerSeeds; }
			set { handlerSeeds = value; }
		}
		public InteractionHandler_Arrow HandlerArrow {
			get { return handlerArrow; }
			set { handlerArrow = value; }
		}
		public InteractionHandler_Boomerang HandlerBoomerang {
			get { return handlerBoomerang; }
			set { handlerBoomerang = value; }
		}
		public InteractionHandler_SwordBeam HandlerSwordBeam {
			get { return handlerSwordBeam; }
			set { handlerSwordBeam = value; }
		}
		public InteractionHandler_RodFire HandlerRodFire {
			get { return handlerRodFire; }
			set { handlerRodFire = value; }
		}
		public InteractionHandler_SwitchHook HandlerSwitchHook {
			get { return handlerSwitchHook; }
			set { handlerSwitchHook = value; }
		}
		public InteractionHandler_ThrownObject HandlerThrownObject {
			get { return handlerThrownObject; }
			set { handlerThrownObject = value; }
		}
		// Effects
		public InteractionHandler_Fire HandlerFire {
			get { return handlerFire; }
			set { handlerFire = value; }
		}
		public InteractionHandler_BombExplosion HandlerBombExplosion {
			get { return handlerBombExplosion; }
			set { handlerBombExplosion = value; }
		}
	}
}
