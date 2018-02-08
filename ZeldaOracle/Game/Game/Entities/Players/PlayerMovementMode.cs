using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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



	public class PlayerMotionTypeOld {

		// Movement control.
		private float	moveSpeed;				// The top-speed for movement.
		private bool	canLedgeJump;			// Can the player jump off ledges?
		private bool	canRoomChange;			// Can the player change rooms?
		private bool	isStrafing;				// The player can only face one direction.

		// Slippery movement.
		private bool	isSlippery;				// Is the movement acceleration-based?
		private float	acceleration;			// Acceleration when moving.
		private float	deceleration;			// Deceleration when not moving.
		private float	minSpeed;				// Minimum speed threshhold used to jump back to zero when decelerating.
		private int		directionSnapCount;		// The number of intervals movement directions should snap to for acceleration-based movement.
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerMotionTypeOld() {
			moveSpeed			= 1.0f;
			canLedgeJump		= true;
			canRoomChange		= true;
			isStrafing			= false;
			isSlippery			= false;
			acceleration		= 0.08f;
			deceleration		= 0.05f;
			minSpeed			= 0.05f;
			directionSnapCount	= 32;
		}
		
		public PlayerMotionTypeOld(PlayerMotionTypeOld copy) {
			moveSpeed			= copy.moveSpeed;
			canLedgeJump		= copy.canLedgeJump;
			canRoomChange		= copy.canRoomChange;
			isStrafing			= copy.isStrafing;
			isSlippery			= copy.isSlippery;
			acceleration		= copy.acceleration;
			deceleration		= copy.deceleration;
			minSpeed			= copy.minSpeed;
			directionSnapCount	= copy.directionSnapCount;
		}

		
		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public float MoveSpeed {
			get { return moveSpeed; }
			set { moveSpeed = value; }
		}

		public bool CanLedgeJump {
			get { return canLedgeJump; }
			set { canLedgeJump = value; }
		}

		public bool CanRoomChange {
			get { return canRoomChange; }
			set { canRoomChange = value; }
		}

		public bool IsStrafing {
			get { return isStrafing; }
			set { isStrafing = value; }
		}

		public bool IsSlippery {
			get { return isSlippery; }
			set { isSlippery = value; }
		}

		public float Acceleration {
			get { return acceleration; }
			set { acceleration = value; }
		}

		public float Deceleration {
			get { return deceleration; }
			set { deceleration = value; }
		}

		public float MinSpeed {
			get { return minSpeed; }
			set { minSpeed = value; }
		}

		public int DirectionSnapCount {
			get { return directionSnapCount; }
			set { directionSnapCount = value; }
		}
	}

}
