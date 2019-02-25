namespace BurningKnight.entity.item.weapon.sword {
	public class Bone : Sword {
		protected void _Init() {
			{
				TimeA = 0.05f;
				UseTime = TimeA;
				Sprite = "item-bone";
				Name = Locale.Get("bone");
				Description = Locale.Get("bone_desc");
				Region = Graphics.GetTexture(Sprite);
			}
		}
	}
}