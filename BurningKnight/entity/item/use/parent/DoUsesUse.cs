using BurningKnight.assets.items;
using BurningKnight.state;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use.parent {
	public abstract class DoUsesUse : ItemUse {
		protected ItemUse[] Uses;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			foreach (var u in Uses) {
				DoAction(entity, item, u);
			}
		}

		protected abstract void DoAction(Entity entity, Item item, ItemUse use);

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			
			if (!settings["uses"].IsJsonArray) {
				settings["uses"] = new JsonArray();
			}
			
			Uses = Items.ParseUses(settings["uses"]);
		}
		
		public static void RenderDebug(JsonValue root) {
			if (!root["uses"].IsJsonArray) {
				root["uses"] = new JsonArray();
			}
			
			ItemEditor.DisplayUse(root, root["uses"]);
		}
	}
}