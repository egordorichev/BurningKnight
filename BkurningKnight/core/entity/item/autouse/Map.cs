using BurningKnight.core.assets;

namespace BurningKnight.core.entity.item.autouse {
	public class Map : Autouse {
		protected void _Init() {
			{
				Name = Locale.Get("map");
				Description = Locale.Get("map_desc");
				Sprite = "item-map";
			}
		}

		public override Void Use() {
			base.Use();
			Dungeon.Level.ExploreAll();
		}

		public Map() {
			_Init();
		}
	}
}
