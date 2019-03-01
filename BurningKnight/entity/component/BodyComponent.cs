using BurningKnight.physics;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.component {
	public class BodyComponent : SaveableComponent {
		public Body Body;
		public Vector2 Acceleration = new Vector2();
		
		public Vector2 Velocity => Body.LinearVelocity;

		public Vector2 Position {
			get => Body.Position;
			set => Body.Position = value;
		}

		public float Angle {
			get => Body.Rotation;
			set => Body.Rotation = value;
		}

		public virtual bool ShouldCollide(Entity entity) {
			if (Entity is CollisionFilterEntity filter) {
				return filter.ShouldCollide(entity);
			}
			
			return true;
		}
		
		public virtual void OnCollision(Entity entity) {
			if (Entity is CollisionListenerEntity listener) {
				listener.OnCollision(entity);
			}
		}

		public virtual void OnCollisionEnd(Entity entity) {
			if (Entity is CollisionListenerEntity listener) {
				listener.OnCollisionEnd(entity);
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Body == null) {
				return;
			}

			var velocity = Body.LinearVelocity;
			velocity.X += Acceleration.X;
			velocity.Y += Acceleration.Y;
			
			Body.LinearVelocity = velocity;
			Entity.Position = Body.Position;
		}

		public override void Load(FileReader reader) {
			base.Load(reader);
			Body?.SetTransform(Entity.Position, 0);
		}
	}
}