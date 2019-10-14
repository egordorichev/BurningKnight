using BurningKnight.state;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class RevealMapUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			
			var level = Run.Level;

			for (var i = 0; i < level.Explored.Length; i++) {
				level.Explored[i] = true;
			}
		}
	}
}