using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.GameStates.RoomStates {
	public class RoomStateReward : RoomState {

		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int CHEST_DURATION = 32;
		private const int HOLD_DURATION = 6;


		//-----------------------------------------------------------------------------
		// Members
		//-----------------------------------------------------------------------------

		private Reward reward;
		private Entity entity;
		private Point2I chestPosition;
		private int timer;
		private bool useChest;
		private Action completeAction;
		private AnimationPlayer animationPlayer;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomStateReward(Reward reward, Entity entity,
			Action completeAction = null)
		{
			this.reward				= reward;
			this.entity				= entity;
			this.completeAction		= completeAction;
			this.updateRoom			= false;
			this.animateRoom		= true;
			this.chestPosition		= Point2I.Zero;
			this.useChest			= false;
			this.timer				= 0;
			this.animationPlayer	= new AnimationPlayer();
		}
		
		public RoomStateReward(Reward reward, Action completeAction = null) 
			: this(reward, null, completeAction)
		{
		}

		public RoomStateReward(Reward reward, Point2I chestPosition)
			: this(reward, null, null)
		{
			this.chestPosition		= chestPosition;
			this.useChest			= true;
		}


		//-----------------------------------------------------------------------------
		// Override Methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			animationPlayer.Play(reward.Sprite);
			timer = -1;
			if (entity != null)
				entity.Graphics.IsVisible = false;
		}

		public override void Update() {
			animationPlayer.Update();
			timer++;
			if (timer == Duration) {
				AudioSystem.PlaySound(GameData.SOUND_FANFARE_ITEM);
				reward.OnDisplayMessage();

				if (reward.HoldType == RewardHoldTypes.OneHand) {
					GameControl.Player.BeginBusyState(1);
					GameControl.Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_RAISE_ONE_HAND);
				}
				else if (reward.HoldType == RewardHoldTypes.TwoHands) {
					GameControl.Player.BeginBusyState(1);
					GameControl.Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_RAISE_TWO_HANDS);
				}
			}
			else if (timer == Duration + 1) {
				// Pop before incase the OnCollect pushes a new game state
				gameControl.PopRoomState();
				reward.OnCollect();
				completeAction?.Invoke();
				return;
			}
		}

		public override void Draw(Graphics2D g) {
			g.PushTranslation(0, GameSettings.HUD_HEIGHT);
			g.PushTranslation(-GameUtil.Bias(RoomControl.ViewControl.Camera.TopLeft));

			if (!reward.HoldInChest && useChest) {
				g.DrawAnimation(animationPlayer, chestPosition + new Point2I(0, -8 - (timer + 2) / 4));
			}
			else if (timer >= Duration) {
				if (reward.HoldType == RewardHoldTypes.TwoHands) {
					g.DrawAnimation(animationPlayer, GameControl.Player.Center + new Point2I(-8, -23));
				}
				else if (reward.HoldType == RewardHoldTypes.OneHand) {
					g.DrawAnimation(animationPlayer, GameControl.Player.Center + new Point2I(-12, -22));
				}
			}

			g.PopTranslation(2);
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public int Duration {
			get { return (useChest ? CHEST_DURATION : HOLD_DURATION); }
		}

		public Action CompleteAction {
			get { return completeAction; }
			set { completeAction = value; }
		}
	}
}
