using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.save;
using BurningKnight.state;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Stand : SolidProp {
		private int id;

		protected override Rectangle GetCollider() {
			return new Rectangle(0, 6, 13, 8);
		}

		public override void PostInit() {
			Sprite = $"stand_{id}";

			base.PostInit();
			var s = GetComponent<InteractableSliceComponent>().Sprite;

			Width = s.Width;
			Height = s.Height;
		}

		public override void AddComponents() {
			base.AddComponents();

		
			AddComponent(new InteractableComponent(Interact));
			AddComponent(new ShadowComponent());

			var r = GetCollider();
			AddComponent(new SensorBodyComponent(r.X - 2, r.Y - Npc.Padding, r.Width + 4, r.Height + Npc.Padding * 2, BodyType.Static));
		}

		private bool Interact(Entity e) {
			var s = (InGameState) Engine.Instance.State;
			var sid = $"top_{id}";

			if (GlobalSave.Exists(sid)) {
				Log.Debug(GlobalSave.GetString($"{sid}_data"));
			} else {
				Log.Debug("no data yet :(");
			}

			return false;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			id = stream.ReadInt32();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteInt32(id);
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			if (ImGui.InputInt("Id", ref id)) {
				RemoveComponent<InteractableSliceComponent>();
				InsertGraphics();
			}
		}
	}
}