using BurningKnight.entity.component;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyStatUse : ItemUse {
		private Stat stat;
		private float value;
		
		public override void Use(Entity entity, Item item) {
			switch (stat) {
				case Stat.InvincibilityTime: {
					entity.GetComponent<HealthComponent>().InvincibilityTimerMax += value;
					break;
				}
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			stat = (Stat) settings["stat"].Int(0);
			value = settings["val"].Number(1);
		}
		
		public static void RenderDebug(JsonValue root) {
			var stat = root["stat"].Int(0);

			if (ImGui.Combo("Stat", ref stat, stats, stats.Length)) {
				root["stat"] = stat;
			}
			
			var value = root["val"].Number(1);

			if (ImGui.InputFloat("Amount", ref value)) {
				root["val"] = value;
			}
		}

		private enum Stat {
			InvincibilityTime,
			
			Total
		}

		private static string[] stats;

		static ModifyStatUse() {
			stats = new string[(int) Stat.Total];

			for (var i = 0; i < (int) Stat.Total; i++) {
				stats[i] = $"{(Stat) i}";
			}
		}
	}
}