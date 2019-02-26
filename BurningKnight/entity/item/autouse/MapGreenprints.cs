namespace BurningKnight.entity.item.autouse {
	public class MapGreenprints : Autouse {
		public MapGreenprints() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("map_greenprints");
				Description = Locale.Get("map_greenprints_desc");
				Sprite = "item-greenprint";
			}
		}

		public override void Use() {
			base.Use();
			Dungeon.Level.ExploreRandom();
		}
	}
}