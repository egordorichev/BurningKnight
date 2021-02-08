using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.mob.desert;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using BurningKnight.level;
using Lens.entity;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.bk {
	public class BkOrbital : Mob {
		protected const float DefaultZ = 2;
		public int Id;
		
		protected override void SetStats() {
			base.SetStats();

			SetMaxHp(50);
			RemoveTag(Tags.LevelSave);

			Width = 12;
			Height = 13;
			TouchDamage = 2;
			
			var body = new SensorBodyComponent(1, 2, 10, 10);
			AddComponent(body);
			body.Body.LinearDamping = 4;
			
			AddComponent(new ZAnimationComponent("bk_orbital"));
			AddComponent(new ZComponent());
			AddComponent(new OrbitalComponent());
			
			AddComponent(new OrbitalComponent {
				Radius = 24,
				Lerp = true
			});

			Depth = Layers.Wall;
			Become<OrbitingState>();
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
					var builder = new ProjectileBuilder(this, "big") {
						Scale = 0.8f,
						LightRadius = 32f,
						Color = ProjectileColor.Orange,
						Damage = 2
					};

					builder.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);
					builder.Shoot(an, 8f);

					var projectile = builder.Build();

					projectile.Center = Center + MathUtils.CreateVector(an, 4f);
					ProjectileCallbacks.AttachUpdateCallback(projectile, TargetProjectileController.Make(Target, 0.2f));
				};
			};
		}

		#region Fly States
		public class OrbitingState : SmartState<BkOrbital> {
			public override void Init() {
				base.Init();

				T = Self.Id * 1f;
				
				var component = Self.GetComponent<ZComponent>();
				Tween.To(DefaultZ, component.Z, x => component.Z = x, 0.4f, Ease.BackOut);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 10f) {
					T = 0;
					Self.Fire();
				}

				var o = Self.GetComponent<OrbitalComponent>().Orbiting;

				if (o == null || o.Done) {
					Self.Kill(Self);
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