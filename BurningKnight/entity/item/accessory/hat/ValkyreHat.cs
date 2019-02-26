namespace BurningKnight.entity.item.accessory.hat {
	public class ValkyreHat : Hat {
		protected void _Init() {
			{
				Skin = "wings";
				Sprite = "item-hat_h";
				Name = Locale.Get("valkyre_hat");
				Description = Locale.Get("valkyre_hat_desc");
			}
		}
	}
}