namespace BurningKnight.core {
	public static class OS {
		public static bool Linux;
		public static bool MacOS;
		public static bool Windows;
		public static bool Switch;
		public static bool Xbox;
		public static bool PS4;
		
		static OS() {
#if LINUX
			Linux = true;
#elif MONOMAC
			MacOS = true;
#elif WINDOWS
			Windows = true;
#elif NSWITCH
			Switch = true;
#elif XBOXONE
			Xbox = true;
#elif PS4
			PS4 = true;
#endif
		}
	}
}