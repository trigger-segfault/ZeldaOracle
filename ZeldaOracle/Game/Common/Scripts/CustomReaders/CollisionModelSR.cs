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

			// Model <name>
			AddCommand("Model", "string name", delegate(CommandParam parameters) {
				modelName = parameters.GetString(0);
				model = new CollisionModel();
			});

			// End
			AddCommand("End", "", delegate(CommandParam parameters) {
				if (model != null) {
					Resources.AddResource(modelName, model);
					model = null;
				}
			});

			// Add x, y, width, height
			AddCommand("Add", "int x, int y, int width, int height", delegate(CommandParam parameters) {
				model.AddBox(
					parameters.GetInt(0),
					parameters.GetInt(1),
					parameters.GetInt(2),
					parameters.GetInt(3));
			});

			// Add x, y, width, height
			AddCommand("Test",
			//"float x, string y, bool width, (int a, int b, (string c)) = (0, 1, (hello)), float height = 2.5f",
			"float x, string y, bool width, (int a, int b, (string c)), float height",
			delegate(CommandParam parameters) {
				Console.WriteLine(CommandParamParser.ToString(parameters));
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
