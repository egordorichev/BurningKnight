namespace BurningKnight.entity.item.weapon.sword {
	public class Guitar : Sword {
		protected void _Init() {
			{
				var Letter = "a";
				Description = Locale.Get("guitar_desc");
				Name = Locale.Get("guitar_" + Letter);
				Damage = 4;
				Sprite = "item-guitar_" + Letter;
				UseTime = 0.5f;
				Region = Graphics.GetTexture(Sprite);
			}
		}
	}
}