using BurningKnight.entity.bomb;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class Clown : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("clown");
			SetMaxHp(3);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 15, 12, 1);
			AddComponent(body);

			body.KnockbackModifier = 2;
			body.Body.LinearDamping = 0;
			
			AddComponent(new SensorBodyComponent(2, 2, 12, 12));
			
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
					target = Self.Center + MathUtils.CreateVector(Self.Target.AngleTo(Self) + Rnd.Float(-1, 1), 96f);
					delay = Rnd.Float(1f, 2f);
					return;
				}
				
				delay = Rnd.Float(0.3f, 1f);
				var toTarget = Self.Target != null && Rnd.Chance();

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
				}

				if (!away && Self.Target != null && Self.DistanceTo(Self.Target) < 32) {
					var bomb = new Bomb(Self, 1);
					Self.Area.Add(bomb);
					bomb.Center = Self.Center;
					bomb.VelocityTo(Self.AngleTo(Self.Target));

					Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("mob_clown_bomb", 2);

					away = true;
					Init();
				}
			}
		}
		#endregion

		public override bool HandleEvent(Event e) {
			if (e is ExplodedEvent ee) {
				if (ee.Who is Mob) {
					// GetComponent<HealthComponent>().ModifyHealth(2, null);
					return true;
				}
			} else if (e is MobTargetChange ev) {
				if (ev.New == null) {
					Become<IdleState>();
				} else {
					Become<RunState>();
				}
			}
			
			return base.HandleEvent(e);
		}

		protected override string GetHurtSfx() {
			return "mob_clown_hurt";
		}

		protected override string GetDeadSfx() {
			return "mob_clown_death";
		}
	}
}