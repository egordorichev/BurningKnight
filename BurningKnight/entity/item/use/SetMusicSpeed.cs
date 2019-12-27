using BurningKnight.util;
using Lens.assets;
using Lens.entity;
using Lens.lightJson;
using Lens.util.tween;

namespace BurningKnight.entity.item.use {
	public class SetMusicSpeed : ItemUse {
		private float speed;

		public override void Use(Entity entity, Item item) {
			Tween.To(speed, Audio.Speed, x => Audio.Speed = x, 0.4f);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			speed = settings["speed"].Number(1f);
		}

		public static void RenderDebug(JsonValue root) {
			root.InputFloat("Speed", "speed", 1f);
		}
	}
}