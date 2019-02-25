using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.autouse {
	public class MapGreenprints : Autouse {
		protected void _Init() {
			{
				Name = Locale.Get("map_greenprints");
				Description = Locale.Get("map_greenprints_desc");
				Sprite = "item-greenprint";
			}
		}

		public override Void Use() {
			base.Use();
			Dungeon.Level.ExploreRandom();
		}

		public MapGreenprints() {
			_Init();
		}
	}
}
