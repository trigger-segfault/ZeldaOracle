using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Common.Graphics;
using ZeldaOracle.Game.Items.Rewards;
using ZeldaOracle.Game.GameStates.RoomStates;
using ZeldaOracle.Game.Tiles;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Common.Audio;

namespace ZeldaOracle.Game.Entities {
	public class CollectibleReward : Collectible {

		private Reward reward;
		private bool showMessage;
		private bool isDrop;
		private bool isSubmergable;
		private bool submerged;


		//-----------------------------------------------------------------------------
		// Constructors
		//-----------------------------------------------------------------------------

		public CollectibleReward(Reward reward, bool isDrop = false, bool isSubmergable = false) {
			this.reward			= reward;
			this.showMessage	= !reward.OnlyShowMessageInChest;
			this.hasDuration	= reward.HasDuration;
			this.isCollectibleWithItems = reward.IsCollectibleWithItems;
			this.isDrop			= isDrop;
			this.isSubmergable	= isSubmergable;
			this.submerged		= false;

			// Physics.
			Physics.CollisionBox		= new Rectangle2I(-4, -9, 8, 8);
			Physics.SoftCollisionBox	= new Rectangle2I(-5, -9, 9, 8);
			Physics.Enable(
				PhysicsFlags.Bounces |
				PhysicsFlags.HasGravity |
				PhysicsFlags.CollideRoomEdge |
				PhysicsFlags.CollideWorld |
				PhysicsFlags.HalfSolidPassable |
				PhysicsFlags.DestroyedInHoles);
			soundBounce = reward.BounceSound;

			// Graphics.
			centerOffset					= new Point2I(0, -5);
			Graphics.DrawOffset				= new Point2I(-8, -13);
			Graphics.RipplesDrawOffset		= new Point2I(0, 1);
			Graphics.IsGrassEffectVisible	= true;
			Graphics.IsRipplesEffectVisible	= true;
		}


		//-----------------------------------------------------------------------------
		// Collection
		//-----------------------------------------------------------------------------

		public override void Collect() {
			if (!submerged || RoomControl.Player.IsSubmerged) {
				if (showMessage || submerged) {
					if (submerged) {
						RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_WATER_SPLASH,
							DepthLayer.EffectSplash, true), position);
					}
					RoomControl.GameControl.PushRoomState(new RoomStateReward(reward));
				}
				else {
					reward.OnCollectNoMessage(GameControl);
				}
				base.Collect();
			}
		}


		//-----------------------------------------------------------------------------
		// Overridden methods
		//-----------------------------------------------------------------------------

		public override void Initialize() {
			base.Initialize();

			if (RoomControl.IsSideScrolling) {
				Physics.CollideWithRoomEdge = false;

				// Drops cannot fall down, but they can rise up.
				if (isDrop) {
					Physics.MaxFallSpeed		= 0.0f;
					Physics.CollideWithWorld	= false;
				}
				else {
					Physics.CollideWithWorld = true;
				}
			}
			
			hasDuration = reward.HasDuration;

			submerged = IsSubmerged;

			Graphics.PlayAnimation(reward.Sprite);
			Graphics.IsVisible = !submerged;
		}

		public override void Update() {
			base.Update();

			if (!IsSubmerged && submerged) {
				submerged = false;
				Graphics.IsVisible = true;
			}
		}

		public override void OnLand() {
			// Disable collisions after landing
			//if (!RoomControl.IsSideScrolling)
				//Physics.CollideWithWorld = false;
		}

		// Called when the entity falls in water.
		public override void OnFallInWater() {
			if (!isSubmergable) {
				base.OnFallInWater();
			}
			else if (!submerged) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_WATER_SPLASH, DepthLayer.EffectSplash, true), position);
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
				submerged = true;
				Graphics.IsVisible = false;
				ZPosition = 0;
				// Cancel bouncing
				Physics.ZVelocity = 0;
			}
		}

		// Called when the entity falls in lava.
		public override void OnFallInLava() {
			if (!isSubmergable) {
				base.OnFallInLava();
			}
			else if (!submerged) {
				RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_LAVA_SPLASH, DepthLayer.EffectSplash, true), position);
				AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
				submerged = true;
				Graphics.IsVisible = false;
				ZPosition = 0;
				// Cancel bouncing
				Physics.ZVelocity = 0;
			}
		}


		//-----------------------------------------------------------------------------
		// Properties
		//-----------------------------------------------------------------------------

		public Reward Reward {
			get { return reward; }
		}

		public bool ShowMessage {
			get { return showMessage; }
			set { showMessage = value; }
		}

		public bool IsSubmergable {
			get { return isSubmergable; }
			set { isSubmergable = value; }
		}

		public bool IsSubmerged {
			get {
				if (!isSubmergable || ZPosition > 0)
					return false;
				Tile tile = RoomControl.TileManager.GetSurfaceTileAtPosition(Position);
				if (tile == null)
					return false;
				return (tile.EnvironmentType == TileEnvironmentType.Water ||
						tile.EnvironmentType == TileEnvironmentType.DeepWater ||
						tile.EnvironmentType == TileEnvironmentType.Ocean ||
						tile.EnvironmentType == TileEnvironmentType.Lava);
			}
		}
	}
}
