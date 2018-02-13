using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Geometry {
	/// <summary>A collection of rectangles forming a collision model for tiles.</summary>
	public class CollisionModel {

		/// <summary>The list of rectangles in the model.</summary>
		private List<Rectangle2I> boxes;
		/// <summary>The pre-calculated bounds of the model.</summary>
		private Rectangle2I bounds;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a collision model from a list of rectangles.</summary>
		public CollisionModel(params Rectangle2I[] boxes) {
			this.bounds = Rectangle2I.Zero;
			this.boxes = new List<Rectangle2I>();
			this.boxes.AddRange(boxes);
			CalcBounds();
		}

		/// <summary>Constructs a copy of the specified collision model.</summary>
		public CollisionModel(CollisionModel copy) {
			this.boxes = new List<Rectangle2I>();
			for (int i = 0; i < copy.boxes.Count; ++i)
				this.boxes.Add(copy.boxes[i]);
			this.bounds = copy.bounds;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Add a new box to the collision model.</summary>
		public CollisionModel AddBox(Rectangle2I box) {
			boxes.Add(box);
			CalcBounds();
			return this;
		}

		/// <summary>Add a new box to the collision model.</summary>
		public CollisionModel AddBox(int x, int y, int width, int height) {
			boxes.Add(new Rectangle2I(x, y, width, height));
			CalcBounds();
			return this;
		}

		/// <summary>Combine the collision model with another collision model.</summary>
		public CollisionModel Combine(CollisionModel model) {
			foreach (Rectangle2I box in model.boxes) {
				AddBox(box);
			}
			CalcBounds();
			return this;
		}

		/// <summary>Combine the collision model with an offset collision model.</summary>
		public CollisionModel Combine(CollisionModel model, Point2I offset) {
			foreach (Rectangle2I box in model.boxes) {
				AddBox(box + offset);
			}
			CalcBounds();
			return this;
		}


		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Calculate the bounds rectangle that contains all the boxes.</summary>
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
		// Static Methods
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the collision model is intersecting the box.</summary>
		public static bool Intersecting(CollisionModel model,
										Vector2F modelPosition,
										Rectangle2F box)
		{
			return Intersecting(model, modelPosition, box, Vector2F.Zero);

		}

		/// <summary>Returns true if the collision model is intersecting the box.</summary>
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

		/// <summary>Casts a rectangle to a collision model.</summary>
		public static explicit operator CollisionModel(Rectangle2I box) {
			return new CollisionModel(box);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the box at the specified index in the list.</summary>
		public Rectangle2I this[int index] {
			get { return boxes[index]; }
			set { boxes[index] = value; }
		}

		/// <summary>Gets or sets the list of boxes.</summary>
		public List<Rectangle2I> Boxes {
			get { return boxes; }
			set { boxes = value; CalcBounds(); }
		}

		/// <summary>Gets the bounds of the collision boxes.</summary>
		public Rectangle2I Bounds {
			get { return bounds; }
		}

		/// <summary>Gets the number of boxes in the collision model.</summary>
		public int BoxCount {
			get { return boxes.Count; }
		}
	}
}
