using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Collisions;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Effects {
	public class BombExplosion : Effect {


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public BombExplosion() :
			base(GameData.ANIM_EFFECT_BOMB_EXPLOSION)
		{
			EnablePhysics(PhysicsFlags.None);
			Physics.CollisionBox		= new Rectangle2F(-12, -12, 24, 24);
			Physics.SoftCollisionBox	= new Rectangle2F(-12, -12, 24, 24);
		}
		

		//-----------------------------------------------------------------------------
		// Collision Handlers
		//-----------------------------------------------------------------------------

		public override void Update() {

			float playbackTime = Graphics.AnimationPlayer.PlaybackTime;

			if (playbackTime > 10) {
				// Collide with Monsters.
				CollisionIterator iterator = new CollisionIterator(this, typeof(Monster), CollisionBoxType.Soft, -1);
				for (iterator.Begin(); iterator.IsGood(); iterator.Next()) {
					Monster monster = iterator.CollisionInfo.Entity as Monster;
					monster.TriggerInteraction(monster.HandlerBombExplosion, this);
					if (IsDestroyed)
						return;
				}
				// Collide with the Player.
				if (Physics.IsCollidingWith(RoomControl.Player, CollisionBoxType.Soft, -1)) {
					RoomControl.Player.Hurt(new DamageInfo(2, Center));
				}
			}

			base.Update();
		}
	}
}
