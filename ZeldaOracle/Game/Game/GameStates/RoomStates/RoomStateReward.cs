using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Control;
using ZeldaOracle.Game.Entities.Players.States;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Game.GameStates.RoomStates {
	public class RoomStateReward : RoomState {

		private Reward reward;
		private Point2I chestPosition;
		private int timer;
		private bool useChest;

		private AnimationPlayer animationPlayer;


		//-----------------------------------------------------------------------------
		// Constants
		//-----------------------------------------------------------------------------

		private const int RaiseDuration = 32;
		private const int NonChestDuration = 6;

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RoomStateReward(Reward reward) {
			this.updateRoom		= false;
			this.animateRoom	= true;
			this.reward			= reward;
			this.chestPosition	= Point2I.Zero;
			this.useChest		= false;
			this.timer			= 0;

			this.animationPlayer	= new AnimationPlayer();
		}
		public RoomStateReward(Reward reward, Point2I chestPosition) {
			this.updateRoom		= false;
			this.animateRoom	= true;
			this.reward			= reward;
			this.chestPosition	= chestPosition;
			this.useChest		= true;
			this.timer			= 0;

			this.animationPlayer	= new AnimationPlayer();
		}

		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void OnBegin() {
			animationPlayer.Play(reward.Animation);
			timer = -1;
		}

		public override void Update() {
			animationPlayer.Update();
			timer++;
			if (timer == (useChest ? RaiseDuration : NonChestDuration)) {
				GameControl.DisplayMessage(new Message(reward.Message));
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
				reward.OnCollect(GameControl);
				gameControl.PopRoomState();
				// Play reward sound
				return;
			}
		}

		public override void Draw(Graphics2D g) {
			g.Translate(0, 16);

			if (reward.HoldType == RewardHoldTypes.Raise && useChest) {
				g.DrawAnimation(animationPlayer, chestPosition + new Point2I(8, -(timer + 2) / 4), 0.3f);
			}
			else if (timer >= (useChest ? RaiseDuration : NonChestDuration)) {
				if (reward.HoldType == RewardHoldTypes.TwoHands) {
					g.DrawAnimation(animationPlayer, GameControl.Player.Position + new Point2I(0, -23), 0.3f);
				}
				else if (reward.HoldType == RewardHoldTypes.OneHand) {
					g.DrawAnimation(animationPlayer, GameControl.Player.Position + new Point2I(-4, -22), 0.3f);
				}
			}

			g.Translate(0, -16);
		}
	}
}
