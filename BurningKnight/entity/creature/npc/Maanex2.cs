using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.state;
using Lens.entity;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.entity.creature.npc {
	public class Maanex2 : Npc {
		private bool interacted;
		
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 12;
			Height = 15;

			AddComponent(new AnimationComponent("maanex"));
			GetComponent<DropsComponent>().Add(new SingleDrop("bk:maanex_head"));

			if (!interacted) {
				AddComponent(new InteractableComponent(Interact) {
					CanInteract = e => !interacted
				});
				
				AddComponent(new SensorBodyComponent(-Padding, -Padding, Width + Padding * 2, Height + Padding * 2));
			}
		}

		private bool Interact(Entity e) {
			return true;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			interacted = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(interacted);
		}
	}
}