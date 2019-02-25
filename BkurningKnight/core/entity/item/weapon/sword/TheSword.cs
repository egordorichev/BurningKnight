using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.weapon.sword {
	public class TheSword : Sword {
		protected void _Init() {
			{
				Name = Locale.Get("the_sword");
				Description = Locale.Get("the_sword_desc");
				Damage = 601;
				UseTime = 0.3f;
				Sprite = "item-claymore_a";
				Region = Graphics.GetTexture(Sprite);
			}
		}


	}
}
