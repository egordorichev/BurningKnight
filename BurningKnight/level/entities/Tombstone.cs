using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Tombstone : Prop {
		public string Item;
		public bool DisableDialog;
		
		public override void AddComponents() {
			base.AddComponents();
			
			Width = 12;
			
			AddComponent(new DialogComponent());

			if (!DisableDialog) {
				AddComponent(new CloseDialogComponent("tomb_0") {
					CanTalk = e => Item != null
				});
			}

			AddComponent(new RectBodyComponent(2, 8, (int) Width - 4, 1, BodyType.Static));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));

			if (!DisableDialog) {
				AddComponent(new InteractableComponent(Interact) {
					CanInteract = e => Item != null
				});
			}

			AddComponent(new InteractableSliceComponent("props", "tombstone"));

			AddComponent(new ShadowComponent());
			AddComponent(new LightComponent(this, 64, new Color(0.7f, 0.6f, 0.3f, 1f)));
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
			
			// fixme: spawn ghosts, dialog should not appear when player is ded
			
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