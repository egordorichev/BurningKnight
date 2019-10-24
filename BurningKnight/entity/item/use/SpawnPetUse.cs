using BurningKnight.entity.component;
using BurningKnight.entity.creature.pet;
using BurningKnight.entity.orbital;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class SpawnPetUse : ItemUse {
		private string pet;

		public override void Use(Entity entity, Item item) {
			var o = PetRegistry.Create(pet, entity);

			if (o == null) {
				Log.Error($"Failed to create pet with id {pet}");
				return;
			}
			
			o.Center = entity.Center + Random.Offset(24);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			pet = settings["pet"].String("");
		}

		public static void RenderDebug(JsonValue root) {
			var id = root["pet"].String("");

			if (ImGui.InputText("Pet", ref id, 128)) {
				root["pet"] = id;
			}

			if (!PetRegistry.Has(id)) {
				ImGui.BulletText("Unknown pet!");
			}
		}
	}
}