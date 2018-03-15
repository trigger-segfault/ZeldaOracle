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

		private Reward reward;
		private RewardAmount amount;
		private RewardAmmo ammo;


		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public RewardSR() {

			//=====================================================================================
			// Type Definitions
			//=====================================================================================
			AddType("Sprite",
				"string spriteName",
				"(string animationName, int substrip)",
				"(string spriteName, string definition)"
			);
			//=====================================================================================
			// ITEM BEGIN/END 
			//=====================================================================================
			AddCommand("REWARD", (int) Modes.Root,
				"string id, string type",
			delegate (CommandParam parameters) {
				string rewardID = parameters.GetString(0);
				string typeName = parameters.GetString(1);
				Type type = GameUtil.FindTypeWithBase<Reward>(typeName, false);

				reward = ReflectionHelper.Construct<Reward, string>(type, rewardID);
				if (reward == null)
					ThrowCommandParseError(type.Name + " does not have a valid constructor!");

				if (TypeHelper.TypeHasBase<RewardAmount>(type))
					amount = (RewardAmount) reward;
				if (TypeHelper.TypeHasBase<RewardAmmo>(type))
					ammo = (RewardAmmo) reward;

				AddResource<Reward>(rewardID, reward);
				Mode = Modes.Reward;
			});
			//=====================================================================================
			AddCommand("CLONE REWARD", (int) Modes.Root,
				"string oldID, string newID",
			delegate (CommandParam parameters) {
				string oldID = parameters.GetString(0);
				string rewardID = parameters.GetString(1);

				Reward oldReward = GetResource<Reward>(oldID);
				Type type = oldReward.GetType();

				reward = ReflectionHelper.Construct<Reward, string>(type, rewardID);
				if (reward == null)
					ThrowCommandParseError(type.Name + " does not have a valid constructor!");
				reward.Clone(oldReward);

				if (TypeHelper.TypeHasBase<RewardAmount>(type))
					amount = (RewardAmount) reward;
				
				AddResource<Reward>(rewardID, reward);
				Mode = Modes.Reward;
			});
			//=====================================================================================
			AddCommand("END", (int) Modes.Reward,
				"",
			delegate (CommandParam parameters) {
				reward = null;
				Mode = Modes.Root;
			});
			//=====================================================================================
			// REWARD BUILDING
			//=====================================================================================
			AddCommand("SPRITE", (int) Modes.Reward,
				"Sprite sprite",
			delegate (CommandParam parameters) {
				reward.Sprite = GetSpriteFromParams(parameters, 0);
			});
			//=====================================================================================
			AddCommand("MESSAGE", (int) Modes.Reward,
				"string message",
			delegate (CommandParam parameters) {
				reward.Message = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("OBTAIN", (int) Modes.Reward,
				"string obtainMessage",
			delegate (CommandParam parameters) {
				reward.ObtainMessage = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("CANTCOLLECT", (int) Modes.Reward,
				"string cantCollectMessage",
			delegate (CommandParam parameters) {
				reward.CantCollectMessage = parameters.GetString(0);
			});
			//=====================================================================================
			AddCommand("HOLDINCHEST", (int) Modes.Reward,
				"bool cantCollectMessage",
			delegate (CommandParam parameters) {
				reward.HoldInChest = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("HOLDTYPE", (int) Modes.Reward,
				"string holdType",
			delegate (CommandParam parameters) {
				reward.HoldType = parameters.GetEnum<RewardHoldTypes>(0, true);
			});
			//=====================================================================================
			AddCommand("PICKUPMESSAGE", (int) Modes.Reward,
				"bool showMessageOnPickup",
			delegate (CommandParam parameters) {
				reward.ShowMessageOnPickup = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("WEAPONINTERACT", (int) Modes.Reward,
				"bool interactWithWeapons",
			delegate (CommandParam parameters) {
				reward.InteractWithWeapons = parameters.GetBool(0);
			});
			//=====================================================================================
			AddCommand("BOUNCESOUND", (int) Modes.Reward,
				"string bounceSound",
			delegate (CommandParam parameters) {
				reward.BounceSound = GetResource<Sound>(parameters.GetString(0));
			});
			//=====================================================================================
			// AMOUNT REWARD BUILDING
			//=====================================================================================
			AddCommand("AMOUNT", (int) Modes.Reward,
				"int amount",
			delegate (CommandParam parameters) {
				if (amount == null)
					ThrowCommandParseError("Can only call AMOUNT when constructing a " +
						"RewardAmount!");
				amount.Amount = parameters.GetInt(0);
			});
			//=====================================================================================
			AddCommand("FULL", (int) Modes.Reward,
				"string fullMessage",
			delegate (CommandParam parameters) {
				if (amount == null)
					ThrowCommandParseError("Can only call FULL when constructing a " +
						"RewardAmount!");
				amount.FullMessage = parameters.GetString(0);
			});
			//=====================================================================================
			// AMMO REWARD BUILDING
			//=====================================================================================
			AddCommand("AMMO", (int) Modes.Reward,
				"string ammo",
			delegate (CommandParam parameters) {
				if (ammo == null)
					ThrowCommandParseError("Can only call AMOUNT when constructing a " +
						"RewardAmmo!");
				ammo.Ammo = GetResource<Ammo>(parameters.GetString(0));
			});
			//=====================================================================================
		}


		//-----------------------------------------------------------------------------
		// Script Commands
		//-----------------------------------------------------------------------------

		/// <summary>Gets a sprite.</summary>
		private ISprite GetSprite(string name) {
			ISprite sprite = GetResource<ISprite>(name);
			if (sprite == null) {
				ThrowCommandParseError("Sprite with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite.</summary>
		private ISprite GetSprite(ISpriteSource source, Point2I index) {
			if (source == null)
				ThrowCommandParseError("Cannot get sprite from source with no sprite source!");
			ISprite sprite = source.GetSprite(index);
			if (sprite == null) {
				ThrowCommandParseError("Sprite at source index '" + index + "' does not exist!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite and confirms its type.</summary>
		private T GetSprite<T>(string name) where T : class, ISprite {
			T sprite = GetResource<ISprite>(name) as T;
			if (sprite == null) {
				ThrowCommandParseError(typeof(T).Name + " with name '" + name + "' does not exist in resources!");
			}
			return sprite;
		}

		/// <summary>Gets a sprite and confirms its type.</summary>
		private T GetSprite<T>(ISpriteSource source, Point2I index) where T : class, ISprite {
			if (source == null)
				ThrowCommandParseError("Cannot get sprite from source with no sprite source!");
			T sprite = source.GetSprite(index) as T;
			if (sprite == null) {
				ThrowCommandParseError(typeof(T).Name + " at source index '" + index + "' does not exist!");
			}
			return sprite;
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(string name, string definition) {
			return GetDefinedSprite(GetSprite<DefinitionSprite>(name), definition);
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(ISpriteSource source, Point2I index, string definition) {
			return GetDefinedSprite(GetSprite<DefinitionSprite>(source, index), definition);
		}

		/// <summary>Gets the sprite of a definition sprite.</summary>
		private ISprite GetDefinedSprite(DefinitionSprite sprite, string definition) {
			ISprite defSprite = sprite.Get(definition);
			if (defSprite == null)
				ThrowCommandParseError("Defined sprite with definition '" + definition + "' does not exist!");
			return defSprite;
		}

		/// <summary>Gets the sprite from one of the many parameter overloads.</summary>
		private ISprite GetSpriteFromParams(CommandParam param, int startIndex = 0) {
			ISpriteSource source;
			Point2I index;
			string definition;
			return GetSpriteFromParams(param, startIndex, out source, out index, out definition);
		}

		/// <summary>Gets the sprite from one of the many parameter overloads and returns the source.</summary>
		private ISprite GetSpriteFromParams(CommandParam param, int startIndex, out ISpriteSource source, out Point2I index, out string definition) {
			// 1: string spriteName
			// 3: (string animationName, int substrip)
			// 4: (string spriteName, string definition)
			source = null;
			index = Point2I.Zero;
			definition = null;

			var param0 = param.GetParam(startIndex);
			if (param0.Type == CommandParamType.String) {
				// Overload 1:
				return GetResource<ISprite>(param.GetString(startIndex));
			}
			else {
				if (param0.GetParam(1).IsValidType(CommandParamType.Integer)) {
					// Overload 3:
					return GetSprite<Animation>(param0.GetString(0)).GetSubstrip(param0.GetInt(1));
				}
				else {
					// Overload 4:
					return GetDefinedSprite(param0.GetString(0), param0.GetString(1));
				}
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		/// <summary>Begins reading the script.</summary>
		protected override void BeginReading() {
			reward     = null;
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

		/// <summary>Gets the source as a sprite sheet.</summary>
		/*private SpriteSheet SpriteSheet {
			get { return source as SpriteSheet; }
		}*/

		/// <summary>Gets the source as a sprite set.</summary>
		/*private SpriteSet SpriteSet {
			get { return source as SpriteSet; }
		}*/

		/// <summary>The mode of the Tileset script reader.</summary>
		private new Modes Mode {
			get { return (Modes) base.Mode; }
			set { base.Mode = (int) value; }
		}
	}

}
