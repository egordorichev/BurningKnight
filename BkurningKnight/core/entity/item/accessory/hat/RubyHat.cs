using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.accessory.hat {
	public class RubyHat : Hat {
		protected void _Init() {
			{
				Skin = "ruby";
				Sprite = "item-hat_f";
				Name = Locale.Get("ruby_hat");
				Description = Locale.Get("ruby_hat_desc");
				Defense = 3;
			}
		}


	}
}
