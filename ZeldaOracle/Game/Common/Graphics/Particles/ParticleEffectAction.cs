using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZeldaOracle.Common;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Graphics.Particles {
public class ParticleEffectAction {

	private ParticleType type;
	private ParticleEmitter emitter;
	private double frequency;
	private double duration;
	private double delay;
	private bool isStream;


	// ================== CONSTRUCTORS ================== //

	public ParticleEffectAction(ParticleEmitter emitter, ParticleType type, double frequency, double duration, double delay, bool isStream) {
		this.emitter   = emitter;
		this.type      = type;
		this.duration  = duration;
		this.frequency = frequency;
		this.delay     = delay;
		this.isStream  = isStream;
	}

	public ParticleEffectAction(ParticleEffectAction clone) {
		this.emitter   = clone.emitter;
		this.type      = clone.type;
		this.duration  = clone.duration;
		this.frequency = clone.frequency;
		this.delay     = clone.delay;
		this.isStream  = clone.isStream;
	}


	// ================== PROPERTIES =================== //

	public ParticleType Type {
		get { return type; }
		set { type = value; }
	}

	public ParticleEmitter Emitter {
		get { return emitter; }
		set { emitter = value; }
	}

	public double Delay {
		get { return delay; }
		set { delay = value; }
	}

	public double Duration {
		get { return duration; }
		set { duration = value; }
	}

	public double Frequency {
		get { return frequency; }
		set { frequency = value; }
	}

	public int Count {
		get { return (int)frequency; }
	}

	public bool IsStream {
		get { return isStream; }
		set { isStream = value; }
	}
}
}
