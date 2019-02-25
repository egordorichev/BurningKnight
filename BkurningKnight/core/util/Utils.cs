namespace BurningKnight.core.util {
	public class Utils {
		public static string PascalCaseToSnakeCase(string String) {
			return String.ReplaceAll("([a-z])([A-Z]+)", "$1_$2").ToLowerCase();
		}
	}
}
