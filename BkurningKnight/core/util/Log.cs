namespace BurningKnight.core.util {
	public class Log {
		public const bool ENABLE_PHYSICS_MESSAGES = false;
		private static bool Force;

		public static Void Report(Throwable T) {
			T.PrintStackTrace();
			Force = true;
			Close();
		}

		public static Void PrintStackTrace() {
			foreach (StackTraceElement S in Thread.CurrentThread().GetStackTrace()) {
				Log.Error(S.ToString());
			}
		}

		public static Void Close() {

		}

		public static Void Error(Object String) {
			try {
				System.Out.Println("\u001B[31m" + String + "\u001B[0m");
			} catch (Exception) {

			}
		}

		public static Void Info(Object String) {
			try {
				System.Out.Println("\u001B[32m" + String + "\u001B[0m");
			} catch (Exception) {

			}
		}

		public static Void Physics(Object String) {
			if (!ENABLE_PHYSICS_MESSAGES || !Version.Debug) {
				return;
			} 

			try {
				System.Out.Println("\u001B[34m" + String + "\u001B[0m");
			} catch (Exception) {

			}
		}
	}
}
