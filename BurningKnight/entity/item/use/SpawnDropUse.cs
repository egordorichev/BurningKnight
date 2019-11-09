using System.Collections.Generic;
using BurningKnight.entity.creature.drop;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public class SpawnDropUse : ItemUse {
		private static string drop;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			
			if (!assets.loot.Drops.Defined.TryGetValue(drop, out var d)) {
				Log.Error($"Unknown drop {drop}");
				return;
			}

			creature.drop.Drop.Create(new List<Drop> {
				d
			}, entity);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			drop = settings["drop"].String("missing");
		}

		public static void RenderDebug(JsonValue root) {
			root.InputText("Drop", "drop");
			
			if (!assets.loot.Drops.Defined.ContainsKey(drop)) {
				ImGui.Text($"Unknown drop {drop}");
			}
		}
	}
}