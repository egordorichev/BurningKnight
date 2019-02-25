namespace BurningKnight.entity.item.accessory.hat {
	public class ShroomHat : Hat {
		protected void _Init() {
			{
				Skin = "red_mushroom";
				Sprite = "item-hat_k";
				Name = Locale.Get("shroom_hat");
				Description = Locale.Get("shroom_hat_desc");
			}
		}
	}
}