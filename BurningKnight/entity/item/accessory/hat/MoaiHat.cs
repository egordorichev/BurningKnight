namespace BurningKnight.entity.item.accessory.hat {
	public class MoaiHat : Hat {
		protected void _Init() {
			{
				Skin = "stone";
				Sprite = "item-hat_a";
				Name = Locale.Get("moai_hat");
				Description = Locale.Get("moai_hat_desc");
			}
		}
	}
}