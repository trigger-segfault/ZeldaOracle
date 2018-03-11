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

		private enum Modes {
			Root,
			Model,
		}

		private CollisionModel model;
		private string	modelName;
		private TemporaryResources resources;


		//-----------------------------------------------------------------------------
		// Override
		//-----------------------------------------------------------------------------

		public CollisionModelSR(TemporaryResources resources = null) {

			this.resources	= resources;
			
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
