using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Control {

	/// <summary>Class used to control the camera.</summary>
	public class ViewControl {
		
		private Rectangle2I		roomBounds;
		private Vector2F		position;
		private float			panSpeed;
		private Vector2F		shakeOffset;
		private bool			clampToRoomBounds;
		private Camera			camera;
		private Entity			target;
		private Vector2F		targetPosition;
		private Rectangle2I		viewport;
		private bool			hasTarget;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ViewControl() {
			camera				= new Camera();
			hasTarget			= false;
			target				= null;
			panSpeed			= GameSettings.VIEW_PAN_SPEED;
			position			= Vector2F.Zero;
			shakeOffset			= Vector2F.Zero;
			clampToRoomBounds	= true;

			viewport = new Rectangle2I(0, GameSettings.HUD_HEIGHT,
				GameSettings.VIEW_WIDTH, GameSettings.VIEW_HEIGHT);
		}


		//-----------------------------------------------------------------------------
		// Targeting
		//-----------------------------------------------------------------------------

		/// <summary>Clear the camera target.</summary>
		public void ClearTarget() {
			target = null;
			hasTarget = false;
		}

		/// <summary>Set the camera target to a point in the room.</summary>
		public void SetTarget(Vector2F point) {
			target = null;
			targetPosition = point;
			hasTarget = true;
		}

		/// <summary>Set the camera target to an entity.</summary>
		public void SetTarget(Entity entity) {
			target = entity;
			hasTarget = true;
		}

		/// <summary>Center the camera's position on its target.</summary>
		public void CenterOnTarget() {
			if (hasTarget)
				SetPosition(GetPositionCenteredOnTarget());
		}

		/// <summary>Returns true if the camera is focused on its target.</summary>
		public bool IsCenteredOnTarget() {
			if (hasTarget)
				return (GMath.Round(position) ==
					GMath.Round(GetPositionCenteredOnTarget()));
			else
				return true;
		}


		//-----------------------------------------------------------------------------
		// Camera Movement
		//-----------------------------------------------------------------------------

		/// <summary>Center the view on the given position.</summary>
		public void CenterOn(Vector2F point) {
			SetPosition(point);
		}
		

		//-----------------------------------------------------------------------------
		// Update
		//-----------------------------------------------------------------------------

		/// <summary>Update camera movement.</summary>
		public void UpdatMovement() {
			Vector2F newPosition = position;
			Vector2F targetPosition = GetPositionCenteredOnTarget();

			// Pan camera position on each axis seperately
			for (int axis = 0; axis < 2; axis++) {
				float distance = targetPosition[axis] - newPosition[axis];
				if (GMath.Abs(distance) > panSpeed)
					newPosition[axis] += GMath.Sign(distance) * panSpeed;
				else
					newPosition[axis] = targetPosition[axis];
			}

			// Clamp the view position
			SetPosition(newPosition);
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------
		
		/// <summary>Set the camera to be centered on the given position.</summary>
		private void SetPosition(Vector2F position) {
			this.position = GetClampedPosition(position);
			camera.Position = this.position + shakeOffset;
		}
		
		/// <summary>Return the camera's target position.</summary>
		private Vector2F GetTargetPosition() {
			if (target != null) {
				targetPosition = target.DrawCenter;
				if (target is Player)
					targetPosition += ((Player) target).ViewFocusOffset;
			}
			return targetPosition;
		}

		/// <summary>Return the camera's position if it where centered on its target.
		/// </summary>
		private Vector2F GetPositionCenteredOnTarget() {
			return GetClampedPosition(GetTargetPosition());
		}

		/// <summary>Return the given camera position clamped to the room's boundaries.
		/// </summary>
		private Vector2F GetClampedPosition(Vector2F position) {
			if (clampToRoomBounds) {
				Rectangle2I clampBounds = roomBounds;
				clampBounds.Inflate(-(camera.ViewSize / 2));
				for (int axis = 0; axis < 2; axis++) {
					if (clampBounds.Size[axis] <= 0) {
						clampBounds.Size[axis] = 0;
						clampBounds.Point[axis] = roomBounds.Center[axis];
					}
				}
				return GMath.Clamp(position, clampBounds);
			}
			else {
				return position;
			}
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The rectangular area of the screen where the camera's view is
		/// displayed.</summary>
		public Rectangle2I Viewport {
			get { return viewport; }
			set {
				viewport = value;
				camera.ViewSize = viewport.Size;
			}
		}

		/// <summary>The view boundaries of the room. The camera's view can be
		/// optionally clamped to these boundaries.</summary>
		public Rectangle2I RoomBounds {
			get { return roomBounds; }
			set {
				roomBounds = value;
				if (clampToRoomBounds)
					SetPosition(position);
			}
		}

		/// <summary>Offset of the camera's view position due to screen shaking.
		/// </summary>
		public Vector2F ShakeOffset {
			get { return shakeOffset; }
			set {
				shakeOffset = value;
				camera.Position = position + shakeOffset;
			}
		}

		/// <summary>True if the camera position should be clamped to the room's
		/// boundaries.</summary>
		public bool ClampToRoomBounds {
			get { return clampToRoomBounds; }
			set { clampToRoomBounds = value; }
		}
		
		/// <summary>The camera state.</summary>
		public Camera Camera {
			get { return camera; }
		}

		/// <summary>The entity target the the camera is focused on.</summary>
		public Entity Target {
			get { return target; }
		}
	}
}
