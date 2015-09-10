using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ZeldaOracle.Common;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Particles {
public class RectArea : EmitterArea {
	private Rectangle2F area;
	private bool edgeOnly;


	// ================== CONSTRUCTORS ================== //

	public RectArea(Rectangle2F area, bool edgeOnly = false) {
		this.area = area;
		this.edgeOnly = edgeOnly;
	}

	public RectArea(double x, double y, double width, double height, bool edgeOnly = false) {
		this.area = new Rectangle2F(x, y, width, height);
		this.edgeOnly = edgeOnly;
	}


	// ================ IMPLEMENTATIONS ================ //

	public Vector2F GetRandomLocation() {
		if (edgeOnly) {
			switch (GRandom.NextInt(4)) {
			case 0:
				return new Vector2F(area.Min.X, GRandom.NextDouble(area.Min.Y, area.Max.Y));
			case 1:
				return new Vector2F(area.Max.X, GRandom.NextDouble(area.Min.Y, area.Max.Y));
			case 2:
				return new Vector2F(GRandom.NextDouble(area.Min.X, area.Max.X), area.Min.Y);
			case 3:
				return new Vector2F(GRandom.NextDouble(area.Min.X, area.Max.X), area.Max.Y);
			}
		}
		return area.Point + GRandom.NextVector(area.Size);
	}

	public EmitterArea Copy() {
		return new RectArea(area, edgeOnly);
	}

	public Vector2F Center {
		get { return area.Center; }
	}

	public Vector2F Point {
		get { return area.Point; }
		set { area.Point = value; }
	}
	public Vector2F Size {
		get { return area.Size; }
		set { area.Size = value; }
	}
	public bool EdgeOnly {
		get { return edgeOnly; }
		set { edgeOnly = value; }
	}

	public double Area {
		get { return (area.Width * area.Height); }
	}
}
}
