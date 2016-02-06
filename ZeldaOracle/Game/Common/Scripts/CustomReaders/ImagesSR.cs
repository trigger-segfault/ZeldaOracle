using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripts {

	public class ImagesSR : NewScriptReader {

		private Image	image;
		private Image	imageTail;
		private string imageName;
		private TemporaryResources resources;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public ImagesSR(TemporaryResources resources = null) {

			this.resources	= resources;

			// Image <name>
			// Image <name> <file-path>
			AddCommand("Image", 
				"string name",
				"string name, string filePath",
			delegate(CommandParam parameters) {
				imageName	= parameters.GetString(0);
				image		= null;
				imageTail	= null;

				if (parameters.ChildCount == 2) {
					image = Resources.LoadImage(Resources.ImageDirectory + parameters.GetString(1), false);
					imageTail = image;
				}
			});

			// End
			AddCommand("End", "", delegate(CommandParam parameters) {
				if (image != null) {
					Resources.AddImage(imageName, image);
					image = null;
				}
			});

			// Variant <name> <file-path>
			AddCommand("Variant",
				"string name, string filePath",
			delegate(CommandParam parameters) {
				Image variant = Resources.LoadImage(Resources.ImageDirectory + parameters.GetString(1), false);
				variant.VariantName	= parameters.GetString(0);

				if (imageTail != null)
					imageTail.NextVariant = variant;
				else
					image = variant;
				imageTail = variant;
			});
		}

		// Begins reading the script.
		protected override void BeginReading() {
			imageName	= "";
			image		= null;
			imageTail	= null;
		}

		// Ends reading the script.
		protected override void EndReading() {

		}
	}
}
