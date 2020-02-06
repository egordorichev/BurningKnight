using System;
using BurningKnight.entity.component;
using BurningKnight.entity.item.use.parent;
using ImGuiNET;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class DoUsesIfUse : DoUsesUse {
		private static string[] options = {
			"full_hp",
			"min_hp",
			"not_full_hp"
		};

		private int option;
		
		public override void Use(Entity entity, Item item) {
			if (IsTrue(entity)) {
				foreach (var u in Uses) {
					u.Use(entity, Item);
				}
			}
		}

		private bool IsTrue(Entity entity) {
			switch (option) {
				case 0: {
					return entity.GetComponent<HealthComponent>().IsFull();
				}
				
				case 1: {
					return Math.Abs(entity.GetComponent<HealthComponent>().Health) < 1.1f;
				}
				
				case 2: {
					return !entity.GetComponent<HealthComponent>().IsFull();
				}

				default: {
					return true;
				}
			}
		}

		protected override void DoAction(Entity entity, Item item, ItemUse use) {
			
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			option = settings["opt"].Int(0);
		}

		public static void RenderDebug(JsonValue root) {
			DoUsesUse.RenderDebug(root);
			ImGui.Separator();

			var option = root["opt"].Int(0);

			if (ImGui.Combo("If", ref option, options, options.Length)) {
				root["opt"] = option;
			}
		}
	}
}