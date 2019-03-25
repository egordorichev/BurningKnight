namespace Lens {
	public class Version {
		public int Major { get; }
		public int Minor { get; }
		public int Update { get; }
		public int Patch { get; }
		public bool Debug { get; }
		public bool Test { get; }
		
		public Version(int major, int minor, int update, int patch, bool debug, bool test) {
			Major = major;
			Minor = minor;
			Update = update;
			Patch = patch;
			Debug = debug;
			Test = test;
		}

		public override string ToString() {
			return Major + "." + Minor + "." + Update + "." + Patch + (Debug ? " debug" : "");
		}
	}
}