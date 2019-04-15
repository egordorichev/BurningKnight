using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.save;
using Lens.entity;
using Lens.util.file;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.paintings {
	public class Painting : SaveableEntity {
		public string Id;

		public override void PostInit() {
			base.PostInit();
			
			AddComponent(new InteractableComponent(Interact));
			AddComponent(new InteractableSliceComponent("paintings", $"{Id}_small"));
			var region = GetComponent<InteractableSliceComponent>().Sprite;

			Width = region.Width;
			Height = region.Height;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));

			Depth = Layers.Door;
		}

		protected virtual bool Interact(Entity entity) {
			// TODO: open up the painting

			return true;
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(Id);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Id = stream.ReadString();
		}
	}
}