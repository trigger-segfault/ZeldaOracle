using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Worlds;

namespace ZeldaOracle.Game.Tiles {
	
	public class TileInteractable : Tile {

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------
		
		public TileInteractable() {

		}


		//-----------------------------------------------------------------------------
		// Interaction Methods
		//-----------------------------------------------------------------------------
		
		// Called when the player hits this tile with the sword.
		public override void OnSwordHit() {
			if (!isMoving && SpecialFlags.HasFlag(TileSpecialFlags.Cuttable))
				Break(true);
		}

		// Called when the player hits this tile with the sword.
		public override void OnBombExplode() {
			if (!isMoving && SpecialFlags.HasFlag(TileSpecialFlags.Bombable))
				Break(true);
		}

		// Called when the tile is burned by a fire.
		public override void OnBurn() {
			if (!isMoving && SpecialFlags.HasFlag(TileSpecialFlags.Burnable))
				Break(true);
		}

		// Called when the player wants to push the tile.
		public override bool OnPush(int direction, float movementSpeed) {
			return Move(direction, 1, movementSpeed);
		}
		
		// Called when the tile is pushed into a hole.
		public override void OnFallInHole() {
			RoomControl.SpawnEntity(new EffectFallingObject(), Center);
			RoomControl.RemoveTile(this);
		}
		
		// Called when the tile is pushed into water.
		public override void OnFallInWater() {
			RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_WATER_SPLASH), Center);
			RoomControl.RemoveTile(this);
		}
		
		// Called when the tile is pushed into lava.
		public override void OnFallInLava() {
			RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_LAVA_SPLASH), Center);
			RoomControl.RemoveTile(this);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsSwitchable {
			get { return SpecialFlags.HasFlag(TileSpecialFlags.Switchable); }
		}

		public bool StaysOnSwitch {
			get { return SpecialFlags.HasFlag(TileSpecialFlags.SwitchStays); }
		}

		public bool BreaksOnSwitch {
			get { return !SpecialFlags.HasFlag(TileSpecialFlags.SwitchStays); }
		}
	}
}
