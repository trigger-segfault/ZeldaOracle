
namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>The settings for drawing an ISprite.</summary>
	public struct SpriteSettings {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		/// <summary>The default sprite draw settings.</summary>
		public static readonly SpriteSettings Default =
			new SpriteSettings(new StyleDefinitions(), new ColorDefinitions());


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

		/// <summary>Constructs the sprite draw settings with the specified styles.</summary>
		public SpriteSettings(StyleDefinitions styles) {
			this.Styles			= styles;
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= 0f;
		}

		/// <summary>Constructs the sprite draw settings with the specified colors.</summary>
		public SpriteSettings(ColorDefinitions colors) {
			this.Styles         = new StyleDefinitions();
			this.Colors			= colors;
			this.PlaybackTime   = 0.0f;
		}

		/// <summary>Constructs the sprite draw settings with the specified time.</summary>
		public SpriteSettings(float playbackTime) {
			this.Styles			= new StyleDefinitions();
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= playbackTime;
		}

		/// <summary>Constructs the sprite draw settings with the specified styles and time.</summary>
		public SpriteSettings(StyleDefinitions styles, float playbackTime) {
			this.Styles			= styles;
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= playbackTime;
		}

		/// <summary>Constructs the sprite draw settings with the specified colors and time.</summary>
		public SpriteSettings(ColorDefinitions colors, float playbackTime) {
			this.Styles			= new StyleDefinitions();
			this.Colors			= colors;
			this.PlaybackTime	= playbackTime;
		}

		/// <summary>Constructs the sprite draw settings with the specified styles and colors.</summary>
		public SpriteSettings(StyleDefinitions styles, ColorDefinitions colors) {
			this.Styles			= styles;
			this.Colors			= colors;
			this.PlaybackTime	= 0f;
		}

		/// <summary>Constructs the sprite draw settings with all of the settings.</summary>
		public SpriteSettings(StyleDefinitions styles, ColorDefinitions colors,
			float playbackTime)
		{
			this.Styles			= styles;
			this.Colors			= colors;
			this.PlaybackTime	= playbackTime;
		}
	}
}
