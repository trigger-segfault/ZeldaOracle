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
		/// <summary>Hit by a sword swing.</summary>
		Sword,
		/// <summary>Hit by a sword spin.</summary>
		SwordSpin,
		/// <summary>Hit by the sword when the player is simple holding the sword out
		/// and strafing.</summary>
		SwordStrafe,
		/// <summary>Hit by a biggoron sword.</summary>
		BiggoronSword,
		/// <summary>Hit by a shield.</summary>
		Shield,
		/// <summary>Hit by a shovel being used.</summary>
		Shovel,
		/// <summary>Touching the player when he attempts to use the Bracelet weapon.
		/// </summary>
		Bracelet,
		
		//-----------------------------------------------------------------------------
		// Seed Interactions
		//-----------------------------------------------------------------------------

		/// <summary>Hit by an ember seed.</summary>
		EmberSeed,
		/// <summary>Hit by a scent seed.</summary>
		ScentSeed,
		/// <summary>Hit by a pegasus seed.</summary>
		PegasusSeed,
		/// <summary>Hit by a gale seed.</summary>
		GaleSeed,
		/// <summary>Hit by a mystery seed.</summary>
		MysterySeed,
		
		
		//-----------------------------------------------------------------------------
		// Projectile Interactions
		//-----------------------------------------------------------------------------

		/// <summary>Hit by an arrow.</summary>
		Arrow,
		/// <summary>Hit by a sword beam projectile.</summary>
		SwordBeam,
		/// <summary>Hit by a projectile from the fire-rod.</summary>
		RodFire,
		/// <summary>Hit by a boomerang.</summary>
		Boomerang,
		/// <summary>Hit by the switch hook.</summary>
		SwitchHook,

		
		//-----------------------------------------------------------------------------
		// Player Interactions
		//-----------------------------------------------------------------------------

		/// <summary>The player presses the 'A' button when facing the entity.
		/// </summary>
		ButtonAction,
		/// <summary>Collides with the player.</summary>
		PlayerContact,
		

		//-----------------------------------------------------------------------------
		// Environment
		//-----------------------------------------------------------------------------
		
		/// <summary>Touches fire.</summary>
		Fire,
		/// <summary>Touches gale.</summary>
		Gale,
		/// <summary>Hit by a bomb explosion.</summary>
		BombExplosion,
		/// <summary>Hit by a thrown object (thrown tiles, not bombs).</summary>
		ThrownObject,
		/// <summary>Hit by a minecart.</summary>
		MineCart,
		/// <summary>Hit by a maget ball entity.</summary>
		MagnetBall,
		/// <summary>Hit by a block (either moving or spawned on top of).</summary>
		Block,


		//-----------------------------------------------------------------------------
		// UNUSED:
		//-----------------------------------------------------------------------------

		//SwordHitShield,			// Their sword hits my shield.
		//BiggoronSwordHitShield,	// Their biggoron sword hits my shield.
		//ShieldHitShield,			// Their shield hits my shield.
		//Parry,


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

	public delegate void ReactionCallback(InteractionInstance interaction);

	public delegate void ReactionStaticCallback(
		Entity reactionEntity, Entity actionEntity, EventArgs args);

	public delegate void ReactionStaticSimpleCallback(Entity reactionEntity);
	
	public delegate void ReactionMemberCallback(
		Entity actionEntity, EventArgs args);

	public delegate void ReactionMemberSimpleCallback();
		

	//-----------------------------------------------------------------------------
	// Reaction Handler
	//-----------------------------------------------------------------------------

	/// <summary>Stores the reaction callback and hit box used for a single interaction
	/// type.</summary>
	public class ReactionHandler {

		/// <summary>The callback method to invoke when this reaction is triggered.
		/// </summary>
		private ReactionCallback callback;
		private ReactionCallback callbackBegin;
		private ReactionCallback callbackEnd;
		/// <summary>Custom hitbox used to detect reactions for this interaction type.
		/// If this is null, then the entity's default interaction box will be used
		/// instead.</summary>
		private Rectangle2F? collisionBox;
		/// <summary>True if the entity's reaction should override its parent's
		/// reaction if they have the same action entity, essentially "protecting" the
		/// parent from its reaction. For example, a monster's sword "Sword" reaction
		/// will protect the monster from its "Sword" reaction, causing a parry instead
		/// of damage.</summary>
		private bool protectParent;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ReactionHandler() {
			callback		= null;
			callbackBegin	= null;
			callbackEnd		= null;
			collisionBox	= null;
			protectParent	= false;
		}


		//-----------------------------------------------------------------------------
		// Reaction Triggering
		//-----------------------------------------------------------------------------

		/// <summary>Trigger this interaction's callbacks using the given subject,
		/// sender, and interaction arguments.</summary>
		public void Trigger(InteractionInstance interaction) {
			callback?.Invoke(interaction);
		}

		public void TriggerBegin(InteractionInstance interaction) {
			callbackBegin?.Invoke(interaction);
		}

		public void TriggerEnd(InteractionInstance interaction) {
			callbackEnd?.Invoke(interaction);
		}


		//-----------------------------------------------------------------------------
		// Reaction Callback Setup
		//-----------------------------------------------------------------------------

		/// <summary>Clear all interaction handlers.</summary>
		public ReactionHandler Clear() {
			callback = null;
			callbackBegin = null;
			callbackEnd = null;
			return this;
		}
		
			
		public ReactionHandler SetBegin(ReactionCallback callback) {
			callbackBegin = callback;
			return this;
		}
		public ReactionHandler SetBegin(ReactionStaticCallback callback) {
			callbackBegin = ToReactionCallback(callback);
			return this;
		}
		public ReactionHandler SetBegin(ReactionStaticSimpleCallback callback) {
			callbackBegin = ToReactionCallback(callback);
			return this;
		}
		public ReactionHandler SetBegin(ReactionMemberCallback callback) {
			callbackBegin = ToReactionCallback(callback);
			return this;
		}
		public ReactionHandler SetBegin(ReactionMemberSimpleCallback callback) {
			callbackBegin = ToReactionCallback(callback);
			return this;
		}
		public ReactionHandler AddBegin(ReactionCallback callback) {
			if (callbackBegin == null)
				callbackBegin = callback;
			else
				callbackBegin += callback;
			return this;
		}
		public ReactionHandler AddBegin(ReactionStaticCallback callback) {
			AddBegin(ToReactionCallback(callback));
			return this;
		}
		public ReactionHandler AddBegin(ReactionStaticSimpleCallback callback) {
			AddBegin(ToReactionCallback(callback));
			return this;
		}
		public ReactionHandler AddBegin(ReactionMemberCallback callback) {
			AddBegin(ToReactionCallback(callback));
			return this;
		}
		public ReactionHandler AddBegin(ReactionMemberSimpleCallback callback) {
			AddBegin(ToReactionCallback(callback));
			return this;
		}

			
		/// <summary>Set the callback for this reaction to a single function.</summary>
		public ReactionHandler Set(ReactionCallback callback) {
			this.callback = callback;
			return this;
		}
		
		public ReactionHandler Set(ReactionStaticCallback callback) {
			this.callback = ToReactionCallback(callback);
			return this;
		}
		
		/// <summary>Set the callback for this reaction, clearing any previous ones.
		/// </summary>
		public ReactionHandler Set(
			ReactionStaticSimpleCallback callback)
		{
			this.callback = ToReactionCallback(callback);
			return this;
		}
		
		/// <summary>Set the callback for this reaction to a single function.</summary>
		public ReactionHandler Set(
			ReactionMemberCallback callback)
		{
			this.callback = ToReactionCallback(callback);
			return this;
		}
		
		/// <summary>Set the callback for this reaction, clearing any previous ones.
		/// </summary>
		public ReactionHandler Set(
			ReactionMemberSimpleCallback callback)
		{
			this.callback = ToReactionCallback(callback);
			return this;
		}
			
		/// <summary>Add a new callback for this interaction.</summary>
		public ReactionHandler Add(ReactionCallback callback) {
			if (this.callback == null)
				this.callback = callback;
			else
				this.callback += callback;
			return this;
		}
			
		/// <summary>Add a new callback for this interaction.</summary>
		public ReactionHandler Add(ReactionStaticCallback callback) {
			return Add(ToReactionCallback(callback));
		}
			
		/// <summary>Add a new callback for this interaction.</summary>
		public ReactionHandler Add(ReactionStaticSimpleCallback callback) {
			return Add(ToReactionCallback(callback));
		}
			
		/// <summary>Add a new callback for this interaction.</summary>
		public ReactionHandler Add(ReactionMemberCallback callback) {
			return Add(ToReactionCallback(callback));
		}
			
		/// <summary>Add a new callback for this interaction.</summary>
		public ReactionHandler Add(ReactionMemberSimpleCallback callback) {
			return Add(ToReactionCallback(callback));
		}
		
		/// <summary>Custom hitbox used to detect reactions for this interaction type.
		/// If this is null, then the entity's default interaction box will be used
		/// instead.</summary>
		public ReactionHandler SetCollisionBox(Rectangle2F? collisionBox) {
			this.collisionBox = collisionBox;
			return this;
		}
		
		/// <summary>True if the entity's reaction should override its parent's
		/// reaction if they have the same action entity, essentially "protecting" the
		/// parent from its reaction. For example, a monster's sword "Sword" reaction
		/// will protect the monster from its "Sword" reaction, causing a parry instead
		/// of damage.</summary>
		public ReactionHandler SetProtectParent(bool protectParent) {
			this.protectParent = protectParent;
			return this;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		private static ReactionCallback ToReactionCallback(
			ReactionStaticCallback callback)
		{
			return delegate(InteractionInstance interaction) {
				callback.Invoke(interaction.ReactionEntity,
					interaction.ActionEntity, interaction.Arguments);
			};
		}
		
		private static ReactionCallback ToReactionCallback(
			ReactionStaticSimpleCallback callback)
		{
			return delegate(InteractionInstance interaction) {
				callback.Invoke(interaction.ReactionEntity);
			};
		}

		private static ReactionCallback ToReactionCallback(
			ReactionMemberCallback callback)
		{
			return delegate(InteractionInstance interaction) {
				callback.Invoke(interaction.ActionEntity, interaction.Arguments);
			};
		}
		
		private static ReactionCallback ToReactionCallback(
			ReactionMemberSimpleCallback callback)
		{
			return delegate(InteractionInstance interaction) {
				callback.Invoke();
			};
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Custom hitbox used to detect reactions for this interaction type.
		/// If this is null, then the entity's default interaction box will be used
		/// instead.</summary>
		public Rectangle2F? CollisionBox {
			get { return collisionBox; }
			set { collisionBox = value; }
		}
		
		/// <summary>True if the entity's reaction should override its parent's
		/// reaction if they have the same action entity, essentially "protecting" the
		/// parent from its reaction. For example, a monster's sword "Sword" reaction
		/// will protect the monster from its "Sword" reaction, causing a parry instead
		/// of damage.</summary>
		public bool ProtectParent {
			get { return protectParent; }
			set { protectParent = value; }
		}
	}
	
		
	//-----------------------------------------------------------------------------
	// Reaction Manager
	//-----------------------------------------------------------------------------
	
	/// <summary>Manages an entity's reactions for all possible interaction types.
	/// </summary>
	public class ReactionManager {

		/// <summary>Reference to the entity for which this class is managing
		/// reactions.</summary>
		private Entity entity;
		/// <summary>List of reaction handlers for each interaction type.</summary>
		private ReactionHandler[] reactionHandlers;
		
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Create an reaction manager for the given subject entity.</summary>
		public ReactionManager(Entity entity) {
			this.entity = entity;
			reactionHandlers = new ReactionHandler[(int) InteractionType.Count];
			for (int i = 0; i < (int) InteractionType.Count; i++)
				reactionHandlers[i] = new ReactionHandler();
		}


		//-----------------------------------------------------------------------------
		// Reaction Triggering
		//-----------------------------------------------------------------------------

		///// <summary>Trigger an interaction with no arguments.</summary>
		//public void Trigger(InteractionType type, Entity sender) {
		//	Trigger(type, sender, EventArgs.Empty);
		//}

		///// <summary>Trigger a reaction with the given arguments.</summary>
		//public void Trigger(InteractionType type,
		//	Entity actionEntity, EventArgs args)
		//{
		//	ReactionHandler handler = Get(type);
		//	handler.Trigger(entity, actionEntity, args);
		//}

		
		//-----------------------------------------------------------------------------
		// Interaction Configuration
		//-----------------------------------------------------------------------------

		/// <summary>Get the interaction handler for the given interaction type.
		/// </summary>
		public ReactionHandler Get(InteractionType type) {
			return reactionHandlers[(int) type];
		}

		/// <summary>Set the callbacks for the given interaction type.</summary>
		public void Set(InteractionType type,
			params ReactionStaticCallback[] reactions)
		{
			ReactionHandler handler = Get(type);
			handler.Clear();
			for (int i = 0; i < reactions.Length; i++)
				handler.Add(reactions[i]);
		}
		
		/// <summary>Set the callbacks for the given interaction type.</summary>
		public void Set(InteractionType type,
			params ReactionMemberCallback[] reactions)
		{
			ReactionHandler handler = Get(type);
			handler.Clear();
			for (int i = 0; i < reactions.Length; i++)
				handler.Add(reactions[i]);
		}

		/// <summary>Set the callbacks for the given interaction type.</summary>
		public void Set(InteractionType type,
			ReactionStaticCallback staticReaction,
			params ReactionMemberCallback[] memberReactions)
		{
			ReactionHandler handler = Get(type);
			handler.Clear();
			handler.Add(staticReaction);
			for (int i = 0; i < memberReactions.Length; i++)
				handler.Add(memberReactions[i]);
		}

		/// <summary>Set the callbacks for the given interaction type.</summary>
		public void Set(InteractionType type,
			ReactionStaticCallback staticReaction1,
			ReactionStaticCallback staticReaction2,
			params ReactionMemberCallback[] memberReactions)
		{
			ReactionHandler handler = Get(type);
			handler.Clear();
			handler.Add(staticReaction1);
			handler.Add(staticReaction2);
			for (int i = 0; i < memberReactions.Length; i++)
				handler.Add(memberReactions[i]);
		}

		/// <summary>Set the callbacks for the given interaction type.</summary>
		public void Set(InteractionType type,
			ReactionStaticCallback staticReaction1,
			ReactionStaticCallback staticReaction2,
			ReactionStaticCallback staticReaction3,
			params ReactionMemberCallback[] memberReactions)
		{
			ReactionHandler handler = Get(type);
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
		public ReactionHandler this[InteractionType type] {
			get { return reactionHandlers[(int) type]; }
			set { reactionHandlers[(int) type] = value; }
		}
		
		/// <summary>Reference to the entity for which this class is managing
		/// interactions.</summary>
		public Entity Entity {
			get { return entity; }
			set { entity = value; }
		}
	}
}
