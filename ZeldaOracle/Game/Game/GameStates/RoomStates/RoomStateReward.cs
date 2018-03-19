using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.GameStates.RoomStates {
	public class RoomStateReward : RoomState {

		private Reward reward;
		private Point2I chestPosition;
		private int timer;
		private bool useChest;
		private Action completeAction;
		private AnimationPlayer animationPlayer;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int RaiseDuration = 32;
		private const int NonChestDuration = 6;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomStateReward(Reward reward, Action completeAction = null) {
			this.reward				= reward;
			this.completeAction		= completeAction;
			this.updateRoom			= false;
			this.animateRoom		= true;
			this.chestPosition		= Point2I.Zero;
			this.useChest			= false;
			this.timer				= 0;
			this.animationPlayer	= new AnimationPlayer();
		}

		public RoomStateReward(Reward reward, Point2I chestPosition) :
			this(reward, null)
		{
			this.chestPosition		= chestPosition;
			this.useChest			= true;
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			animationPlayer.Play(reward.Sprite);
			timer = -1;
		}

		public override void Update() {
			animationPlayer.Update();
			timer++;
			if (timer == (useChest ? RaiseDuration : NonChestDuration)) {
				AudioSystem.PlaySound(GameData.SOUND_FANFARE_ITEM);
				reward.OnDisplayMessage();
				//GameControl.DisplayMessage(new Message(reward.Message));

				if (reward.HoldType == RewardHoldTypes.OneHand) {
					GameControl.Player.BeginBusyState(1);
					GameControl.Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_RAISE_ONE_HAND);
				}
				else if (reward.HoldType == RewardHoldTypes.TwoHands) {
					GameControl.Player.BeginBusyState(1);
					GameControl.Player.Graphics.PlayAnimation(GameData.ANIM_PLAYER_RAISE_TWO_HANDS);
				}
			}
			else if (timer == (useChest ? RaiseDuration : NonChestDuration) + 1) {
				// Pop before incase the OnCollect pushes a new game state
				gameControl.PopRoomState();
				reward.OnCollect();
				completeAction?.Invoke();
				return;
			}
		}

		public override void Draw(Graphics2D g) {
			g.PushTranslation(0, GameSettings.HUD_HEIGHT);
			g.PushTranslation(-RoomControl.ViewControl.Camera.TopLeft);

			if (!reward.HoldInChest && useChest) {
				g.DrawAnimation(animationPlayer, chestPosition + new Point2I(0, -8 - (timer + 2) / 4));
			}
			else if (timer >= (useChest ? RaiseDuration : NonChestDuration)) {
				if (reward.HoldType == RewardHoldTypes.TwoHands) {
					g.DrawAnimation(animationPlayer, GameControl.Player.Center + new Point2I(-8, -23));
				}
				else if (reward.HoldType == RewardHoldTypes.OneHand) {
					g.DrawAnimation(animationPlayer, GameControl.Player.Center + new Point2I(-12, -22));
				}
			}

			g.PopTranslation(2);
		}

		public Action CompleteAction {
			get { return completeAction; }
			set { completeAction = value; }
		}
	}
}
