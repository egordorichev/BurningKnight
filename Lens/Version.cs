namespace Lens {
	public class Version {
		public readonly uint Major;
		public readonly uint Minor;
		public readonly uint Update;
		public readonly uint Patch;
		public readonly bool Test;
		public readonly bool Dev;
		public readonly uint Id;
		public readonly string Name;
		
		public Version(string name, uint id, uint major, uint minor, uint update, uint patch, bool test, bool dev) {
			Name = name;
			Id = id;
			Major = major;
			Minor = minor;
			Update = update;
			Patch = patch;
			Test = test;
			Dev = dev;
		}

		public override string ToString() {
			return $"{Major}.{Minor}.{Update}{(Patch == 0 ? "" : $".{Patch}")}{(Test ? " alpha" : "")}{(Dev ? " dev" : "")} {Name}";
		}
	}
}