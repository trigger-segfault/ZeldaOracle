using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Input.Controls {
	/// <summary>A class to store analog stick data.</summary>
	public class AnalogStick {

		/// <summary>The 4 directional controls.</summary>
		private InputControl[] directions;
		/// <summary>The disabled state of the control.</summary>
		private DisableState disabledState;
		/// <summary>The position of the analog stick.</summary>
		private Vector2F position;
		/// <summary>The dead zone of the analog stick.</summary>
		private float deadZone;
		/// <summary>The dead zone for the directional controls.</summary>
		private Vector2F directionDeadZone		= new Vector2F(0.83f, 0.83f);


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		/// <summary>Constructs the default control.</summary>
		public AnalogStick() {
			this.directions			= new InputControl[4];
			this.disabledState		= DisableState.Enabled;
			this.position			= Vector2F.Zero;
			this.deadZone			= 0.28f;
			this.directionDeadZone	= new Vector2F(0.83f, 0.83f);

			for (int i = 0; i < 4; i++)
				this.directions[i] = new InputControl();
		}


		//-----------------------------------------------------------------------------
		// Updating
		//-----------------------------------------------------------------------------

		/// <summary>Called every step to update the control state.</summary>
		public void Update(int time, Vector2F position) {
			// Apply the dead zone to the position
			if (position.Length <= deadZone) {
				position = Vector2F.Zero;
			}

			// Update the control state based on its position
			if (disabledState == DisableState.Enabled) {
				this.position = position;

				// Update the directional controls
				directions[Directions.Right].Update(time, position.X > DirectionDeadZone.X);
				directions[Directions.Down].Update(time, position.Y > DirectionDeadZone.Y);
				directions[Directions.Left].Update(time, position.X < -DirectionDeadZone.X);
				directions[Directions.Up].Update(time, position.Y < -DirectionDeadZone.Y);
			}
			else if (disabledState == DisableState.DisabledUntilRelease) {
				if (position.IsZero) {
					disabledState	= DisableState.Enabled;
					for (int i = 0; i < 4; i++)
						directions[i].Enable();
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Management
		//-----------------------------------------------------------------------------

		/// <summary>Resets the control state.</summary>
		public void Reset(bool release = false) {
			position = Vector2F.Zero;
			for (int i = 0; i < 4; i++)
				directions[i].Reset(release);
		}
		/// <summary>Enables the control.</summary>
		public void Enable() {
			disabledState = DisableState.Enabled;
			for (int i = 0; i < 4; i++)
				directions[i].Enable();
		}
		/// <summary>Disables the control.</summary>
		public void Disable(bool untilRelease = false) {
			if (untilRelease) {
				if (!position.IsZero)
					disabledState = DisableState.DisabledUntilRelease;
				else
					position	= Vector2F.Zero;
			}
			else {
				position		= Vector2F.Zero;
				disabledState	= DisableState.Disabled;
			}
			for (int i = 0; i < 4; i++)
				directions[i].Disable(untilRelease);
		}


		//-----------------------------------------------------------------------------
		// Control States
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the analog stick is centered.</summary>
		public bool IsZero() {
			return (GMath.Abs(position) <= deadZone);
		}
		/// <summary>Returns true if the analog stick is disabled.</summary>
		public bool IsDisabled() {
			return (disabledState != DisableState.Enabled);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The position of the analog stick.</summary>
		public Vector2F Position {
			get { return position; }
		}

		/// <summary>The dead zone of the analog stick.</summary>
		public float DeadZone {
			get { return deadZone; }
			set { deadZone = GMath.Clamp(GMath.Abs(value), 0f, 1f); }
		}

		/// <summary>The dead zone for the directional controls.</summary>
		public Vector2F DirectionDeadZone {
			get { return directionDeadZone; }
			set { directionDeadZone = value; }
		}

		/// <summary>The right control of the analog stick.</summary>
		public InputControl Right {
			get { return directions[Directions.Right]; }
		}

		/// <summary>The down control of the analog stick.</summary>
		public InputControl Down {
			get { return directions[Directions.Down]; }
		}

		/// <summary>The Left control of the analog stick.</summary>
		public InputControl Left {
			get { return directions[Directions.Left]; }
		}

		/// <summary>The up control of the analog stick.</summary>
		public InputControl Up {
			get { return directions[Directions.Up]; }
		}
	}
}
