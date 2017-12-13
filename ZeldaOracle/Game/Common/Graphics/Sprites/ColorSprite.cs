using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {

	public struct ColoredSprite {
		public string ColorGroup { get; set; }
		public BasicSprite Sprite { get; set; }

		public ColoredSprite(string colorGroup, BasicSprite sprite) {
			this.ColorGroup	= colorGroup;
			this.Sprite		= sprite;
		}
	}

	public class ColorSprite : ISprite {
		/// <summary>The group for this sprite's coloration.
		/// <para>Empty means the default group.</para></summary>
		private string colorationGroup;
		/// <summary>The collection of this sprite's colorations.</summary>
		private Dictionary<string, BasicSprite> colorations;
		/// <summary>The default sprite for this color sprite.
		/// <para>This must also be contained in colorations.</para></summary>
		private BasicSprite defaultSprite;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ColorSprite(string colorationGroup) {
			this.colorationGroup	= colorationGroup;
			this.colorations		= new Dictionary<string, BasicSprite>();
			this.defaultSprite		= null;
		}

		public ColorSprite(string colorationGroup, BasicSprite firstSprite, string firstColorGroup) {
			this.colorationGroup	= colorationGroup;
			this.colorations		= new Dictionary<string, BasicSprite>();
			this.defaultSprite		= firstSprite;
			this.colorations.Add(firstColorGroup, firstSprite);
		}

		public ColorSprite(ColorSprite copy) {
			this.colorationGroup	= copy.colorationGroup;
			this.colorations		= new Dictionary<string, BasicSprite>(copy.colorations);
			this.defaultSprite		= copy.defaultSprite;
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
			return new ColorSprite(this);
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
			if (defaultSprite != null)
				return defaultSprite.GetBounds(settings);
			return Rectangle2I.Zero;
		}

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I Bounds {
			get {
				if (defaultSprite != null)
					return defaultSprite.Bounds;
				return Rectangle2I.Zero;
			}
		}

		//-----------------------------------------------------------------------------
		// Accessors
		//-----------------------------------------------------------------------------

		public IEnumerable<ColoredSprite> GetColorations() {
			foreach (var pair in colorations) {
				yield return new ColoredSprite(pair.Key, pair.Value);
			}
		}

		public ISprite GetColoration(string colorGroup) {
			if (colorGroup == null)
				throw new ArgumentNullException("Color group cannot be null!");
			ISprite sprite;
			colorations.TryGetValue(colorGroup, out sprite);
			return sprite;
		}

		public bool ContainsColoration(string colorGroup) {
			if (colorGroup == null)
				throw new ArgumentNullException("Color group cannot be null!");
			return colorations.ContainsKey(colorGroup);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void AddColoration(string colorGroup, BasicSprite sprite) {
			if (colorGroup == null)
				throw new ArgumentNullException("Color group cannot be null!");
			if (sprite == null)
				throw new ArgumentNullException("Sprite cannot be null!");
			if (defaultSprite == null)
				defaultSprite = sprite;
			colorations.Add(colorGroup, sprite);
		}

		public void SetStyle(string colorGroup, BasicSprite sprite) {
			if (colorGroup == null)
				throw new ArgumentNullException("Color group cannot be null!");
			if (sprite == null)
				throw new ArgumentNullException("Sprite cannot be null!");
			if (defaultSprite == null)
				defaultSprite = sprite;
			colorations[colorGroup] = sprite;
		}

		public void RemoveStyle(string colorGroup) {
			if (colorGroup == null)
				throw new ArgumentNullException("Color group cannot be null!");
			colorations.Remove(colorGroup);
			if (colorations.Count == 0)
				defaultSprite = null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the group for this sprite's coloration.</summary>
		public string ColorationGroup {
			get { return colorationGroup; }
			set { colorationGroup = value; }
		}

		/// <summary>Gets the default sprite.</summary>
		public BasicSprite DefaultSprite {
			get { return defaultSprite; }
		}
	}
}
