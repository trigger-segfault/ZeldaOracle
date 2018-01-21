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

namespace ZeldaEditor.Control {

	public static class EditorResources {

		private static Dictionary<KeyValuePair<ZeldaImage, int>, BitmapSource> resourceImages;
		private static Dictionary<KeyValuePair<SpritePart, int>, BitmapSource> resourceSprites;


		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		// Initialize the editor resources.
		public static void Initialize() {
			resourceImages = new Dictionary<KeyValuePair<ZeldaImage, int>, BitmapSource>();
			resourceSprites = new Dictionary<KeyValuePair<SpritePart, int>, BitmapSource>();
		}


		//-----------------------------------------------------------------------------
		// Image Resources
		//-----------------------------------------------------------------------------


		// Get a bitmap from an image resource name.
		public static BitmapSource GetImage(string imageName, int variantID = -1) {
			return GetImage(ZeldaResources.GetImage(imageName), variantID);
		}


		// Get a bitmap from an image resource.
		public static BitmapSource GetImage(ZeldaImage image, int variantID = -1) {
			if (variantID == -1)
				variantID = image.VariantID;
			var pair = new KeyValuePair<ZeldaImage, int>(image, variantID);
			// If the image's bitmap is already loaded, then return it.
			if (resourceImages.ContainsKey(pair))
				return resourceImages[pair];

			// Save the XNA image to memory as a png, and load it as a bitmap.
			MemoryStream memoryStream = new MemoryStream();
			image.GetVariant(variantID).Texture.SaveAsPng(memoryStream, image.Texture.Width, image.Texture.Height);
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
		}
		// Get a bitmap from an image resource.
		public static Canvas GetSprite(ISprite sprite, int variantID = -1, StyleDefinitions styleDefinitions = null) {
			Canvas canvas = new Canvas();
			canvas.HorizontalAlignment = HorizontalAlignment.Left;
			canvas.VerticalAlignment = VerticalAlignment.Top;
			/*for (ZeldaSprite part = sprite; part != null; part = part.NextPart) {
				canvas.Children.Add(GetSpritePart(part, variantID));
			}*/
			/*foreach (SpritePart part in sprite.GetParts(new SpriteDrawSettings(styleDefinitions, variantID))) {
				canvas.Children.Add(GetSpritePart(part, variantID));
			}*/
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
