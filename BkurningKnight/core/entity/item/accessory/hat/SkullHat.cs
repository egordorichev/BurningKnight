using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.accessory.hat {
	public class SkullHat : Hat {
		protected void _Init() {
			{
				Skin = "skull";
				Sprite = "item-hat_i";
				Name = Locale.Get("skull_hat");
				Description = Locale.Get("skull_hat_desc");
			}
		}


	}
}
