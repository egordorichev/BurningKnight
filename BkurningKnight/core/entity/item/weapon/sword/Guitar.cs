using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.weapon.sword {
	public class Guitar : Sword {
		protected void _Init() {
			{
				string Letter = "a";
				Description = Locale.Get("guitar_desc");
				Name = Locale.Get("guitar_" + Letter);
				Damage = 4;
				Sprite = "item-guitar_" + Letter;
				UseTime = 0.5f;
				Region = Graphics.GetTexture(this.Sprite);
			}
		}


	}
}
