namespace BurningKnight.entity.item.accessory.hat {
	public class FungiHat : Hat {
		protected void _Init() {
			{
				Skin = "brown_mushroom";
				Sprite = "item-hat_l";
				Name = Locale.Get("fungi_hat");
				Description = Locale.Get("fungi_hat_desc");
			}
		}
	}
}