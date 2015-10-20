using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Tiles;
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
			Physics.AddCollisionHandler(typeof(Monster), CollisionBoxType.Soft, CollideWithMonster);
			Physics.AddCollisionHandler(typeof(Player), CollisionBoxType.Soft, CollideWithPlayer);

			// TODO: bomb explosion collision ignores z-position.
		}
		

		//-----------------------------------------------------------------------------
		// Collision Handlers
		//-----------------------------------------------------------------------------

		private void CollideWithMonster(Entity entity) {
			float playbackTime = Graphics.AnimationPlayer.PlaybackTime;
			if (playbackTime > 10) {
				Monster monster = entity as Monster;
				monster.TriggerInteraction(monster.HandlerBombExplosion, this);
			}
		}

		private void CollideWithPlayer(Entity entity) {
			float playbackTime = Graphics.AnimationPlayer.PlaybackTime;
			if (playbackTime > 10) {
				Player player = entity as Player;
				player.Hurt(new DamageInfo(2, Center));
			}
		}
	}
}
