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

		//private static Dictionary<KeyValuePair<ZeldaImage, int>, BitmapSource> resourceImages;
		//private static Dictionary<KeyValuePair<SpritePart, int>, BitmapSource> resourceSprites;

		private static Dictionary<UnmappedSpriteLookup, UnmappedWpfSprite> unmappedSprites;
		private static EditorControl editorControl;


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		// Initialize the editor resources.
		public static void Initialize(EditorControl editorControl) {
			//resourceImages = new Dictionary<KeyValuePair<ZeldaImage, int>, BitmapSource>();
			//resourceSprites = new Dictionary<KeyValuePair<SpritePart, int>, BitmapSource>();
			unmappedSprites = new Dictionary<UnmappedSpriteLookup, UnmappedWpfSprite>();
			EditorResources.editorControl = editorControl;
		}


		//-----------------------------------------------------------------------------
		// Image Resources
		//-----------------------------------------------------------------------------


		// Get a bitmap from an image resource name.
		/*public static BitmapSource GetImage(string imageName) {
			return GetImage(ZeldaResources.GetImage(imageName));
		}


		// Get a bitmap from an image resource.
		public static BitmapSource GetImage(ZeldaImage image) {
			var pair = new KeyValuePair<ZeldaImage, int>(image);
			// If the image's bitmap is already loaded, then return it.
			if (resourceImages.ContainsKey(pair))
				return resourceImages[pair];

			// Save the XNA image to memory as a png, and load it as a bitmap.
			MemoryStream memoryStream = new MemoryStream();
			image.Texture.SaveAsPng(memoryStream, image.Texture.Width, image.Texture.Height);
			BitmapSource bitmap = BitmapFactory.LoadSourceFromStream(memoryStream);
			bitmap.Freeze();
			// Add the new bitmap to the resource map.
			resourceImages[pair] = bitmap;
			return bitmap;
		}
		// Get a bitmap from an image resource.
		public static Image GetSpritePart(SpritePart sprite, int variantID = -1) {
			BitmapSource bitmap;
			// If the image's bitmap is already loaded, then return it.
			var pair = new KeyValuePair<SpritePart, int>(sprite, variantID);
			if (resourceSprites.ContainsKey(pair)) {
				bitmap = resourceSprites[pair];
			}
			else {
				BitmapSource image = GetImage(sprite.Image, variantID);
				bitmap = new CroppedBitmap(image, new Int32Rect(sprite.SourceRect.X, sprite.SourceRect.Y, sprite.SourceRect.Width, sprite.SourceRect.Height));
				bitmap.Freeze();
				// Add the new bitmap to the resource map.
				resourceSprites[pair] = bitmap;
			}
			Image spriteImage = new Image();
			spriteImage.Source = bitmap;
			Canvas.SetLeft(spriteImage, sprite.DrawOffset.X);
			Canvas.SetTop(spriteImage, sprite.DrawOffset.Y);
			return spriteImage;
		}
		// Get a bitmap from an image resource name.
		public static Canvas GetSprite(string spriteName, int variantID = -1) {
			return GetSprite(ZeldaResources.GetSprite(spriteName), variantID);
		}*/
		// Get a bitmap from an image resource.
		
		public static Canvas UnmapSprite(ISprite sprite) {
			return UnmapSprite(sprite, SpriteDrawSettings.Default, GameData.PAL_TILES_DEFAULT, GameData.PAL_ENTITIES_DEFAULT);
		}

		public static Canvas UnmapSprite(ISprite sprite, SpriteDrawSettings settings) {
			return UnmapSprite(sprite, settings, GameData.PAL_TILES_DEFAULT, GameData.PAL_ENTITIES_DEFAULT);
		}

		public static Canvas UnmapSprite(ISprite sprite, Palette tilePalette, Palette entityPalette) {
			return UnmapSprite(sprite, SpriteDrawSettings.Default, tilePalette, entityPalette);
		}

		public static Canvas UnmapSprite(ISprite sprite, SpriteDrawSettings settings, Palette tilePalette, Palette entityPalette) {
			UnmappedSpriteLookup lookup = new UnmappedSpriteLookup(sprite, settings, tilePalette, entityPalette);
			UnmappedWpfSprite unmappedSprite = null;
			if (!unmappedSprites.TryGetValue(lookup, out unmappedSprite)) {
				Graphics2D g = new Graphics2D(ZeldaResources.SpriteBatch);
				UnmappedSprite zeldaUnmappedSprite = Unmapping.UnmapSprite(g, sprite, settings, tilePalette, entityPalette);
				MemoryStream memoryStream = new MemoryStream();
				ZeldaImage image = zeldaUnmappedSprite.Image;
				image.Texture.SaveAsPng(memoryStream, image.Texture.Width, image.Texture.Height);
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
		// Get a bitmap from an image resource name.
		/*public static Canvas GetAnimation(string animationName, int variantID) {
			return GetAnimation(ZeldaResources.GetAnimation(animationName), variantID);
		}
		// Get a bitmap from an image resource.
		public static Canvas GetAnimation(ZeldaAnimation animation, int variantID) {
			//if (variantID == -1)
			//	variantID = animation.Frames[0].Image.VariantID;
			Canvas canvas = new Canvas();
			canvas.HorizontalAlignment = HorizontalAlignment.Left;
			canvas.VerticalAlignment = VerticalAlignment.Top;
			int time = 0;

			for (int i = 0; i < animation.FrameCount; ++i) {
				ZeldaAnimationFrame frame = animation.Frames[i];
				if (time < frame.StartTime)
					break;
				if (time < frame.StartTime + frame.Duration || (time >= animation.Duration && frame.StartTime + frame.Duration == animation.Duration)) {
					for (ZeldaSprite part = frame.Sprite; part != null; part = part.NextPart) {
						canvas.Children.Add(GetSpritePart(part, variantID));
					}
				}
			}

			return canvas;
		}*/
	}
}
