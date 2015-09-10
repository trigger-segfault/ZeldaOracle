using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Particles {
public interface EmitterArea {
	Vector2F GetRandomLocation();
	EmitterArea Copy();
	Vector2F Center { get; }
	double Area { get; }
}
}
