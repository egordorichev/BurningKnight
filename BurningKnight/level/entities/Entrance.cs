using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.editor;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util.file;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Entrance : SaveableEntity, PlaceableEntity {
		public int To;
		
		private bool Interact(Entity entity) {
			Run.Depth = 0;
			return true;
		}

		private bool CanInteract(Entity e) {
			return Run.Depth == -1;
		}

		public override void AddComponents() {
			base.AddComponents();

			To = 0; // Run.Depth - 1;
			
			Width = 14;
			Height = 15;
			
			AddTag(Tags.Entrance);
			
			AddComponent(new InteractableComponent(Interact) {
				OnStart = entity => {
					if (entity is LocalPlayer) {
						Engine.Instance.State.Ui.Add(new InteractFx(this, Locale.Get("ascend")));
					}
				},
				
				CanInteract = CanInteract
			});
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
			AddComponent(new InteractableSliceComponent("props", "entrance"));
			AddComponent(new ShadowComponent(RenderShadow));
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			To = stream.ReadInt16();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteInt16((short) To);
		}

		public override void RenderImDebug() {
			ImGui.InputInt("To", ref To);
		}
	}
}