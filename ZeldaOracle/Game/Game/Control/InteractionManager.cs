using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Collisions;

namespace ZeldaOracle.Game.Control {

	public class InteractionManager {
		
		private RoomControl roomControl;
		private List<InteractionCollision> interactions;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public InteractionManager(RoomControl roomControl) {
			this.roomControl = roomControl;
			this.interactions = new List<InteractionCollision>();
		}


		//-----------------------------------------------------------------------------
		// Interaction Processing
		//-----------------------------------------------------------------------------

		/// <summary>Process all entity interactions.</summary>
		public void ProcessInteractions() {
			// Detect new or sustaining interactions
			DetectInteractions();

			// Remove any interactions that are no longer occurring
			PruneInvalidInteractions();

			// Trigger interactions
			TriggerAllInteractions();
		}

		/// <summary>Detect all currently occurring interactions.</summary>
		private void DetectInteractions() {
			foreach (Entity actionEntity in roomControl.ActiveEntities) {
				if (actionEntity.Interactions.IsEnabled &&
					actionEntity.Interactions.InteractionType != InteractionType.None)
				{
					Rectangle2F actionBox = actionEntity.Interactions.InteractionBox;
					DetectReactionsFromEntity(actionEntity,
						actionEntity.Interactions.InteractionType, actionBox,
						actionEntity.Interactions.InteractionEventArgs);
				}
			}
			
			// Detect interactions for collision with moving blocks
			foreach (Entity actionEntity in roomControl.ActiveEntities) {
				if (!actionEntity.Interactions.IsEnabled)
					continue;
				foreach (Collision collision in actionEntity.Physics.Collisions) {
					if (collision.IsTile && ((collision.Tile.IsMoving &&
						collision.Direction ==
							collision.Tile.MoveDirection.Reverse()) ||
						(collision.InitialPenetration > 0.0f &&
						collision.Tile.IsMovable)))
					{
						Entity tileProxy = new Entity();
						tileProxy.Position = collision.Tile.Center;
						TileEventArgs arguments = new TileEventArgs() {
							Tile = collision.Tile
						};

						// Udpate or create the interaction between these two entities
						InteractionCollision interaction = GetInteractionCollision(
							actionEntity, tileProxy, InteractionType.Block);
						interaction.StayAlive = true;
						interaction.ActionBox =
							actionEntity.Interactions.InteractionBox;
						interaction.ReactionBox = Rectangle2F.Translate(
							collision.Tile.CollisionModel.Boxes[
								collision.Source.CollisionBoxIndex],
							collision.Tile.Position);
						interaction.Arguments =arguments;
						interaction.Duration++;
					}
				}
			}
		}

		/// <summary>Detect all interaction collisions caused by the given entity and
		/// interaction type.</summary>
		private void DetectReactionsFromEntity(Entity actionEntity,
			InteractionType type, Rectangle2F actionBox, EventArgs arguments, bool autoDetected = true)
		{
			Rectangle2F positionedActionBox = Rectangle2F.Translate(
				actionBox, actionEntity.Position);

			// Find all reacting entities
			foreach (Entity reactionEntity in roomControl.ActiveEntities) {
				Rectangle2F reactionBox = reactionEntity.Interactions.InteractionBox;
				Rectangle2F positionedReactionBox = Rectangle2F.Translate(
					reactionBox, reactionEntity.Position);

				if (reactionEntity != actionEntity &&
					reactionEntity.Interactions.IsEnabled &&
					positionedReactionBox.Intersects(positionedActionBox))
				{
					// Udpate or create the interaction between these two entities
					InteractionCollision interaction = GetInteractionCollision(
						actionEntity, reactionEntity, type);
					interaction.AutoDetected	= autoDetected;
					interaction.ActionBox		= actionBox;
					interaction.ReactionBox		= reactionBox;
					interaction.Arguments		= arguments;
					interaction.StayAlive		= true;
					interaction.Duration++;
				}
			}
		}

		/// <summary>Get the interaction collision between two entities if one exists,
		/// or else create an new interaction collision.</summary>
		private InteractionCollision GetInteractionCollision(
			Entity actionEntity, Entity reactionEntity, InteractionType type)
		{
			// Find a cached reaction
			InteractionCollision interaction = interactions.FirstOrDefault(
				i => i.ActionEntity == actionEntity &&
					i.ReactionEntity == reactionEntity && i.Type == type);

			// If none exists, then create a new reaction
			if (interaction == null) {
				interaction = new InteractionCollision() {
					ActionEntity = actionEntity,
					ReactionEntity = reactionEntity,
					Type = type,
				};
				interactions.Add(interaction);
				actionEntity.Interactions.CurrentActions.Add(interaction);
				reactionEntity.Interactions.CurrentReactions.Add(interaction);
			}

			return interaction;
		}

		/// <summary>Remove any interactions which are no longer valid.</summary>
		private void PruneInvalidInteractions() {
			for (int i = 0; i < interactions.Count; i++) {
				InteractionCollision interaction = interactions[i];

				if (!interaction.StayAlive) {
					interactions.RemoveAt(i--);
					interaction.ActionEntity.Interactions
						.CurrentActions.Remove(interaction);
					interaction.ReactionEntity.Interactions
						.CurrentReactions.Remove(interaction);
				}
				else {
					interaction.StayAlive = false;
				}
			}
		}

		/// <summary>Trigger all the occurring interactions.</summary>
		private void TriggerAllInteractions() {
			foreach (InteractionCollision interaction in interactions) {
				if (interaction.IsValid()) {
					interaction.ReactionEntity.Interactions.Trigger(interaction.Type,
						interaction.ActionEntity, interaction.Arguments);
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Interaction Interface
		//-----------------------------------------------------------------------------

		/// <summary>Instantly detect and trigger an reactions for an entity.</summary>
		public void TriggerInstantReaction(Entity actionEntity, InteractionType type) {
			Rectangle2F actionBox = actionEntity.Interactions.PositionedInteractionBox;
			Rectangle2F positionedActionBox = Rectangle2F.Translate(
				actionBox, actionEntity.Position);

			// Find all reacting entities
			foreach (Entity reactionEntity in roomControl.ActiveEntities) {
				Rectangle2F reactionBox = reactionEntity.Interactions.InteractionBox;
				Rectangle2F positionedReactionBox = Rectangle2F.Translate(
					reactionBox, reactionEntity.Position);

				if (reactionEntity != actionEntity &&
					reactionEntity.Interactions.IsEnabled &&
					positionedReactionBox.Intersects(positionedActionBox))
				{
					Entity actualActionEntity = actionEntity;
					if (actionEntity.Parent != null)
						actualActionEntity = actionEntity.Parent;

					reactionEntity.Interactions.Trigger(type, actualActionEntity,
						actionEntity.Interactions.InteractionEventArgs);

					if (!actionEntity.IsAliveAndInRoom ||
						!actionEntity.Interactions.IsEnabled)
						break;
				}
			}
		}

		/// <summary>Cause an action to happen for this frame.</summary>
		public void TriggerReaction(Entity actionEntity, InteractionType type)
		{
			DetectReactionsFromEntity(actionEntity, type,
				actionEntity.Interactions.InteractionBox,
				actionEntity.Interactions.InteractionEventArgs, false);
		}

		/// <summary>Cause an action to happen for this frame.</summary>
		public void TriggerReaction(Entity actionEntity, InteractionType type,
			Rectangle2F actionBox, EventArgs arguments = null)
		{
			DetectReactionsFromEntity(actionEntity, type, actionBox, arguments, false);
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public RoomControl RoomControl {
			get { return roomControl; }
		}
		
		public GameControl GameControl {
			get { return roomControl.GameControl; }
		}

		/// <summary>List of all occurring interactions.</summary>
		public List<InteractionCollision> Interactions {
			get { return interactions; }
		}
	}
}
