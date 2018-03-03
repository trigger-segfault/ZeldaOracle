using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Entities.Units;
using ZeldaOracle.Common.Graphics.Sprites;

namespace ZeldaOracle.Game.Entities.Players.States.SwingStates {
	public class PlayerSwingState : PlayerState {

		
		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------
		
		private const int LUNGE_CBOX_DIST = 6; // How much lunging extends the tool collision box.

		// Standard tool collision boxes for swinging with lunging.
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
		
		// Standard tool collision boxes for swinging without lunging.
		private readonly Rectangle2I[,] SWING_TOOL_BOXES_NOLUNGE = new Rectangle2I[,] {
			{
				new Rectangle2I(-8, -8 - 10, 16, 10),
				new Rectangle2I(8, -8 - 10, 10, 16 + 6),
				new Rectangle2I(8 - LUNGE_CBOX_DIST, 8 - 10, 19, 10),
			}, {
				new Rectangle2I(8, -8, 10, 16),
				new Rectangle2I(-8 + 10 - 6, -8 - 10, 16 + 6, 10),
				new Rectangle2I(-8, -8 - 19 + LUNGE_CBOX_DIST, 10, 19),
			}, {
				new Rectangle2I(-8, -8 - 10, 16, 10),
				new Rectangle2I(-8 - 10, -8 - 10, 10, 16 + 6),
				new Rectangle2I(-8 - 19 + LUNGE_CBOX_DIST, 8 - 10, 19, 10),
			}, {
				new Rectangle2I(-8 - 10, -8, 10, 16),
				new Rectangle2I(-8 - 10, 8, 16 + 6, 10),
				new Rectangle2I(8 - 10, 8 - LUNGE_CBOX_DIST, 10, 19),
			}
		};

		
		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private ItemWeapon				weapon;
		protected UnitTool				playerTool;

		// Settings
		protected int				swingAnglePullBack;
		protected bool				lunge;
		protected bool				isReswingable;
		protected int[]				swingAngleDurations;
		protected WindingOrder[]	swingWindingOrders;
		protected Rectangle2I[,]	swingCollisionBoxesLunge;
		protected Rectangle2I[,]	swingCollisionBoxesNoLunge;
		protected Animation			weaponSwingAnimationLunge;
		protected Animation         weaponSwingAnimation;
		private Dictionary<int, Action>	timedActions;

		// Internal state
		private Direction				swingDirection;	 // The player the direction is in when he starts swinging.
		private Angle					swingAngleStart;
		private WindingOrder			swingAngleDirection;
		private Angle					swingAngle;
		private int						swingAngleIndex;
		private Rectangle2I[,]			swingCollisionBoxes;
		protected bool					isLunging;
	

		//-----------------------------------------------------------------------------
		// Constructors & Configuration
		//-----------------------------------------------------------------------------

		public PlayerSwingState() {
			weapon = null;
			timedActions = new Dictionary<int, Action>();

			StateParameters.ProhibitMovementControlOnGround = true;

			InitStandardSwing(GameData.ANIM_SWORD_SWING,
				GameData.ANIM_SWORD_MINECART_SWING);
		}
		
		protected void InitStandardSwing(
			Animation toolAnimationLunge, Animation toolAnimation)
		{
			lunge					= true;
			isReswingable			= true;
			swingAnglePullBack		= 2;
			swingAngleDurations		= new int[] { 3, 3, 12 };

			swingCollisionBoxesLunge	= SWING_TOOL_BOXES;
			swingCollisionBoxesNoLunge	= SWING_TOOL_BOXES_NOLUNGE;

			weaponSwingAnimationLunge	= toolAnimationLunge;
			weaponSwingAnimation		= toolAnimation;

			swingWindingOrders = new WindingOrder[Direction.Count];
			swingWindingOrders[Direction.Right]	= WindingOrder.Clockwise;
			swingWindingOrders[Direction.Up]	= WindingOrder.CounterClockwise;
			swingWindingOrders[Direction.Left]	= WindingOrder.CounterClockwise;
			swingWindingOrders[Direction.Down]	= WindingOrder.CounterClockwise;
		}

		
		//-----------------------------------------------------------------------------
		// Swing methods
		//-----------------------------------------------------------------------------

		/// <summary>Add an action to be triggered at the given time after the swing
		/// has begun.</summary>
		public void AddTimedAction(int time, Action action) {
			timedActions[time] = action;
		}

		/// <summary>Perform a swing for the given facing direction.</summary>
		private void Swing(Direction direction) {
			swingDirection = direction;
			swingAngleDirection = swingWindingOrders[swingDirection];
			swingAngleStart = swingDirection.ToAngle()
				.Rotate(swingAngleStart, swingAngleDirection);
			swingAngle = swingAngleStart;
			swingAngleIndex = 0;

			player.Direction = direction;

			// Equip the player tool
			playerTool = GetSwingTool();
			player.EquipTool(playerTool);
			playerTool.AnimationPlayer.SubStripIndex = direction;
			
			// Do not lunge while in minecart
			isLunging = (lunge && !player.IsInMinecart);

			player.Graphics.PlayAnimation(GetPlayerSwingAnimation(isLunging));
			if (isLunging) {
				playerTool.PlayAnimation(weaponSwingAnimationLunge);
				swingCollisionBoxes = swingCollisionBoxesLunge;
			}
			else {
				playerTool.PlayAnimation(weaponSwingAnimation);
				swingCollisionBoxes = swingCollisionBoxesNoLunge;
			}

			OnSwingBegin();
			
			// Perform an initial swing tile peak
			Vector2F hitPoint = player.Center + (swingAngle.ToPoint() * 13);
			OnSwingTilePeak(swingAngle, hitPoint);

			// Set the tool's initial collision box
			Rectangle2I toolBox = swingCollisionBoxes[swingDirection,
				GMath.Min(swingCollisionBoxes.Length - 1, swingAngleIndex)];
			toolBox.Point += (Point2I) player.CenterOffset;
			playerTool.Interactions.InteractionBox = toolBox;
			
			// Trigger actions that occur at time 0
			if (timedActions.ContainsKey(0))
				timedActions[0].Invoke();
		}
		
		
		//-----------------------------------------------------------------------------
		// Virtual Methods
		//-----------------------------------------------------------------------------
		
		public virtual Animation GetPlayerSwingAnimation(bool lunge) {
			if (lunge)
				return player.Animations.Swing;
			return player.Animations.SwingNoLunge;
		}
		
		public virtual UnitTool GetSwingTool() {
			return player.ToolVisual;
		}

		public virtual void OnSwingBegin() {}

		public virtual void OnSwingEnd() {
			End();
		}

		public virtual void OnSwingTilePeak(int angle, Vector2F hitPoint) {}

		public virtual void OnSwingEntityPeak(int angle, Rectangle2F collisionBox) {}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin(PlayerState previousState) {
			playerTool = player.ToolVisual;
			playerTool.DrawAboveUnit = false;
			Swing(player.UseDirection);
		}
		
		public override void OnEnd(PlayerState newState) {
			player.UnequipTool(playerTool);
			playerTool.DrawAboveUnit = true;
		}

		public override void OnExitMinecart() {
			player.Graphics.SetAnimation(GetPlayerSwingAnimation(isLunging));
		}

		public override void OnEnterMinecart() {
			// Being in a minecart disables lunging
			if (!isLunging) {
				isLunging = true;
				swingCollisionBoxes = swingCollisionBoxesNoLunge;
			}

			player.Graphics.SetAnimation(GetPlayerSwingAnimation(isLunging));
		}

		public override void Update() {
			// Get the start time for the current swing angle
			int swingAngleStartTime = 0;
			for (int i = 0; i < swingAngleIndex; i++)
				swingAngleStartTime += swingAngleDurations[i];

			// Check if it is time to change the swing angle
			bool changedAngles = false;
			int time = (int) playerTool.AnimationPlayer.PlaybackTime + 1;
			int t = 0;
			for (int angleIndex = 0;
				angleIndex < swingAngleDurations.Length; angleIndex++)
			{
				if (t == time && swingAngleIndex != angleIndex) {
					swingAngleIndex	= angleIndex;
					swingAngle = swingAngle.Rotate(1, swingAngleDirection);
					changedAngles = true;
					break;
				}
				t += swingAngleDurations[angleIndex];
			}

			// If the swing angle changed, then trigger a swing tile peak
			if (changedAngles) {
				Vector2F hitPoint = player.Center + (swingAngle.ToPoint() * 13);
				OnSwingTilePeak(swingAngle, hitPoint);
			}
			
			// Update the swing colliison box for the current swing angle
			Rectangle2I toolBox = swingCollisionBoxes[swingDirection,
				GMath.Min(swingCollisionBoxes.Length - 1, swingAngleIndex)];
			toolBox.Point += (Point2I) player.CenterOffset;
			playerTool.CollisionBox = toolBox;

			// Invoke any occuring timed actions
			if (timedActions.ContainsKey(time))
				timedActions[time].Invoke();

			// If the action button is pressed, then reset the swing
			if (isReswingable && weapon.IsEquipped && weapon.IsButtonPressed())
				Swing(player.UseDirection);

			// End the swing upon completing the animation
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

		public Direction SwingDirection {
			get { return swingDirection; }
		}

		public WindingOrder SwingAngleDirection {
			get { return swingAngleDirection; }
		}

		public Angle SwingStartAngle {
			get { return swingAngleStart; }
		}

		public int SwingAngleIndex {
			get { return swingAngleIndex; }
		}

	}
}
