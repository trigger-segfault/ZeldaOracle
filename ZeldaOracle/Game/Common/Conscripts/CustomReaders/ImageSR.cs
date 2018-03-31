using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Graphics;

namespace ZeldaOracle.Common.Conscripts.CustomReaders {

	public class ImageSR : ConscriptRunner{

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

				Image image = Image.FromContent(Resources.ImageDirectory + imagePath);
				AddResource<Image>(imageName, image);
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void OnBeginReading() {
			
		}

		/// <summary>Ends reading the script.</summary>
		protected override void OnEndReading() {
			OnBeginReading();
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
