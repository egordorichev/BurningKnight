using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;
using Pico8Emulator;

namespace BurningKnight.entity.item.use {
	public class EnableCurseUse : ItemUse {
		private string curse;

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			Curse.Enable(curse);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			curse = settings["curse"].String(Curse.OfUnknown);
		}

		public static void RenderDebug(JsonValue root) {
			root.InputText("Curse", "curse");
		}
	}
}