using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZeldaOracle.Common.Graphics {
public class DrawMode {
	private SpriteSortMode sortMode;
	private BlendState blendState;
	private SamplerState samplerState;
	private DepthStencilState depthStencilState;
	private RasterizerState rasterizerState;
	private Effect effect;
	private Matrix transform;



	// ================== CONSTRUCTORS ================== //

	public DrawMode() {
		sortMode          = SpriteSortMode.Immediate;
		blendState        = BlendState.NonPremultiplied;
		samplerState      = SamplerState.LinearClamp;
		depthStencilState = DepthStencilState.None;
		rasterizerState   = RasterizerState.CullNone;
		effect            = null;
		transform         = Matrix.Identity;
	}

	public DrawMode(DrawMode copy) {
		sortMode          = copy.sortMode;
		blendState        = copy.blendState;
		samplerState      = copy.samplerState;
		depthStencilState = copy.depthStencilState;
		rasterizerState   = copy.rasterizerState;
		effect            = copy.effect;
		transform         = copy.transform;
	}



	// ================== PROPERTIES =================== //

	public SpriteSortMode SortMode {
		get { return sortMode; }
		set { sortMode = value; }
	}

	public BlendState BlendState {
		get { return blendState; }
		set { blendState = value; }
	}

	public SamplerState SamplerState {
		get { return samplerState; }
		set { samplerState = value; }
	}

	public DepthStencilState DepthStencilState {
		get { return depthStencilState; }
		set { depthStencilState = value; }
	}

	public RasterizerState RasterizerState {
		get { return rasterizerState; }
		set { rasterizerState = value; }
	}

	public Effect Effect {
		get { return effect; }
		set { effect = value; }
	}

	public Matrix Transform {
		get { return transform; }
		set { transform = value; }
	}
}
}
