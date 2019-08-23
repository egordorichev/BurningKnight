using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.file;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.mob {
	public class Mimic : Mob {
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 20;
			Height = 12;
			
			AddComponent(new AnimationComponent("mimic") {
				Offset = new Vector2(-1, -6)
			});
			
			AddComponent(new RectBodyComponent(1, 6, Width - 2, Height - 14, BodyType.Static));
			AddComponent(new ShadowComponent());
			AddComponent(new PoolDropsComponent(ItemPool.Chest, 1f, 1, 1));
			AddComponent(new SparkEmitterComponent(1f, 30f));

			RemoveTag(Tags.MustBeKilled);

			GetComponent<HealthComponent>().InitMaxHealth = 10 + Run.Depth * 5;
			GetComponent<StateComponent>().Become<IdleState>();
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			if (!stream.ReadBoolean()) {
				Discover();
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(GetComponent<StateComponent>().StateInstance is IdleState);
		}

		private void Discover() {
			RemoveComponent<SparkEmitterComponent>();
			
			AddTag(Tags.MustBeKilled);
			GetComponent<StateComponent>().Become<OpenState>();
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent hme) {
				if (hme.Amount < 0) {
					var s = GetComponent<StateComponent>().StateInstance;

					if (s is IdleState) {
						Discover();
					} else if (s is HiddenState) {
						return true;
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		#region Mimic States
		private class IdleState : SmartState<Mimic> {
			public override void Update(float dt) {
				base.Update(dt);
				var room = Self.GetComponent<RoomComponent>().Room;

				if (room == null) {
					return;
				}

				foreach (var p in room.Tagged[Tags.Player]) {
					if (p.DistanceTo(Self) < 64f) {
						Self.Discover();
					}
				}
			}
		}
		
		private class OpenState : SmartState<Mimic> {
			private bool tweened;
			
			public override void Update(float dt) {
				base.Update(dt);

				if (T > 3f && !tweened) {
					var a = Self.GetComponent<AnimationComponent>();
					
					Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
					Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.2f);
						Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.2f);

						Become<HiddenState>();
					};
				}
			}
		}

		private class HiddenState : SmartState<Mimic> {
			private bool tweened;
			
			public override void Update(float dt) {
				base.Update(dt);

				if (T > 3f && !tweened) {
					tweened = true;
					var a = Self.GetComponent<AnimationComponent>();
					
					Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
					Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.2f);
						Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.2f);

						Become<OpenState>();
					};
				}
			}
		}
		#endregion
	}
}