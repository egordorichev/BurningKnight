using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.jungle;
using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using Lens.entity;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.projectile {
	public class Projectile : Entity, CollisionFilterEntity {
		public Projectile Parent; // Potentially not needed
		public Entity Owner;
		public Entity FirstOwner; // Potentially not needed
		public Color Color = ProjectileColor.Red;
		public ProjectileFlags Flags;

		public List<Entity> EntitiesHurt = new List<Entity>(); // can we get rid of it?

		public float Damage = 1;
		public float T = -1;

		/*
		 * systems
		 *
		 * collision (breaks, hurts)
		 * callbacks
		 * damage
		 */

		public override void Update(float dt) {
			base.Update(dt);

			if (T > 0) {
				T -= dt;

				if (T <= 0) {
					Done = true;
					return;
				}
			}

			var bodyComponent = GetAnyComponent<BodyComponent>();

			if (Owner is Player || Owner is Sniper) {
				Position += bodyComponent.Body.LinearVelocity * dt;
			}

			if (!HasFlag(ProjectileFlags.ManualRotation)) {
				if (HasFlag(ProjectileFlags.AutomaticRotation)) {
					bodyComponent.Body.Rotation += dt * 10;
				} else {
					bodyComponent.Body.Rotation = bodyComponent.Body.LinearVelocity.ToAngle();
				}
			}
		}

		public bool HasFlag(ProjectileFlags flag) {
			return (Flags & flag) == flag;
		}

		public bool ShouldCollide(Entity entity) {
			return false;
		}

		public virtual void Resize(float scale) {
			var graphics = GetComponent<ProjectileGraphicsComponent>();

			var w = graphics.Sprite.Source.Width * scale;
			var h = graphics.Sprite.Source.Height * scale;
			var center = Center;

			Width = w;
			Height = h;
			Center = center;

			if (HasComponent<CircleBodyComponent>()) {
				GetComponent<CircleBodyComponent>().Resize(0, 0, w / 2f, w / 2, true);
			} else {
				GetComponent<RectBodyComponent>().Resize(0, 0, w, h, true);
			}
		}
	}
}