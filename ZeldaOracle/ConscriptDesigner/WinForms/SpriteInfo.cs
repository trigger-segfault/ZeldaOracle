using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ConscriptDesigner.WinForms {
	public class SpriteInfo {
		public string Name { get; set; }
		public ISprite Sprite { get; set; }
		public Rectangle2I Bounds { get; set; }

		public int SubstripIndex { get; set; }

		public bool HasSubstrips {
			get {
				if (Sprite is Animation)
					return (SubstripIndex != 0 || ((Animation) Sprite).HasSubstrips);
				return false;
			}
		}

		public SpriteInfo(string spriteName, ISprite sprite, int substripIndex = 0) {
			Name = spriteName;
			Sprite = sprite;
			Bounds = sprite.Bounds;
			SubstripIndex = substripIndex;
		}
	}
}
