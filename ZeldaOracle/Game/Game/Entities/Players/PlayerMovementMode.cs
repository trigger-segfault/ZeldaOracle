using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Entities.Players {

	public class PlayerMotionType {

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
