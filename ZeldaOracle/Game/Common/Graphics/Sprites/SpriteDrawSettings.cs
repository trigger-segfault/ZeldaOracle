using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>The settings for drawing an ISprite.</summary>
	public struct SpriteDrawSettings {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The default sprite draw settings.</summary>
		public static readonly SpriteDrawSettings Default = new SpriteDrawSettings(new StyleDefinitions(), new ColorDefinitions());


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		/// <summary>The style definitions for style sprites.</summary>
		public StyleDefinitions Styles { get; set; }
		/// <summary>The color definitions for color sprites.</summary>
		public ColorDefinitions Colors { get; set; }
		/// <summary>The playback time for animations.</summary>
		public float PlaybackTime { get; set; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public SpriteDrawSettings(StyleDefinitions styles) {
			this.Styles			= styles;
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= 0f;
		}

		public SpriteDrawSettings(ColorDefinitions colors) {
			this.Styles         = new StyleDefinitions();
			this.Colors			= colors;
			this.PlaybackTime   = 0.0f;
		}

		public SpriteDrawSettings(float playbackTime) {
			this.Styles			= new StyleDefinitions();
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= playbackTime;
		}

		public SpriteDrawSettings(StyleDefinitions styles, float playbackTime) {
			this.Styles			= styles;
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= playbackTime;
		}

		public SpriteDrawSettings(ColorDefinitions colors, float playbackTime) {
			this.Styles			= new StyleDefinitions();
			this.Colors			= colors;
			this.PlaybackTime	= playbackTime;
		}

		public SpriteDrawSettings(StyleDefinitions styles, ColorDefinitions colors) {
			this.Styles			= styles;
			this.Colors			= colors;
			this.PlaybackTime	= 0f;
		}

		public SpriteDrawSettings(StyleDefinitions styles, ColorDefinitions colors,
			float playbackTime)
		{
			this.Styles			= styles;
			this.Colors			= colors;
			this.PlaybackTime	= playbackTime;
		}
	}
}
