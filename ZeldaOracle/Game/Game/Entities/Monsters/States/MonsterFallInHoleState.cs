using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Tiles;

namespace ZeldaOracle.Game.Entities.Monsters.States {
	
	public class MonsterFallInHoleState : MonsterState {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterFallInHoleState() {
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(MonsterState previousState) {
			monster.Graphics.AnimationPlayer.Speed = 2.0f;
			monster.Graphics.AnimationPlayer.Play();
			monster.Physics.Velocity = Vector2F.Zero;
			monster.IsPassable = true;
		}

		public override void OnEnd(MonsterState newState) {
			monster.Graphics.AnimationPlayer.Speed = 1.0f;
			monster.IsPassable = false;
		}

		public override void Update() {

			// Return to normal if not in a hole.
			if (!monster.Physics.IsInHole) {
				monster.BeginNormalState();
				return;
			}

			// Slip toward the hole's center.
			float slipSpeed = 0.2f;
			Point2I tileLocation = monster.RoomControl.GetTileLocation(monster.Position);
			Vector2F tilePosition = tileLocation * GameSettings.TILE_SIZE;
			Vector2F tileCenter = tilePosition + new Vector2F(8, 8);
			Rectangle2F holeRect = new Rectangle2F(tilePosition.X + 6, tilePosition.Y + 8, 4, 6);
			
			bool fallInHole = holeRect.Contains(monster.Position);

			Vector2F trajectory = tileCenter - monster.Center;
			if (trajectory.Length > slipSpeed) {
				monster.Physics.Velocity = (tileCenter - monster.Center).Normalized * slipSpeed;
			}
			else {
				fallInHole = true;
				monster.Physics.Velocity = Vector2F.Zero;
				monster.SetPositionByCenter(tileCenter);
			}

			if (fallInHole) {
				monster.RoomControl.SpawnEntity(new EffectFallingObject(), monster.Center);
				AudioSystem.PlaySound(GameData.SOUND_OBJECT_FALL);
				monster.Destroy();
			}
		}
	}
}
