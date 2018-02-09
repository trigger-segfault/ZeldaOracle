
namespace ZeldaOracle.Game.Entities.Players {
	
	public class PlayerMotionType {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerMotionType() {
			MovementSpeed		= 1.0f;
			IsSlippery			= false;
			Acceleration		= 0.08f;
			Deceleration		= 0.05f;
			MinSpeed			= 0.05f;
			DirectionSnapCount	= 32;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>The default movement speed</summary>
		public float MovementSpeed { get; set; }

		/// <summary>Is the movement acceleration-based?
		public bool	 IsSlippery { get; set; }

		/// <summary>Acceleration when moving</summary>
		public float Acceleration { get; set ; }

		/// <summary>Deceleration when not moving.</summary>
		public float Deceleration { get; set ; }

		/// <summary>Minimum speed threshhold used to jump back to zero when decelerating</summary>
		public float MinSpeed { get; set ; }

		/// <summary>The number of intervals movement directions should snap to for acceleration-based movement</summary>
		public int DirectionSnapCount { get; set ; }
	}
}
