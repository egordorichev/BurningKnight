using BurningKnight.assets.items;
using BurningKnight.state;
using Lens.entity;
using Lens.lightJson;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class RandomUse : ItemUse {
		public ItemUse[] Uses;
		
		public override void Use(Entity entity, Item item) {
			Uses[Random.Int(Uses.Length)].Use(entity, item);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Uses = Items.ParseUses(settings["uses"]);
		}

		public static void RenderDebug(JsonValue root) {
			ItemEditor.DisplayUse(root["uses"]);
		}
	}
}