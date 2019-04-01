using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.save;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class Exit : SaveableEntity {
		public int To;
		
		public override void Init() {
			base.Init();
			Depth = Layers.Entrance;
		}

		private bool Interact(Entity entity) {
			Run.Depth = To;
			return true;
		}
		
		private void OnInteractionStart(Entity entity) {
			if (entity is LocalPlayer) {
				Area.Add(new InteractFx(this, Locale.Get("descend")));
			}			
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new InteractableComponent(Interact) {
				OnStart = OnInteractionStart
			});
			
			AddComponent(new InteractableSliceComponent("props", "exit"));
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