using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Input;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Items.Weapons {
	public class ItemBow : Item {


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public ItemBow() : base() {
			id			= "item_bow";
			name		= new string[] { "Wooden Bow" };
			description	= new string[] { "Weapon of a marksman." };
			maxLevel	= 0;
			currentAmmo	= 0;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		// Called when the items button is pressed (A or B).
		public override void OnButtonPress() {
			// Shoot an arrow!
			
			Projectile projectile = new Projectile();
				
			// General
			projectile.Owner			= Player;
			projectile.Position			= new Vector2F(Player.X, Player.Y - 8) + (Directions.ToVector(Player.MoveDirection) * 8.0f);
			projectile.ZPosition		= player.ZPosition;
			projectile.Angle			= Directions.ToAngle(player.MoveDirection);
			projectile.Physics.Velocity	= Directions.ToVector(Player.MoveDirection) * 3.0f;

			player.Direction = player.MoveDirection;

			// Graphics.
			projectile.Graphics.SubStripIndex = projectile.Angle;
			projectile.Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW);

			// Physics.
			projectile.Physics.CollisionBox	= new Rectangle2F(-2, -2, 4, 4);
			projectile.EnablePhysics(PhysicsFlags.CollideWorld | PhysicsFlags.LedgePassable |
								PhysicsFlags.HalfSolidPassable | PhysicsFlags.DestroyedOutsideRoom);

			// Crash event.
			Vector2F v = projectile.Physics.Velocity;
			projectile.EventCollision += delegate() {
				// Create crash effect.
				Effect effect = new Effect();
				effect.Position = projectile.Position;
				effect.CreateDestroyTimer(32);
					
				effect.Physics.Velocity		= -(v.Normalized) * 0.25f;
				effect.Physics.ZVelocity	= 1;
				effect.Physics.Gravity		= 0.07f;
				effect.EnablePhysics(PhysicsFlags.HasGravity);
					
				effect.Graphics.IsShadowVisible = false;
				effect.Graphics.PlayAnimation(GameData.ANIM_PROJECTILE_PLAYER_ARROW_CRASH);

				RoomControl.SpawnEntity(effect);
				projectile.Destroy();
			};

			RoomControl.SpawnEntity(projectile);

			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_THROW);
			player.BeginState(new PlayerBusyState(10));
		}

	}
}
