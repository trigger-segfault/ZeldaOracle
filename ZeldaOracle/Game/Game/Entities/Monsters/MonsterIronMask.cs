using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Util;
using ZeldaOracle.Game.Entities.Monsters.Tools;
using ZeldaOracle.Game.Entities.Players;

namespace ZeldaOracle.Game.Entities.Monsters {

	public class MonsterIronMask : BasicMonster {

		private class MaskEntity : Entity {

			private bool isBeingPulled;
			private int dissapearTimer;


			//-------------------------------------------------------------------------
			// Constructors
			//-------------------------------------------------------------------------

			public MaskEntity() {
				Graphics.DrawOffset = new Point2I(-8, -8);
				Graphics.DepthLayer = DepthLayer.EffectMonsterExplosion;
			}


			//-------------------------------------------------------------------------
			// Overridden Methods
			//-------------------------------------------------------------------------

			public override void Initialize() {
				isBeingPulled = true;
				Graphics.PlayAnimation(GameData.ANIM_MONSTER_IRON_MASK_MASK);
			}

			public override void Update() {
				if (isBeingPulled) {
					Player player = RoomControl.Player;

					// Move toward the player
					Vector2F vectorToPlayer = player.Center - Center;
					position += vectorToPlayer.Normalized *
						GameSettings.MONSTER_IRON_MASK_MASK_MOVE_SPEED;

					// Check if we have reached the player
					if (vectorToPlayer.Length <= 10.0f) {
						Destroy();
						return;
					}

					// Check if the player stopped using the magnet gloves
					if (player.WeaponState !=
						player.MagnetGlovesState)
					{
						dissapearTimer = 0;
						isBeingPulled = false;
						Graphics.IsFlickering = true;
						Graphics.FlickerAlternateDelay = 1;
					}
				}
				else {
					// Destroy after a delay
					dissapearTimer++;
					if (dissapearTimer >=
						GameSettings.MONSTER_IRON_MASK_MASK_DISSAPEAR_DELAY)
						Destroy();
				}

				base.Update();
			}
		}

		private enum IronMaskState {
			Masked,
			Unmasking,
			Unmasked,
		}

		private MonsterToolShield mask;
		private GenericStateMachine<IronMaskState> stateMachine;
		private int unmaskTimer;

		
		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public MonsterIronMask() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 2;
			Color			= MonsterColor.Red;
			animationMove	= GameData.ANIM_MONSTER_IRON_MASK;
			
			// Movement
			moveSpeed					= 0.5f;
			changeDirectionsOnCollide	= true;
			syncAnimationWithDirection	= true;
			stopTime.Set(30, 60);
			moveTime.Set(30, 50);
			
			// Reactions
			Reactions[InteractionType.BombExplosion].Set(MonsterReactions.Kill);

			// Mask shield tool
			mask = new MonsterToolShield();
			mask.Hitboxes[Direction.Right]	= new Rectangle2F(8 - 6, -8, 6, 16);
			mask.Hitboxes[Direction.Left]	= new Rectangle2F(-8, -8, 6, 16);
			mask.Hitboxes[Direction.Up]		= new Rectangle2F(-8, -8, 16, 6);
			mask.Hitboxes[Direction.Down]	= new Rectangle2F(-8, 8 - 6, 16, 6);

			// State Machine
			stateMachine = new GenericStateMachine<IronMaskState>();
			stateMachine.AddState(IronMaskState.Masked)
				.OnBegin(OnBeginMaskedState)
				.OnUpdate(OnUpdateMaskedState);
			stateMachine.AddState(IronMaskState.Unmasking)
				.OnBegin(OnBeginUnmaskingState)
				.OnUpdate(OnUpdateUnmaskingState);
			stateMachine.AddState(IronMaskState.Unmasked)
				.OnBegin(OnBeginUnmaskedState)
				.OnUpdate(OnUpdateUnmaskedState);
		}


		//-----------------------------------------------------------------------------
		// State Callbacks
		//-----------------------------------------------------------------------------

		private void OnBeginMaskedState() {
			EquipTool(mask);
		}

		private void OnUpdateMaskedState() {
			if (IsMaskBeingPulled())
				stateMachine.BeginState(IronMaskState.Unmasking);
			else
				base.UpdateAI();
		}

		private void OnBeginUnmaskingState() {
			StopMoving();
			unmaskTimer = 0;
		}

		private void OnUpdateUnmaskingState() {
			unmaskTimer++;

			// Shake left and right
			if (unmaskTimer % 4 == 0)
				position += new Vector2F(1, 0);
			else if (unmaskTimer % 4 == 2)
				position -= new Vector2F(1, 0);
			
			// Unmask after a delay
			if (unmaskTimer >= GameSettings.MONSTER_IRON_MASK_UNMASK_DELAY)
				stateMachine.BeginState(IronMaskState.Unmasked);
			else if (!IsMaskBeingPulled())
				stateMachine.BeginState(IronMaskState.Masked);
		}

		private void OnBeginUnmaskedState() {
			UnequipTool(mask);

			// Spawn the unmasked mask entity after a 2 tick delay
			RoomControl.ScheduleEvent(2, delegate() {
				MaskEntity maskEntity = new MaskEntity();
				RoomControl.SpawnEntity(maskEntity, Center);
			});

			// Setup new movement style
			animationMove = GameData.ANIM_MONSTER_IRON_MASK_UNMASKED;
			syncAnimationWithDirection = false;
			playAnimationOnlyWhenMoving = false;
			stopTime.Set(0);

			StartMoving();
		}

		private void OnUpdateUnmaskedState() {
			base.UpdateAI();
		}
		

		//-----------------------------------------------------------------------------
		// Internal Methods
		//-----------------------------------------------------------------------------

		/// <summary>Returns true if the player is using the magnetic gloves close
		/// enough to the Iron Mask to pull its mask off.</summary>
		private bool IsMaskBeingPulled() {
			Player player = RoomControl.Player;
			Vector2F vectorToMonster = Center - player.Center;

			return (player.WeaponState == player.MagnetGlovesState &&
				player.Direction == Direction.FromVector(vectorToMonster) &&
				vectorToMonster.Dot(player.Direction.ToVector()) <=
					GameSettings.MONSTER_IRON_MASK_MAGNETIC_PULL_RANGE);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();
			stateMachine.InitializeOnState(IronMaskState.Masked);
		}

		public override void UpdateAI() {
			stateMachine.Update();
		}
	}
}
