using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Game.ResourceData;
using ZeldaOracle.Game.Entities;

namespace ZeldaOracle.Game.Tiles.ActionTiles {
	
	public class ActionTileData : BaseTileData {

		private Point2I			size; // TODO: make this refer to pixels, not tiles.
		private Point2I			position;
		private ISprite			sprite;		// NOTE: This would only be visible in the editor.
		//private bool			isVisible;	// Is the action visible in-game?
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ActionTileData() {
			sprite			= null;
			position		= Point2I.Zero;
			size			= Point2I.One;

			// TODO: Switch this to color
			/*properties.Set("image_variant", 0)
				.SetDocumentation("Image Variant ID", "", "", "Internal",
				"The image variant to draw the sprtie with.", true, false);*/
		}
		
		public ActionTileData(Type type) :
			this()
		{
			this.type = type;
		}

		/// <summary>Clones the specified action data.</summary>
		public override void Clone(BaseResourceData baseCopy) {
			base.Clone(baseCopy);

			ActionTileData copy = (ActionTileData) baseCopy;
			size				= copy.size;
			position			= copy.position;
			sprite				= copy.sprite;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes data after a change in the final type.<para/>
		/// This needs to be extended for each non-abstract class in order
		/// to make use of compile-time generic arguments within
		/// ResourceDataInitializing.InitializeData.</summary>
		public override void InitializeData(Type previousType) {
			ResourceDataInitializing.InitializeData(
				this, OutputType, Type, previousType);
		}

		/// <summary>Initializes data after a change in the final entity type.<para/>
		/// This needs to be extended for each non-abstract class in order
		/// to make use of compile-time generic arguments within
		/// ResourceDataInitializing.InitializeData.</summary>
		public override void InitializeEntityData(Type previousType) {
			ResourceDataInitializing.InitializeData(
				this, typeof(Entity), EntityType, previousType);
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets the base output type for this resource data.</summary>
		public override Type OutputType {
			get { return typeof(ActionTile); }
		}

		public override ISprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}

		public override Point2I Size {
			get { return size; }
			set { size = GMath.Max(Point2I.One, value); }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Point2I Position {
			get { return position; }
			set { position = value; }
		}
	}
}
