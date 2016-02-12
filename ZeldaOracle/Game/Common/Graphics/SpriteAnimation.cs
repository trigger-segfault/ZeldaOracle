using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeldaOracle.Common.Graphics {

	// Represents either a sprite, an animation, or null.
	public class SpriteAnimation {

		// The object that can be either a sprite or an animation.
		public object value;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public SpriteAnimation() {
			this.value = null;
		}

		public SpriteAnimation(Sprite sprite) {
			this.value = sprite;
		}

		public SpriteAnimation(Animation animation) {
			this.value = animation;
		}

		public SpriteAnimation(SpriteAnimation spriteAnimation) {
			this.value = spriteAnimation.value;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Set(Sprite sprite) {
			this.value = sprite;
		}

		public void Set(Animation animation) {
			this.value = animation;
		}

		public void Set(SpriteAnimation spriteAnimation) {
			if (spriteAnimation != null)
				value = spriteAnimation.value;
			else
				value = null;
		}

		public void SetNull() {
			this.value = null; 
		}


		//-----------------------------------------------------------------------------
		// Operators
		//-----------------------------------------------------------------------------

		public static implicit operator SpriteAnimation(Sprite sprite) {
			return new SpriteAnimation(sprite);
		}

		public static implicit operator SpriteAnimation(Animation animation) {
			return new SpriteAnimation(animation);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets if the value is a sprite.
		public bool IsSprite {
			get { return value is Sprite; }
		}

		// Gets if the value is an animation.
		public bool IsAnimation {
			get { return value is Animation; }
		}

		// Gets if the value is null.
		public bool IsNull {
			get { return value == null; }
		}

		// Gets the value as an object.
		public object Value {
			get { return value; }
			set {
				this.value = value;
			}
		}

		// Gets or sets the value as a sprite.
		public Sprite Sprite {
			get {
				if (value is Sprite)
					return (Sprite)value;
				return null;
			}
			set { this.value = value; }
		}

		// Gets or sets the value as an animation.
		public Animation Animation {
			get {
				if (value is Animation)
					return (Animation)value;
				return null;
			}
			set { this.value = value; }
		}
	}
}
