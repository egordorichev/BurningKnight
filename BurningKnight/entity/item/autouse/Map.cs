namespace BurningKnight.entity.item.autouse {
	public class Map : Autouse {
		public Map() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("map");
				Description = Locale.Get("map_desc");
				Sprite = "item-map";
			}
		}

		public override void Use() {
			base.Use();
			Dungeon.Level.ExploreAll();
		}
	}
}