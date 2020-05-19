using Pico8Emulator.lua;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Pico8Emulator.unit.mem {
	public class MemoryUnit : Unit {
		public const int Size = RamAddress.End;

		public readonly byte[] ram = new byte[Size];
		public DrawState drawState;

		public MemoryUnit(Emulator emulator) : base(emulator) {
			drawState = new DrawState(this);
		}

		public override void DefineApi(LuaInterpreter script) {
			base.DefineApi(script);

			script.AddFunction("memset", (Func<int, byte, int, object>)Memset);
			script.AddFunction("memcpy", (Func<int, int, int, object>)Memcpy);
			script.AddFunction("peek", (Func<int, byte>)Peek);
			script.AddFunction("poke", (Func<int, byte, object>)Poke);

			script.AddFunction("fget", (Func<int, byte?, object>)Fget);
			script.AddFunction("fset", (Action<int, byte?, bool?>)Fset);
			script.AddFunction("mget", (Func<int, int, byte>)Mget);
			script.AddFunction("mset", (Action<int, int, byte>)Mset);

			drawState.DefineApi(script);
		}

		public void LoadCartridgeData(byte[] cartridgeRom) {
			Buffer.BlockCopy(cartridgeRom, 0x0, ram, 0, 0x4300);
		}

		public object Memset(int destination, byte val, int len) {
			for (int i = 0; i < len; i++) {
				ram[destination + i] = val;
			}

			return null;
		}

		public object Memcpy(int destination, int source, int len) {
			Buffer.BlockCopy(ram, source, ram, destination, len);

			return null;
		}

		public object Memcpy(int destination, int source, int len, byte[] src) {
			Buffer.BlockCopy(src, source, ram, destination, len);

			return null;
		}

		public byte Peek(int address) {
			if (address < 0 || address >= 0x8000) {
				return 0;
			}

			return ram[address];
		}

		public object Poke(int address, byte val) {
			/*
			 * FIXME: better error handling
			 */
			Trace.Assert(address >= 0 && address < 0x8000, "bad memory access");
			ram[address] = val;

			return null;
		}

		public int Peek2(int addr) {
			if (addr < 0 || addr >= 0x8000 - 1) {
				return 0;
			}

			return ram[addr] | (ram[addr + 1] << 8);
		}

		public object Poke2(int address, int val) {
			/*
			 * FIXME: better error handling
			 */
			Trace.Assert(address >= 0 && address < 0x8000, "bad memory access");

			ram[address] = (byte)(val & 0xff);
			ram[address + 1] = (byte)((val >> 8) & 0xff);

			return null;
		}

		public double Peek4(int address) {
			if (address < 0 || address >= 0x8000 - 3) return 0;
			int right = ram[address] | (ram[address + 1] << 8);
			int left = ((ram[address + 2] << 16) | (ram[address + 3] << 24));

			return Util.FixedToFloat(left + right);
		}

		public object Poke4(int address, double val) {
			/*
			 * FIXME: better error handling
			 */
			Trace.Assert(address >= 0 && address < 0x8000, "bad memory access");

			Int32 f = Util.FloatToFixed(val);

			ram[address] = (byte)(f & 0xff);
			ram[address + 1] = (byte)((f >> 8) & 0xff);
			ram[address + 2] = (byte)((f >> 16) & 0xff);
			ram[address + 3] = (byte)((f >> 24) & 0xff);

			return null;
		}

		public object Fget(int n, byte? f = null) {
			if (f.HasValue) {
				return (Peek(RamAddress.GfxProps + n) & (1 << f)) != 0;
			}

			return Peek(RamAddress.GfxProps + n);
		}

		public void Fset(int n, byte? f = null, bool? v = null) {
			if (!f.HasValue) {
				return;
			}

			if (v.HasValue) {
				if (v.Value) {
					Poke(RamAddress.GfxProps + n, (byte)(Peek(RamAddress.GfxProps + n) | (1 << f)));
				}
				else {
					Poke(RamAddress.GfxProps + n, (byte)(Peek(RamAddress.GfxProps + n) & ~(1 << f)));
				}
			}
			else {
				Poke(RamAddress.GfxProps + n, (byte)(Peek(RamAddress.GfxProps + n) | f));
			}
		}

		public byte Mget(int x, int y) {
			int addr = (y < 32 ? RamAddress.Map : RamAddress.GfxMap);
			y = y & 0x1f;
			int index = ((y << 7) + x);

			if (index < 0 || index > 32 * 128 - 1) {
				return 0x0;
			}

			return ram[index + addr];
		}

		public void Mset(int x, int y, byte v) {
			int addr = (y < 32 ? RamAddress.Map : RamAddress.GfxMap);
			y = y & 0x1f;
			int index = ((y << 7) + x);

			if (index < 0 || index > 32 * 128 - 1) {
				return;
			}

			ram[index + addr] = v;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte GetPixel(int x, int y, int offset = RamAddress.Screen) {
			int index = (y * 128 + x) / 2;

			if (index < 0 || index > 64 * 128 - 1) {
				return 0x10;
			}

			return Util.GetHalf(ram[index + offset], (x & 1) == 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WritePixel(int x, int y, byte color, int offset) {
			int index = (((y << 7) + x) >> 1) + offset;

			if ((x & 1) == 0) {
				ram[index] = (byte)((byte)(ram[index] & 0xf0) | (color & 0x0f));
			}
			else {
				ram[index] = (byte)((byte)(ram[index] & 0x0f) | ((color & 0x0f) << 4));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void WritePixel(int x, int y, byte color) {
			if (x < drawState.ClipLeft || y < drawState.ClipTop || x > drawState.ClipRight || y > drawState.ClipBottom)
			{
				return;
			}

			int index = (((y << 7) + x) >> 1) + 0x6000;

			if ((x & 1) == 0) {
				ram[index] = (byte)((byte)(ram[index] & 0xf0) | (color & 0x0f));
			}
			else {
				ram[index] = (byte)((byte)(ram[index] & 0x0f) | ((color & 0x0f) << 4));
			}
		}
	}
}