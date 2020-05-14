using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.bk;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.mob.boss {
	public class BkHead : Boss {
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new BkGraphicsComponent("demon"));
			AddComponent(new RectBodyComponent(2, 4, 12, 15, BodyType.Dynamic, true));
			AddComponent(new AimComponent(AimComponent.AimType.Target));


			GetComponent<RectBodyComponent>().Body.LinearDamping = 2;
			Depth = Layers.FlyingMob;
		}

		protected override void OnTargetChange(Entity target) {
			base.OnTargetChange(target);

			if (target != null) {
				Become<IdleState>();
			}
		}

		public override bool InAir() {
			return true;
		}

		public override void SelectAttack() {
			base.SelectAttack();
			Become<IdleState>();
		}

		private float t;

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;
			
			if (Target != null) {
				var force = 40f * dt;
				var a = AngleTo(Target);

				GetComponent<RectBodyComponent>().Velocity += new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
			}
		}

		private int counter;

		private void WarnLaser(float angle, Vector2? offset = null) {
			var projectile = Projectile.Make(this, "circle", angle, 20f);

			projectile.AddLight(32f, Projectile.RedLight);
			projectile.Center += MathUtils.CreateVector(angle, 8);

			projectile.CanBeBroken = false;
			projectile.CanBeReflected = false;

			if (offset != null) {
				projectile.Center += offset.Value;
			}
		}

		#region Demon States
		private class IdleState : SmartState<BkHead> {
			public override void Init() {
				base.Init();
				Self.TouchDamage = 2;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T <= 1f) {
					return;
				}

				switch (Self.counter) {
					case 0: {
						Become<LaserSnipeState>();
						break;
					}
					
					case 1: {
						Become<BulletHellState>();
						break;
					}
				}

				Self.counter = (Self.counter + 1) % 2;
			}
		}

		private class LaserSnipeState : SmartState<BkHead> {
			private float delay;
			private int count;
			
			public override void Update(float dt) {
				base.Update(dt);
				delay -= dt;
				
				if (delay <= 0) {
					if (count >= 3) {
						Become<IdleState>();
						return;
					}
					
					delay = 0.5f;
					var a = Self.AngleTo(Self.Target);
					Self.WarnLaser(a);

					Timer.Add(() => {
						var laser = Laser.Make(Self, a, 0, damage: 2, scale: 3, range: 64);
						laser.LifeTime = 1f;
						laser.Position = Self.Center;
					}, 0.2f);

					count++;
				}
			}
		}

		private class BulletHellState : SmartState<BkHead> {
			private float sinceLast;
		
			public override void Update(float dt) {
				base.Update(dt);
				sinceLast -= dt;

				if (sinceLast <= 0) {
					sinceLast = 0.3f; // Self.InFirstPhase ? 0.5f : 0.3f;
					var amount = 4;

					for (var i = 0; i < amount; i++) {
						var a = Math.PI * 2 * ((float) i / amount) + (Math.Cos(Self.t * 2f) * Math.PI) * (i % 2 == 0 ? -1 : 1);
						var projectile = Projectile.Make(Self, "small", a, 5f + (float) Math.Cos(Self.t * 2f) * 2f, scale: 1f);
						
						projectile.CanBeBroken = false;
						projectile.CanBeReflected = false;
						projectile.Color = ProjectileColor.DesertRainbow[Rnd.Int(ProjectileColor.DesertRainbow.Length)];
					}
				}

				if (T >= 5f) {
					Become<IdleState>();
				}
			}
		}
		#endregion
	}
}