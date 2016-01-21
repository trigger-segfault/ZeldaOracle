using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Entities.Monsters.States;

namespace ZeldaOracle.Game.Entities.Monsters {

	public enum MonsterRespawnType {
		Never	= 0,
		Always	= 1,
		Normal	= 2,
	}

	public enum MonsterColor {
		Red		= 0,
		Blue	= 1,
		Green	= 2,
		Orange	= 3,
	}

	// MonsterState:
	// - MonsterBurnState
	// - MonsterGaleState
	// - MonsterStunState
	// - MonsterFallInHoleState

	public partial class Monster : Unit, ZeldaAPI.Monster {
		
		private Properties properties;
		private int contactDamage;
		private InteractionHandler[] interactionHandlers;
		protected MonsterColor color;
		
		// The current monster state.
		private MonsterState state;
		// The previous monster state.
		private MonsterState previousState;

		protected bool isBurnable;
		protected bool isGaleable;
		protected bool isStunnable;

		// States.
		/*private MonsterBurnState		stateBurn;
		private MonsterStunState		stateStun;
		private MonsterFallInHoleState	stateFallInHole;
		private MonsterGaleState		stateGale;*/
		


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Monster() {
			// With player:
			// - Top: 4 overlap
			// - Bottom: 3 overlap?
			// - Sides: 3 overlap	

			color = MonsterColor.Red;
			properties = new Properties();

			// Physics.
			Physics.CollisionBox		= new Rectangle2I(-5, -9, 10, 10);
			Physics.SoftCollisionBox	= new Rectangle2I(-6, -11, 12, 11);
			Physics.CollideWithWorld	= true;
			Physics.CollideWithRoomEdge	= true;
			Physics.AutoDodges			= true;
			Physics.HasGravity			= true;
			Physics.IsDestroyedInHoles	= true;

			// Graphics.
			Graphics.DepthLayer			= DepthLayer.Monsters;
			Graphics.DepthLayerInAir	= DepthLayer.InAirMonsters;
			Graphics.DrawOffset			= new Point2I(-8, -14);
			centerOffset				= new Point2I(0, -6);

			// Monster & unit settings.
			knockbackSpeed			= GameSettings.MONSTER_KNOCKBACK_SPEED;
			hurtKnockbackDuration	= GameSettings.MONSTER_HURT_KNOCKBACK_DURATION;
			bumpKnockbackDuration	= GameSettings.MONSTER_BUMP_KNOCKBACK_DURATION;
			hurtInvincibleDuration	= GameSettings.MONSTER_HURT_INVINCIBLE_DURATION;
			hurtFlickerDuration		= GameSettings.MONSTER_HURT_FLICKER_DURATION;
			contactDamage			= 1;
			isGaleable				= true;
			isBurnable				= true;
			isStunnable				= true;
			isKnockbackable			= true;
			
			interactionHandlers = new InteractionHandler[(int) InteractionType.Count];
			for (int i = 0; i < (int) InteractionType.Count; i++)
				interactionHandlers[i] = new InteractionHandler();

			// Setup default interactions.
			// Seeds
			SetReaction(InteractionType.EmberSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept,	Reactions.Damage);
			SetReaction(InteractionType.PegasusSeed,	SenderReactions.Intercept,	Reactions.Stun);
			SetReaction(InteractionType.GaleSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.MysterySeed,	Reactions.MysterySeed);
			// Projectiles
			SetReaction(InteractionType.Arrow,			SenderReactions.Destroy,	Reactions.Damage);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Destroy,	Reactions.Damage);
			SetReaction(InteractionType.RodFire,		SenderReactions.Intercept);
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept,	Reactions.Stun);
			SetReaction(InteractionType.SwitchHook,		Reactions.SwitchHook);
			// Environment
			SetReaction(InteractionType.Fire,			Reactions.Burn);
			SetReaction(InteractionType.Gale,			Reactions.Gale);
			SetReaction(InteractionType.BombExplosion,	Reactions.Damage);
			SetReaction(InteractionType.ThrownObject,	Reactions.Damage);
			SetReaction(InteractionType.MineCart,		Reactions.SoftKill);
			SetReaction(InteractionType.Block,			Reactions.Damage);
			// Tools
			SetReaction(InteractionType.Sword,			SenderReactions.Intercept, Reactions.DamageByLevel(1, 2, 3));
			SetReaction(InteractionType.SwordSpin,		Reactions.Damage2);
			SetReaction(InteractionType.BiggoronSword,	Reactions.Damage3);
			SetReaction(InteractionType.Shield,			SenderReactions.Bump, Reactions.Bump);
			SetReaction(InteractionType.Shovel,			Reactions.Bump);
			// Player
			SetReaction(InteractionType.Pickup,			Reactions.None);
			SetReaction(InteractionType.ButtonAction,	Reactions.None);
			SetReaction(InteractionType.PlayerContact,	OnTouchPlayer);
			
			SetReaction(InteractionType.Parry,			Reactions.Parry);
			//SetReaction(InteractionType.Parry,			SenderReactions.Bump, Reactions.Bump,
				//Reactions.ContactEffect(new Effect(GameData.ANIM_EFFECT_CLING, DepthLayer.EffectCling)));
		}
		
		
		//-----------------------------------------------------------------------------
		// Monster States
		//-----------------------------------------------------------------------------
				
		// Begin the given monster state.
		public void BeginState(MonsterState newState) {
			if (state != newState) {
				if (state != null) {
					state.End(newState);
				}
				previousState = state;
				state = newState;
				newState.Begin(this, previousState);
			}
		}
		
		public void BeginNormalState() {
			BeginState(new MonsterNormalState());
		}

		public virtual void UpdateAI() {

		}
		
		
		//-----------------------------------------------------------------------------
		// Monster Reactions
		//-----------------------------------------------------------------------------

		public bool Stun() {
			if (isStunnable && ((state is MonsterNormalState) || (state is MonsterStunState))) {
				AudioSystem.PlaySound(GameData.SOUND_MONSTER_HURT);
				BeginState(new MonsterStunState(GameSettings.MONSTER_STUN_DURATION));
				return true;
			}
			return false;
		}

		public bool EnterGale(EffectGale gale) {
			if (isGaleable && ((state is MonsterNormalState) || (state is MonsterStunState))) {
				BeginState(new MonsterGaleState(gale));
				return true;
			}
			return false;
		}

		public bool Burn(int damage) {
			if (!IsInvincible && isBurnable && ((state is MonsterNormalState) || (state is MonsterStunState))) {
				BeginState(new MonsterBurnState(damage));
				return true;
			}
			return false;
		}

		public void OnTouchPlayer(Entity sender, EventArgs args) {
			Player player = (Player) sender;
			player.Hurt(contactDamage, Center);
		}

		public virtual void OnSeedHit(SeedEntity seed) {
			// For mystery seeds, create the effect for another random seed type.
			if (seed.SeedType == SeedType.Mystery) {
				int rand = GRandom.NextInt(4);
				if (rand == 0)
					seed.SeedType = SeedType.Ember;
				else if (rand == 1)
					seed.SeedType = SeedType.Scent;
				else if (rand == 2)
					seed.SeedType = SeedType.Pegasus;
				else
					seed.SeedType = SeedType.Gale;
			}

			seed.TriggerMonsterReaction(this);
		}

		
		//-----------------------------------------------------------------------------
		// Interactions & Reactions
		//-----------------------------------------------------------------------------
		
		// Trigger an interaction.
		public void TriggerInteraction(InteractionType type, Entity sender) {
			TriggerInteraction(type, sender, EventArgs.Empty);
		}

		// Trigger an interaction with the given arguments.
		public void TriggerInteraction(InteractionType type, Entity sender, EventArgs args) {
			InteractionHandler handler = GetInteraction(type);
			handler.Trigger(this, sender, args);
		}

		// Get the interaction handler for the given interaction type.
		protected InteractionHandler GetInteraction(InteractionType type) {
			return interactionHandlers[(int) type];
		}
		
		// Set the reactions to the given interaction type.
		// The reaction functions are called in the order they are specified.
		protected void SetReaction(InteractionType type, params InteractionStaticDelegate[] reactions) {
			InteractionHandler handler = GetInteraction(type);
			handler.Clear();
			for (int i = 0; i < reactions.Length; i++)
				handler.Add(reactions[i]);
		}
		
		// Set the reactions to the given interaction type.
		// The reaction functions are called in the order they are specified.
		protected void SetReaction(InteractionType type, params InteractionMemberDelegate[] reactions) {
			InteractionHandler handler = GetInteraction(type);
			handler.Clear();
			for (int i = 0; i < reactions.Length; i++)
				handler.Add(reactions[i]);
		}

		
		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		public void ChooseImageVariant() {
			switch (color) {
				case MonsterColor.Red:		Graphics.ImageVariant = GameData.VARIANT_RED;		break;
				case MonsterColor.Blue:		Graphics.ImageVariant = GameData.VARIANT_BLUE;		break;
				case MonsterColor.Green:	Graphics.ImageVariant = GameData.VARIANT_GREEN;		break;
				case MonsterColor.Orange:	Graphics.ImageVariant = GameData.VARIANT_ORANGE;	break;
			}
		}

		
		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			health = healthMax;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_OCTOROK);
			ChooseImageVariant();
			
			// Begin the default monster state.
			BeginNormalState();
			previousState = state;
		}

		public override void Die(){
			Effect explosion = new Effect(GameData.ANIM_EFFECT_MONSTER_EXPLOSION, DepthLayer.EffectMonsterExplosion);
			AudioSystem.PlaySound(GameData.SOUND_MONSTER_DIE);
			RoomControl.SpawnEntity(explosion, Center);
			Properties.SetBase("dead", true);
			base.Die();
		}

		public override void OnKnockbackEnd() {
			base.OnKnockbackEnd();
		}

		public override void OnHurt(DamageInfo damage) {
			AudioSystem.PlaySound(GameData.SOUND_MONSTER_HURT);
		}

		public override void OnFallInHole() {
			// Begin the fall-in-hole state (slipping into a hole).
			if (state is MonsterNormalState) {
				BeginState(new MonsterFallInHoleState());
			}
		}

		public override void UpdateGraphics() {
			ChooseImageVariant();
			base.UpdateGraphics();
		}

		private void CollideMonsterAndPlayer() {
			Player player = RoomControl.Player;

			IEnumerable<UnitTool> monsterTools = Enumerable.Empty<UnitTool>();
			IEnumerable<UnitTool> playerTools = Enumerable.Empty<UnitTool>();
			
			if (!IsInvincible && !IsBeingKnockedBack)
				monsterTools = EquippedTools.Where(t => t.IsPhysicsEnabled && t.IsSwordOrShield);
			if (!player.IsInvincible && !player.IsBeingKnockedBack)
				playerTools = player.EquippedTools.Where(t => t.IsPhysicsEnabled && t.IsSwordOrShield);
						
			// 1. (M-M) MonsterTools to PlayerTools
			// 2. (M-1) MonsterTools to Player
			// 4. (M-1) PlayerTools to Monster
			// 2. (1-1) Monster to Player

			// Collide my tools with player's tools.
			foreach (UnitTool monsterTool in monsterTools) {
				foreach (UnitTool playerTool in playerTools) {
					if (monsterTool.PositionedCollisionBox.Intersects(playerTool.PositionedCollisionBox)) {
						Vector2F contactPoint = playerTool.PositionedCollisionBox.Center;

						TriggerInteraction(InteractionType.Parry, player, new ParryInteractionArgs() {
							ContactPoint	= contactPoint,
							MonsterTool		= monsterTool,
							SenderTool		= playerTool
						});

						playerTool.OnParry(this, contactPoint);
				
						RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_CLING, DepthLayer.EffectCling), contactPoint);

						return;
					}
				}
			}

			// Collide my tools with the player.
			bool parry = false;
			foreach (UnitTool tool in monsterTools) {
				if (player.Physics.PositionedSoftCollisionBox.Intersects(tool.PositionedCollisionBox)) {
					player.Hurt(contactDamage, Center);
					parry = true;
					break;
				}
			}

			// Collide with the player's tools.
			foreach (UnitTool tool in playerTools) {
				if (Physics.PositionedSoftCollisionBox.Intersects(tool.PositionedCollisionBox)) {
					tool.OnHitMonster(this);
					parry = true;
					break;
				}
			}

			// Check collisions with player.
			if (!parry && !IsStunned && !IsBurning && !IsInGale && !IsFallingInHole &&
				physics.IsCollidingWith(player, CollisionBoxType.Soft))
			{
				TriggerInteraction(InteractionType.PlayerContact, player);
			}
		}

		public override void Update() {
			// Update the current monster state.
			state.Update();

			// Collide player and monster and their tools.
			CollideMonsterAndPlayer();

			base.Update();
		}

		public override void Draw(RoomGraphics g) {
			state.DrawUnder(g);
			base.Draw(g);
			state.DrawOver(g);
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

		public bool IsStunned {
			get { return (state is MonsterStunState); }
		}
		
		public bool IsBurning {
			get { return (state is MonsterBurnState); }
		}
		
		public bool IsInGale {
			get { return (state is MonsterGaleState); }
		}
		
		public bool IsFallingInHole {
			get { return (state is MonsterFallInHoleState); }
		}
	}
}
