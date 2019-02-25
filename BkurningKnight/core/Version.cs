using BurningKnight.core.entity.level.save;

namespace BurningKnight.core {
	public static class Version {
		public const bool Debug = true;
		public const bool ShowAlphaWarning = false;
		public const double Major = 0.1;
		public const double Minor = 5.0;
		public static string String = AsString();

		public static string AsString() {
			return "v" + Major + "." + Minor + (Debug ? " dev" : "");
		}

		static Version() {
			GameSave.PlayedAlpha = true;
		}
	}
}
