using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaOracle.Common.Graphics.Sprites {
	/// <summary>The settings for drawing an ISprite.</summary>
	public struct SpriteDrawSettings {

		public static readonly SpriteDrawSettings Default = new SpriteDrawSettings(new StyleDefinitions());

		/// <summary>The style definitions for style sprites.</summary>
		public StyleDefinitions Styles { get; set; }
		/// <summary>The color group to use for color sprites.</summary>
		public string ColorGroup { get; set; }
		/// <summary>The playback time for animations.</summary>
		public float PlaybackTime { get; set; }
		/// <summary>Only used for legacy support.</summary>
		public int VariantID { get; set; }

		public SpriteDrawSettings(StyleDefinitions styles) {
			this.Styles         = styles;
			this.ColorGroup     = null;
			this.PlaybackTime   = 0.0f;
			this.VariantID      = 0;
		}

		public SpriteDrawSettings(string colorGroup) {
			this.Styles         = StyleDefinitions.Empty;
			this.ColorGroup     = colorGroup;
			this.PlaybackTime   = 0.0f;
			this.VariantID      = 0;
		}

		public SpriteDrawSettings(float playbackTime) {
			this.Styles         = StyleDefinitions.Empty;
			this.ColorGroup     = null;
			this.PlaybackTime   = playbackTime;
			this.VariantID      = 0;
		}

		public SpriteDrawSettings(int variantID) {
			this.Styles         = StyleDefinitions.Empty;
			this.ColorGroup     = null;
			this.PlaybackTime   = 0.0f;
			this.VariantID      = variantID;
		}

		public SpriteDrawSettings(StyleDefinitions styles, int variantID) {
			this.Styles         = styles;
			this.ColorGroup     = null;
			this.PlaybackTime   = 0.0f;
			this.VariantID      = variantID;
		}

		public SpriteDrawSettings(StyleDefinitions styles, float playbackTime) {
			this.Styles         = styles;
			this.ColorGroup     = null;
			this.PlaybackTime   = playbackTime;
			this.VariantID      = 0;
		}

		public SpriteDrawSettings(string colorGroup, float playbackTime) {
			this.Styles         = StyleDefinitions.Empty;
			this.ColorGroup     = colorGroup;
			this.PlaybackTime   = playbackTime;
			this.VariantID      = 0;
		}

		public SpriteDrawSettings(int variantID, float playbackTime) {
			this.Styles         = StyleDefinitions.Empty;
			this.ColorGroup     = null;
			this.PlaybackTime   = playbackTime;
			this.VariantID      = variantID;
		}

		public SpriteDrawSettings(StyleDefinitions styles, string colorGroup, float playbackTime) {
			this.Styles			= styles;
			this.ColorGroup		= colorGroup;
			this.PlaybackTime   = playbackTime;
			this.VariantID      = 0;
		}

		public SpriteDrawSettings(StyleDefinitions styles, int variantID, float playbackTime) {
			this.Styles         = styles;
			this.ColorGroup     = null;
			this.PlaybackTime   = playbackTime;
			this.VariantID      = variantID;
		}

		public SpriteDrawSettings(string colorGroup, int variantID, float playbackTime) {
			this.Styles         = StyleDefinitions.Empty;
			this.ColorGroup     = colorGroup;
			this.PlaybackTime   = playbackTime;
			this.VariantID      = variantID;
		}

		public SpriteDrawSettings(StyleDefinitions styles, string colorGroup, int variantID, float playbackTime) {
			this.Styles         = styles;
			this.ColorGroup     = colorGroup;
			this.PlaybackTime   = playbackTime;
			this.VariantID      = variantID;
		}
	}
}
