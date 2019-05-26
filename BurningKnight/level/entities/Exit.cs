using BurningKnight.entity;
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
	public class Exit : SaveableEntity, PlaceableEntity {
		public int To;
		
		public override void Init() {
			base.Init();
			Depth = Layers.Entrance;
		}

		private bool Interact(Entity entity) {
			Run.Depth = To;
			return true;
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 16;
			Height = 14;
			
			AddComponent(new InteractableComponent(Interact) {
				OnStart = entity => {
					if (entity is LocalPlayer) {
						Engine.Instance.State.Ui.Add(new InteractFx(this, Locale.Get("descend")));
					}
				}
			});
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
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

		public override void RenderImDebug() {
			ImGui.InputInt("To", ref To);
		}
	}
}