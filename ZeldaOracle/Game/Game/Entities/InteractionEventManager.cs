using System;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities {
	
	//-----------------------------------------------------------------------------
	// Interaction Type
	//-----------------------------------------------------------------------------

	/// <summary>The possible interaction types.</summary>
	public enum InteractionType {

		None = -1,
		
		
		//-----------------------------------------------------------------------------
		// Weapon Interactions
		//-----------------------------------------------------------------------------

		/// <summary>Hit by a sword.
		/// <para/>Default = SenderReactions.Intercept,
		/// Reactions.DamageByLevel(1, 2, 3)</summary>
		Sword,
		/// <summary>Hit by a spinning sword.
		/// <para/>Default = Reactions.Damage2</summary>
		SwordSpin,
		/// <summary>Hit by a biggoron sword.
		/// <para/>Default = Reactions.Damage3</summary>
		BiggoronSword,
		/// <summary>Hit by a shield.
		/// <para/>Default = SenderReactions.Bump, Reactions.Bump</summary>
		Shield,
		/// <summary>Hit by a shovel being used.
		/// <para/>Default = Reactions.Bump</summary>
		Shovel,
		/// <summary>TODO.
		/// <para/>Default = Reactions.None</summary>
		Parry,
		/// <summary>Attempt to use the bracelet to pickup or grab.
		/// <para/>Default = Reactions.None</summary>
		Bracelet,
		
		//-----------------------------------------------------------------------------
		// Seed Interactions
		//-----------------------------------------------------------------------------

		/// <summary>Hit by an ember seed.
		/// <para/>Default = SenderReactions.Intercept</summary>
		EmberSeed,
		/// <summary>Hit by a scent seed.
		/// <para/>Default = SenderReactions.Intercept, Reactions.Damage</summary>
		ScentSeed,
		/// <summary>Hit by a pegasus seed.
		/// <para/>Default = SenderReactions.Intercept, Reactions.Stun</summary>
		PegasusSeed,
		/// <summary>Hit by a gale seed.
		/// <para/>Default = SenderReactions.Intercept</summary>
		GaleSeed,
		/// <summary>Hit by a mystery seed.
		/// <para/>Default = Reactions.MysterySeed</summary>
		MysterySeed,
		
		
		//-----------------------------------------------------------------------------
		// Projectile Interactions
		//-----------------------------------------------------------------------------

		/// <summary>Hit by an arrow.
		/// <para/>Default = SenderReactions.Intercept, Reactions.Damage</summary>
		Arrow,
		/// <summary>Hit by a sword beam projectile.
		/// <para/>Default = SenderReactions.Destroy, Reactions.Damage</summary>
		SwordBeam,
		/// <summary>Hit by a projectile from the fire-rod.
		/// <para/>Default = SenderReactions.Intercept</summary>
		RodFire,
		/// <summary>Hit by a boomerang.
		/// <para/>Default = SenderReactions.Intercept, Reactions.Stun</summary>
		Boomerang,
		/// <summary>Hit by the switch hook.
		/// <para/>Default = SenderReactions.Intercept, Reactions.SwitchHook</summary>
		SwitchHook,

		
		//-----------------------------------------------------------------------------
		// Player Interactions
		//-----------------------------------------------------------------------------

		/// <summary>The player presses the 'A' button when facing the entity.
		/// <para/>Default = Reactions.None</summary>
		ButtonAction,
		/// <summary>Collides with the player.
		/// <para/>Default = OnTouchPlayer</summary>
		PlayerContact,
		

		//-----------------------------------------------------------------------------
		// Environment
		//-----------------------------------------------------------------------------
		
		/// <summary>Touches fire.
		/// <para/>Default = Reactions.Burn</summary>
		Fire,
		/// <summary>Touches gale.
		/// <para/>Default = Reactions.Gale</summary>
		Gale,
		/// <summary>Hit by a bomb explosion.
		/// <para/>Default = Reactions.Damage</summary>
		BombExplosion,
		/// <summary>Hit by a thrown object (thrown tiles, not bombs).
		/// <para/>Default = Reactions.Damage</summary>
		ThrownObject,
		/// <summary>Hit by a minecart.</summary>
		/// <para/>Default = Reactions.SoftKill</summary>
		MineCart,
		/// <summary>Hit by a maget ball entity.</summary>
		MagnetBall,
		/// <summary>Hit by a block (either moving or spawned on top of).
		/// <para/>Default = Reactions.Damage</summary>
		Block,


		//-----------------------------------------------------------------------------
		// UNUSED:
		//-----------------------------------------------------------------------------

		//SwordHitShield,			// Their sword hits my shield.
		//BiggoronSwordHitShield,	// Their biggoron sword hits my shield.
		//ShieldHitShield,		// Their shield hits my shield.


		//-----------------------------------------------------------------------------

		Count,
	};

		
	//-----------------------------------------------------------------------------
	// Interaction Event Arguments
	//-----------------------------------------------------------------------------
	
	public class InteractionArgs : EventArgs {
		public Vector2F ContactPoint { get; set; }
	}

	public class WeaponInteractionEventArgs : EventArgs {
		public ItemWeapon Weapon { get; set; }
		public UnitTool Tool { get; set; }
	}
	
	public class TileEventArgs : InteractionArgs {
		public Tile Tile { get; set; }
	}
	
	public class ParryInteractionArgs : InteractionArgs {
		public UnitTool SenderTool { get; set; }
		public UnitTool MonsterTool { get; set; }
	}


	//-----------------------------------------------------------------------------
	// Interaction Delegates
	//-----------------------------------------------------------------------------

	public delegate void InteractionMemberDelegate(
		Entity sender, EventArgs args);

	public delegate void InteractionStaticDelegate(
		Entity subject, Entity sender, EventArgs args);
	

	//-----------------------------------------------------------------------------
	// Interaction Handler
	//-----------------------------------------------------------------------------

	public class InteractionHandler {

		private InteractionStaticDelegate handler;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public InteractionHandler() {
			handler = null;
		}


		//-----------------------------------------------------------------------------
		// Interaction Triggering
		//-----------------------------------------------------------------------------

		/// <summary>Trigger this interaction's callbacks using the given subject,
		/// sender, and interaction arguments.</summary>
		public void Trigger(Entity entity, Entity sender, EventArgs args) {
			if (handler != null)
				handler.Invoke(entity, sender, args);
		}


		//-----------------------------------------------------------------------------
		// Interaction Configuration
		//-----------------------------------------------------------------------------

		/// <summary>Clear all interaction handlers.</summary>
		public InteractionHandler Clear() {
			handler = null;
			return this;
		}
		
		/// <summary>Set the callback for this reaction to a single function.</summary>
		public InteractionHandler Set(
			InteractionMemberDelegate callback)
		{
			handler = ToStaticInteractionDelegate(callback);
			return this;
		}
			
		/// <summary>Set the callback for this reaction to a single function.</summary>
		public InteractionHandler Set(
			InteractionStaticDelegate callback)
		{
			handler = callback;
			return this;
		}
			
		/// <summary>Add a new callback for this interaction.</summary>
		public InteractionHandler Add(
			InteractionMemberDelegate callback)
		{
			return Add(ToStaticInteractionDelegate(callback));
		}
			
		/// <summary>Add a new callback for this interaction.</summary>
		public InteractionHandler Add(
			InteractionStaticDelegate callback)
		{
			if (handler == null)
				handler = callback;
			else
				handler += callback;
			return this;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Convert a member callback, which has an implicit subject, to a
		/// static callback, which has the subject as a parameter.</summary>
		private static InteractionStaticDelegate ToStaticInteractionDelegate(
			InteractionMemberDelegate memberDelegate)
		{
			return delegate(Entity subject, Entity sender, EventArgs args) {
				memberDelegate.Invoke(sender, args);
			};
		}
	}
	
		
	//-----------------------------------------------------------------------------
	// Interaction Manager
	//-----------------------------------------------------------------------------

	public class InteractionEventManager {

		/// <summary>The subject entity which is interacting.</summary>
		private Entity entity;
		/// <summary>List of interaction handlers for each interaction type.</summary>
		private InteractionHandler[] interactionHandlers;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Create an interaction manager for the given subject entity.
		/// </summary>
		public InteractionEventManager(Entity entity) {
			this.entity = entity;
			interactionHandlers =
				new InteractionHandler[(int) InteractionType.Count];
			for (int i = 0; i < (int) InteractionType.Count; i++)
				interactionHandlers[i] = new InteractionHandler();
		}


		//-----------------------------------------------------------------------------
		// Interaction Triggering
		//-----------------------------------------------------------------------------

		/// <summary>Trigger an interaction with no arguments.</summary>
		public void Trigger(InteractionType type, Entity sender) {
			Trigger(type, sender, EventArgs.Empty);
		}

		/// <summary>Trigger an interaction with the given arguments.</summary>
		public void Trigger(InteractionType type, Entity sender, EventArgs args) {
			InteractionHandler handler = Get(type);
			handler.Trigger(entity, sender, args);
		}

		
		//-----------------------------------------------------------------------------
		// Interaction Configuration
		//-----------------------------------------------------------------------------

		/// <summary>Get the interaction handler for the given interaction type.
		/// </summary>
		public InteractionHandler Get(InteractionType type) {
			return interactionHandlers[(int) type];
		}

		/// <summary>Set the callbacks for the given interaction type.</summary>
		public void Set(InteractionType type,
			params InteractionStaticDelegate[] reactions)
		{
			InteractionHandler handler = Get(type);
			handler.Clear();
			for (int i = 0; i < reactions.Length; i++)
				handler.Add(reactions[i]);
		}
		
		/// <summary>Set the callbacks for the given interaction type.</summary>
		public void Set(InteractionType type,
			params InteractionMemberDelegate[] reactions)
		{
			InteractionHandler handler = Get(type);
			handler.Clear();
			for (int i = 0; i < reactions.Length; i++)
				handler.Add(reactions[i]);
		}

		/// <summary>Set the callbacks for the given interaction type.</summary>
		public void Set(InteractionType type,
			InteractionStaticDelegate staticReaction,
			params InteractionMemberDelegate[] memberReactions)
		{
			InteractionHandler handler = Get(type);
			handler.Clear();
			handler.Add(staticReaction);
			for (int i = 0; i < memberReactions.Length; i++)
				handler.Add(memberReactions[i]);
		}

		/// <summary>Set the callbacks for the given interaction type.</summary>
		public void Set(InteractionType type,
			InteractionStaticDelegate staticReaction1,
			InteractionStaticDelegate staticReaction2,
			params InteractionMemberDelegate[] memberReactions)
		{
			InteractionHandler handler = Get(type);
			handler.Clear();
			handler.Add(staticReaction1);
			handler.Add(staticReaction2);
			for (int i = 0; i < memberReactions.Length; i++)
				handler.Add(memberReactions[i]);
		}

		/// <summary>Set the callbacks for the given interaction type.</summary>
		public void Set(InteractionType type,
			InteractionStaticDelegate staticReaction1,
			InteractionStaticDelegate staticReaction2,
			InteractionStaticDelegate staticReaction3,
			params InteractionMemberDelegate[] memberReactions)
		{
			InteractionHandler handler = Get(type);
			handler.Clear();
			handler.Add(staticReaction1);
			handler.Add(staticReaction2);
			handler.Add(staticReaction3);
			for (int i = 0; i < memberReactions.Length; i++)
				handler.Add(memberReactions[i]);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>Get the interaction handler for the given interaction type.
		/// </summary>
		public InteractionHandler this[InteractionType type] {
			get { return interactionHandlers[(int) type]; }
			set { interactionHandlers[(int) type] = value; }
		}
		
		/// <summary>The subject entity which is interacting.</summary>
		public Entity Entity {
			get { return entity; }
			set { entity = value; }
		}
	}
}
