using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ZeldaOracle.Common;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Particles {
	public class CircleArea : EmitterArea {
		private Vector2F center;
		private double radius;
		private bool edgeOnly;


		// ================== CONSTRUCTORS ================== //

		public CircleArea(double radius, bool edgeOnly = false) :
			this(0, 0, radius, edgeOnly) {
		}

		public CircleArea(double x, double y, double radius, bool edgeOnly = false) {
			this.center   = new Vector2F(x, y);
			this.radius   = radius;
			this.edgeOnly = edgeOnly;
		}

		public CircleArea(Vector2F center, double radius, bool edgeOnly = false) {
			this.center   = center;
			this.radius   = radius;
			this.edgeOnly = edgeOnly;
		}


		// ================ IMPLEMENTATIONS ================ //

		public Vector2F GetRandomLocation() {
			double r = (edgeOnly ? radius : GRandom.NextDouble(radius));
			return center + new Vector2F(r, GRandom.NextDouble(360), true);
		}

		public EmitterArea Copy() {
			return new CircleArea(center, radius, edgeOnly);
		}

		public Vector2F Center {
			get { return center; }
			set { center = value; }
		}

		public double Radius {
			get { return radius; }
			set { radius = value; }
		}
		public bool EdgeOnly {
			get { return edgeOnly; }
			set { edgeOnly = value; }
		}

		public double Area {
			get { return (GMath.Pi * radius * radius); }
		}
	}
}
