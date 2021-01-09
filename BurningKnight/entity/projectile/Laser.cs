using System;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level;
using BurningKnight.physics;
using Lens;
using Lens.entity;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	/*public class Laser : Projectile {
		public float LifeTime = 1.5f;
		public bool Dynamic = true;
		public float AdditionalAngle;
		public float Range = 15f;
		public Vector2 End;
		public bool PlayerRotated;

		public float Angle {
			get => BodyComponent.Body.Rotation;
			set => BodyComponent.Body.Rotation = value;
		}
		
		private Laser() {
			BreaksFromWalls = false;
			Spectral = true;
			CanBeBroken = false;
			CanBeReflected = false;
			PreventDespawn = true;
			ManualRotation = true;
		}

		public static Laser Make(Entity owner, float a, float additional, Item item = null, float damage = 1, float scale = 1f, float range = -1, Laser parent = null) {
			var laser = new Laser();

			laser.Damage = damage;
			laser.StarterOwner = owner;
			laser.Owner = owner;
			laser.Color = ProjectileColor.Red;
			laser.DieOffscreen = false;
			laser.PreventSpectralBreak = true;

			if (parent != null) {
				laser.Color = parent.Color;
				laser.Parent = parent;
				laser.Range = parent.Range * 0.5f;
			}

			owner.Area.Add(laser);
			
			var graphics = new LaserGraphicsComponent("projectiles", "laser");
			laser.AddComponent(graphics);
			laser.Scale = scale;

			if (parent != null) {
				laser.Scale *= 0.7f;
			}

			if (range > 0) {
				laser.Range = range;
			}

			owner.HandleEvent(new ProjectileCreatedEvent {
				Owner = owner,
				Item = item,
				Projectile = laser
			});

			laser.Width = 32;
			laser.Height = 9 * laser.Scale;

			laser.CreateBody();
			laser.AdditionalAngle = additional;
			laser.BodyComponent.Body.Rotation = a + additional;

			return laser;
		}

		public override void AddComponents() {
			base.AddComponents();
			AddTag(Tags.Laser);
		}

		private void CreateBody() {
			AddComponent(BodyComponent = new RectBodyComponent(0, -Height * 0.5f, Width, Height));
		}

		public override bool BreaksFrom(Entity entity, BodyComponent body) {
			return false;
		}

		private static bool RayShouldCollide(Entity entity) {
			return entity is ProjectileLevelBody || entity is Level || entity is Door;
		}

		public void Recalculate() {
			var min = 1f;

			Vector2 closest;
			var aim = Owner.GetComponent<AimComponent>();

			if (PlayerRotated) {
				Position = aim.Center;
			}
			
			var from = Position;
			
			if (PlayerRotated) {
				closest = Position + MathUtils.CreateVector((aim.RealAim - from).ToAngle(), Range * 5);
			} else {
				closest = Position + MathUtils.CreateVector(BodyComponent.Body.Rotation, Range * 5);
			}
				
			Physics.World.RayCast((fixture, point, normal, fraction) => {
				if (min > fraction && fixture.Body.UserData is BodyComponent b && RayShouldCollide(b.Entity)) {
					min = fraction;
					closest = point;
				}
				
				return min;
			}, from, closest);

			var v = (from - closest);
			var len = v.Length();

			if (Math.Abs(len - Width) > 1) {
				Width = len;
				BodyComponent.Resize(0, -Height * 0.5f, Width, Height);
			}

			var a = v.ToAngle() - Math.PI + AdditionalAngle;

			if (PlayerRotated) {
				BodyComponent.Body.Rotation = (float) a;
			}

			End = Position + MathUtils.CreateVector(a, Width);
		}

		private float lastClear;

		public override void Update(float dt) {
			base.Update(dt);

			lastClear += dt;

			if (lastClear >= 0.1f) {
				lastClear = 0;
				EntitiesHurt.Clear();
			}

			if (LifeTime > 0) {
				LifeTime -= dt;

				if (LifeTime <= 0) {
					Break();
					Done = true;
					LifeTime = 0;
					return;
				}
			}

			if (Dynamic) {
				Recalculate();
			}
		}

		public override void Resize(float newScale) {
			Scale = newScale;
			Height = 9 * Scale;
			GetComponent<RectBodyComponent>().Resize(0, 0, Width, Height, true);
		}
	}*/
}