using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class SpawnBombUse : ConsumeUse {
		public int Timer;
		
		public override void Use(Entity entity, Item item) {
			var bomb = new Bomb();
			entity.Area.Add(bomb);
			bomb.Center = entity.Center;
			bomb.MoveToMouse();
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			Timer = settings["timer"].Int(3);
		}
	}
}