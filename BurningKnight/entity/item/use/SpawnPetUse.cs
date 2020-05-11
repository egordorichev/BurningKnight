using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.pet;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class SpawnPetUse : ItemUse {
		private string pet;
		private bool random;
		private bool onlyIfHasNone;

		public override void Use(Entity entity, Item item) {
			if (onlyIfHasNone) {
				var inventory = entity.GetComponent<InventoryComponent>();

				foreach (var i in inventory.Items) {
					if (ItemPool.Pet.Contains(i.Data.Pools)) {
						return;
					}
				}
			}
			
			if (random) {
				entity.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd(Items.Generate(ItemPool.Pet), entity.Area));
				return;
			}
			
			var o = PetRegistry.Create(pet, entity);

			if (o == null) {
				Log.Error($"Failed to create pet with id {pet}");
				return;
			}
			
			o.Center = entity.Center + Rnd.Offset(24);
			AnimationUtil.Poof(o.Center, entity.Depth + 1);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			pet = settings["pet"].String("");
			random = settings["random"].Bool(false);
			onlyIfHasNone = settings["oin"].Bool(false);
		}

		public static void RenderDebug(JsonValue root) {
			root.Checkbox("Only if has none", "oin", false);
			
			if (root.Checkbox("Random", "random", false)) {
				return;
			}
			
			if (!PetRegistry.Has(root.InputText("Pet", "pet", "", 128))) {
				ImGui.BulletText("Unknown pet!");
			}
		}
	}
}