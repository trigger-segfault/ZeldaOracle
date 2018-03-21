using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Worlds;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Tiles.ActionTiles {

	public class ActionTileDataInstance : BaseTileDataInstance {

		private Point2I			position;
		private ISprite			sprite;
		

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public ActionTileDataInstance() {

		}

		public ActionTileDataInstance(ActionTileData tileData) :
			base(tileData)
		{
			this.sprite		= tileData.Sprite;
		}

		public ActionTileDataInstance(ActionTileData tileData, Point2I position) :
			base(tileData) {
			this.position   = position;
			this.sprite     = (tileData != null ? tileData.Sprite : null);
		}

		public ActionTileDataInstance(ActionTileDataInstance actionTile) :
			this()
		{
			Clone(actionTile);
		}

		public override void Clone(BaseTileDataInstance copy) {
			base.Clone(copy);
			if (copy is ActionTileDataInstance) {
				this.position	= ((ActionTileDataInstance) copy).position;
				this.sprite		= ((ActionTileDataInstance) copy).sprite;
			}
		}
		
		public override BaseTileDataInstance Duplicate() {
			ActionTileDataInstance copy = new ActionTileDataInstance();
			copy.Clone(this);
			return copy;
		}


		//-----------------------------------------------------------------------------
		// Overridden Properties
		//-----------------------------------------------------------------------------


		/// <summary>Gets or sets the pixel position of the action in the room.</summary>
		public override Point2I Position {
			get { return position; }
			set { position = value; }
		}

		/// <summary>Gets the pixel position of the action from the start of the
		/// level.</summary>
		public override Point2I LevelPosition {
			get { return room.LevelPosition + Position; }
		}

		/// <summary>Gets the pixel bounds of the action in the room.</summary>
		public override Rectangle2I Bounds {
			get { return new Rectangle2I(Position, PixelSize); }
		}

		/// <summary>Gets the pixel bounds of the action in the level.</summary>
		public override Rectangle2I LevelBounds {
			get { return new Rectangle2I(LevelPosition, PixelSize); }
		}

		/// <summary>Gets or sets the size of the action in pixels.</summary>
		public override Point2I PixelSize {
			get {
				return GMath.Max(Point2I.One, properties.Get("size",
					(Point2I) GameSettings.TILE_SIZE));
			}
			set { properties.Set("size", value); }
		}

		/// <summary>Gets or sets the size of the action in tiles.</summary>
		public override Point2I TileSize {
			get {
				return (PixelSize + GameSettings.TILE_SIZE - 1) /
					GameSettings.TILE_SIZE;
			}
			set { PixelSize = value * GameSettings.TILE_SIZE; }
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		/// <summary>Gets or sets the ActionTileData this instance was constructed
		/// from.</summary>
		public ActionTileData ActionTileData {
			get { return (ActionTileData) tileData; }
			set { base.BaseData = value; }
		}

		public int SubStripIndex {
			get { return properties.GetInteger("substrip_index"); }
			set { properties.Set("substrip_index", value); }
		}
	}
}
