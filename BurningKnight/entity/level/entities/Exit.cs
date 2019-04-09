using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.save;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.level.entities {
	public class Exit : SaveableEntity {
		public int To;
		
		public override void Init() {
			base.Init();
			Depth = Layers.Entrance;
		}

		public override void PostInit() {
			base.PostInit();
			Log.Error("Got depth " + To);
		}

		private bool Interact(Entity entity) {
			Run.Depth = To;
			return true;
		}
		
		private void OnInteractionStart(Entity entity) {
			if (entity is LocalPlayer) {
				Area.Add(new InteractFx(this, Locale.Get("descend")));
			}

			AlwaysVisible = true;
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 16;
			Height = 14;
			
			AddComponent(new InteractableComponent(Interact) {
				OnStart = OnInteractionStart
			});
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
			AddComponent(new InteractableSliceComponent("props", "exit"));
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			To = stream.ReadInt16();
			Log.Error("Loaded depth " + To);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			Log.Error("Save depth " + To);
			stream.WriteInt16((short) To);
		}
	}
}