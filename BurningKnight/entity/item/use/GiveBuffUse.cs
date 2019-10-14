using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public class GiveBuffUse : ItemUse {
		protected string Buff;
		protected float Time;

		public override void Use(Entity entity, Item item) {
			var b = BuffRegistry.Create(Buff);

			if (b == null) {
				Log.Error($"Unknown buff {Buff}");
				return;
			}

			if (Time < 0) {
				b.Infinite = true;
			} else {
				b.TimeLeft = b.Duration = Time;
			}
			
			entity.GetComponent<BuffsComponent>().Add(b);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			Time = settings["time"].Number(1);
			Buff = settings["buff"].AsString ?? "";
		}
		
		public static void RenderDebug(JsonValue root) {
			var time = root["time"].Number(1);
			var buff = root["buff"].AsString ?? "";

			if (ImGui.InputText("Buff", ref buff, 128)) {
				root["buff"] = buff;
			}

			if (!BuffRegistry.All.ContainsKey(buff)) {
				ImGui.BulletText("Unknown buff!");
			}

			var infinite = time < 0;

			if (ImGui.Checkbox("Infinite?", ref infinite)) {
				time = infinite ? -1 : 1;
			}

			if (!infinite) {
				if (ImGui.InputFloat("Duration", ref time)) {
					root["time"] = time;
				}
			}
		}
	}
}