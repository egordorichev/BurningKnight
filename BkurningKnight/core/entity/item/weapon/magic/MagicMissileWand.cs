using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.weapon.magic {
	public class MagicMissileWand : Wand {
		protected void _Init() {
			{
				Name = Locale.Get("magic_missile_wand");
				Description = Locale.Get("magic_missile_wand_desc");
				Sprite = "item-wand_b";
				Damage = 4;
				Mana = 2;
				Projectile = Graphics.GetTexture("particle-big");
			}
		}


	}
}
