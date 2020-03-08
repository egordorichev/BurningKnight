namespace BurningKnight.level.variant {
	public class LevelVariant {
		public static string Regular = "regular";
		public static string Sand = "sand";
		public static string Flooded = "flooded";
		public static string Webbed = "webbed";
		public static string Snow = "snow";
		public static string Chasm = "chasm";
		public static string Gold = "gold";
		public static string Forest = "forest";
		
		private string id;

		public string Id => id;

		public LevelVariant(string id) {
			this.id = id;
		}

		public virtual void ModifyPainter(Painter painter) {
			
		}

		public virtual void PostInit(Level level) {
			
		}
	}
}