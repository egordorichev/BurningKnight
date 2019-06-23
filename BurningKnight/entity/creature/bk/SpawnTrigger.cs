using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.save;
using Lens.entity;
using Lens.util.camera;
using Lens.util.file;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.bk {
	public class SpawnTrigger : SaveableEntity {
		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
		}

		private bool triggered;
		private float t;

		public override void Update(float dt) {
			base.Update(dt);

			if (triggered) {
				Camera.Instance.Shake(32);
				t += dt;

				if (t >= 2f) {
					Done = true;
					
					var bk = new BurningKnight();
					Area.Add(bk);
					bk.Center = Center;
				}
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte((byte) Width);
			stream.WriteByte((byte) Height);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			Width = stream.ReadByte();
			Height = stream.ReadByte();
		}

		public override bool HandleEvent(Event e) {
			if (triggered) {
				return base.HandleEvent(e);
			}
			
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Player) {
					triggered = true;
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}