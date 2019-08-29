using BurningKnight.entity.component;
using BurningKnight.entity.orbital;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util;

namespace BurningKnight.entity.item.use {
	public class SpawnOrbitalUse : ItemUse {
		private string orbital;

		public override void Use(Entity entity, Item item) {
			var o = OrbitalRegistry.Create(orbital, entity);

			if (o == null) {
				Log.Error($"Failed to create orbital with id {orbital}");
				return;
			}
			
			entity.GetComponent<OrbitGiverComponent>().AddOrbiter(o);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			orbital = settings["orbital"].String("");
		}

		public static void RenderDebug(JsonValue root) {
			var id = root["orbital"].String("");

			if (ImGui.InputText("Orbital", ref id, 128)) {
				root["orbital"] = id;
			}

			if (!OrbitalRegistry.Has(id)) {
				ImGui.BulletText("Unknown orbital!");
			}
		}
	}
}