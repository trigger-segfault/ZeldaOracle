using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Control {

	public class InteractionManager {
		
		private RoomControl roomControl;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public InteractionManager(RoomControl roomControl) {
			this.roomControl = roomControl;
		}


		//-----------------------------------------------------------------------------
		// Physics Update
		//-----------------------------------------------------------------------------

		/// <summary>Process all entity interactions.</summary>
		public void ProcessInteractions() {
			foreach (Entity sender in roomControl.ActiveEntities) {
				if (sender.Interactions.IsEnabled &&
					sender.Interactions.InteractionType != InteractionType.None)
				{
					ProcessEntityInteractions(sender,
						sender.Interactions.InteractionType);
				}
			}
		}

		/// <summary>Process the interactions that an sender entity triggers upon
		/// other entities.</summary>
		public void ProcessEntityInteractions(Entity sender, InteractionType type) {
			Rectangle2F senderBox = sender.Interactions.PositionedInteractionBox;

			foreach (Entity subject in roomControl.ActiveEntities) {
				Rectangle2F subjectBox =
					subject.Interactions.GetInteractionBox(type);
				subjectBox.Point += subject.Position;

				if (sender != subject && subject.Interactions.IsEnabled &&
					subjectBox.Intersects(senderBox))
				{
					Entity actualSender = sender;
					if (sender.Parent != null)
						actualSender = sender.Parent;
					subject.Interactions.Trigger(type, actualSender,
						sender.Interactions.InteractionEventArgs);
					if (sender.IsDestroyed)
						return;
				}
			}
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
	}
}
