namespace Pico8Emulator.unit.mem {
	public static class RamAddress {
		public const int Gfx = 0x0;
		public const int GfxMap = 0x1000;
		public const int Map = 0x2000;
		public const int GfxProps = 0x3000;
		public const int Song = 0x3100;
		public const int Sfx = 0x3200;
		public const int User = 0x4300;
		public const int Cart = 0x5e00;
		public const int Palette0 = 0x5f00;
		public const int Palette1 = 0x5f10;
		public const int ClipLeft = 0x5f20;
		public const int ClipTop = 0x5f21;
		public const int ClipRight = 0x5f22;
		public const int ClipBottom = 0x5f23;
		public const int DrawColor = 0x5f25;
		public const int CursorX = 0x5f26;
		public const int CursorY = 0x5f27;
		public const int CameraX = 0x5f28;
		public const int CameraY = 0x5f2a;
		public const int FillPattern = 0x5f31;
		public const int LineX = 0x5f3c;
		public const int LineY = 0x5f3e;
		public const int ScreenX = 0x5F2C;
		public const int ScreenY = 0x5F2C;
		public const int Screen = 0x6000;
		public const int End = 0x8000;
	}
}