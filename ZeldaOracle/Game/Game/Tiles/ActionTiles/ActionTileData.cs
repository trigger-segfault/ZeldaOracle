using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Graphics.Sprites;

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
			
			properties.Set("image_variant", 0)
				.SetDocumentation("Image Variant ID", "", "", "Internal",
				"The image variant to draw the sprtie with.", true, false);
		}
		
		public ActionTileData(Type type) :
			this()
		{
			this.type = type;
		}
		
		
		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------
		
		public override ISprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}

		public override Point2I Size {
			get { return size; }
			set { size = value; }
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
