using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.state;
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
			
			AddComponent(new RoomComponent());
			AddComponent(new DialogComponent());

			if (!DisableDialog) {
				AddComponent(new CloseDialogComponent("tomb_0") {
					CanTalk = e => Item != null
				});
			}

			AddComponent(new RectBodyComponent(0, 2, 12, 14, BodyType.Static));
			AddComponent(new SensorBodyComponent(-Npc.Padding, -Npc.Padding, Width + Npc.Padding * 2, Height + Npc.Padding * 2, BodyType.Static));

			if (!DisableDialog) {
				AddComponent(new InteractableComponent(Interact) {
					CanInteract = e => Item != null
				});
			}

			AddComponent(new InteractableSliceComponent("props", "tombstone"));

			AddComponent(new ShadowComponent());
			AddComponent(new HealthComponent {
				RenderInvt = true,
				InitMaxHealth = 3
			});
			
			AddComponent(new LightComponent(this, 64, new Color(0.7f, 0.6f, 0.3f, 1f)));
			
			Subscribe<RoomChangedEvent>();
			GetComponent<DialogComponent>().Dialog.Voice = 10;
		}

		public override bool HandleEvent(Event e) {
			if (e is DiedEvent d) {
				Interact(d.From);
				return true;
			} else if (e is RoomChangedEvent rce) {
				if (rce.Who is Player && rce.New == GetComponent<RoomComponent>().Room) {
					// Daddy? What did they do with you?!?!
					rce.Who.GetComponent<DialogComponent>().StartAndClose("player_0", 3f);
				}
			}
			
			return base.HandleEvent(e);
		}

		private bool Interact(Entity entity) {
			var i = Items.CreateAndAdd(Item, entity.Area);

			if (i != null) {
				i.CenterX = CenterX;
				i.Y = Bottom;
			}
			
			Item = null;
			UpdateSprite();
			Run.AddCurse();

			GetComponent<DialogComponent>().Close();
			
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