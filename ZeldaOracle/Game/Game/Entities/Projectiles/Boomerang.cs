using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Items;

namespace ZeldaOracle.Game.Entities.Projectiles {
	public class Boomerang : Projectile {

		private bool isReturning;
		private float speed;
		private int timer;
		private int returnDelay;
		private int level;


		public Boomerang(int level) {
			this.level = level;

			if (level == Item.Level1) {
				speed = GameSettings.PROJECTILE_BOOMERANG_SPEED_1;
				returnDelay = GameSettings.PROJECTILE_BOOMERANG_RETURN_DELAY_1;
			}
			else {
				speed = GameSettings.PROJECTILE_BOOMERANG_SPEED_2;
				returnDelay = GameSettings.PROJECTILE_BOOMERANG_RETURN_DELAY_2;
			}

			// Physics.
			Physics.CollisionBox		= new Rectangle2F(-1, -1, 2, 2);
			Physics.SoftCollisionBox	= new Rectangle2F(-1, -1, 2, 2);
			EnablePhysics(PhysicsFlags.CollideWorld | PhysicsFlags.LedgePassable |
					PhysicsFlags.HalfSolidPassable | PhysicsFlags.CollideRoomEdge);

			// Add a monster collision handler.
			Physics.AddCollisionHandler(typeof(Monster), CollisionBoxType.Soft, delegate(Entity entity) {
				Monster monster = entity as Monster;
				monster.TriggerInteraction(monster.HandlerBoomerang, this);
			});
		}


		public void BeginReturn() {
			isReturning = true;
			physics.Flags &= ~(PhysicsFlags.CollideRoomEdge | PhysicsFlags.CollideWorld);
		}

		public override void Initialize() {
			base.Initialize();

			if (level == Item.Level1)
				Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_BOOMERANG_1);
			else
				Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_BOOMERANG_2);

			isReturning = false;
			timer = 0;
			physics.Velocity = Angles.ToVector(angle) * speed;
		}

		public override void Update() {


			if (isReturning) {
				Vector2F trajectory = RoomControl.Player.Center - Center;
				if (trajectory.Length <= speed) {
					Destroy();
				}
				else {
					physics.Velocity = trajectory.Normalized * speed;
				}
			}
			else {
				timer++;
				if (physics.IsColliding) {
					CollisionType type = CollisionType.RoomEdge;
					// Create cling effect.
					for (int i = 0; i < 4; i++) {
						if (Physics.CollisionInfo[i].IsColliding) {
							type = Physics.CollisionInfo[i].Type;
							break;
						}
					}
					if (type == CollisionType.Tile) {
						Effect effect = new Effect(GameData.ANIM_EFFECT_CLING);
						RoomControl.SpawnEntity(effect, position, zPosition);
					}
					BeginReturn();
				}
				if (timer > returnDelay)
					BeginReturn();
			}

			base.Update();
		}
	}
}
