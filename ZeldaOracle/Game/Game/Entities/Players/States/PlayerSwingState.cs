using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities.Players.States {
	public class PlayerSwingState : PlayerState {

		private const int SWING_TILE_PEAK_DELAY = 6;

		private readonly int[] SWING_ENTITY_PEAK_DELAYS = { 1, 4, 7 };

		private PlayerState nextState;
		private Animation weaponAnimation;
		private int equipSlot;
		private bool hasPeaked;

		private readonly Point2I[] SWING_OFFSETS = {
			new Point2I(0, 8),
			new Point2I(0, 16),
			new Point2I(8, 16),
			new Point2I(16, 16)
		};

		private readonly Rectangle2I swingStartTop = new Rectangle2I(-9, -20, 18, 14);
		private readonly Rectangle2I swingMidTopRight = new Rectangle2I(9, -18, 11, 14);
		private readonly Rectangle2I swingEndRight = new Rectangle2I(10, -4, 18, 13);

		private readonly Rectangle2I[] SWING_BASIC = new Rectangle2I[] {
			new Rectangle2I(10, -4, 18, 13),
			new Rectangle2I(9, -18, 16, 16),
			new Rectangle2I(-10, -26, 12, 19),
			new Rectangle2I(-20, -18, 16, 16),
			new Rectangle2I(-28, -4, 18, 13),
			new Rectangle2I(-20, 10, 16, 16),
			new Rectangle2I(-4, 12, 13, 19),
			new Rectangle2I(4, 9, 16, 16)

		};

		private readonly Rectangle2I[] SWING_UP = {
			new Rectangle2I(10, -11, 12, 19),
			new Rectangle2I(6, -18, 14, 15),
			new Rectangle2I(-10, -26, 12, 19)
		};
		private readonly Rectangle2I[] SWING_DOWN = {
			new Rectangle2I(-21, -9, 12, 19),
			new Rectangle2I(-20, 10, 14, 15),
			new Rectangle2I(-3, 12, 12, 19)
		};
		private readonly Rectangle2I[] SWING_RIGHT = {
			new Rectangle2I(-9, -20, 18, 13),
			new Rectangle2I(6, -18, 14, 15),
			new Rectangle2I(10, -4, 18, 13)
		};
		private readonly Rectangle2I[] SWING_LEFT = {
			new Rectangle2I(-9, -20, 18, 13),
			new Rectangle2I(-20, -18, 14, 15),
			new Rectangle2I(-28, -4, 18, 13)
		};

		private readonly Rectangle2I[,] SWING_DIRECTIONS = new Rectangle2I[4,3] {
			{
				new Rectangle2I(-9, -20, 18, 13),
				new Rectangle2I(6, -18, 14, 15),
				new Rectangle2I(10, -4, 18, 13)
			}, {
				new Rectangle2I(10, -11, 12, 19),
				new Rectangle2I(6, -18, 14, 15),
				new Rectangle2I(-10, -26, 12, 19)
			}, {
				new Rectangle2I(-9, -20, 18, 13),
				new Rectangle2I(-20, -18, 14, 15),
				new Rectangle2I(-28, -4, 18, 13)
			}, {
				new Rectangle2I(-21, -9, 12, 19),
				new Rectangle2I(-20, 10, 14, 15),
				new Rectangle2I(-3, 12, 12, 19)
			}
		};
	
		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwingState() {
			this.weaponAnimation	= null;
			this.nextState			= null;
			this.equipSlot			= 0;
			this.hasPeaked			= false;
		}
		
		
		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------

		public virtual void OnSwingTilePeak() {
			// TODO: Create a sword subclass for this and override this method?
			if (player.IsInAir)
				return;
			Vector2F hitPoint = player.Center + (Directions.ToVector(player.Direction) * 13);
			Point2I location = player.RoomControl.GetTileLocation(hitPoint);
			if (player.RoomControl.IsTileInBounds(location)) {
				Tile tile = player.RoomControl.GetTopTile(location);
				if (tile != null)
					tile.OnSwordHit();
			}
		}
		public virtual void OnSwingEntityPeak(int swingIndex) {
			// TODO: Create a sword subclass for this and override this method?
			if (player.IsInAir)
				return;
			Rectangle2F hitRect = (Rectangle2F)SWING_DIRECTIONS[player.Direction, swingIndex] + player.Center;
			for (int i = 0; i < player.RoomControl.Entities.Count; i++) {
				Entity e = player.RoomControl.Entities[i];
				if (e.Physics.PositionedSoftCollisionBox.Colliding(hitRect)) {
					if (e is Collectible && (e as Collectible).IsPickupable) {
						(e as Collectible).Collect();
					}
				}
			}
		}
		

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.Direction = player.UseDirection;
			player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_SWING);
			player.Movement.MoveCondition = PlayerMoveCondition.OnlyInAir;
			player.toolAnimation.Animation = weaponAnimation;
			player.toolAnimation.SubStripIndex = player.Direction;
			player.toolAnimation.Play();
			
			hasPeaked = false;

			AudioSystem.PlayRandomSound("Items/slash_1", "Items/slash_2", "Items/slash_3");
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
			player.toolAnimation.Animation = null;
		}

		public override void Update() {
			base.Update();

			// Check for the swing peak.
			if (player.toolAnimation.PlaybackTime == SWING_TILE_PEAK_DELAY)
				OnSwingTilePeak();

			for (int i = 0; i < 3; i++) {
				if (player.toolAnimation.PlaybackTime == SWING_ENTITY_PEAK_DELAYS[i])
					OnSwingEntityPeak(i);
			}

			// Reset the swing
			if (player.GameControl.Inventory.GetSlotButton(equipSlot).IsPressed()) {
				player.Direction = player.UseDirection;
				player.toolAnimation.SubStripIndex = player.Direction;
				player.Graphics.PlayAnimation();
				player.toolAnimation.Play();
				AudioSystem.PlayRandomSound("Items/slash_1", "Items/slash_2", "Items/slash_3");
			}

			// End the swing
			if (player.Graphics.IsAnimationDone)
				player.BeginState(nextState);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public PlayerState NextState {
			get { return nextState; }
			set { nextState = value; }
		}

		public Animation WeaponAnimation {
			get { return weaponAnimation; }
			set { weaponAnimation = value; }
		}

		public int EquipSlot {
			get { return equipSlot; }
			set { equipSlot = value; }
		}
	}
}
