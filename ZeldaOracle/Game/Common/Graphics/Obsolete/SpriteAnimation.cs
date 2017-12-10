using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Graphics.Sprites;

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

		public SpriteAnimation(SpriteOld sprite) {
			this.value = sprite;
		}

		public SpriteAnimation(AnimationOld animation) {
			this.value = animation;
		}

		public SpriteAnimation(SpriteAnimation spriteAnimation) {
			this.value = spriteAnimation.value;
		}


		//-----------------------------------------------------------------------------
		// Mutators
		//-----------------------------------------------------------------------------

		public void Set(SpriteOld sprite) {
			this.value = sprite;
		}

		public void Set(AnimationOld animation) {
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

		public static implicit operator SpriteAnimation(SpriteOld sprite) {
			return new SpriteAnimation(sprite);
		}

		public static implicit operator SpriteAnimation(AnimationOld animation) {
			return new SpriteAnimation(animation);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		// Gets if the value is a sprite.
		public bool IsSprite {
			get { return value is SpriteOld; }
		}

		// Gets if the value is an animation.
		public bool IsAnimation {
			get { return value is AnimationOld; }
		}

		// Gets if the value is a sprite interface.
		public bool IsISprite {
			get { return value is ISprite; }
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
		public SpriteOld Sprite {
			get {
				if (value is SpriteOld)
					return (SpriteOld)value;
				return null;
			}
			set { this.value = value; }
		}

		// Gets or sets the value as a sprite interface.
		public ISprite ISprite {
			get {
				if (value is ISprite)
					return (ISprite) value;
				return null;
			}
			set { this.value = value; }
		}

		// Gets or sets the value as an animation.
		public AnimationOld Animation {
			get {
				if (value is AnimationOld)
					return (AnimationOld)value;
				return null;
			}
			set { this.value = value; }
		}
	}
}
