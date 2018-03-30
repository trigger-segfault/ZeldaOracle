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
	/// <summary>The data structure detailing an action tile that is not confined
	/// to the tile grid.</summary>
	public class ActionTileData : BaseTileData {
		
		/// <summary>The sprite for the action tile data that is only visible in the
		/// editor.</summary>
		private ISprite sprite;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public ActionTileData() {
			sprite			= null;

			// TODO: Switch this to color
			/*properties.Set("image_variant", 0)
				.SetDocumentation("Image Variant ID", "", "", "Internal",
				"The image variant to draw the sprtie with.", true, false);*/

			properties.Set("pixel_size", new Point2I(GameSettings.TILE_SIZE))
				.SetDocumentation("Size", "General", "The size of the tile in pixels.").Hide();
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
			sprite				= copy.sprite;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		/// <summary>Initializes data after a change in the final type.<para/>
		/// This needs to be extended for each non-abstract class in order
		/// to make use of compile-time generic arguments within
		/// ResourceDataInitializing.InitializeData.</summary>
		protected override void InitializeData(Type previousType) {
			ResourceDataInitializing.InitializeData(
				this, OutputType, Type, previousType);
		}

		/// <summary>Initializes data after a change in the final entity type.<para/>
		/// This needs to be extended for each non-abstract class in order
		/// to make use of compile-time generic arguments within
		/// ResourceDataInitializing.InitializeData.</summary>
		protected override void InitializeEntityData(Type previousType) {
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

		/// <summary>Gets or sets the sprite of the action data.</summary>
		public override ISprite Sprite {
			get { return sprite; }
			set { sprite = value; }
		}

		/// <summary>Gets or sets the size of the action data in pixels.</summary>
		public override Point2I PixelSize {
			get {
				return GMath.Max(Point2I.One, properties.Get("size",
					(Point2I) GameSettings.TILE_SIZE));
			}
			set { properties.Set("size", value); }
		}

		/// <summary>Gets or sets the size of the action data in tiles.</summary>
		public override Point2I TileSize {
			get {
				return (PixelSize + GameSettings.TILE_SIZE - 1) /
					GameSettings.TILE_SIZE;
			}
			set { PixelSize = value * GameSettings.TILE_SIZE; }
		}
	}
}
