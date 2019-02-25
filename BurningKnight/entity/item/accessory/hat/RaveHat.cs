namespace BurningKnight.entity.item.accessory.hat {
	public class RaveHat : Hat {
		protected void _Init() {
			{
				Skin = "ravi";
				Sprite = "item-hat_d";
				Name = Locale.Get("rave_hat");
				Description = Locale.Get("rave_hat_desc");
			}
		}
	}
}