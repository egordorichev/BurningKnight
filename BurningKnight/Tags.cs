using Lens.entity;

namespace BurningKnight {
	public class Tags {
		public static int Player = new BitTag("player");
		public static int Mob = new BitTag("mob");
		public static int PlayerSave = new BitTag("player_save");
		public static int LevelSave = new BitTag("level_save");
		
		public static int Room = new BitTag("room");
		public static int MustBeKilled = new BitTag("mbk");
		
		public static int HasShadow = new BitTag("shadow");
	}
}