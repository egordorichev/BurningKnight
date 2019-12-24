using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using Lens.entity;
using Lens.util.file;

namespace BurningKnight.level.entities.statue {
	public class Statue : SolidProp {
		protected bool Broken;
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => !Broken
			});
			
			AddComponent(new ShadowComponent());
		}

		public override void PostInit() {
			base.PostInit();

			if (Broken) {
				UpdateSprite();
			}
			
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2));
		}

		protected virtual bool Interact(Entity e) {
			return false;
		}

		protected void Break() {
			if (Broken) {
				return;
			}

			Broken = true;
			UpdateSprite();
		}

		protected virtual void UpdateSprite() {
			GetComponent<InteractableSliceComponent>().Sprite = CommonAse.Props.GetSlice($"broken_{Sprite}");
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Broken = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(Broken);
		}
	}
}