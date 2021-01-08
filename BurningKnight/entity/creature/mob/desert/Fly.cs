using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.projectile;
using BurningKnight.level;
using Lens.entity;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.desert {
	public class Fly : Mob {
		protected const float DefaultZ = 2;
		
		protected override void SetStats() {
			base.SetStats();

			SetMaxHp(3);
			
			var body = new SensorBodyComponent(1, 1, 10, 8);
			AddComponent(body);
			body.Body.LinearDamping = 4;
			
			AddComponent(new ZAnimationComponent("fly"));
			AddComponent(new ZComponent());
			
			AddComponent(new OrbitalComponent {
				Radius = 12,
				Lerp = true
			});

			Depth = Layers.Wall;
			Become<IdleState>();
		}

		public override bool InAir() {
			return true;
		}

		private void Fire() {
			if (Target == null) {
				return;
			}
			
			GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");
			var a = GetComponent<ZAnimationComponent>();
					
			Tween.To(0.6f, a.Scale.X, x => a.Scale.X = x, 0.2f);
			Tween.To(1.6f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
				Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
				Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

					Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
					Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f);
				
					var an = AngleTo(Target);
					var projectile = Projectile.Make(this, "small", an, 8f, false, 0, null, 0.8f);

					projectile.Center = Center + MathUtils.CreateVector(an, 2f);
					projectile.AddLight(32f, ProjectileColor.Red);
				};
			};
		}

		#region Fly States
		public class IdleState : SmartState<Fly> {
			private bool searched;
			private Entity target;

			public override void Init() {
				base.Init();
				
				var component = Self.GetComponent<ZComponent>();
				Tween.To(0, component.Z, x => component.Z = x, 0.4f, Ease.BackOut);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (!searched) {
					searched = true;
					target = Self.GetComponent<RoomComponent>().Room?.FindClosest(Self.Center, Tags.Mob, e => !e.HasComponent<OrbitalComponent>() && !(e is WallWalker || e is Boss));
				}

				if (target != null) {
					if (!target.HasComponent<OrbitGiverComponent>()) {
						target.AddComponent(new OrbitGiverComponent());
					}
					
					target.GetComponent<OrbitGiverComponent>().AddOrbiter(Self);
					Become<OrbitingState>();
					return;
				}

				if (Self.Target == null) {
					return;
				}
				
				var dx = Self.DxTo(Self.Target);
				var dy = Self.DyTo(Self.Target);
				var d = MathUtils.Distance(dx, dy);

				var s = dt * 300;

				Self.GetComponent<SensorBodyComponent>().Velocity += new Vector2(dx / d * s, dy / d * s);
				Self.PushFromOtherEnemies(dt, e => e.InAir());

				if (T >= 3f) {
					T = 0;
					Self.Fire();
				}
			}
		}
		
		public class OrbitingState : SmartState<Fly> {
			public override void Init() {
				base.Init();
				
				var component = Self.GetComponent<ZComponent>();
				Tween.To(DefaultZ, component.Z, x => component.Z = x, 0.4f, Ease.BackOut);
			}

			public override void Update(float dt) {
				base.Update(dt);

				var orbiting = Self.GetComponent<OrbitalComponent>().Orbiting;
				
				if (orbiting == null) {
					Become<IdleState>();
					return;
				}
				
				if (orbiting.TryGetComponent<ZComponent>(out var z)) {
					Self.GetComponent<ZComponent>().Z = z.Z + DefaultZ;
				}
				
				if (T >= 3f) {
					T = 0;
					Self.Fire();
				}
			}
		}
		#endregion

		public override bool ShouldCollide(Entity entity) {
			return !(entity is Level) && base.ShouldCollide(entity);
		}

		protected override string GetDeadSfx() {
			return "mob_fly_death";
		}
	}
}