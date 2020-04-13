using BurningKnight.entity.creature.player;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyProjectileTextureUse : ItemUse {
		private string texture;
		
		public override void Use(Entity entity, Item item) {
			if (entity is Player p) {
				p.ProjectileTexture = texture;
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			texture = settings["texture"].String("rect");
		}

		public static void RenderDebug(JsonValue root) {
			root.InputText("Texture", "texture", "rect");
		}
	}
}