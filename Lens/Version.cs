namespace Lens {
	public class Version {
		public readonly uint Major;
		public readonly uint Minor;
		public readonly uint Update;
		public readonly uint Patch;
		public readonly bool Debug;
		public readonly bool Dev;
		public readonly uint Id;
		
		public Version(uint id, uint major, uint minor, uint update, uint patch, bool debug, bool test) {
			Id = id;
			Major = major;
			Minor = minor;
			Update = update;
			Patch = patch;
			Debug = debug;
			Dev = test;
		}

		public override string ToString() {
			return $"{Major}.{Minor}.{Update}{(Patch == 0 ? "" : $".{Patch}")}{(Debug ? " alpha" : "")}{(Dev ? " dev" : "")}";
		}
	}
}