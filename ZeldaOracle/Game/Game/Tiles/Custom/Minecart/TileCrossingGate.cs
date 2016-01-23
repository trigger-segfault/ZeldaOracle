using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;

namespace ZeldaOracle.Game.Tiles {
	/*
	 * Sprite Index:
	 * 0 - horizontal
	 * 1 - vertical
	 * 2 - right/up
	 * 3 - up/left
	 * 4 - left/down
	 * 5 - down/right
	*/

	public class TileCrossingGate : Tile, ZeldaAPI.CrossingGate {

		public const string PROP_RAISED			= "raised";
		public const string PROP_FACING_LEFT	= "face_left";


		private Tile dummySolidTile;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public TileCrossingGate() {
			animationPlayer = new AnimationPlayer();
		}


		//-----------------------------------------------------------------------------
		// Crossing Gate methods
		//-----------------------------------------------------------------------------

		public void Raise() {
			if (!IsRaised) {
				Properties.Set("raised", true);
				dummySolidTile.IsSolid = false;
				animationPlayer.Play(GameData.ANIM_TILE_CROSSING_GATE_RAISE);
				AudioSystem.PlaySound(GameData.SOUND_CROSSING_GATE);
			}
		}
		
		public void Lower() {
			if (IsRaised) {
				Properties.Set("raised", false);
				dummySolidTile.IsSolid = true;
				animationPlayer.Play(GameData.ANIM_TILE_CROSSING_GATE_LOWER);
				AudioSystem.PlaySound(GameData.SOUND_CROSSING_GATE);
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnInitialize() {
			if (IsRaised)
				animationPlayer.Play(GameData.ANIM_TILE_CROSSING_GATE_RAISE);
			else
				animationPlayer.Play(GameData.ANIM_TILE_CROSSING_GATE_LOWER);
			
			bool isFacingLeft = Properties.GetBoolean("face_left", false);

			animationPlayer.PlaybackTime = animationPlayer.Animation.Duration;
			animationPlayer.SubStripIndex = (isFacingLeft ? 1 : 0);

			CollisionModel = (isFacingLeft ? GameData.MODEL_EDGE_W : GameData.MODEL_EDGE_E);

			Point2I trackLocation = Location;
			if (isFacingLeft)
				trackLocation.X -= 1;
			else
				trackLocation.X += 1;

			// Create the dummy tile to serve as the solid
			dummySolidTile = Tile.CreateTile(new TileData());
			dummySolidTile.IsSolid			= true;
			dummySolidTile.CollisionModel	= new CollisionModel(new Rectangle2I(0, 0, 16, 8));
			dummySolidTile.ClingWhenStabbed	= false;
			dummySolidTile.IsSolid			= !IsRaised;
			RoomControl.PlaceTile(dummySolidTile, trackLocation, Layer);			
		}

		public override void Update() {
			base.Update();
			
			animationPlayer.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsRaised {
			get { return Properties.GetBoolean("raised", false); }
		}
	}
}
