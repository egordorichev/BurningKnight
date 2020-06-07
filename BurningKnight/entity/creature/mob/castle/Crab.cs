using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.level;
using BurningKnight.level.entities;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class Crab : Mob {
		protected override void SetStats() {
			base.SetStats();

			vertical = Rnd.Chance();
			
			Width = 18;
			Height = 14;
			
			AddAnimation("crab");
			SetMaxHp(3);
			
			Become<WaitState>();

			var body = new RectBodyComponent(3, 13, 12, 1);
			AddComponent(body);
			body.Body.LinearDamping = 10;

			AddComponent(new SensorBodyComponent(3, 2, 12, 12));
			AddDrops(new SingleDrop("bk:crabs_claw", 0.01f));
		}

		protected override void OnTargetChange(Entity target) {
			base.OnTargetChange(target);

			if (target != null) {
				Become<IdleState>();
			} else {
				Become<WaitState>();
			}
		}

		private bool vertical;
		
		#region Bandit States
		public class WaitState : SmartState<Crab> {
			
		}
		
		public class IdleState : SmartState<Crab> {
			private Vector2 velocity;
			private bool a;
			private float timer;

			public void Flip(bool force = false) {
				a = !a;

				timer = Rnd.Float(1f, 10f);

				if (force || Rnd.Chance(25)) {
					Self.vertical = !Self.vertical;
					a = Rnd.Chance();
				}

				var f = 40;
				velocity = new Vector2(Self.vertical ? 0 : (a ? -f : f), Self.vertical ? (a ? -f : f) : 0);
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;

				var an = Self.GetComponent<MobAnimationComponent>();
				an.Animation.Frame = (uint) Rnd.Int(4);
				an.Animate();
			}
			
			public override void Init() {
				base.Init();

				a = Rnd.Chance();
				Flip();
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);
				var b = Self.GetComponent<RectBodyComponent>();

				if (T >= timer || b.Velocity.Length() < 20) {
					Flip(T >= timer);
					T = 0;
					return;
				}
				
				b.Velocity = velocity;
			}
		}
		#endregion

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity is Door || ev.Entity is Level || ev.Entity is Chasm || (ev.Entity is SolidProp && !(ev.Entity is BreakableProp))) {
					var s = GetComponent<StateComponent>().StateInstance;

					if (s is IdleState i) {
						i.Flip();
					}
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}