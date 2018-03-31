using ZeldaOracle.Common.Conscripts.Commands;
using ZeldaOracle.Common.Geometry;

namespace ZeldaOracle.Common.Conscripts.CustomReaders {
	public class CollisionModelSR : ConscriptRunner {

		private enum Modes {
			Root,
			Model,
		}

		private CollisionModel model;
		private string	modelName;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public CollisionModelSR() {
			
			//=====================================================================================
			AddCommand("MODEL", (int) Modes.Root,
				"string name",
			delegate(CommandParam parameters) {
				modelName = parameters.GetString(0);
				model = new CollisionModel();
				AddResource(modelName, model);
				Mode = Modes.Model;
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Model,
				"",
			delegate(CommandParam parameters) {
				model = null;
				Mode = Modes.Root;
			});
			//=====================================================================================
			AddCommand("ADD", (int) Modes.Model,
				"Rectangle box",
			delegate(CommandParam parameters) {
				model.AddBox(parameters.GetRectangle(0));
			});
			//=====================================================================================
			AddCommand("COMBINE", (int) Modes.Model,
				"string modelName, Point offset = (0, 0)",
			delegate (CommandParam parameters) {
				model.Combine(
					GetResource<CollisionModel>(parameters.GetString(0)),
					parameters.GetPoint(1));
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void OnBeginReading() {
			modelName	= "";
			model		= null;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void OnEndReading() {
			OnBeginReading();
		}


		//-----------------------------------------------------------------------------
		// Internal Properties
		//-----------------------------------------------------------------------------

		/// <summary>The mode of the Collision Model script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}
}
