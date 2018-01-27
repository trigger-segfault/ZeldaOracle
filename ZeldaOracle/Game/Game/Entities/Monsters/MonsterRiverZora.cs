using System;
using System.Collections.Generic;
using ZeldaOracle.Common.Audio;
using ZeldaOracle.Common.Geometry;
using ZeldaOracle.Game.Entities.Effects;
using ZeldaOracle.Game.Entities.Players;
using ZeldaOracle.Game.Entities.Projectiles.MagicProjectiles;

namespace ZeldaOracle.Game.Entities.Monsters {
	
	public class MonsterStateMachine<IDType> where IDType : struct, IConvertible {

		public class State {
			public IDType ID { get; set; }
			public StateBeginDelegate BeginFunction { get; set; }
			public StateUdpateDelegate UpdateFunction { get; set; }
			public StateEndDelegate EndFunction { get; set; }
			public MonsterStateMachine<IDType> StateMachine { get; set; }
			public bool IsActive { get; set; }
			
			public State() {
				IsActive = false;
			}

			public virtual void Begin() {
				if (BeginFunction != null)
					BeginFunction();
				IsActive = true;
			}
			public virtual void Update() {
				if (UpdateFunction != null)
					UpdateFunction();
			}
			public virtual void End() {
				if (EndFunction != null)
					EndFunction();
				IsActive = false;
			}
		}

		public class TimedState : State {
			private int timer;
			public int Duration { get; set; }
			public IDType NextState { get; set; }
			private List<Tuple<int, StateEventDelegate>> timedEvents;

			public TimedState() {
				timedEvents = new List<Tuple<int, StateEventDelegate>>();
			}

			public TimedState AddEvent(int time, StateEventDelegate function) {
				timedEvents.Add(new Tuple<int, StateEventDelegate>
					(time, function));
				return this;
			}

			public TimedState OnBegin(StateBeginDelegate function) {
				BeginFunction = function;
				return this;
			}

			public TimedState OnEnd(StateEndDelegate function) {
				EndFunction = function;
				return this;
			}

			public TimedState OnUpdate(StateUdpateDelegate function) {
				UpdateFunction = function;
				return this;
			}

			public TimedState AppendEvent(int delay, StateEventDelegate function) {
				int time = delay;
				if (timedEvents.Count > 0)
					time += timedEvents[timedEvents.Count - 1].Item1;
				timedEvents.Add(new Tuple<int, StateEventDelegate>
					(time, function));
				return this;
			}

			public TimedState SetDuration(int duration) {
				Duration = duration;
				return this;
			}

			public override void Begin() {
				base.Begin();
				timer = 0;
			}

			public override void Update() {
				base.Update();
				if (IsActive) {
					timer++;
					foreach (var timedEvent in timedEvents) {
						if (timedEvent.Item1 == timer)
							timedEvent.Item2();
					}
					if (Duration >= 0 && timer > Duration) {
						//if (NextState != null)
							//StateMachine.BeginState(NextState);
						//else
							StateMachine.NextState();
					}
				}
			}
		}
		
		public delegate void StateBeginDelegate();
		public delegate void StateUdpateDelegate();
		public delegate void StateEndDelegate();
		public delegate void StateEventDelegate();

		private Dictionary<IDType, State> states;
		private State currentState;
		private List<IDType> stateIds;

		public MonsterStateMachine() {
			states = new Dictionary<IDType, State>();
			stateIds = new List<IDType>();
			foreach (IDType id in Enum.GetValues(typeof(IDType)))
				stateIds.Add(id);
		}

		public TimedState AddTimedState(IDType id, int duration,
			StateBeginDelegate begin, StateUdpateDelegate update)
		{
			return AddTimedState(id, duration, begin, null, update);
		}

		public TimedState AddTimedState(IDType id, int duration,
			StateBeginDelegate begin, StateEndDelegate end, StateUdpateDelegate update)
		{
			TimedState state = new TimedState() {
				ID				= id,
				BeginFunction	= begin,
				EndFunction		= end,
				UpdateFunction	= update,
				StateMachine	= this,
				Duration		= duration,
			};
			states[id] = state;
			return state;
		}

		public TimedState AddTimedState(IDType id, int duration, IDType nextId, StateBeginDelegate begin, StateEndDelegate end,
			StateUdpateDelegate update)
		{
			TimedState state = new TimedState() {
				ID				= id,
				BeginFunction	= begin,
				EndFunction		= end,
				UpdateFunction	= update,
				StateMachine	= this,
				Duration		= duration,
				NextState		= nextId,
			};
			states[id] = state;
			return state;
		}

		public TimedState AddState(IDType id) {
			TimedState state = new TimedState() {
				ID				= id,
				BeginFunction	= null,
				EndFunction		= null,
				UpdateFunction	= null,
				StateMachine	= this,
				Duration		= -1,
			};
			states[id] = state;
			return state;
		}

		public void AddState(IDType id, StateBeginDelegate begin, StateEndDelegate end,
			StateUdpateDelegate update)
		{
			states[id] = new State() {
				ID				= id,
				BeginFunction	= begin,
				EndFunction		= end,
				UpdateFunction	= update,
				StateMachine	= this,
			};
		}

		public void AddState(IDType id, StateBeginDelegate begin, StateUdpateDelegate update) {
			AddState(id, begin, null, update);
		}

		public void AddState(IDType id, StateUdpateDelegate update) {
			AddState(id, null, null, update);
		}

		public void Update() {
			if (currentState != null)
				currentState.Update();
		}

		public void BeginState(IDType id) {
			if (currentState != null)
				currentState.End();
			currentState = states[id];
			if (currentState != null)
				currentState.Begin();
		}

		public void NextState() {
			int index = stateIds.IndexOf(currentState.ID);
			index = (index + 1) % stateIds.Count;
			BeginState(stateIds[index]);
		}
	}

	public class MonsterRiverZora : Monster {
		
		private enum RiverZoraState {
			Submerged,
			Resurfacing,
			Resurfaced,
		}
		
		private RiverZoraState riverZoraState;
		private FireballProjectile fireball;
		private bool isShooting;
		private int timer;
		private MonsterStateMachine<RiverZoraState> stateMachine;

		
		//-----------------------------------------------------------------------------
		// Constructor
		//-----------------------------------------------------------------------------

		public MonsterRiverZora() {
			// General
			MaxHealth		= 1;
			ContactDamage	= 2;
			color			= MonsterColor.Red;
			isGaleable		= false;
			isKnockbackable	= false;
			isFlying		= true;

			// Physics
			Physics.HasGravity			= false;
			Physics.CollideWithWorld	= false;
			Physics.IsDestroyedInHoles	= false;

			// Weapon interactions
			SetReaction(InteractionType.Sword,			Reactions.Kill);
			SetReaction(InteractionType.BiggoronSword,	Reactions.Kill);
			SetReaction(InteractionType.SwordSpin,		Reactions.Kill);
			SetReaction(InteractionType.Shield,			SenderReactions.Bump);
			SetReaction(InteractionType.Shovel,			SenderReactions.Bump);
			// Seed interactions
			SetReaction(InteractionType.ScentSeed,		SenderReactions.Intercept);
			SetReaction(InteractionType.PegasusSeed,	SenderReactions.Intercept);
			// Projectile interactions
			SetReaction(InteractionType.Boomerang,		SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.Arrow,			SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.SwordBeam,		SenderReactions.Intercept, Reactions.Kill);
			SetReaction(InteractionType.SwitchHook,		SenderReactions.Intercept, Reactions.Kill);

			stateMachine = new MonsterStateMachine<RiverZoraState>();
			stateMachine.AddState(RiverZoraState.Submerged)
				.OnBegin(BeginSubmerged)
				.SetDuration(48);
			stateMachine.AddState(RiverZoraState.Resurfacing)
				.OnBegin(BeginResurfacing)
				.SetDuration(48);
			stateMachine.AddState(RiverZoraState.Resurfaced)
				.OnBegin(BeginResurfaced)
				.AppendEvent(48,	OpenMouth)
				.AppendEvent(9,		SpawnFireball)
				.AppendEvent(9,		ShootFireball)
				.AppendEvent(22,	CloseMouth)
				.AppendEvent(9,		stateMachine.NextState)
				.OnEnd(EndResurfaced);
		}


		//-----------------------------------------------------------------------------
		// States
		//-----------------------------------------------------------------------------

		public void BeginSubmerged() {
			IsPassable = true;
			Graphics.IsVisible = false;
		}

		public void BeginResurfacing() {
			Graphics.IsVisible = true;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_RIVER_ZORA_WATER_SWIRLS);
		}

		public void BeginResurfaced() {
			IsPassable = false;
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_RIVER_ZORA);
		}

		public void OpenMouth() {
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_RIVER_ZORA_SHOOT);
		}

		public void SpawnFireball() {
			fireball = new FireballProjectile();
			ShootProjectile(fireball, Vector2F.Zero);
		}

		public void ShootFireball() {
			Vector2F vectorToPlayer = RoomControl.Player.Center - Center;
			fireball.Physics.Velocity = vectorToPlayer.Normalized *
				GameSettings.MONSTER_GOPONGA_FLOWER_SHOOT_SPEED;
			fireball = null;
		}

		public void CloseMouth() {
			Graphics.PlayAnimation(GameData.ANIM_MONSTER_RIVER_ZORA);
		}

		public void EndResurfaced() {
			IsPassable = true;
			RoomControl.SpawnEntity(new Effect(GameData.ANIM_EFFECT_WATER_SPLASH,
				DepthLayer.EffectSplash, true), position);
			AudioSystem.PlaySound(GameData.SOUND_PLAYER_WADE);
		}


		//-----------------------------------------------------------------------------
		// Overridden Methods
		//-----------------------------------------------------------------------------
		
		public override void Initialize() {
			base.Initialize();
			stateMachine.BeginState(RiverZoraState.Submerged);
		}

		public override void OnDestroy() {
			base.OnDestroy();
			
			// If we spawned a fireball and have not shot it, then destroy it
			if (fireball != null)
				fireball.Destroy();
		}

		public override void UpdateAI() {
			stateMachine.Update();
		}
	}
}
