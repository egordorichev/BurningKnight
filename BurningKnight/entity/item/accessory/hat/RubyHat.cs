namespace BurningKnight.entity.item.accessory.hat {
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