using BurningKnight.entity.events;
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
		
		public Vector2 Velocity {
			get => Body.LinearVelocity;
			set => Body.LinearVelocity = value;
		}

		public Vector2 Position {
			get => Body.Position;
			set => Body.Position = value;
		}

		public float Angle {
			get => Body.Rotation;
			set => Body.Rotation = value;
		}

		public override void Init() {
			base.Init();

			Entity.PositionChanged += () => {
				if (Body != null) {
					Body.Position = Entity.Position;
				}
			};
		}

		public virtual bool ShouldCollide(Entity entity) {
			if (Entity is CollisionFilterEntity filter) {
				return filter.ShouldCollide(entity);
			}
			
			return true;
		}
		
		public virtual void OnCollision(Entity entity) {
			Entity.HandleEvent(new CollisionStartedEvent {
				Entity = entity
			});
		}

		public virtual void OnCollisionEnd(Entity entity) {
			Entity.HandleEvent(new CollisionEndedEvent {
				Entity = entity
			});
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Body == null) {
				return;
			}

			var velocity = Body.LinearVelocity;
			velocity.X += Acceleration.X;
			velocity.Y += Acceleration.Y;

			if (Entity.GraphicsComponent != null && !Entity.GraphicsComponent.CustomFlip && velocity.Length() > 0.1f) {
				Entity.GraphicsComponent.Flipped = velocity.X < 0;
			}
			
			Body.LinearVelocity = velocity;
			Entity.Position = Body.Position;
		}

		public override void Load(FileReader reader) {
			base.Load(reader);
			Body?.SetTransform(Entity.Position, 0);
		}
	}
}