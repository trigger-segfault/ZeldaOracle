using ZeldaOracle.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Game.Control {
	public class ViewControl {
		
		private Vector2F		position;
		private Point2I			viewSize;
		private Rectangle2I		bounds;
		private float			panSpeed;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ViewControl() {
			this.viewSize		= GameSettings.VIEW_SIZE;
			this.panSpeed		= GameSettings.VIEW_PAN_SPEED;
			this.position		= Vector2F.Zero;
		}
		

		//-----------------------------------------------------------------------------
		// Helper functions
		//-----------------------------------------------------------------------------
		
		// Return the position the view should be at to be centered on the given point.
		public Vector2F GetPositionCenteredOn(Vector2F point) {
			return GetClampedPosition(point - ((Vector2F) viewSize * 0.5f));
		}

		// Return the given view position clamped to the bounds.
		public Vector2F GetClampedPosition(Vector2F position) {
			return GMath.Clamp(position, (Vector2F) bounds.TopLeft,
				(Vector2F) (bounds.BottomRight - viewSize));
		}

		public bool IsCenteredOnPosition(Vector2F position) {
			return (GMath.Round(this.position) == GMath.Round(GetPositionCenteredOn(position)));
		}


		//-----------------------------------------------------------------------------
		// View movement
		//-----------------------------------------------------------------------------

		// Center the view on the given position.
		public void CenterOn(Vector2F point) {
			position = GetPositionCenteredOn(point);
		}

		// Pan the view over time to be centered at the given point.
		public void PanTo(Vector2F point) {
			Vector2F goalPosition = GetPositionCenteredOn(point);
			
			// Pan on each axis seperately
			for (int i = 0; i < 2; i++) {
				float diff = goalPosition[i] - position[i];
				if (Math.Abs(diff) > panSpeed)
					position[i] += Math.Sign(diff) * panSpeed;
				else
					position[i] = goalPosition[i];
			}

			// Clamp the view position.
			position = GetClampedPosition(position);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Vector2F Position {
			get { return position; }
			set { position = GetClampedPosition(value); }
		}

		public Point2I ViewSize {
			get { return viewSize; }
			set { viewSize = value; }
		}

		public Rectangle2I Bounds {
			get { return bounds; }
			set { bounds = value; }
		}

		public Rectangle2I ViewRectangle {
			get { return new Rectangle2I((Point2I) GMath.Round(position), viewSize); }
		}
	}
}
