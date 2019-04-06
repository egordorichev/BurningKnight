namespace Lens {
	public class Version {
		public readonly int Major;
		public readonly int Minor;
		public readonly int Update;
		public readonly int Patch;
		public readonly bool Debug;
		public readonly bool Dev;
		
		public Version(int major, int minor, int update, int patch, bool debug, bool test) {
			Major = major;
			Minor = minor;
			Update = update;
			Patch = patch;
			Debug = debug;
			Dev = test;
		}

		public override string ToString() {
			return $"{Major}.{Minor}.{Update}.{Patch}{(Debug ? " debug" : "")}";
		}
	}
}