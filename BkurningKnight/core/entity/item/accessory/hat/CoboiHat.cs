using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.accessory.hat {
	public class CoboiHat : Hat {
		protected void _Init() {
			{
				Skin = "cowboy";
				Sprite = "item-hat_j";
				Name = Locale.Get("coboi_hat");
				Description = Locale.Get("coboi_hat_desc");
			}
		}


	}
}
