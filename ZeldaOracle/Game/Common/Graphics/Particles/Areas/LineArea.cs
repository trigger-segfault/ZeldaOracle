using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ZeldaOracle.Common;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Particles {
public class LineArea : EmitterArea {
	private Vector2F end1;
	private Vector2F end2;


	// ================== CONSTRUCTORS ================== //

	public LineArea(Vector2F end1, Vector2F end2) {
		this.end1 = end1;
		this.end2 = end2;
	}

	public LineArea(double x1, double y1, double x2, double y2) {
		this.end1 = new Vector2F(x1, y1);
		this.end2 = new Vector2F(x2, y2);
	}


	// ================ IMPLEMENTATIONS ================ //

	public Vector2F GetRandomLocation() {
		return end1 + (end2 - end1) * GRandom.NextDouble();// Vector2.Lerp(end1, end2, GMath.Randomf());
	}

	public EmitterArea Copy() {
		return new LineArea(end1, end2);
	}

	public Vector2F Center {
		get { return (end1 + end2) * 0.5; }
	}

	public Vector2F End1 {
		get { return end1; }
		set { end1 = value; }
	}
	public Vector2F End2 {
		get { return end2; }
		set { end2 = value; }
	}

	public double Area {
		get { return 0.0; }
	}
}
}
