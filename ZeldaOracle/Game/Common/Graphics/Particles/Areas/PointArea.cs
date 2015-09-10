using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ZeldaOracle.Common;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Particles {
public class PointArea : EmitterArea {
	private Vector2F point;



	// ================== CONSTRUCTORS ================== //

	public PointArea() {
		this.point = Vector2F.Zero;
	}

	public PointArea(double x, double y) {
		this.point = new Vector2F(x, y);
	}

	public PointArea(Vector2F point) {
		this.point = point;
	}

	public PointArea(Point2I point) {
		this.point = point;
	}



	// ================ IMPLEMENTATIONS ================ //

	public Vector2F GetRandomLocation() {
		return point;
	}

	public EmitterArea Copy() {
		return new PointArea(point);
	}

	public Vector2F Center {
		get { return point; }
	}

	public Vector2F Point {
		get { return point; }
		set { point = value; }
	}

	public double Area {
		get { return 0.0; }
	}
}
}
