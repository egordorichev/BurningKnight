using Pico8Emulator.lua;
using Pico8Emulator.unit.graphics;
using System;
using System.Runtime.CompilerServices;

namespace Pico8Emulator.unit.mem {
	public class DrawState {
		private MemoryUnit _memory;
		private byte[] _ram;

		public DrawState(MemoryUnit mem) {
			_memory = mem;
			_ram = mem.ram;


			_ram[RamAddress.Palette0] = 0x10;
			_ram[RamAddress.Palette1] = 0x0;

			for (var i = 1; i < Palette.Size; i++) {
				_ram[RamAddress.Palette0 + i] = (byte)i;
				_ram[RamAddress.Palette1 + i] = (byte)i;
			}

			ClipLeft = 0;
			ClipTop = 0;
			ClipRight = 127;
			ClipBottom = 127;
		}

		public void DefineApi(LuaInterpreter script) {
			script.AddFunction("color", (Action<byte?>)Color);
			script.AddFunction("cursor", (Action<int?, int?>)Cursor);
			script.AddFunction("fillp", (Action<double?>)Fillp);
			script.AddFunction("camera", (Action<int?, int?>)Camera);
			script.AddFunction("pal", (Action<int?, int?, int>)Pal);
			script.AddFunction("palt", (Action<int?, bool>)Palt);
			script.AddFunction("clip", (Action<int?, int?, int?, int?>)Clip);

			Palt();
		}

		public int CursorX {
			get => _ram[RamAddress.CursorX];
			set => _ram[RamAddress.CursorX] = (byte)value;
		}

		public int CursorY {
			get => _ram[RamAddress.CursorY];
			set => _ram[RamAddress.CursorY] = (byte)value;
		}

		private int _cameraX;
		public int CameraX {
			get => _cameraX;
			set {
				_ram[RamAddress.CameraX] = (byte)(value & 0xff);
				_ram[RamAddress.CameraX + 1] = (byte)(value >> 8);
				_cameraX = value;
			}
		}

		private int _cameraY;
		public int CameraY {
			get => _cameraY;
			set {
				_ram[RamAddress.CameraY] = (byte)(value & 0xff);
				_ram[RamAddress.CameraY + 1] = (byte)(value >> 8);
				_cameraY = value;
			}
		}

		public int LineX {
			get => ((sbyte)(_ram[RamAddress.LineX + 1]) << 8) | _ram[RamAddress.LineX];
			set {
				_ram[RamAddress.LineX] = (byte)(value & 0xff);
				_ram[RamAddress.LineX + 1] = (byte)(value >> 8);
			}
		}

		public int LineY {
			get => ((sbyte)(_ram[RamAddress.LineY + 1]) << 8) | _ram[RamAddress.LineY];
			set {
				_ram[RamAddress.LineY] = (byte)(value & 0xff);
				_ram[RamAddress.LineY + 1] = (byte)(value >> 8);
			}
		}

		/*
		 * FIXME: completely broken
		 */
		public int ScreenX {
			get {
				// byte i = memory.Peek(Address.ScreenX);
				return 128;
			}
		}

		/*
		 * FIXME: completely broken
		 */
		public int ScreenY {
			get {
				// byte i = Peek(Address.ScreenY);
				return 128;
			}
		}

		public int FillPattern {
			get => _fillPattern;
			set {
				_ram[RamAddress.FillPattern] = (byte)(value & 0xff);
				_ram[RamAddress.FillPattern + 1] = (byte)(value >> 8 & 0xff);
				_fillPattern = value;
			}
		}

		private int _fillPattern;

		public bool FillpTransparent {
			get => _ram[RamAddress.FillPattern + 2] != 0;
			set => _ram[RamAddress.FillPattern + 2] = (byte)(value ? 1 : 0);
		}

		private byte _clipLeft;
		public byte ClipLeft {
			get => _clipLeft;
			set {
				_ram[RamAddress.ClipLeft] = value;
				_clipLeft = (byte)(value & 0x7f);
			}
		}

		private byte _clipTop;
		public byte ClipTop {
			get => _clipTop;
			set {
				_ram[RamAddress.ClipTop] = value;
				_clipTop = (byte)(value & 0x7f);
			}
		}

		private byte _clipRight;
		public byte ClipRight {
			get => _clipRight;
			set {
				_ram[RamAddress.ClipRight] = value;
				_clipRight = (byte)(value & 0x7f);
			}
		}

		private byte _clipBottom;
		public byte ClipBottom {
			get => _clipBottom;
			set {
				_ram[RamAddress.ClipBottom] = value;
				_clipBottom = (byte)(value & 0x7f);
			}
		}

		public byte DrawColor {
			get => _ram[RamAddress.DrawColor];
			set => _ram[RamAddress.DrawColor] = (byte)(value & 0xff);
		}

		public void Fillp(double? p = null) {
			if (!p.HasValue) {
				p = 0;
			}

			FillPattern = (int)p.Value;
			FillpTransparent = Math.Floor(p.Value) < p.Value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetFillPBit(int x, int y) {
			x &= 0b11;
			y &= 0b11;

			var i = (y << 2) + x;
			return (FillPattern & (1 << 15) >> i) >> (15 - i);
		}

		public void Cursor(int? x, int? y) {
			CursorX = x ?? 0;
			CursorY = y ?? 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Color(byte? col) {
			DrawColor = col ?? 6;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte GetDrawColor(int color) {
			return _ram[RamAddress.Palette0 + (color & 0x0f)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetScreenColor(int color) {
			return _ram[RamAddress.Palette1 + (color & 0x0f)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetTransparent(int col) {
			col &= 0x0f;

			_ram[RamAddress.Palette0 + col] &= 0x0f;
			_ram[RamAddress.Palette0 + col] |= 0x10;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsTransparent(int col) {
			return (_ram[RamAddress.Palette0 + col] & 0x10) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ResetTransparent(int col) {
			_ram[RamAddress.Palette0 + (col & 0x0f)] &= 0x0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDrawPalette(int c0, int c1) {
			var t = IsTransparent(c0);
			_ram[RamAddress.Palette0 + (c0 & 0x0f)] = (byte)(c1 & 0x0f);
			Palt(c0, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetScreenPalette(int c0, int c1) {
			_ram[RamAddress.Palette1 + (c0 & 0x0f)] = (byte)(c1 & 0x0f);
		}

		public void Camera(int? x = null, int? y = null) {
			CameraX = x ?? 0;
			CameraY = y ?? 0;
		}

		public void Palt(int? col = null, bool t = false) {
			if (!col.HasValue && !t) {
				SetTransparent(0);

				for (byte i = 1; i < 16; i++) {
					ResetTransparent(i);
				}

				return;
			}

			if (t) {
				SetTransparent(col.Value);
			}
			else {
				ResetTransparent(col.Value);
			}
		}

		public void Pal(int? c0 = null, int? c1 = null, int p = 0) {
			if (!c0.HasValue || !c1.HasValue) {
				for (byte i = 0; i < 16; i++) {
					SetDrawPalette(i, i);
					SetScreenPalette(i, i);
				}
				
				Palt();
				return;
			}

			if (p == 0) {
				SetDrawPalette(c0.Value, c1.Value);
			}
			else if (p == 1) {
				SetScreenPalette(c0.Value, c1.Value);
			}
		}

		public void Clip(int? x = null, int? y = null, int? w = null, int? h = null) {
			if (!x.HasValue || !y.HasValue || !w.HasValue || !h.HasValue) {
				ClipLeft = 0;
				ClipTop = 0;
				ClipRight = 127;
				ClipBottom = 127;

				return;
			}

			ClipLeft = (byte)x.Value;
			ClipTop = (byte)y.Value;
			ClipRight = (byte)(x.Value + w.Value);
			ClipBottom = (byte)(y.Value + h.Value);
		}
	}
}