using Box2DX.Common;
using Box2DX.Dynamics;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class BodyComponent : Component {
		public Body Body;
		public Vector2 Acceleration = new Vector2();
		public Vector2 Velocity {
			get {
				var velocity = Body.GetLinearVelocity();
				return new Vector2(velocity.X, velocity.Y);
			}
		}

		public Vec2 Velocity2 => Body.GetLinearVelocity();

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

			var velocity = Body.GetLinearVelocity();
			velocity.X += Acceleration.X;
			velocity.Y += Acceleration.Y;
			
			Body.SetLinearVelocity(velocity);

			Acceleration.X = 0;
			Acceleration.Y = 0;
		}
	}
}