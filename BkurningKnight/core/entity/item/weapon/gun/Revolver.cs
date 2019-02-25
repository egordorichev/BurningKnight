using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.weapon.gun {
	public class Revolver : Gun {
		protected void _Init() {
			{
				Accuracy = -3;
				string Letter = "a";
				Sprite = "item-gun_" + Letter;
				Name = Locale.Get("gun_" + Letter);
				Description = Locale.Get("gun_desc");
				Region = Graphics.GetTexture(Sprite);
				UseTime = 0.5f;
			}
		}


	}
}
