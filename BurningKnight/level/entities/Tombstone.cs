using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.util.camera;
using Lens.util.file;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Tombstone : Prop {
		public string Item;
		
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 12;
			
			AddComponent(new DialogComponent());
			AddComponent(new CloseDialogComponent("tomb_0"));
			
			AddComponent(new RectBodyComponent(2, 8, (int) Width - 4, 1, BodyType.Static));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => Item != null
			});
			
			AddComponent(new InteractableSliceComponent("props", "tombstone"));
			AddComponent(new ShadowComponent());
		}

		private bool Interact(Entity entity) {
			var i = Items.CreateAndAdd(Item, entity.Area);

			if (i != null) {
				i.CenterX = CenterX;
				i.Y = Bottom;
			}
			
			Item = null;
			UpdateSprite();
			
			AnimationUtil.Poof(Center);
			Camera.Instance.Shake(16);
			
			// fixme: spawn ghosts, dialog does not appear
			
			return true;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			Item = stream.ReadString();
			UpdateSprite();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(Item);
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			var has = Item != null;

			if (ImGui.Checkbox("Has item", ref has)) {
				Item = has ? "" : null;
				UpdateSprite();
			}

			if (has) {
				ImGui.InputText("Item##itm", ref Item, 128);
			}
		}

		private void UpdateSprite() {
			GetComponent<InteractableSliceComponent>().Set("props", Item == null ? "broken_tombstone" : "tombstone");
		}
	}
}