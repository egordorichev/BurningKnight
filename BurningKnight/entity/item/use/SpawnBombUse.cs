using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class SpawnBombUse : ConsumeUse {
		public int Timer;
		
		public override void Use(Entity entity, Item item) {
			var bomb = new Entity();
			
			bomb.Center = entity.Center;
			bomb.AddComponent(new ExplodeComponent {
				Timer = Timer
			});
						
			entity.Area.Add(bomb);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Timer = settings["timer"].Int(3);
		}
	}
}