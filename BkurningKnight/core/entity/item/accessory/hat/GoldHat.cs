using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.accessory.hat {
	public class GoldHat : Hat {
		protected void _Init() {
			{
				Skin = "gold";
				Sprite = "item-hat_g";
				Name = Locale.Get("gold_hat");
				Description = Locale.Get("gold_hat_desc");
				Defense = 2;
			}
		}


	}
}
