using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Graphics {
/** <summary>
 * A sprite sheet is a collection of images, buffered
 * in one single image, and sprites, holding information
 * on how to draw those images.
 * </summary> */
public class SpriteSheet {

	//=========== MEMBERS ============
	#region Members

	/** <summary> The name identifier of the sprite sheet. </summary> */
	private string name;
	/** <summary> The list of sprites in the sprite sheet. </summary> */
	private List<Sprite> sprites;
	/** <summary> The image of the sprite sheet. </summary> */
	private Image image;

	#endregion
	//========= CONSTRUCTORS =========
	#region Constructors

	/** <summary> Constructs the default sprite sheet. </summary> */
	public SpriteSheet(string name, Image image) {
		this.name		= name;
		this.image		= image;
		this.sprites	= new List<Sprite>();
	}
	/** <summary> Constructs the default sprite sheet. </summary> */
	public SpriteSheet(string name, Image image, SpriteSheet template) {
		this.name		= name;
		this.image		= image;
		this.sprites	= new List<Sprite>();
		for (int i = 0; i < template.Count; ++i)
			AddSprite(new Sprite(template[i]));
	}

	#endregion
	//========== PROPERTIES ==========
	#region Properties

	/** <summary> The name identifier of the sprite sheet. </summary> */
	public string Name {
		get { return name; }
	}
	/** <summary> The number of sprites in the sheet. </summary> */
	public int Count {
		get { return sprites.Count; }
	}

	/** <summary> Gets the sprite stored at the given index. </summary> */
	public Sprite this[int i] {
		get { return sprites[i]; }
		set { sprites[i] = value; }
	}

	/** <summary> Gets the sprite with the given name. </summary> */
	public Sprite this[string name] {
		get {
			for (int i = 0; i < sprites.Count; ++i) {
				if (sprites[i].Name.Equals(name))
					return sprites[i];
			}
			return null;
		}
	}

	/** <summary> The image of the sprite sheet. </summary> */
	public Image Image {
		get { return image; }
	}
	/** <summary> Gets the list of sprites. </summary> */
	public List<Sprite> Sprites {
		get { return sprites; }
	}

	#endregion
	//=========== SPRITES ============
	#region Sprites

	/** <summary> Adds the specified sprite to the sheet. </summary> */
	public Sprite AddSprite(Sprite sprite) {
		sprites.Add(sprite);
		sprite.Sheet = this;
		return sprite;
	}
	/** <summary> Adds a sprite with the given information to the sheet. </summary> */
	public Sprite AddSprite(string name, int frameX, int frameY, int frameWidth,
							int frameHeight, int offsetX, int offsetY, int sourceWidth,
							int sourceHeight, float originX, float originY) {
		Sprite sprite     = new Sprite();
		sprite.Name       = name;
		sprite.Sheet      = this;
		sprite.FrameRect  = new Rectangle2I(frameX, frameY, frameWidth, frameHeight);
		sprite.Offset     = new Vector2F(offsetX, offsetY);
		sprite.SourceSize = new Point2I(sourceWidth, sourceHeight);
		sprite.Origin     = new Vector2F(originX, originY);
		sprites.Add(sprite);
		return sprite;
	}
	/** <summary> Centers the origins of all sprites in the sheet. </summary> */
	public void CenterOrigins() {
		AlignOrigins(0.5f, 0.5f);
	}
	/** <summary> Aligns the origins of all sprites in the sheet. </summary> */
	public void AlignOrigins(float xPos, float yPos) {
		for (int i = 0; i < sprites.Count; ++i)
			sprites[i].AlignOrigin(xPos, yPos);
	}
	/** <summary> Create a grid of equally sized sprites, all having a shared size and origin. </summary> */
	public void CreateSpriteGrid(int spriteWidth, int spriteHeight, float originX, float originY,
								 int spriteCount, int columns, int offsetX, int offsetY, int spacing) {
		for (int i = 0; i < spriteCount; ++i) {
			int x = i % columns;
			int y = i / columns;
			Rectangle2I rect = new Rectangle2I(
				offsetX + ((spriteWidth  + spacing) * x),
				offsetY + ((spriteHeight + spacing) * y),
				spriteWidth, spriteHeight);
			Vector2F origin = new Vector2F(originX, originY);
			AddSprite(new Sprite("", this, rect, origin));
		}
	}

	// Create an animation strip from sprites that share a common
	// prefix, with a suffix consisting of an underscore and a number,
	// representing the frame of that sprite. If the sprite mathces
	// the prefix exactly, then it is returned as a one-frame strip.
	/*public AnimationStrip CreateAnimationStrip(float speed, bool loops, string spriteNamePrefix) {
		AnimationStrip strip = new AnimationStrip(speed, loops);
		bool foundFrame = true;

		for (int index = 1; foundFrame; ++index) {
			foundFrame = false;

			for (int i = 0; i < sprites.Count && !foundFrame; ++i) {
				string name = sprites[i].Name;

				if (name.Equals(spriteNamePrefix)) {
					strip.AddFrame(sprites[i]);
					return strip;
				}
				else if (name.Equals(spriteNamePrefix + "_" + index)) {
					strip.AddFrame(sprites[i]);
					foundFrame = true;
				}
			}
		}

		return strip;
	}*/
	/** <summary> Finds the sprite with the name matching the regular expression. </summary> */
	public Sprite FindSprite(string regex) {
		return FindSprite(regex, RegexOptions.None);
	}
	/** <summary> Finds the sprite with the name matching the regular expression. </summary> */
	public Sprite FindSprite(string regex, RegexOptions options) {
		for (int i = 0; i < sprites.Count; ++i) {
			if (Regex.IsMatch(sprites[i].Name, regex, options))
				return sprites[i];
		}
		return null;
	}
	/** <summary> Finds the sprites with the names matching the regular expression. </summary> */
	public List<Sprite> FindSprites(string regex) {
		return FindSprites(regex, RegexOptions.None);
	}
	/** <summary> Finds the sprites with the names matching the regular expression. </summary> */
	public List<Sprite> FindSprites(string regex, RegexOptions options) {
		List<Sprite> results = new List<Sprite>();
		for (int i = 0; i < sprites.Count; ++i) {
			if (Regex.IsMatch(sprites[i].Name, regex, options))
				results.Add(sprites[i]);
		}
		return results;
	}

	#endregion
}
} // end namespace
