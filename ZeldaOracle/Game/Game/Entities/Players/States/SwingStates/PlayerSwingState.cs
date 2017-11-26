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
		private int						swingDirection;	 // The player the direction is in when he starts swinging.
		private int						swingAngleStart;
		private WindingOrder			swingAngleDirection;
		private int						swingAngle;
		private int						swingAngleIndex;
		private Rectangle2I[,]			swingCollisionBoxes;

		// Settings.
		protected int				swingAnglePullBack;
		protected bool				lunge;
		protected bool				isReswingable;
		protected int[]				swingAngleDurations;
		protected WindingOrder[]	swingWindingOrders;
		protected Rectangle2I[,]	swingCollisionBoxesLunge;
		protected Rectangle2I[,]	swingCollisionBoxesNoLunge;
		protected Animation			weaponSwingAnimationLunge;
		protected Animation			weaponSwingAnimation;
		protected Animation			playerSwingAnimation;
		protected Animation			playerSwingAnimationLunge;
		protected Animation			playerSwingAnimationInMinecart;
		protected Animation			playerSwingAnimationUnderwater;
		private Dictionary<int, Action>	timedActions;

	

		//-----------------------------------------------------------------------------
		// Constructors & Setup
		//-----------------------------------------------------------------------------

		public PlayerSwingState() {
			weapon = null;
			timedActions = new Dictionary<int, Action>();

			InitStandardSwing(GameData.ANIM_SWORD_SWING, GameData.ANIM_SWORD_MINECART_SWING);
		}
		
		protected void InitStandardSwing(Animation toolAnimationLunge, Animation toolAnimation) {
			this.lunge							= true;
			this.isReswingable					= true;
			this.swingAnglePullBack				= 2;
			this.swingAngleDurations			= new int[] { 3, 3, 12 };

			this.swingCollisionBoxesLunge		= SWING_TOOL_BOXES;
			this.swingCollisionBoxesNoLunge		= SWING_TOOL_BOXES_NOLUNGE;

			this.weaponSwingAnimationLunge		= toolAnimationLunge;
			this.weaponSwingAnimation			= toolAnimation;
			this.playerSwingAnimationLunge		= GameData.ANIM_PLAYER_SWING;
			this.playerSwingAnimation			= GameData.ANIM_PLAYER_MINECART_SWING;
			this.playerSwingAnimationInMinecart	= GameData.ANIM_PLAYER_MINECART_SWING;
			this.playerSwingAnimationUnderwater	= GameData.ANIM_PLAYER_MERMAID_SWING;
			
			this.swingWindingOrders = new WindingOrder[] {
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
			playerTool = GetSwingTool();
			player.EquipTool(playerTool);
			playerTool.AnimationPlayer.SubStripIndex = direction;
			
			if (player.IsInMinecart) {
				playerTool.PlayAnimation(weaponSwingAnimation);
				player.Graphics.PlayAnimation(playerSwingAnimationInMinecart);
				swingCollisionBoxes = swingCollisionBoxesNoLunge;
			}
			else if (player.RoomControl.IsUnderwater) {
				playerTool.PlayAnimation(weaponSwingAnimation);
				player.Graphics.PlayAnimation(playerSwingAnimationUnderwater);
				swingCollisionBoxes = swingCollisionBoxesNoLunge;
			}
			else if (lunge) {
				playerTool.PlayAnimation(weaponSwingAnimationLunge);
				player.Graphics.PlayAnimation(playerSwingAnimationLunge);
				swingCollisionBoxes = swingCollisionBoxesLunge;
			}
			else {
				playerTool.PlayAnimation(weaponSwingAnimation);
				player.Graphics.PlayAnimation(playerSwingAnimation);
				swingCollisionBoxes = swingCollisionBoxesNoLunge;
			}

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

		public virtual void OnSwingTilePeak(int angle, Vector2F hitPoint) {

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

		public override void OnExitMinecart() {
			if (player.Graphics.Animation == playerSwingAnimationInMinecart)
				player.Graphics.SetAnimation(playerSwingAnimation);
		}

		public override void OnEnterMinecart() {
			// Being in a minecart disables lunging.
			if (player.Graphics.Animation == playerSwingAnimation ||
				player.Graphics.Animation == playerSwingAnimationLunge)
			{
				player.Graphics.SetAnimation(playerSwingAnimationInMinecart);
				swingCollisionBoxes = swingCollisionBoxesNoLunge;
			}
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
				OnSwingTilePeak(swingAngle, hitPoint);
			}
				
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
