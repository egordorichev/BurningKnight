using BurningKnight.entity.item;
using BurningKnight.entity.item.use;
using Lens.entity;
using Lens.util;

namespace TestMod {
	public class TestUse : ItemUse {
		public void Use(Entity entity, Item item) {
			Log.Debug("USSSSE");
		}
	}
}