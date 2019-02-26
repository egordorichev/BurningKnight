namespace BurningKnight.entity.item.accessory.hat {
	public class DunceHat : Hat {
		protected void _Init() {
			{
				Skin = "dunce";
				Sprite = "item-hat_c";
				Name = Locale.Get("dunce_hat");
				Description = Locale.Get("dunce_hat_desc");
			}
		}
	}
}