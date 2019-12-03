using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.level.entities;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.room.controllable.spikes {
	public class Spikes : RoomControllable {
		private static TextureRegion tile;
		private static Vector2 offset = new Vector2(0, 5);
		
		protected List<Entity> Colliding = new List<Entity>();

		public override void Init() {
			base.Init();

			InitState();
			Y -= offset.Y;
		}

		protected virtual void InitState() {
			On = false;
		}

		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			Height = 21;

			if (tile == null) {
				tile = CommonAse.Props.GetSlice("spikes_base");
			}

			Area.Add(new RenderTrigger(this, RenderBase, Layers.Entrance));
			
			AddComponent(new StateComponent());
			AddComponent(new AnimationComponent("spikes"));
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new AudioEmitterComponent());
			AddComponent(new RectBodyComponent(2, 4, 12, 12, BodyType.Static, true));
			
			GetComponent<StateComponent>().Become<HiddenState>();
		}

		private void RenderBase() {
			Graphics.Render(tile, Position + offset);
		}

		private void RenderShadow() {
			GraphicsComponent.Offset.Y -= 7;
			GraphicsComponent.Render(true);
			GraphicsComponent.Offset.Y += 7;
		}
		
		public override void TurnOn() {
			base.TurnOn();
			var s = GetComponent<StateComponent>();

			if (s.StateInstance is HiddenState || s.StateInstance is HidingState) {
				s.Become<ShowingState>();
			}
		}

		public void TurnOnSlowly() {
			if (On) {
				return;
			}

			On = true;
			var s = GetComponent<StateComponent>();

			if (s.StateInstance is HiddenState || s.StateInstance is HidingState) {
				s.Become<FshowingState>();
			}
		}

		public override void TurnOff() {
			base.TurnOff();
			var s = GetComponent<StateComponent>();

			if (s.StateInstance is IdleState || s.StateInstance is ShowingState || s.StateInstance is FshowingState) {
				s.Become<HidingState>();
			}
		}

		protected void Hurt() {
			foreach (var c in Colliding) {
				if (!(c is Creature cc && cc.InAir()) || c is ExplodingBarrel) {
					c.GetComponent<HealthComponent>().ModifyHealth(-1, this);
				}
			}
		}

		#region Spike States
		protected class IdleState : SmartState<Spikes> {
			public override void Update(float dt) {
				base.Update(dt);
				Self.Hurt();
			}
		}

		protected class HiddenState : SmartState<Spikes> {
			
		}

		protected class ShowingState : SmartState<Spikes> {
			public override void Init() {
				base.Init();

				if (Self.OnScreen) {
					Self.GetComponent<AudioEmitterComponent>().Emit("level_spike");
				}

				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);
				var a = Self.GetComponent<AnimationComponent>().Animation;

				if (a.Frame > 3) {
					Self.Hurt();
				}
				
				if (a.Paused) {
					Self.GetComponent<StateComponent>().Become<IdleState>();
				}
			}
		}
		
		protected class FshowingState : SmartState<Spikes> {
			private bool playedSfx;
			
			public override void Init() {
				base.Init();
				Self.GetComponent<AudioEmitterComponent>().Emit("level_spike_peaking");
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);
				var a = Self.GetComponent<AnimationComponent>().Animation;

				if (a.Frame > 2) {
					Self.Hurt();

					if (!playedSfx) {
						playedSfx = true;

						if (Self.OnScreen) {
							Self.GetComponent<AudioEmitterComponent>().EmitRandomized("level_spike");
						}
					}
				}
				
				if (a.Paused) {
					Self.GetComponent<StateComponent>().Become<IdleState>();
				}
			}
		}

		protected class HidingState : SmartState<Spikes> {
			public override void Init() {
				base.Init();
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);
				var a = Self.GetComponent<AnimationComponent>().Animation;

				if (a.Frame < 2) {
					Self.Hurt();
				}

				if (a.Paused) {
					Self.GetComponent<StateComponent>().Become<HiddenState>();
				}
			}
		}
		#endregion

		protected virtual bool ShouldHurt(Entity e) {
			return e.HasComponent<HealthComponent>();
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (ShouldHurt(cse.Entity)) {
					Colliding.Add(cse.Entity);
				}
			} else if (e is CollisionEndedEvent cee) {
				Colliding.Remove(cee.Entity);
			}
			
			return base.HandleEvent(e);
		}
	}
}