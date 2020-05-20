using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.room.controllable;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Belt : Support {
		private const float Speed = 32;

		private static Vector2[] velocities = {
			new Vector2(0, -Speed), new Vector2(0, Speed),
			new Vector2(-Speed, 0), new Vector2(Speed, 0)
		};
		
		private Vector2 velocity;
		protected Direction Direction = Direction.Left;

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new SensorBodyComponent(-1, -1, 18, 18, BodyType.Static));
			
			Depth = Layers.Entrance;
		}

		public override void PostInit() {
			base.PostInit();
			velocity = velocities[(int) Direction];

			switch (Direction) {
				case Direction.Left: {
					AddComponent(new AnimationComponent("belt"));
					break;
				}
				
				case Direction.Right: {
					AddComponent(new AnimationComponent("belt"));
					GraphicsComponent.Flipped = true;

					break;
				}
				
				case Direction.Up: {
					AddComponent(new AnimationComponent("belt_up"));
					break;
				}
				
				case Direction.Down: {
					AddComponent(new AnimationComponent("belt_down"));
					break;
				}
			}
		}

		public override void Update(float dt) {
			base.Update(dt);
			GetComponent<AnimationComponent>().Animation.Frame = (uint) (Engine.Time * 32) % 4;
		}

		public override void Apply(Entity e, float dt) {
			var b = e.GetAnyComponent<BodyComponent>();

			if (b == null || !ShouldMove(e)) {
				return;
			}
			
			base.Apply(e, dt);
			b.Position += velocity * dt;
		}

		protected virtual bool ShouldMove(Entity e) {
			return !(e is Creature c && c.InAir()) && !Engine.EditingLevel;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Direction = (Direction) stream.ReadByte();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte((byte) Direction);
		}
	}
}