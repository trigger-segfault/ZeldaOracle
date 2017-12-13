using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {

	public struct DefinedSprite {
		public string Definition { get; set; }
		public ISprite Sprite { get; set; }

		public DefinedSprite(string definition, ISprite sprite) {
			this.Definition = definition;
			this.Sprite     = sprite;
		}
	}

	public abstract class DefinitionSprite : ISprite {
		/// <summary>The group for this sprite's definitions.</summary>
		private string group;
		/// <summary>The collection of this sprite's definitions.</summary>
		private Dictionary<string, ISprite> definitions;
		/// <summary>The default sprite for this group.
		/// <para>This must also be contained in definitions.</para></summary>
		private ISprite defaultSprite;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public DefinitionSprite(string group) {
			this.group          = group;
			this.definitions    = new Dictionary<string, ISprite>();
			this.defaultSprite  = null;
		}

		public DefinitionSprite(string group, ISprite firstSprite, string firstDefinition) {
			this.group          = group;
			this.definitions    = new Dictionary<string, ISprite>();
			this.defaultSprite  = firstSprite;
			this.definitions.Add(firstDefinition, firstSprite);
		}

		public DefinitionSprite(DefinitionSprite copy) {
			this.group          = copy.group;
			this.definitions    = new Dictionary<string, ISprite>(copy.definitions);
			this.defaultSprite  = copy.defaultSprite;
		}


		//-----------------------------------------------------------------------------
		// Definition Sprite Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the definition for this sprite from the draw settings.</summary>
		public abstract string GetDefinition(SpriteDrawSettings settings);


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public IEnumerable<SpritePart> GetParts(SpriteDrawSettings settings) {
			string definition = GetDefinition(settings);
			if (definition != null) {
				ISprite sprite;
				definitions.TryGetValue(definition, out sprite);
				if (sprite != null)
					return sprite.GetParts(settings);
			}
			if (defaultSprite != null)
				return defaultSprite.GetParts(settings);
			return Enumerable.Empty<SpritePart>();
		}

		/// <summary>Clones the sprite.</summary>
		public abstract ISprite Clone();

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteDrawSettings settings) {
			string definition = GetDefinition(settings);
			if (definition != null) {
				ISprite sprite;
				definitions.TryGetValue(definition, out sprite);
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

		public IEnumerable<DefinedSprite> GetDefinitions() {
			foreach (var pair in definitions) {
				yield return new DefinedSprite(pair.Key, pair.Value);
			}
		}

		public ISprite Get(string definition) {
			if (definition == null)
				throw new ArgumentNullException("Definition cannot be null!");
			ISprite sprite;
			definitions.TryGetValue(definition, out sprite);
			return sprite;
		}

		public bool Contains(string definition) {
			if (definition == null)
				throw new ArgumentNullException("Definition cannot be null!");
			return definitions.ContainsKey(definition);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Add(string definintion, ISprite sprite) {
			if (definintion == null)
				throw new ArgumentNullException("Definition cannot be null!");
			if (sprite == null)
				throw new ArgumentNullException("Sprite cannot be null!");
			if (defaultSprite == null)
				defaultSprite = sprite;
			definitions.Add(definintion, sprite);
		}

		public void Set(string definintion, ISprite sprite) {
			if (definintion == null)
				throw new ArgumentNullException("Definition cannot be null!");
			if (sprite == null)
				throw new ArgumentNullException("Sprite cannot be null!");
			if (defaultSprite == null)
				defaultSprite = sprite;
			definitions[definintion] = sprite;
		}

		public void Remove(string definintion) {
			if (definintion == null)
				throw new ArgumentNullException("Definition cannot be null!");
			definitions.Remove(definintion);
			if (definitions.Count == 0)
				defaultSprite = null;
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the group for this sprite's definitions.</summary>
		public string Group {
			get { return group; }
			set { group = value; }
		}

		/// <summary>Gets the default sprite.</summary>
		public ISprite DefaultSprite {
			get { return defaultSprite; }
		}
	}

	public class ColorSprite : DefinitionSprite {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ColorSprite(string group) :
			base(group) { }

		public ColorSprite(string group, ISprite firstSprite, string firstDefinition) :
			base(group, firstSprite, firstDefinition) { }

		public ColorSprite(ColorSprite copy) :
			base(copy) { }


		//-----------------------------------------------------------------------------
		// Definition Sprite Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the definition for this sprite from the draw settings.</summary>
		public override string GetDefinition(SpriteDrawSettings settings) {
			if (settings.Colors != null) {
				// 'all' overrides all color groups
				return settings.Colors.Get("all") ?? settings.Colors.Get(Group);
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Clones the sprite.</summary>
		public override ISprite Clone() {
			return new ColorSprite(this);
		}
	}

	public class StyleSprite : DefinitionSprite {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public StyleSprite(string group) :
			base(group) { }

		public StyleSprite(string group, ISprite firstSprite, string firstDefinition) :
			base(group, firstSprite, firstDefinition) { }

		public StyleSprite(StyleSprite copy) :
			base(copy) { }


		//-----------------------------------------------------------------------------
		// Definition Sprite Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the definition for this sprite from the draw settings.</summary>
		public override string GetDefinition(SpriteDrawSettings settings) {
			if (settings.Styles != null) {
				return settings.Styles.Get(Group);
			}
			return null;
		}


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Clones the sprite.</summary>
		public override ISprite Clone() {
			return new StyleSprite(this);
		}
	}
}
