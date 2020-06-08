using BurningKnight.assets;
using Lens.graphics;

namespace BurningKnight.level.rooms {
	public static class RoomTypeHelper {
		public static TextureRegion[] Icons;

		static RoomTypeHelper() {
			Icons = new TextureRegion[(int) RoomType.Hidden + 1];
			
			LoadIcon(RoomType.Shop, "shop");
			LoadIcon(RoomType.Secret, "secret");
			LoadIcon(RoomType.Boss, "boss");
			LoadIcon(RoomType.Exit, "exit");
			LoadIcon(RoomType.Entrance, "entrance");
			LoadIcon(RoomType.Challenge, "challenge");
			LoadIcon(RoomType.Spiked, "spiked");
			LoadIcon(RoomType.Scourged, "scourged");
			LoadIcon(RoomType.Treasure, "treasure");
		}

		private static void LoadIcon(RoomType type, string id) {
			Icons[(int) type] = CommonAse.Ui.GetSlice(id);
		}
		
		public static bool ShouldBeDisplayOnMap(RoomType type) {
			return type == RoomType.Shop || type == RoomType.Secret || type == RoomType.Boss || type == RoomType.Exit ||
			       type == RoomType.Entrance || type == RoomType.Challenge || type == RoomType.Spiked ||
			       type == RoomType.Scourged || type == RoomType.Treasure;
		}
	}
}