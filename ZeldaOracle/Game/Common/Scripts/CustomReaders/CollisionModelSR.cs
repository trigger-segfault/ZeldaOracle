using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Scripts.Commands;
using ZeldaOracle.Game;

namespace ZeldaOracle.Common.Scripts.CustomReaders {

	public class CollisionModelSR : ScriptReader {

		private CollisionModel model;
		private string	modelName;
		private TemporaryResources resources;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public CollisionModelSR(TemporaryResources resources = null) {

			this.resources	= resources;
			
			//=====================================================================================
			AddCommand("Model", "string name",
			delegate(CommandParam parameters) {
				modelName = parameters.GetString(0);
				model = new CollisionModel();
			});
			//=====================================================================================
			AddCommand("End", "",
			delegate(CommandParam parameters) {
				if (model != null) {
					Resources.AddResource(modelName, model);
					model = null;
				}
			});
			//=====================================================================================
			AddCommand("Add", "int x, int y, int width, int height",
			delegate(CommandParam parameters) {
				model.AddBox(
					parameters.GetInt(0),
					parameters.GetInt(1),
					parameters.GetInt(2),
					parameters.GetInt(3));
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			modelName	= "";
			model		= null;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {

		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new CollisionModelSR();
		}
	}
}
