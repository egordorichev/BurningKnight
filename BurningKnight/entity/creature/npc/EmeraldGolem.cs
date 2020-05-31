using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.item.use;
using BurningKnight.save;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.util.file;
using Steamworks;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.npc {
	public class EmeraldGolem : Npc {
		private const int Amount = 20;
		private bool broken;
		
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 31;
			Height = 33;
			
			AddComponent(new AnimationComponent("emerald_golem"));
			AddComponent(new CloseDialogComponent("eg_0"));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding, BodyType.Static));
			AddComponent(new RectBodyComponent(2, 17, 27, 16, BodyType.Static));
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => !broken
			});
			
			GetComponent<DialogComponent>().Dialog.Voice = 15;
		}

		private bool Interact(Entity e) {
			var inv = e.GetComponent<InventoryComponent>();
			
			for (var i = 0; i < 20; i++) {
				inv.Pickup(Items.CreateAndAdd("bk:emerald", e.Area));
			}
			
			return true;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			broken = stream.ReadBoolean();
			// todo: update the sprite
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(broken);
		}
	}
}