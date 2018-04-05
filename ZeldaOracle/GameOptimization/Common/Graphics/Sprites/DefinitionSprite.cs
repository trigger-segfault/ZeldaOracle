using System;
using System.Collections.Generic;
using System.Linq;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics.Sprites {

	/// <summary>A structure representing a defined sprite in a definition sprite.</summary>
	public struct DefinedSprite {
		/// <summary>The definition of the defined sprite.</summary>
		public string Definition { get; set; }
		/// <summary>The actual sprite of the defined sprite.</summary>
		public ISprite Sprite { get; set; }

		/// <summary>Constructs the defined sprite.</summary>
		public DefinedSprite(string definition, ISprite sprite) {
			this.Definition = definition;
			this.Sprite     = sprite;
		}
	}

	/// <summary>A sprite that defines the base for sprites with named lookups.</summary>
	public abstract class DefinitionSprite : ISprite {
		/// <summary>The group for this sprite's definitions.</summary>
		private string group;
		/// <summary>The collection of this sprite's definitions.</summary>
		private Dictionary<string, ISprite> definitions;
		/// <summary>The default sprite for this group.<para/>
		/// This must also be contained in definitions.</summary>
		private ISprite defaultSprite;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a definition sprite with the specified group name.</summary>
		public DefinitionSprite(string group) {
			this.group			= group;
			definitions			= new Dictionary<string, ISprite>();
			defaultSprite		= null;
		}

		/// <summary>Constructs a definition sprite with the specified group name and
		/// first sprite.</summary>
		public DefinitionSprite(string group, ISprite firstSprite,
			string firstDefinition)
		{
			this.group			= group;
			definitions			= new Dictionary<string, ISprite>();
			defaultSprite		= firstSprite;

			// Call Add for containment checks
			Add(firstDefinition, firstSprite);
		}

		/// <summary>Constructs a copy of the definition sprite.</summary>
		public DefinitionSprite(DefinitionSprite copy) {
			group				= copy.group;
			definitions			= new Dictionary<string, ISprite>(copy.definitions);
			defaultSprite		= copy.defaultSprite;
		}


		//-----------------------------------------------------------------------------
		// Definition Sprite Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the definition for this sprite from the draw settings.</summary>
		public abstract string GetDefinition(SpriteSettings settings);


		//-----------------------------------------------------------------------------
		// ISprite Overrides
		//-----------------------------------------------------------------------------

		/// <summary>Gets the drawable parts for the sprite.</summary>
		public SpritePart GetParts(SpriteSettings settings) {
			string definition = GetDefinition(settings);
			if (definition != null) {
				ISprite sprite;
				definitions.TryGetValue(definition, out sprite);
				if (sprite != null)
					return sprite.GetParts(settings);
			}
			if (defaultSprite != null)
				return defaultSprite.GetParts(settings);
			return null;
		}

		/// <summary>Gets all sprites contained by this sprite including this one.</summary>
		public IEnumerable<ISprite> GetAllSprites() {
			yield return this;
			foreach (var pair in definitions) {
				foreach (ISprite subsprite in pair.Value.GetAllSprites()) {
					yield return subsprite;
				}
			}
		}

		/// <summary>Returns true if this sprite contains the specified sprite.</summary>
		public bool ContainsSubSprite(ISprite sprite) {
			return GetAllSprites().Contains(sprite);
		}

		/// <summary>Clones the sprite.</summary>
		public abstract ISprite Clone();

		/// <summary>Gets the draw boundaries of the sprite.</summary>
		public Rectangle2I GetBounds(SpriteSettings settings) {
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

		/// <summary>Gets the list of definitions in this sprite.</summary>
		public IEnumerable<DefinedSprite> GetDefinitions() {
			foreach (var pair in definitions) {
				yield return new DefinedSprite(pair.Key, pair.Value);
			}
		}

		/// <summary>Gets the defined sprite with the specified name.</summary>
		public ISprite Get(string definition) {
			if (definition == null)
				throw new ArgumentNullException("Definition cannot be null!");
			ISprite sprite;
			definitions.TryGetValue(definition, out sprite);
			return sprite;
		}

		/// <summary>Returns true if the specified definition exists in the sprite.</summary>
		public bool Contains(string definition) {
			if (definition == null)
				throw new ArgumentNullException("Definition cannot be null!");
			return definitions.ContainsKey(definition);
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		/// <summary>Adds the sprite definition to the sprite.</summary>
		public void Add(string definintion, ISprite sprite) {
			if (sprite != null && sprite.ContainsSubSprite(this))
				throw new SpriteContainmentException(this, sprite);
			if (definintion == null)
				throw new ArgumentNullException("Definition cannot be null!");
			if (sprite == null)
				throw new ArgumentNullException("Sprite cannot be null!");
			if (defaultSprite == null)
				defaultSprite = sprite;
			definitions.Add(definintion, sprite);
		}

		/// <summary>Sets the sprite definition for the sprite.</summary>
		public void Set(string definintion, ISprite sprite) {
			if (sprite != null && sprite.ContainsSubSprite(this))
				throw new SpriteContainmentException(this, sprite);
			if (definintion == null)
				throw new ArgumentNullException("Definition cannot be null!");
			if (sprite == null)
				throw new ArgumentNullException("Sprite cannot be null!");
			if (defaultSprite == null)
				defaultSprite = sprite;
			definitions[definintion] = sprite;
		}

		/// <summary>Removes the definition from the sprite.</summary>
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

	/// <summary>A sprite with lookups for different color groups.</summary>
	public class ColorSprite : DefinitionSprite {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a color sprite with the specified group name.</summary>
		public ColorSprite(string group) :
			base(group) { }

		/// <summary>Constructs a color sprite with the specified group name and
		/// first sprite.</summary>
		public ColorSprite(string group, ISprite firstSprite, string firstDefinition) :
			base(group, firstSprite, firstDefinition) { }

		/// <summary>Constructs a copy of the color sprite.</summary>
		public ColorSprite(ColorSprite copy) :
			base(copy) { }


		//-----------------------------------------------------------------------------
		// Definition Sprite Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the definition for this sprite from the draw settings.</summary>
		public override string GetDefinition(SpriteSettings settings) {
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


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the default basic sprite. Digs into contained styled sprites
		/// if needed.</summary>
		public BasicSprite DefaultBasicSprite {
			get {
				if (DefaultSprite is BasicSprite)
					return (BasicSprite) DefaultSprite;
				else if (DefaultSprite is StyleSprite)
					return (BasicSprite) ((StyleSprite) DefaultSprite).DefaultSprite;
				return null;
			}
		}

		/// <summary>Gets the default sprite as a style sprite.</summary>
		public StyleSprite DefaultStyleSprite {
			get { return (StyleSprite) DefaultSprite; }
		}
	}

	/// <summary>A sprite with lookups for different styles.</summary>
	public class StyleSprite : DefinitionSprite {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		/// <summary>Constructs a style sprite with the specified group name.</summary>
		public StyleSprite(string group) :
			base(group) { }

		/// <summary>Constructs a style sprite with the specified group name and
		/// first sprite.</summary>
		public StyleSprite(string group, ISprite firstSprite, string firstDefinition) :
			base(group, firstSprite, firstDefinition) { }

		/// <summary>Constructs a copy of the style sprite.</summary>
		public StyleSprite(StyleSprite copy) :
			base(copy) { }


		//-----------------------------------------------------------------------------
		// Definition Sprite Abstract Methods
		//-----------------------------------------------------------------------------

		/// <summary>Gets the definition for this sprite from the draw settings.</summary>
		public override string GetDefinition(SpriteSettings settings) {
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
