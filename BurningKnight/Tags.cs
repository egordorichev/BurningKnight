using Lens.entity;

namespace BurningKnight {
	public class Tags {
		public static int Player = new BitTag("player");
		public static int LocalPlayer = new BitTag("local_player");
		public static int Mob = new BitTag("mob");
		public static int BurningKnight = new BitTag("burning_knight");
		public static int PlayerSave = new BitTag("player_save");
		public static int LevelSave = new BitTag("level_save");
		
		public static int Room = new BitTag("room");
		public static int MustBeKilled = new BitTag("must_be_killed");
		public static int Gramophone = new BitTag("gramophone");
		
		public static int HasShadow = new BitTag("shadow");
		public static int Mess = new BitTag("mess");
		
		public static int Checkpoint = new BitTag("checkpoint");
		public static int Entrance = new BitTag("entrance");
		public static int Item = new BitTag("item");
		public static int ShopKeeper = new BitTag("shop_keeper");

		public static int Torch = new BitTag("torch");
		public static int Button = new BitTag("button");
		
		public static string[] AllTags;
		
		static Tags() {
			AllTags = new string[BitTag.Total];
			var i = 0;
			
			foreach (var t in BitTag.Tags) {
				AllTags[i] = t.Name;
				
				i++;
				
				if (i == BitTag.Total) {
					break;
				}
			}
		}
	}
}