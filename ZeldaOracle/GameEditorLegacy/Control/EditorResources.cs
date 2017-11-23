using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZeldaResources = ZeldaOracle.Common.Content.Resources;
using ZeldaImage = ZeldaOracle.Common.Graphics.Image;

namespace ZeldaEditor.Control {
	public static class EditorResources {

		private static Dictionary<ZeldaImage, Bitmap> resourceBitmaps;
		
		
		//-----------------------------------------------------------------------------
		// Initialization
		//-----------------------------------------------------------------------------

		// Initialize the editor resources.
		public static void Initialize() {
			resourceBitmaps = new Dictionary<ZeldaImage, Bitmap>();
		}


		//-----------------------------------------------------------------------------
		// Image Resources
		//-----------------------------------------------------------------------------


		// Get a bitmap from an image resource name.
		public static Bitmap GetBitmap(string imageName) {
			return GetBitmap(ZeldaResources.GetImage(imageName));
		}


		// Get a bitmap from an image resource.
		public static Bitmap GetBitmap(ZeldaImage image) {
			// If the image's bitmap is already loaded, then return it.
			if (resourceBitmaps.ContainsKey(image))
				return resourceBitmaps[image];

			// Save the XNA image to memory as a png, and load it as a bitmap.
			MemoryStream ms = new System.IO.MemoryStream();
			image.Texture.SaveAsPng(ms, image.Texture.Width, image.Texture.Height);
			Bitmap bitmap = new System.Drawing.Bitmap(ms);

			// Add the new bitmap to the resource map.
			resourceBitmaps[image] = bitmap;
			return bitmap;
		}
	}
}
