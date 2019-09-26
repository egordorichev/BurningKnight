using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class Clown : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("clown");
			SetMaxHp(2);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.KnockbackModifier = 2;
			body.Body.LinearDamping = 0;
			
			AddDrops(new SingleDrop("bk:bomb", 0.1f));

			TouchDamage = 0;
		}

		#region Clown States
		public class IdleState : SmartState<Clown> {
			public override void Init() {
				base.Init();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}
		}
		
		public class RunState : SmartState<Clown> {
			private Vector2 target;
			private float delay;
			private bool away;
			
			public override void Init() {
				base.Init();
				T = 0;
				
				if (away) {
					target = Self.Center + MathUtils.CreateVector(Self.Target.AngleTo(Self) + Random.Float(-1, 1), 96f);
					delay = Random.Float(1f, 2f);
					return;
				}
				
				delay = Random.Float(0.3f, 1f);
				var toTarget = Self.Target != null && Random.Chance();

				if (toTarget) {
					target = Self.Target.Center;
				} else {
					target = Self.GetComponent<RoomComponent>().Room.GetRandomFreeTile();
				}
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				var dx = Self.DxTo(target);
				var dy = Self.DyTo(target);
				var d = MathUtils.Distance(dx, dy);
				var s = dt * 150;

				var b = Self.GetComponent<RectBodyComponent>();
				b.Velocity += new Vector2(dx / d * s, dy / d * s);

				if (d <= 8 || T >= delay) {
					if (away) {
						away = false;
					}
					
					Init();
					return;
				}

				if (!away && Self.Target != null && Self.DistanceTo(Self.Target) < 8) {
					var bomb = new Bomb(1);
					Self.Area.Add(bomb);
					bomb.Center = Self.Center;
					bomb.VelocityTo(Self.AngleTo(Self.Target));

					away = true;
					Init();
				}
			}
		}
		#endregion

		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent) {
				GetComponent<HealthComponent>().ModifyHealth(2, null);
				return true;
			} else if (e is MobTargetChange ev) {
				if (ev.New == null) {
					Become<IdleState>();
				} else {
					Become<RunState>();
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}