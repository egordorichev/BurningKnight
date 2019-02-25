namespace BurningKnight.entity.item.accessory.hat {
	public class VikingHat : Hat {
		protected void _Init() {
			{
				Defense = 1;
				Skin = "viking";
				Sprite = "item-hat_b";
				Name = Locale.Get("viking_hat");
				Description = Locale.Get("viking_hat_desc");
			}
		}
	}
}