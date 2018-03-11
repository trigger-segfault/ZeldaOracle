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

		private enum Modes {
			Root,
			Image,
		}

		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public ImageSR() {
			
			//=====================================================================================
			AddCommand("IMAGE", (int) Modes.Root,
				"string filePath",
				"string name, string filePath",
			delegate(CommandParam parameters) {
				string imageName = parameters.GetString(0);
				string imagePath = imageName;
				if (parameters.ChildCount == 2)
					imagePath = parameters.GetString(1);

				Image image = Resources.LoadImage(Resources.ImageDirectory + imagePath, false);
				AddResource<Image>(imageName, image);
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {

		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new ImageSR();
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>The mode of the Image script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
