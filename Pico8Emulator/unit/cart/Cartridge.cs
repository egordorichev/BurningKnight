using Pico8Emulator.lua;

namespace Pico8Emulator.unit.cart {
	public class Cartridge {
		public const string CartDataPath = "cartdata/";
		public const int CartDataSize = 64;
		public const int RomSize = 0x8005;

		public byte[] rom = new byte[RomSize];
		public string code;
		public string path;

		public int[] cartData = new int[CartDataSize];
		public string cartDataId = "";

		public LuaInterpreter interpreter;
	}
}