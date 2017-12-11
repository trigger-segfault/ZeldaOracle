using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripts.Commands;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripts.CustomReaders {

	public class ImageSR : ScriptReader {

		private Image	image;
		private Image	imageTail;
		private string imageName;
		private TemporaryResources resources;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public ImageSR(TemporaryResources resources = null) {

			this.resources	= resources;
			
			//=====================================================================================
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
			//=====================================================================================
			AddCommand("End", "",
			delegate(CommandParam parameters) {
				if (image != null) {
					Resources.AddImage(imageName, image);
					image = null;
				}
			});
			//=====================================================================================
			AddCommand("Variant", "string name, string filePath",
			delegate(CommandParam parameters) {
				Image variant = Resources.LoadImage(Resources.ImageDirectory + parameters.GetString(1), false);
				variant.VariantName	= parameters.GetString(0);

				if (imageTail != null)
					imageTail.NextVariant = variant;
				else
					image = variant;
				imageTail = variant;
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			imageName	= "";
			image		= null;
			imageTail	= null;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {

		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new ImageSR();
		}
	}
}
