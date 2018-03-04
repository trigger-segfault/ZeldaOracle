using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities {

	/// <summary>The entity component that has settings for how an entity interacts
	/// with other entities.</summary>
	public class InteractionComponent : EntityComponent {
		
		// Internal Members -----------------------------------------------------------

		/// <summary>The class which manages the entity's reactions for all interaction
		/// types.</summary>
		private ReactionManager reactionManager;
		
		// Interaction Settings -------------------------------------------------------
		
		/// <summary>The Defualt hitbox box used to detect when two entities are
		/// interacting. Each reaction handler can also specify a custom hitbox to use
		/// for detecting that specific interaction type, which will be used instead of
		/// the default interaction box.</summary>
		private Rectangle2F interactionBox;
		/// <summary>The interaction type which the entity triggers reactions for.
		/// </summary>
		private InteractionType interactionType;
		/// <summary>Arguments used for interactions that the entity triggers.
		/// </summary>
		private EventArgs interactionEventArgs;
		/// <summary>True if the entity's actions should override its parent's actions
		/// if they have the same reaction entity, essentially "protecting" the parent
		/// from triggering its action. For example, the player's sword "Sword" action
		/// will protect the player from triggering its "PlayerContact" action if they
		/// are both touching the same entity.</summary>
		private bool protectParentAction;

		// Interaction State ----------------------------------------------------------
		
		/// <summary>List of Interaction Instances which the entity triggered upon
		/// other entities.</summary>
		private List<InteractionInstance> currentActions;
		/// <summary>List of Interaction Instances to which the entity is reacting.
		/// </summary>
		private List<InteractionInstance> currentReactions;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public InteractionComponent(Entity entity) :
			base(entity)
		{
			interactionBox			= Rectangle2F.Zero;
			reactionManager			= new ReactionManager(entity);
			interactionType			= InteractionType.None;
			interactionEventArgs	= EventArgs.Empty;
			currentActions			= new List<InteractionInstance>();
			currentReactions		= new List<InteractionInstance>();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		/// <summary>Get the hitbox used to detect reactions for the given interaction
		/// type.</summary>
		public Rectangle2F GetInteractionBox(InteractionType interactionType) {
			return interactionBox;
		}


		//-----------------------------------------------------------------------------
		// Interaction Settings
		//-----------------------------------------------------------------------------

		/// <summary>Clear the callbacks for all interaction types.</summary>
		public void ClearReactions() {
			if (reactionManager != null) {
				for (int i = 0; i < (int) InteractionType.Count; i++)
					reactionManager[(InteractionType) i].Clear();
			}
		}

		/// <summary>Set the reactions to the given interaction type. The reaction
		/// functions are called in the order they are specified.</summary>
		public void SetReaction(InteractionType type,
			params ReactionStaticCallback[] reactions)
		{
			reactionManager.Set(type, reactions);
		}

		/// <summary>Set the reactions to the given interaction type. The reaction
		/// functions are called in the order they are specified.</summary>
		public void SetReaction(InteractionType type,
			params ReactionMemberCallback[] reactions)
		{
			reactionManager.Set(type, reactions);
		}

		public void SetReaction(InteractionType type,
			ReactionStaticCallback staticReaction,
			params ReactionMemberCallback[] memberReactions)
		{
			reactionManager.Set(type, staticReaction, memberReactions);
		}

		public void SetReaction(InteractionType type,
			ReactionStaticCallback staticReaction1,
			ReactionStaticCallback staticReaction2,
			params ReactionMemberCallback[] memberReactions)
		{
			reactionManager.Set(type,
				staticReaction1, staticReaction2, memberReactions);
		}

		public void SetReaction(InteractionType type,
			ReactionStaticCallback staticReaction1,
			ReactionStaticCallback staticReaction2,
			ReactionStaticCallback staticReaction3,
			params ReactionMemberCallback[] memberReactions)
		{
			reactionManager.Set(type, staticReaction1, staticReaction2,
				staticReaction3, memberReactions);
		}
		

		//-----------------------------------------------------------------------------
		// Interaction Queries
		//-----------------------------------------------------------------------------

		public bool IsMeetingEntity(Entity reactionEntity, InteractionType type,
			Rectangle2F actionBox)
		{
			if (!reactionEntity.Interactions.IsEnabled)
				return false;
			Rectangle2F rectionBox =
				reactionEntity.Interactions.GetInteractionBox(type);
			Rectangle2F positionedRectionBox =
				Rectangle2F.Translate(rectionBox, reactionEntity.Position);
			Rectangle2F positionedActionBox =
				Rectangle2F.Translate(actionBox, Entity.Position);
			return positionedActionBox.Intersects(positionedRectionBox);
		}
		

		//-----------------------------------------------------------------------------
		// Interaction Triggering
		//-----------------------------------------------------------------------------

		/// <summary>Trigger a reaction for.</summary>
		public void Trigger(InteractionType type, Entity actionEntity) {
			if (reactionManager != null)
				reactionManager.Trigger(type, actionEntity);
		}

		/// <summary>Trigger a reaction with the given arguments.</summary>
		public void Trigger(InteractionType type, Entity actionEntity, EventArgs args) {
			if (reactionManager != null)
				reactionManager.Trigger(type, actionEntity, args);
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Get the interaction handler for the given interaction type.
		/// </summary>
		private ReactionHandler GetInteraction(InteractionType type) {
			if (reactionManager == null)
				return null;
			return reactionManager[type];
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Return the interaction type for the given seed type</summary>
		public static InteractionType GetSeedInteractionType(SeedType seedType) {
			return (InteractionType)
				((int) InteractionType.EmberSeed + (int) seedType);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The class which manages the entity's reactions for all interaction
		/// types.</summary>
		public ReactionManager ReactionManager {
			get { return reactionManager; }
		}
		
		/// <summary>The Defualt hitbox box used to detect when two entities are
		/// interacting. Each reaction handler can also specify a custom hitbox to use
		/// for detecting that specific interaction type, which will be used instead of
		/// the default interaction box.</summary>
		public Rectangle2F InteractionBox {
			get { return interactionBox; }
			set { interactionBox = value; }
		}
		
		/// <summary>The interaction box translated to the entity's current position.
		/// </summary>
		public Rectangle2F PositionedInteractionBox {
			get { return Rectangle2F.Translate(interactionBox, entity.Position); }
		}
		
		/// <summary>The interaction type which the entity triggers reactions for.
		/// </summary>
		public InteractionType InteractionType {
			get { return interactionType; }
			set { interactionType = value; }
		}
		
		/// <summary>Arguments used for interactions that the entity triggers.
		/// </summary>
		public EventArgs InteractionEventArgs {
			get { return interactionEventArgs; }
			set { interactionEventArgs = value; }
		}
		
		/// <summary>List of Interaction Instances which the entity triggered upon
		/// other entities.</summary>
		public List<InteractionInstance> CurrentActions {
			get { return currentActions; }
		}

		/// <summary>List of Interaction Instances to which the entity is reacting.
		/// </summary>
		public List<InteractionInstance> CurrentReactions {
			get { return currentReactions; }
		}
		
		/// <summary>True if the entity's actions should override its parent's actions
		/// if they have the same reaction entity, essentially "protecting" the parent
		/// from triggering its action. For example, the player's sword "Sword" action
		/// will protect the player from triggering its "PlayerContact" action if they
		/// are both touching the same entity.</summary>
		public bool ProtectParentAction {
			get { return protectParentAction; }
			set { protectParentAction = value; }
		}
	}
}
