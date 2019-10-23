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

		protected virtual bool Interact(Entity entity) {
			((InGameState) Engine.Instance.State).TransitionToBlack(entity.Center, () => {
				if (To == 1 || Run.Depth == -2) { // To 1 or in tutorial
					Run.StartNew();
				} else {
					Run.Depth = To;
				}
			});

			return true;
		}

		protected virtual string GetFxText() {
			return Locale.Get(Run.Depth == 0 ? "new_run" : "descend");
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 16;
			Height = 14;
			
			To = Run.Depth + 1;
			
			AddComponent(new InteractableComponent(Interact) {
				OnStart = entity => {
					if (entity is LocalPlayer && Run.Depth != -2) {
						Engine.Instance.State.Ui.Add(new InteractFx(this, GetFxText()));
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