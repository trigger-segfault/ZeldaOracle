using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {

	public struct StyledSprite {
		public string Style { get; set; }
		public ISprite Sprite { get; set; }

		public StyledSprite(string style, ISprite sprite) {
			this.Style      = style;
			this.Sprite     = sprite;
		}
	}

	public class StyleSprite : ISprite {
		/// <summary>The group for this sprite's style.</summary>
		private string styleGroup;
		/// <summary>The collection of this sprite's styles.</summary>
		private Dictionary<string, ISprite> styles;
		/// <summary>The default sprite for this style.
		/// <para>This must also be contained in styles.</para></summary>
		private ISprite defaultSprite;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public StyleSprite(string styleGroup) {
			this.styleGroup     = styleGroup;
			this.styles         = new Dictionary<string, ISprite>();
			this.defaultSprite  = null;
		}

		public StyleSprite(string styleGroup, ISprite firstSprite, string firstStyle) {
			this.styleGroup     = styleGroup;
			this.styles         = new Dictionary<string, ISprite>();
			this.defaultSprite  = firstSprite;
			this.styles.Add(firstStyle, firstSprite);
		}

		public StyleSprite(StyleSprite copy) {
			this.styleGroup     = copy.styleGroup;
			this.styles         = new Dictionary<string, ISprite>(copy.styles);
			this.defaultSprite  = copy.defaultSprite;
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public IEnumerable<SpritePart> GetParts(SpriteDrawSettings settings) {
			if (settings.Styles != null) {
				string style = settings.Styles.GetStyle(styleGroup);
				if (style != null) {
					ISprite sprite;
					styles.TryGetValue(style, out sprite);
					if (sprite != null)
						return sprite.GetParts(settings);
				}
			}
			return defaultSprite.GetParts(settings);
		}

		/// <summary>Clones the sprite.</summary>
		public ISprite Clone() {
			return new StyleSprite(this);
		}
		
		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteDrawSettings settings) {
			string style = settings.Styles.GetStyle(styleGroup);
			if (style != null) {
				ISprite sprite;
				styles.TryGetValue(style, out sprite);
				if (sprite != null)
					return sprite.GetBounds(settings);
			}
			return defaultSprite.GetBounds(settings);
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get { return defaultSprite.Bounds; }
		}

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public IEnumerable<StyledSprite> GetStyles() {
			foreach (var pair in styles) {
				yield return new StyledSprite(pair.Key, pair.Value);
			}
		}

		public ISprite GetStyle(string style) {
			if (style == null)
				throw new ArgumentNullException("Style cannot be null!");
			ISprite sprite;
			styles.TryGetValue(style, out sprite);
			return sprite;
		}

		public bool ContainsStyle(string style) {
			if (style == null)
				throw new ArgumentNullException("Style cannot be null!");
			return styles.ContainsKey(style);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void AddStyle(string style, ISprite sprite) {
			if (style == null)
				throw new ArgumentNullException("Style cannot be null!");
			if (sprite == null)
				throw new ArgumentNullException("Sprite cannot be null!");
			if (defaultSprite == null)
				defaultSprite = sprite;
			styles.Add(style, sprite);
		}

		public void SetStyle(string style, ISprite sprite) {
			if (style == null)
				throw new ArgumentNullException("Style cannot be null!");
			if (sprite == null)
				throw new ArgumentNullException("Sprite cannot be null!");
			if (defaultSprite == null)
				defaultSprite = sprite;
			styles[style] = sprite;
		}

		public void RemoveStyle(string style) {
			if (style == null)
				throw new ArgumentNullException("Style cannot be null!");
			styles.Remove(style);
			if (styles.Count == 0)
				defaultSprite = null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the group for this sprite's style.</summary>
		public string StyleGroup {
			get { return styleGroup; }
			set { styleGroup = value; }
		}

		/// <summary>Gets the default sprite.</summary>
		public ISprite DefaultSprite {
			get { return defaultSprite; }
		}
	}
}
