using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.save;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;
using Lens.util.file;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Entrance : SaveableEntity {
		public int To;

		private bool Interact(Entity entity) {
			Run.Depth = To;
			return true;
		}
		
		private void OnInteractionStart(Entity entity) {
			if (entity is LocalPlayer) {
				Area.Add(new InteractFx(this, Locale.Get("ascend")));
			}

			AlwaysVisible = true;
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 14;
			Height = 15;
			
			AddComponent(new InteractableComponent(Interact) {
				OnStart = OnInteractionStart
			});
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
			AddComponent(new InteractableSliceComponent("props", "entrance"));
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			To = stream.ReadInt16();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteInt16((short) To);
		}
	}
}