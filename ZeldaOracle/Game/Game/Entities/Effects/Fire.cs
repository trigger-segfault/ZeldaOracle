using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Collisions;

namespace ZeldaOracle.Game.Entities.Effects {
	public class Fire : Effect {

		private int timer;
		private bool isAbsorbed;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public Fire() :
			base(GameData.ANIM_EFFECT_SEED_EMBER)
		{
			EnablePhysics(PhysicsFlags.HasGravity);
			
			Physics.SoftCollisionBox = new Rectangle2F(-6, -6, 12, 12);

			Graphics.DrawOffset	= new Point2I(0, -2);
			Graphics.DepthLayer	= DepthLayer.EffectFire;
			isAbsorbed = false;
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			timer = 0;
		}

		public override void OnDestroyTimerDone() {
			// Burn tiles.
			Point2I location = RoomControl.GetTileLocation(position);
			if (RoomControl.IsTileInBounds(location)) {
				Tile tile = RoomControl.GetTopTile(location);
				if (tile != null)
					tile.OnBurn();
			}
			Destroy();
		}

		public override void Update() {
			timer++;

			if (timer > 3) {
				if (isAbsorbed) {
					Destroy();
					return;
				}
				else {
					// Collide with monsters.
					CollisionIterator iterator = new CollisionIterator(this, typeof(Monster), CollisionBoxType.Soft);
					for (iterator.Begin(); iterator.IsGood(); iterator.Next()) {
						Monster monster = iterator.CollisionInfo.Entity as Monster;
						monster.TriggerInteraction(InteractionType.Fire, this);
						if (IsDestroyed)
							return;
					}
				}
			}

			base.Update();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public bool IsAbsorbed {
			get { return isAbsorbed; }
			set { isAbsorbed = value; }
		}
	}
}
