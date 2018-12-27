namespace Lens {
	public class Version {
		public int Major { get; }
		public int Minor { get; }
		public int Update { get; }
		public int Patch { get; }
		public bool Debug { get; }
		
		public Version(int major, int minor, int update, int patch, bool debug) {
			Major = major;
			Minor = minor;
			Update = update;
			Patch = patch;
			Debug = debug;
		}

		public override string ToString() {
			return Major + "." + Minor + "." + Update + "." + Patch + (Debug ? " debug" : "");
		}
	}
}