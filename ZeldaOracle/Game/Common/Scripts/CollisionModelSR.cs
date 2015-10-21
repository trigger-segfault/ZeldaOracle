using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripts {

	public class CollisionModelSR : NewScriptReader {

		private CollisionModel model;
		private string	modelName;
		private TemporaryResources resources;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public CollisionModelSR(TemporaryResources resources = null) {

			this.resources	= resources;

			// Sprite <name>
			AddCommand("Model", delegate(CommandParam parameters) {
				modelName = parameters.GetString(0);
				model = new CollisionModel();
			});

			// End
			AddCommand("End", delegate(CommandParam parameters) {
				if (model != null) {
					Resources.AddResource(modelName, model);
					model = null;
				}
			});

			// Variant <name> <file-path>
			AddCommand("Add", delegate(CommandParam parameters) {
				model.AddBox(
					parameters.GetInt(0),
					parameters.GetInt(1),
					parameters.GetInt(2),
					parameters.GetInt(3));
			});
		}

		// Begins reading the script.
		protected override void BeginReading() {
			modelName	= "";
			model		= null;
		}

		// Ends reading the script.
		protected override void EndReading() {

		}
	}
}
