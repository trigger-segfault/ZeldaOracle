using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Geometry {
	
	public class CollisionModel {
		private List<Rectangle2I> boxes;
		private Rectangle2I bounds;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CollisionModel(params Rectangle2I[] boxes) {
			this.bounds = Rectangle2I.Zero;
			this.boxes = new List<Rectangle2I>();
			this.boxes.AddRange(boxes);
			CalcBounds();
		}

		public CollisionModel(CollisionModel copy) {
			bounds = Rectangle2I.Zero;
			boxes = new List<Rectangle2I>();
			for (int i = 0; i < copy.boxes.Count; ++i)
				boxes.Add(copy.boxes[i]);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------
		
		// Add a new box to the collision model.
		public CollisionModel AddBox(Rectangle2I box) {
			boxes.Add(box);
			CalcBounds();
			return this;
		}
		
		// Add a new box to the collision model.
		public CollisionModel AddBox(int x, int y, int width, int height) {
			boxes.Add(new Rectangle2I(x, y, width, height));
			CalcBounds();
			return this;
		}


		//-----------------------------------------------------------------------------
		// Internal
		//-----------------------------------------------------------------------------
		
		// Calculate the bounds rectangle that contains all the boxes.
		private void CalcBounds() {
			if (boxes.Count == 0) {
				bounds = Rectangle2I.Zero;
				return;
			}
			bounds = boxes[0];
			for (int i = 1; i < boxes.Count; ++i)
				bounds = Rectangle2I.Union(bounds, boxes[i]);
		}


		//-----------------------------------------------------------------------------
		// Static methods
		//-----------------------------------------------------------------------------

		public static bool Intersecting(CollisionModel model,
										Vector2F modelPosition,
										Rectangle2F box,
										Vector2F boxPosition)
		{
			Rectangle2F boxTranslated = Rectangle2F.Translate(box, boxPosition - modelPosition);
			for (int i = 0; i < model.boxes.Count; i++) {
				Rectangle2F modelBox = model.boxes[i];
				if (boxTranslated.Intersects(modelBox))
					return true;
			}
			return false;
		}
		
		public static explicit operator CollisionModel(Rectangle2I box) {
			return new CollisionModel(box);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Rectangle2I this[int index] {
			get { return boxes[index]; }
			set { boxes[index] = value; }
		}

		public List<Rectangle2I> Boxes {
			get { return boxes; }
			set { boxes = value; CalcBounds(); }
		}
		
		public Rectangle2I Bounds {
			get { return bounds; }
		}

		public int BoxCount {
			get { return boxes.Count; }
		}
	}
}
