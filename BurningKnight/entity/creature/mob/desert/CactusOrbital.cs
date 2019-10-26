using System;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util;
using Lens.util.tween;

namespace BurningKnight.entity.creature.mob.desert {
	public class CactusOrbital : Mob {
		protected const float DefaultZ = 2;
		
		protected override void SetStats() {
			base.SetStats();

			SetMaxHp(5);
			
			AddComponent(new ZAnimationComponent("cactus_orbital"));
			AddComponent(new RectBodyComponent(1, 1, 8, 7));
			AddComponent(new ZComponent());
			
			AddComponent(new OrbitalComponent {
				Radius = 12
			});
			
			Become<IdleState>();
		}
		
		public override float GetSpawnChance() {
			return 0.5f;
		}

		public override bool InAir() {
			return true;
		}

		private void Fire() {
			if (Target == null) {
				return;
			}
			
			var a = GetComponent<ZAnimationComponent>();
					
			Tween.To(0.6f, a.Scale.X, x => a.Scale.X = x, 0.2f);
			Tween.To(1.6f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
				Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
				Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

					Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
					Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f);
				
					var an = AngleTo(Target);
					var projectile = Projectile.Make(this, "spike", an, 6f, false);

					projectile.Center = Center + MathUtils.CreateVector(an, 2f);
					projectile.AddLight(32f, Projectile.RedLight);

					projectile.OnDeath += (p, t) => {
						an -= (float) Math.PI;

						var ap = Projectile.Make(this, "spike", an - 0.2f, 8f, false, 0, projectile, 0.6f);
						ap.Center = projectile.Center;
						ap.AddLight(32f, Projectile.RedLight);

						var bp = Projectile.Make(this, "spike", an + 0.2f, 8f, false, 0, projectile, 0.6f);
						bp.Center = projectile.Center;
						bp.AddLight(32f, Projectile.RedLight);
					};
				};
			};
		}

		#region Cactus States
		public class IdleState : SmartState<CactusOrbital> {
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
					target = Self.GetComponent<RoomComponent>().Room?.FindClosest(Self.Center, Tags.Mob, e => !e.HasComponent<OrbitalComponent>());
				}

				if (target != null) {
					if (!target.HasComponent<OrbitGiverComponent>()) {
						target.AddComponent(new OrbitGiverComponent());
					}
					
					target.GetComponent<OrbitGiverComponent>().AddOrbiter(Self);
					Become<OrbitingState>();
					return;
				}

				if (T >= 2f) {
					T = 0;
					Self.Fire();
				}
			}
		}
		
		public class OrbitingState : SmartState<CactusOrbital> {
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
				
				if (T >= 1f) {
					T = 0;
					Self.Fire();
				}
			}
		}
		#endregion
	}
}