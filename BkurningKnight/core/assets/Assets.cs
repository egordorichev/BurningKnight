namespace BurningKnight.core.assets {
	public class Assets {
		public static AssetManager Manager;
		private static bool Done;
		public static bool FinishedLoading;

		public static Void Init() {
			Manager = new AssetManager();
			Audio.TargetAssets();
			Graphics.TargetAssets();
		}

		public static bool UpdateLoading() {
			bool Val = Manager.Update();

			if (Val && !Done) {
				Done = true;
				FinishedLoading = true;
				LoadAssets();
			} 

			return Val;
		}

		public static Void FinishLoading() {
			FinishedLoading = true;
			Manager.FinishLoading();
			LoadAssets();
		}

		public static Void LoadAssets() {
			Audio.LoadAssets();
			Graphics.LoadAssets();
		}

		public static Void Destroy() {
			Graphics.Destroy();
			Audio.Destroy();

			if (Manager != null) {
				Manager.Dispose();
			} 
		}
	}
}
