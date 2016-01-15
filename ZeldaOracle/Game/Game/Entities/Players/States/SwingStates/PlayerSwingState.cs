using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Main;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Game.Items.Weapons;
using ZeldaOracle.Game.Entities.Units;

namespace ZeldaOracle.Game.Entities.Players.States.SwingStates {
	public class PlayerSwingState : PlayerState {

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
		
		// Sword swinging collision boxes for each direction its swung.
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
		// Constants
		//-----------------------------------------------------------------------------
		
		// Collision boxes for the swing weapon for each of 8 angles.
		private readonly Rectangle2I[] SWING_ANGLE_BOXES = new Rectangle2I[] {
			new Rectangle2I(10, -11, 12, 19),
			new Rectangle2I(6, -18, 14, 15),
			new Rectangle2I(-9, -20, 18, 13),
			new Rectangle2I(-20, -18, 14, 15),
			new Rectangle2I(-21, -9, 12, 19),
			new Rectangle2I(-20, 10, 14, 15),
			new Rectangle2I(-4, 12, 13, 19), //// UNVERIFIED!!!
			new Rectangle2I(6, 10, 14, 15),
		};

		// Collision boxes for the swing weapon when lunged for each fo 4 directions.
		private readonly Rectangle2I[] SWING_LUNGE_BOXES = {
			new Rectangle2I(10, -4, 18, 13),
			new Rectangle2I(-10, -26, 12, 19),
			new Rectangle2I(-28, -4, 18, 13),
			new Rectangle2I(-3, 12, 12, 19)
		};

		private readonly Rectangle2I[,] SWING_TOOL_BOXES = new Rectangle2I[,] {
			{
				new Rectangle2I(-8, -8 - 10, 16, 10),
				new Rectangle2I(8, -8 - 10, 10, 16 + 6),
				new Rectangle2I(8, 8 - 10, 19, 10),
			}, {
				new Rectangle2I(8, -8, 10, 16),
				new Rectangle2I(-8 + 10 - 6, -8 - 10, 16 + 6, 10),
				new Rectangle2I(-8, -8 - 19, 10, 19),
			}, {
				new Rectangle2I(-8, -8 - 10, 16, 10),
				new Rectangle2I(-8 - 10, -8 - 10, 10, 16 + 6),
				new Rectangle2I(-8 - 19, 8 - 10, 19, 10),
			}, {
				new Rectangle2I(-8 - 10, -8, 10, 16),
				new Rectangle2I(-8 - 10, 8, 16 + 6, 10),
				new Rectangle2I(8 - 10, 8, 10, 19),
			}
		};

		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private ItemWeapon				weapon;
		private int						swingDirection;	 // The player the direction is in when he starts swinging.
		private int						swingAngleStart;
		private WindingOrder			swingAngleDirection;
		private int						swingAngle;
		private int						swingAngleIndex;
		private Dictionary<int, Action>	timedActions;

		// Settings.
		protected int				swingAnglePullBack;
		protected bool				lunge;
		protected bool				isReswingable;
		protected int[]				swingAngleDurations;
		protected Rectangle2I[]		swingWeaponCollisionBoxes;
		protected Rectangle2I[]		swingAngleCollisionBoxes; // Collision boxes for each angle of the weapon.
		protected Rectangle2I[]		swingLungeCollisionBoxes; // Collision boxes when lunging (for 4 directions).
		protected int[]				swingStartAngles;
		protected WindingOrder[]	swingWindingOrders;
		protected Animation			weaponSwingAnimation;
		protected Animation			playerSwingAnimation;

		protected Rectangle2I[,]	swingCollisionBoxes;

		protected UnitTool			playerTool;
	

		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public PlayerSwingState() {
			weapon						= null;
			isReswingable				= true;
			swingAngleDurations			= new int[] { 3, 3, 12 }; // Sword
			swingAngleCollisionBoxes	= SWING_ANGLE_BOXES;
			swingLungeCollisionBoxes	= SWING_LUNGE_BOXES;
			swingAnglePullBack			= 2;
			lunge						= true; // lunges forward 3 pixels.
			weaponSwingAnimation		= GameData.ANIM_SWORD_SWING;
			playerSwingAnimation		= GameData.ANIM_PLAYER_SWING;
			timedActions				= new Dictionary<int, Action>();

			swingCollisionBoxes			= SWING_TOOL_BOXES;

			swingWindingOrders = new WindingOrder[] {
				WindingOrder.Clockwise,
				WindingOrder.CounterClockwise,
				WindingOrder.CounterClockwise,
				WindingOrder.CounterClockwise
			};
		}
		
		
		//-----------------------------------------------------------------------------
		// Swing methods
		//-----------------------------------------------------------------------------

		// Add an action to be invoked at the given time of the swing.
		public void AddTimedAction(int time, Action action) {
			timedActions[time] = action;
		}

		private void Swing(int direction) {
			swingDirection		= direction;
			swingAngleDirection	= swingWindingOrders[swingDirection];
			swingAngleStart		= Directions.ToAngle(swingDirection);
			swingAngleStart		= Angles.Subtract(swingAngleStart, swingAnglePullBack, swingAngleDirection);
			swingAngle			= swingAngleStart;
			swingAngleIndex		= 0;

			player.Direction = direction;
			player.Graphics.PlayAnimation(playerSwingAnimation);

			playerTool = GetSwingTool();
			player.EquipTool(playerTool);
			playerTool.PlayAnimation(weaponSwingAnimation);
			playerTool.AnimationPlayer.SubStripIndex = direction;

			OnSwingBegin();
			
			// Perform an initial swing tile peak.
			Vector2F hitPoint = player.Center + (Angles.ToVector(swingAngle, false) * 13);
			Point2I hitTileLocation = player.RoomControl.GetTileLocation(hitPoint);
			OnSwingTilePeak(swingAngle, hitTileLocation);
			
			// Invoke any actions set to occur at time 0.
			if (timedActions.ContainsKey(0))
				timedActions[0].Invoke();

			
				
			Rectangle2I toolBox = swingCollisionBoxes[swingDirection, Math.Min(swingCollisionBoxes.Length - 1, swingAngleIndex)];
			toolBox.Point += (Point2I) player.CenterOffset;
			playerTool.CollisionBox = toolBox;
		}
		
		
		//-----------------------------------------------------------------------------
		// Virtual methods
		//-----------------------------------------------------------------------------
		
		public virtual UnitTool GetSwingTool() {
			return player.ToolVisual;
		}

		public virtual void OnSwingBegin() {

		}

		public virtual void OnSwingEnd() {
			player.BeginNormalState();
		}

		public virtual void OnSwingTilePeak(int angle, Point2I tileLocation) {

		}

		public virtual void OnSwingEntityPeak(int angle, Rectangle2F collisionBox) {

		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			player.Movement.MoveCondition = PlayerMoveCondition.OnlyInAir;
			playerTool = player.ToolVisual;
			Swing(player.UseDirection);
		}
		
		public override void OnEnd(PlayerState newState) {
			player.Movement.MoveCondition = PlayerMoveCondition.FreeMovement;
			player.UnequipTool(playerTool);
		}

		public override void Update() {
			base.Update();
		
			// Find the start time for the current swing angle.
			int swingAngleStartTime = 0;
			for (int i = 0; i < swingAngleIndex; i++)
				swingAngleStartTime += swingAngleDurations[i];

			// Check for changing swing angles.
			bool changedAngles = false;
			int t = 0;
			int time = (int) playerTool.AnimationPlayer.PlaybackTime;
			for (int i = 0; i < swingAngleDurations.Length; i++) {
				if (time == t && swingAngleIndex != i) {
					swingAngleIndex	= i;
					swingAngle		= Angles.Add(swingAngle, 1, swingAngleDirection);
					changedAngles	= true;
					break;
				}
				t += swingAngleDurations[i];
			}

			// Check for a swing tile peak (tile peaks happen just as the angle is changed).
			if (changedAngles) {
				Vector2F hitPoint = player.Center + (Angles.ToVector(swingAngle, false) * 13);
				Point2I hitTileLocation = player.RoomControl.GetTileLocation(hitPoint);
				OnSwingTilePeak(swingAngle, hitTileLocation);
			}

			// Check for a swing entity peak (entity peaks happen 1 frame after changing angles).
			/*
			if (time - swingAngleStartTime == 1) {
				Rectangle2F box = swingAngleCollisionBoxes[swingAngle];
				if (lunge && swingAngleIndex == swingAngleDurations.Length - 1)
					box = swingLungeCollisionBoxes[swingDirection];
				
				box.Point += player.Center;
				OnSwingEntityPeak(swingAngle, box);
			}
			*/
				
			Rectangle2I toolBox = swingCollisionBoxes[swingDirection, Math.Min(swingCollisionBoxes.Length - 1, swingAngleIndex)];
			toolBox.Point += (Point2I) player.CenterOffset;
			playerTool.CollisionBox = toolBox;

			// Invoke any occuring timed actions.
			if (timedActions.ContainsKey(time))
				timedActions[time].Invoke();

			// Reset the swing when the button is pressed again.
			if (isReswingable && weapon.IsEquipped && weapon.IsButtonPressed())
				Swing(player.UseDirection);

			// End the swing.
			if (playerTool.AnimationPlayer.IsDone && player.Graphics.IsAnimationDone)
				OnSwingEnd();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public ItemWeapon Weapon {
			get { return weapon; }
			set { weapon = value; }
		}

		public int SwingDirection {
			get { return swingDirection; }
		}

		public WindingOrder SwingAngleDirection {
			get { return swingAngleDirection; }
		}

		public int SwingStartAngle {
			get { return swingAngleStart; }
		}

		public int SwingAngleIndex {
			get { return swingAngleIndex; }
		}

	}
}
