using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZeldaResources = ZeldaOracle.Common.Content.Resources;
using ZeldaImage = ZeldaOracle.Common.Graphics.Image;
using System.Windows.Media.Imaging;
using ZeldaEditor.Util;
using System.Windows;
using System.Windows.Controls;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;
using ZeldaOracle.Common.Geometry;
using Image = System.Windows.Controls.Image;

namespace ZeldaEditor.Control {

	public class UnmappedWpfSprite {
		public BitmapSource Bitmap { get; }
		public Point2I DrawOffset { get; }

		public UnmappedWpfSprite(BitmapSource bitmap, Point2I drawOffset) {
			this.Bitmap = bitmap;
			this.DrawOffset = drawOffset;
		}
	}

	public static class EditorResources {

		private static EditorControl editorControl;
		private static Dictionary<UnmappedSpriteLookup, UnmappedWpfSprite> unmappedSprites;


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		// Initialize the editor resources.
		public static void Initialize(EditorControl editorControl) {
			EditorResources.editorControl = editorControl;
			unmappedSprites = new Dictionary<UnmappedSpriteLookup, UnmappedWpfSprite>();
		}


		//-----------------------------------------------------------------------------
		// Image Resources
		//-----------------------------------------------------------------------------
		
		public static Canvas UnmapSprite(ISprite sprite) {
			return UnmapSprite(sprite, SpriteSettings.Default, GameData.PAL_TILES_DEFAULT, GameData.PAL_ENTITIES_DEFAULT);
		}

		public static Canvas UnmapSprite(ISprite sprite, SpriteSettings settings) {
			return UnmapSprite(sprite, settings, GameData.PAL_TILES_DEFAULT, GameData.PAL_ENTITIES_DEFAULT);
		}

		public static Canvas UnmapSprite(ISprite sprite, Palette tilePalette, Palette entityPalette) {
			return UnmapSprite(sprite, SpriteSettings.Default, tilePalette, entityPalette);
		}

		public static Canvas UnmapSprite(ISprite sprite, SpriteSettings settings, Palette tilePalette, Palette entityPalette) {
			UnmappedSpriteLookup lookup = new UnmappedSpriteLookup(sprite, settings, tilePalette, entityPalette);
			UnmappedWpfSprite unmappedSprite = null;
			if (!unmappedSprites.TryGetValue(lookup, out unmappedSprite)) {
				Graphics2D g = new Graphics2D();
				UnmappedSprite zeldaUnmappedSprite = Unmapping.UnmapSprite(g, sprite, settings, tilePalette, entityPalette);
				MemoryStream memoryStream = new MemoryStream();
				ZeldaImage image = zeldaUnmappedSprite.Image;
				image.Texture2D.SaveAsPng(memoryStream, image.Texture2D.Width, image.Texture2D.Height);
				BitmapSource bitmap = BitmapFactory.LoadSourceFromStream(memoryStream);
				bitmap.Freeze();
				unmappedSprite = new UnmappedWpfSprite(bitmap, zeldaUnmappedSprite.DrawOffset);
				unmappedSprites.Add(lookup, unmappedSprite);
			}
			
			Canvas canvas = new Canvas();
			canvas.HorizontalAlignment = HorizontalAlignment.Left;
			canvas.VerticalAlignment = VerticalAlignment.Top;

			Image canvasImage = new Image();
			canvasImage.Stretch = System.Windows.Media.Stretch.Uniform;
			canvasImage.Source = unmappedSprite.Bitmap;
			canvas.Children.Add(canvasImage);
			Canvas.SetLeft(canvasImage, unmappedSprite.DrawOffset.X);
			Canvas.SetTop(canvasImage, unmappedSprite.DrawOffset.Y);
			
			return canvas;
		}
	}
}
