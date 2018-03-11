using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Game.Control {

	/// <summary>Class used to represent the state of a camera, including its position
	/// and viewport size.</summary>
	public class Camera {
		
		/// <summary>The center position of the camera.</summary>
		private Vector2F position;
		/// <summary>The size of the camera's viewport on the screen.</summary>
		private Point2I viewSize;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Camera() {
			viewSize = GameSettings.VIEW_SIZE;
			position = Vector2F.Zero;
		}
		

		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>The center position of the camera.</summary>
		public Vector2F Position {
			get { return position; }
			set { position = value; }
		}

		/// <summary>The size of the camera's viewport on the screen.</summary>
		public Point2I ViewSize {
			get { return viewSize; }
			set { viewSize = value; }
		}

		/// <summary>The top-left corner of the camera's viewport.</summary>
		public Vector2F TopLeft {
			get { return (position - ((Vector2F) viewSize * 0.5f)); }
		}

		/// <summary>The width of the camera's viewport.</summary>
		public int Width {
			get { return viewSize.X; }
			set { viewSize.X = value; }
		}

		/// <summary>The height of the camera's viewport.</summary>
		public int Height {
			get { return viewSize.Y; }
			set { viewSize.Y = value; }
		}

		/// <summary>The left edge of the camera's viewport.</summary>
		public float Left {
			get { return (position.X - (viewSize.X / 2)); }
		}

		/// <summary>The right edge of the camera's viewport.</summary>
		public float Right {
			get { return (position.X + (viewSize.X / 2)); }
		}

		/// <summary>The top edge of the camera's viewport.</summary>
		public float Top {
			get { return (position.Y - (viewSize.Y / 2)); }
		}

		/// <summary>The bottom edge of the camera's viewport.</summary>
		public float Bottom {
			get { return (position.Y + (viewSize.Y / 2)); }
		}

		/// <summary>The rectangular area of the room that the camera is viewing.
		/// </summary>
		public Rectangle2F RoomBounds {
			get {
				return new Rectangle2F(
					position - ((Vector2F) viewSize * 0.5f), viewSize);
			}
		}
	}
}
