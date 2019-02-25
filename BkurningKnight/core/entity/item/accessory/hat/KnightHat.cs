using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.accessory.hat {
	public class KnightHat : Hat {
		protected void _Init() {
			{
				Skin = "knight";
				Sprite = "item-hat_m";
				Name = Locale.Get("knight_hat");
				Description = Locale.Get("knight_hat_desc");
				Defense = 2;
			}
		}


	}
}
