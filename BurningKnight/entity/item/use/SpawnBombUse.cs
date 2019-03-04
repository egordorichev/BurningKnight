using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class SpawnBombUse : ConsumeUse {
		public override void Use(Player player, Item item) {
			base.Use(player, item);
			
			var bomb = new Entity();
			bomb.AddComponent(new ExplodeComponent(3));
						
			player.Area.Add(bomb);
		}
	}
}