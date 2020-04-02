using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.util;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities {
	public class Stand : SolidProp {
		private static int[] heights = {22, 17, 14};
		
		private int id;

		protected override Rectangle GetCollider() {
			return new Rectangle(0, (int) heights[id] - 8, 13, 8);
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
			AddComponent(new DialogComponent());
		}

		private bool Interact(Entity e) {
			if (!GlobalSave.Exists($"top_{id}")) {
				GetComponent<DialogComponent>().StartAndClose("no_score_yet", 3);
				return true;
			}
			
			var s = (InGameState) Engine.Instance.State;

			s.InStats = true;
			s.Paused = true;
			
			s.ReturnFromStats = () => {
				s.Paused = false;
				s.InStats = false;
			};

			s.ShowStats(id);
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