using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Effects;

namespace ZeldaOracle.Game.Entities {
	
	public class InteractionComponent : EntityComponent {

		private Rectangle2F[] interactionBoxes;
		private Direction direction;
		private Rectangle2F interactionBox;

		public InteractionComponent(Entity entity) :
			base(entity)
		{
			interactionBox = Rectangle2F.Zero;
		}
	}
}
