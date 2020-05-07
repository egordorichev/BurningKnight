using System;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using BurningKnight.level;
using BurningKnight.physics;
using Lens;
using Lens.entity;
using Lens.util;

namespace BurningKnight.entity.projectile {
	public class Laser : Projectile {
		public float LifeTime = 1.5f;
		public bool Dynamic = true;
		public float AdditionalAngle;
		
		private Laser() {
			BreaksFromWalls = false;
			Spectral = true;
			CanBeBroken = false;
			CanBeReflected = false;
			PreventDespawn = true;
			ManualRotation = true;
		}

		public static Laser Make(Entity owner, float a, float additional, float damage = 1, float scale = 1f) {
			var laser = new Laser();
			
			laser.Damage = damage;
			laser.StarterOwner = owner;
			laser.Owner = owner;

			owner.Area.Add(laser);
			
			var graphics = new LaserGraphicsComponent("projectiles", "laser");
			laser.AddComponent(graphics);

			laser.Scale = scale;
			laser.Width = 32;
			laser.Height = 9 * scale;

			laser.CreateBody();
			laser.AdditionalAngle = additional;
			laser.BodyComponent.Body.Rotation = a + additional;

			return laser;
		}

		private void CreateBody() {
			AddComponent(BodyComponent = new RectBodyComponent(0, -Height * 0.5f, Width, Height));
		}

		public override bool BreaksFrom(Entity entity) {
			return false;
		}

		private static bool RayShouldCollide(Entity entity) {
			return entity is ProjectileLevelBody || entity is Door;
		}

		public void Recalculate() {
			var min = 1f;
			var aim = Owner.GetComponent<AimComponent>();
			var from = aim.Center;
			var to = aim.RealAim;
			var closest = from + MathUtils.CreateVector(BodyComponent.Body.Rotation, Display.UiWidth);
				
			Physics.World.RayCast((fixture, point, normal, fraction) => {
				if (min > fraction && fixture.Body.UserData is BodyComponent b && RayShouldCollide(b.Entity)) {
					min = fraction;
					closest = point;
				}
				
				return min;
			}, from, to);

			var len = (from - closest).Length();

			if (Math.Abs(len - Width) > 2) {
				Width = len;
				BodyComponent.Resize(0, -Height * 0.5f, Width, Height);
			}
		}

		private float lastClear;

		public override void Update(float dt) {
			base.Update(dt);

			lastClear += dt;

			if (lastClear >= 0.3f) {
				lastClear = 0;
				EntitiesHurt.Clear();
			}

			if (LifeTime > 0) {
				LifeTime -= dt;

				if (LifeTime <= 0) {
					Done = true;
					LifeTime = 0;
					return;
				}
			}

			if (Dynamic) {
				Recalculate();
			}
		}

		public override void AdjustScale(float newScale) {
			Scale = newScale;
			Height = 9 * Scale;
			GetComponent<RectBodyComponent>().Resize(0, 0, Width, Height, true);
		}
	}
}