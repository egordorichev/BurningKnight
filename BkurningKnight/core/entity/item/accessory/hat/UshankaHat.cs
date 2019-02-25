using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.accessory.hat {
	public class UshankaHat : Hat {
		protected void _Init() {
			{
				Skin = "ushanka";
				Sprite = "item-hat_e";
				Name = Locale.Get("ushanka_hat");
				Description = Locale.Get("ushanka_hat_desc");
			}
		}


	}
}
