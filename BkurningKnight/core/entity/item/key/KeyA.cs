using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.key {
	public class KeyA : Key {
		protected void _Init() {
			{
				Description = Locale.Get("key_a_desc");
				Name = Locale.Get("key_a");
				Sprite = "item-key A";
			}
		}


	}
}
