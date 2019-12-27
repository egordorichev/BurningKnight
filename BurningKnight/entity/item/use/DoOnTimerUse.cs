using BurningKnight.entity.item.use.parent;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util.timer;

namespace BurningKnight.entity.item.use {
	public class DoOnTimerUse : DoUsesUse {
		private float time;
		
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			Timer.Add(() => {
				foreach (var u in Uses) {
					u.Item = item;
					u.Use(entity, item);
				}
			}, time);
		}

		protected override void DoAction(Entity entity, Item item, ItemUse use) {
			
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			time = settings["time"].Number(1f);
		}

		public static void RenderDebug(JsonValue root) {
			root.InputFloat("Time", "time", 1f);
			ImGui.Separator();
			DoUsesUse.RenderDebug(root);
		}
	}
}