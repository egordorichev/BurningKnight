using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class SpawnBombUse : ConsumeUse {
		public override void Use(Entity entity, Item item) {
			var bomb = new Entity();
			
			bomb.Center = entity.Center;
			bomb.AddComponent(new ExplodeComponent(3));
						
			entity.Area.Add(bomb);
		}
	}
}