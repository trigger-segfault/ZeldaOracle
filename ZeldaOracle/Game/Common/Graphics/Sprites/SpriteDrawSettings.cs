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
		/// <summary>The variant ID of the image. (Only used for legacy support)</summary>
		public int VariantID { get; set; }
		/// <summary>The clipping of the sprite.</summary>
		public Rectangle2I? Clipping { get; set; }


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public SpriteDrawSettings(StyleDefinitions styles, Rectangle2I? clipping = null) {
			this.Styles			= styles;
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= 0f;
			this.VariantID		= 0;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(ColorDefinitions colors, Rectangle2I? clipping = null) {
			this.Styles         = new StyleDefinitions();
			this.Colors			= colors;
			this.PlaybackTime   = 0.0f;
			this.VariantID      = 0;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(float playbackTime, Rectangle2I? clipping = null) {
			this.Styles			= new StyleDefinitions();
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= playbackTime;
			this.VariantID		= 0;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(int variantID, Rectangle2I? clipping = null) {
			this.Styles			= new StyleDefinitions();
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= 0f;
			this.VariantID		= variantID;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(StyleDefinitions styles, int variantID, Rectangle2I? clipping = null) {
			this.Styles			= styles;
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= 0f;
			this.VariantID		= variantID;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(StyleDefinitions styles, float playbackTime, Rectangle2I? clipping = null) {
			this.Styles			= styles;
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= playbackTime;
			this.VariantID		= 0;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(ColorDefinitions colors, float playbackTime, Rectangle2I? clipping = null) {
			this.Styles			= new StyleDefinitions();
			this.Colors			= colors;
			this.PlaybackTime	= playbackTime;
			this.VariantID		= 0;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(int variantID, float playbackTime, Rectangle2I? clipping = null) {
			this.Styles			= new StyleDefinitions();
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= playbackTime;
			this.VariantID		= variantID;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(StyleDefinitions styles, ColorDefinitions colors, Rectangle2I? clipping = null) {
			this.Styles			= styles;
			this.Colors			= colors;
			this.PlaybackTime	= 0f;
			this.VariantID		= 0;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(StyleDefinitions styles, ColorDefinitions colors,
			float playbackTime, Rectangle2I? clipping = null)
		{
			this.Styles			= styles;
			this.Colors			= colors;
			this.PlaybackTime	= playbackTime;
			this.VariantID		= 0;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(StyleDefinitions styles, int variantID,
			float playbackTime, Rectangle2I? clipping = null)
		{
			this.Styles			= styles;
			this.Colors			= new ColorDefinitions();
			this.PlaybackTime	= playbackTime;
			this.VariantID		= variantID;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(ColorDefinitions colors, int variantID,
			float playbackTime, Rectangle2I? clipping = null)
		{
			this.Styles			= new StyleDefinitions();
			this.Colors			= colors;
			this.PlaybackTime	= playbackTime;
			this.VariantID		= variantID;
			this.Clipping		= clipping;
		}

		public SpriteDrawSettings(StyleDefinitions styles, ColorDefinitions colors,
			int variantID, float playbackTime, Rectangle2I? clipping = null)
		{
			this.Styles			= styles;
			this.Colors			= colors;
			this.PlaybackTime	= playbackTime;
			this.VariantID		= variantID;
			this.Clipping		= clipping;
		}
	}
}
