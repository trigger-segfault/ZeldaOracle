using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Content;
using ZeldaOracle.Common.Content.ResourceBuilders;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Common.Graphics.Sprites;
using ZeldaOracle.Common.Scripting;
using ZeldaOracle.Common.Scripts.Commands;
using ZeldaOracle.Game;
using ZeldaOracle.Game.Entities.Monsters;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Tiles.ActionTiles;
using ZeldaOracle.Game.Items;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Items.Rewards;

namespace ZeldaOracle.Common.Scripts.CustomReaders {
	public class RewardSR : ScriptReader {

		private enum Modes {
			Root,
			Reward,
		}

		private RewardData rewardData;
		

		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RewardSR() {
			
			//=====================================================================================
			// ITEM BEGIN/END 
			//=====================================================================================
			AddCommand("REWARD", (int) Modes.Root,
				"string id, string type = \"Reward\"",
			delegate (CommandParam parameters) {
				string rewardID = parameters.GetString(0);
				string typeName = parameters.GetString(1);
				Type type = GameUtil.FindTypeWithBase<Reward>(typeName, false);
				
				rewardData = new RewardData();
				rewardData.ResourceName = rewardID;
				rewardData.Type = type;
				
				AddResource<RewardData>(rewardID, rewardData);
				Mode = Modes.Reward;
			});
			//=====================================================================================
			AddCommand("CLONE", (int) Modes.Reward,
				"string id",
			delegate (CommandParam parameters) {
				string rewardID = parameters.GetString(0);

				RewardData cloneData = GetResource<RewardData>(rewardID);
				if (cloneData.Type != rewardData.Type)
					ThrowCommandParseError("Cannot clone rewards with different types!");
				
				rewardData.Clone(cloneData);
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Reward,
				"",
			delegate (CommandParam parameters) {
				rewardData = null;

				Mode = Modes.Root;
			});
			//=====================================================================================
			// GENERAL BUILDING
			//=====================================================================================
			AddCommand("PROPERTY", (int) Modes.Reward,
				"(string name, var value)",
			delegate (CommandParam parameters) {
				SetProperty(rewardData, parameters);
			});
			//=====================================================================================
			// REWARD BUILDING
			//=====================================================================================
			AddCommand("SPRITE", (int) Modes.Reward,
				"string sprite",
			delegate (CommandParam parameters) {
				rewardData.Sprite = GetResource<ISprite>(parameters.GetString(0));
			});
			//=====================================================================================
			AddCommand("MESSAGE", (int) Modes.Reward,
				"string message",
			delegate (CommandParam parameters) {
				rewardData.Message = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("OBTAIN", (int) Modes.Reward,
				"string obtainMessage",
			delegate (CommandParam parameters) {
				rewardData.ObtainMessage = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("CANTCOLLECT", (int) Modes.Reward,
				"string cantCollectMessage",
			delegate (CommandParam parameters) {
				rewardData.CantCollectMessage = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("HOLDINCHEST", (int) Modes.Reward,
				"bool cantCollectMessage",
			delegate (CommandParam parameters) {
				rewardData.HoldInChest = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("HOLDTYPE", (int) Modes.Reward,
				"string holdType",
			delegate (CommandParam parameters) {
				rewardData.HoldType = parameters.GetEnum<RewardHoldTypes>(0, true);
			});
			//=====================================================================================
			AddCommand("PICKUPMESSAGE", (int) Modes.Reward,
				"bool showMessageOnPickup",
			delegate (CommandParam parameters) {
				rewardData.ShowPickupMessage = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("WEAPONINTERACT", (int) Modes.Reward,
				"bool interactWithWeapons",
			delegate (CommandParam parameters) {
				rewardData.WeaponInteract = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("BOUNCESOUND", (int) Modes.Reward,
				"string bounceSound",
			delegate (CommandParam parameters) {
				rewardData.BounceSound = GetResource<Sound>(parameters.GetString(0));
			});
			//=====================================================================================
			// AMOUNT REWARD BUILDING
			//=====================================================================================
			AddCommand("AMOUNT", (int) Modes.Reward,
				"int amount",
			delegate (CommandParam parameters) {
				if (!TypeHelper.TypeHasBase<RewardAmount>(rewardData.Type))
					ThrowCommandParseError("Can only call AMOUNT when constructing a " +
						"RewardAmount!");
				rewardData.Amount = parameters.GetInt(0);
			});
			//=====================================================================================
			AddCommand("FULL", (int) Modes.Reward,
				"string fullMessage",
			delegate (CommandParam parameters) {
				if (!TypeHelper.TypeHasBase<RewardAmount>(rewardData.Type))
					ThrowCommandParseError("Can only call FULL when constructing a " +
						"RewardAmount!");
				rewardData.FullMessage = parameters.GetString(0);
			});
			//=====================================================================================
			// AMMO REWARD BUILDING
			//=====================================================================================
			AddCommand("AMMO", (int) Modes.Reward,
				"string ammo",
			delegate (CommandParam parameters) {
				if (!TypeHelper.TypeHasBase<RewardAmmo>(rewardData.Type))
					ThrowCommandParseError("Can only call AMOUNT when constructing a " +
						"RewardAmmo!");
				rewardData.AmmoData = GetResource<AmmoData>(parameters.GetString(0));
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			rewardData = null;
		}

		/// <summary>Ends reading the script.</summary>
		protected override void EndReading() {

		}

		/// <summary>Creates a new script reader of the derived type.</summary>
		protected override ScriptReader CreateNew() {
			return new RewardSR();
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------
		
		/// <summary>The mode of the Tileset script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}

}
