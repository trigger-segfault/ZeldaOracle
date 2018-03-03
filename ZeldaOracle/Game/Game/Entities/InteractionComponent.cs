using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Entities {

	public static class GeneralReactions {

		public static void TriggerButtonReaction(
			Entity a, Entity b, EventArgs args)
		{
			a.RoomControl.Player.TriggeredButtonReaction = true;
		}
	}

	public class InteractionComponent : EntityComponent {

		//private Rectangle2F[] interactionBoxes;
		//private Direction direction;
		private Rectangle2F interactionBox;

		/// <summary>The interaction manager.</summary>
		private InteractionEventManager interactionManager;

		private InteractionType interactionType;
		private EventArgs interactionEventArgs;

		private List<InteractionInstance> currentActions;
		private List<InteractionInstance> currentReactions;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public InteractionComponent(Entity entity) :
			base(entity)
		{
			interactionBox			= Rectangle2F.Zero;
			interactionManager		= new InteractionEventManager(entity);
			interactionType			= InteractionType.None;
			interactionEventArgs	= null;
			currentActions			= new List<InteractionInstance>();
			currentReactions		= new List<InteractionInstance>();
		}


		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public Rectangle2F GetInteractionBox(InteractionType interactionType) {
			return interactionBox;
		}


		//-----------------------------------------------------------------------------
		// Configuration
		//-----------------------------------------------------------------------------
		
		public void EnableInteractionCallbacks() {
			if (interactionManager == null)
				interactionManager = new InteractionEventManager(entity);
		}

		/// <summary>Get the interaction handler for the given interaction type.
		/// </summary>
		public InteractionHandler GetInteraction(InteractionType type) {
			if (interactionManager == null)
				return null;
			return interactionManager[type];
		}

		/// <summary>Clear the callbacks for all interaction types.</summary>
		public void ClearReactions() {
			if (interactionManager != null) {
				for (int i = 0; i < (int) InteractionType.Count; i++)
					interactionManager[(InteractionType) i].Clear();
			}
		}

		/// <summary>Set the reactions to the given interaction type. The reaction
		/// functions are called in the order they are specified.</summary>
		public void SetReaction(InteractionType type,
			params InteractionStaticDelegate[] reactions)
		{
			EnableInteractionCallbacks();
			interactionManager.Set(type, reactions);
		}

		/// <summary>Set the reactions to the given interaction type. The reaction
		/// functions are called in the order they are specified.</summary>
		public void SetReaction(InteractionType type,
			params InteractionMemberDelegate[] reactions)
		{
			EnableInteractionCallbacks();
			interactionManager.Set(type, reactions);
		}

		public void SetReaction(InteractionType type,
			InteractionStaticDelegate staticReaction,
			params InteractionMemberDelegate[] memberReactions)
		{
			EnableInteractionCallbacks();
			interactionManager.Set(type, staticReaction, memberReactions);
		}

		public void SetReaction(InteractionType type,
			InteractionStaticDelegate staticReaction1,
			InteractionStaticDelegate staticReaction2,
			params InteractionMemberDelegate[] memberReactions)
		{
			EnableInteractionCallbacks();
			interactionManager.Set(type, staticReaction1, staticReaction2, memberReactions);
		}

		public void SetReaction(InteractionType type,
			InteractionStaticDelegate staticReaction1,
			InteractionStaticDelegate staticReaction2,
			InteractionStaticDelegate staticReaction3,
			params InteractionMemberDelegate[] memberReactions)
		{
			EnableInteractionCallbacks();
			interactionManager.Set(type, staticReaction1, staticReaction2,
				staticReaction3, memberReactions);
		}

		

		//-----------------------------------------------------------------------------
		// Interaction Triggering
		//-----------------------------------------------------------------------------

		/// <summary>Trigger an interaction.</summary>
		public void Trigger(InteractionType type, Entity sender) {
			if (interactionManager != null)
				interactionManager.Trigger(type, sender);
		}

		/// <summary>Trigger an interaction with the given arguments.</summary>
		public void Trigger(InteractionType type, Entity sender, EventArgs args) {
			if (interactionManager != null)
				interactionManager.Trigger(type, sender, args);
		}


		//-----------------------------------------------------------------------------
		// Static Methods
		//-----------------------------------------------------------------------------
		
		public static InteractionType GetSeedInteractionType(SeedType seedType) {
			return (InteractionType) ((int) InteractionType.EmberSeed + (int) seedType);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public InteractionEventManager InteractionManager {
			get { return interactionManager; }
		}
		
		public Rectangle2F InteractionBox {
			get { return interactionBox; }
			set { interactionBox = value; }
		}
		
		public Rectangle2F PositionedInteractionBox {
			get { return Rectangle2F.Translate(interactionBox, entity.Position); }
		}
		
		public InteractionType InteractionType {
			get { return interactionType; }
			set { interactionType = value; }
		}

		public EventArgs InteractionEventArgs {
			get { return interactionEventArgs; }
			set { interactionEventArgs = value; }
		}

		public List<InteractionInstance> CurrentActions {
			get { return currentActions; }
		}

		public List<InteractionInstance> CurrentReactions {
			get { return currentReactions; }
		}
	}
}
