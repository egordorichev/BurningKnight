using Pico8Emulator.lua;
using Pico8Emulator.unit.mem;
using System;
using System.Runtime.CompilerServices;

namespace Pico8Emulator.unit.graphics {
	public class GraphicsUnit : Unit {
		public const int ScreenSize = 128 * 128;

		public int drawCalls;

		public GraphicsUnit(Emulator emulator) : base(emulator) {
			emulator.GraphicsBackend.CreateSurface();
		}

		public override void Init() {
			base.Init();

			Emulator.Memory.drawState.DrawColor = 6;

			Emulator.Memory.ram[RamAddress.Palette0] = 0x10;
			Emulator.Memory.ram[RamAddress.Palette1] = 0x0;

			for (int i = 1; i < 16; ++i) {
				Emulator.Memory.ram[RamAddress.Palette0 + i] = (byte)i;
				Emulator.Memory.ram[RamAddress.Palette1 + i] = (byte)i;
			}

			Emulator.Memory.ram[RamAddress.ClipLeft] = 0;
			Emulator.Memory.ram[RamAddress.ClipTop] = 0;
			Emulator.Memory.ram[RamAddress.ClipRight] = 127;
			Emulator.Memory.ram[RamAddress.ClipBottom] = 127;
		}

		public override void DefineApi(LuaInterpreter script) {
			base.DefineApi(script);

			script.AddFunction("pset", (Action<int, int, byte?>)Pset);
			script.AddFunction("pget", (Func<int, int, byte>)Pget);
			script.AddFunction("flip", (Action)Flip);
			script.AddFunction("cls", (Action<byte?>)Cls);

			script.AddFunction("spr", (Action<int, int?, int?, int?, int?, bool, bool>)Spr);
			script.AddFunction("sspr", (Action<int, int, int, int, int, int, int?, int?, bool, bool>)Sspr);

			script.AddFunction("print", (Action<string, int?, int?, byte?>)Print);
			script.AddFunction("map", (Action<int?, int?, int?, int?, int?, int?, byte?>)Map);
			// Old name of map
			script.AddFunction("mapdraw", (Action<int?, int?, int?, int?, int?, int?, byte?>)Map);

			script.AddFunction("line", (Action<int, int, int?, int?, byte?>)Line);
			script.AddFunction("rect", (Action<int, int, int, int, byte?>)Rect);
			script.AddFunction("rectfill", (Action<int, int, int, int, byte?>)Rectfill);
			script.AddFunction("circ", (Action<int, int, double?, byte?>)Circ);
			script.AddFunction("circfill", (Action<int, int, double?, byte?>)Circfill);

			script.AddFunction("sset", (Action<int, int, byte?>)Sset);
			script.AddFunction("sget", (Func<int, int, byte>)Sget);
		}

		public void Cls(byte? color) {
			var c = 0;

			if (color.HasValue) {
				var v = color.Value & 0x0f;
				c = v | (v << 4);
			}

			for (var i = 0; i < 0x2000; i++) {
				Emulator.Memory.ram[RamAddress.Screen + i] = (byte)c;
			}
		}
		
		private static string InvertCasing(string s) {
			char[] c = s.ToCharArray();
			char[] cUpper = s.ToUpper().ToCharArray();
			char[] cLower = s.ToLower().ToCharArray();

			for (int i = 0; i < c.Length; i++) {
				if (c[i] == cUpper[i]) {
					c[i] = cLower[i];
				} else {
					c[i] = cUpper[i];
				}
			}

			return new string(c);
		}

		public void Print(object s, int? x = null, int? y = null, byte? col = null) {
			if (x.HasValue) {
				Emulator.Memory.drawState.CursorX = x.Value;
			}
			else {
				x = Emulator.Memory.drawState.CursorX;
			}

			if (y.HasValue) {
				Emulator.Memory.drawState.CursorY = y.Value;
			}
			else {
				y = Emulator.Memory.drawState.CursorY;
				Emulator.Memory.drawState.CursorY += 6;
			}

			x -= Emulator.Memory.drawState.CameraX;
			y -= Emulator.Memory.drawState.CameraY;

			if (col.HasValue) {
				Emulator.Memory.drawState.DrawColor = col.Value;
			}

			var c = Emulator.Memory.drawState.DrawColor;
			var xOrig = x.Value;
			var prtStr = InvertCasing(s.ToString());

			foreach (var l in prtStr) {
				if (l == '\n') {
					y += 6;
					x = xOrig;
					continue;
				}

				if (Font.dictionary.ContainsKey(l)) {
					byte[,] digit = Font.dictionary[l];

					for (int i = 0; i < digit.GetLength(0); i += 1) {
						for (int j = 0; j < digit.GetLength(1); j += 1) {
							if (digit[i, j] == 1) {
								DrawPixel(x.Value + j, y.Value + i, c);
							}
						}
					}

					x += digit.GetLength(1) + 1;
				}
			}
		}

		public void Map(int? cellX, int? cellY, int? sx, int? sy, int? cellW, int? cellH, byte? layer = null) {
			var x = cellX ?? 0;
			var y = cellY ?? 0;
			var px = sx ?? 0;
			var py = sy ?? 0;
			var tw = cellW ?? 16;
			var th = cellH ?? 16;

			for (var h = 0; h < th; h++) {
				for (var w = 0; w < tw; w++) {
					var addr = (y + h) < 32 ? RamAddress.Map : RamAddress.GfxMap;
					var spr = Emulator.Memory.Peek(addr + (((y + h) & 0x1f) << 7) + x + w);

					// Spr index 0 is reserved for empty tiles
					if (spr == 0) {
						continue;
					}

					// If layer has not been specified, draw regardless
					if (!layer.HasValue || layer.Value == 0 || ((byte)Emulator.Memory.Fget(spr) & layer.Value) != 0) {
						Spr(spr, px + 8 * w, py + 8 * h, 1, 1);
					}
				}
			}
		}

		public void Flip() {
			Emulator.GraphicsBackend.Flip();
		}

		public void Spr(int n, int? xx = null, int? yy = null, int? w = null, int? h = null, bool flipX = false, bool flipY = false) {
			if (n < 0 || n > 255) {
				return;
			}

			var x = xx ?? 0;
			var y = yy ?? 0;

			x -= Emulator.Memory.drawState.CameraX;
			y -= Emulator.Memory.drawState.CameraY;

			var sprX = (n & 0x0f) << 3;
			var sprY = (n >> 4) << 3;
			var width = 1;
			var height = 1;

			if (w.HasValue) {
				width = w.Value;
			}

			if (h.HasValue) {
				height = h.Value;
			}

			for (var i = 0; i < 8 * width; i++) {
				for (var j = 0; j < 8 * height; j++) {
					Spset(x + (flipX ? 8 * width - i : i), y + (flipY ? 8 * height - j : j), Sget(i + sprX, j + sprY));
				}
			}

			drawCalls++;
		}

		public void Sspr(int sx, int sy, int sw, int sh, int dx, int dy, int? dw = null, int? dh = null,
			bool flipX = false, bool flipY = false) {
			dx -= Emulator.Memory.drawState.CameraX;
			dy -= Emulator.Memory.drawState.CameraY;

			if (!dw.HasValue) {
				dw = sw;
			}

			if (!dh.HasValue) {
				dh = sh;
			}

			float ratioX = sw / (float)dw.Value;
			float ratioY = sh / (float)dh.Value;
			float x = sx;
			float screenX = dx;
			float y;
			float screenY;

			while (x < sx + sw && screenX < dx + dw) {
				y = sy;
				screenY = dy;

				while (y < sy + sh && screenY < dy + dh) {
					Spset((flipX ? dx + dw.Value - ((int)screenX - dx) : (int)screenX),
						(flipY ? dy + dh.Value - ((int)screenY - dy) : (int)screenY), Sget((int)x, (int)y));

					y += ratioY;
					screenY += 1;
				}

				x += ratioX;
				screenX += 1;
			}
		}

		public byte Sget(int x, int y) {
			return Emulator.Memory.GetPixel(x, y, RamAddress.Gfx);
		}

		public void Sset(int x, int y, byte? col = null) {
			if (col.HasValue) {
				Emulator.Memory.drawState.DrawColor = col.Value;
			}

			Emulator.Memory.WritePixel(x, y, Emulator.Memory.drawState.DrawColor, RamAddress.Gfx);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Pset(int x, int y, byte? col = null) {
			x -= Emulator.Memory.drawState.CameraX;
			y -= Emulator.Memory.drawState.CameraY;

			if (!col.HasValue) {
				col = Emulator.Memory.drawState.DrawColor;
			}

			DrawPixel(x, y, col.Value);

			Emulator.Memory.drawState.DrawColor = (byte)(col.Value & 0x0f);
		}

		/// <summary>
		/// _A special kind of Pset only used in the Spr and Sspr functions.
		/// It removes Fillp functionality and default color (DrawColor variable) update, 
		/// since that should only work for functions like circ() and rect().
		/// </summary>
		/// <param name="x"> X screen position. </param>
		/// <param name="y"> Y screen position. </param>
		/// <param name="col"> Color to draw pixel. </param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Spset(int x, int y, byte col) {
			// If the pixel is transparent, don't draw anything.
			if (!Emulator.Memory.drawState.IsTransparent(col)) {
				Emulator.Memory.WritePixel(x, y, Emulator.Memory.drawState.GetDrawColor(col & 0x0f));
			}
		}

		public byte Pget(int x, int y) {
			return Emulator.Memory.GetPixel(x, y);
		}

		public void Rect(int x0, int y0, int x1, int y1, byte? col = null) {
			Line(x0, y0, x1, y0, col);
			Line(x0, y0, x0, y1, col);
			Line(x1, y1, x1, y0, col);
			Line(x1, y1, x0, y1, col);
		}

		public void Rectfill(int x0, int y0, int x1, int y1, byte? col = null) {
			if (y0 > y1) {
				Util.Swap(ref y0, ref y1);
			}

			for (var y = y0; y <= y1; y++) {
				Line(x0, y, x1, y, col);
			}
		}

		public void Line(int x0, int y0, int? x1 = null, int? y1 = null, byte? col = null) {
			if (x1.HasValue) {
				Emulator.Memory.drawState.LineX = x1.Value;
			}

			if (y1.HasValue) {
				Emulator.Memory.drawState.LineY = y1.Value;
			}

			if (col.HasValue) {
				Emulator.Memory.drawState.DrawColor = col.Value;
			}

			var x0_screen = x0 - Emulator.Memory.drawState.CameraX;
			var y0_screen = y0 - Emulator.Memory.drawState.CameraY;
			var x1_screen = Emulator.Memory.drawState.LineX - Emulator.Memory.drawState.CameraX;
			var y1_screen = Emulator.Memory.drawState.LineY - Emulator.Memory.drawState.CameraY;

			DrawLine(x0_screen, y0_screen, x1_screen, y1_screen, Emulator.Memory.drawState.DrawColor);
		}

		public void Circ(int x, int y, double? r, byte? col = null) {
			if (col.HasValue) {
				Emulator.Memory.drawState.DrawColor = col.Value;
			}

			x -= Emulator.Memory.drawState.CameraX;
			y -= Emulator.Memory.drawState.CameraY;

			DrawCircle(x, y, (int)Math.Ceiling(r ?? 1), false);
		}

		public void Circfill(int x, int y, double? r, byte? col = null) {
			if (col.HasValue) {
				Emulator.Memory.drawState.DrawColor = col.Value;
			}

			x -= Emulator.Memory.drawState.CameraX;
			y -= Emulator.Memory.drawState.CameraY;

			DrawCircle(x, y, (int)(r ?? 1), true);
		}

		private void Plot4(int x, int y, int offX, int offY, bool fill) {
			var c = Emulator.Memory.drawState.DrawColor;
			if (fill) {
				DrawLine((x - offX), (y + offY), (x + offX), (y + offY), c);

				if (offY != 0) {
					DrawLine((x - offX), (y - offY), (x + offX), (y - offY), c);
				}
			}
			else {
				DrawPixel((x - offX), (y + offY), c);
				DrawPixel((x + offX), (y + offY), c);

				if (offY != 0) {
					DrawPixel((x - offX), (y - offY), c);
					DrawPixel((x + offX), (y - offY), c);
				}
			}
		}

		//
		// Pure draw functions.
		//

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DrawCircle(int posX, int posY, int r, bool fill) {
			var x = r;
			var y = 0;
			double err = 1 - r;

			while (y <= x) {
				Plot4(posX, posY, x, y, fill);

				if (err < 0) {
					err = err + 2 * y + 3;
				}
				else {
					if (x != y) {
						Plot4(posX, posY, y, x, fill);
					}

					x = x - 1;
					err = err + 2 * (y - x) + 3;
				}

				y = y + 1;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DrawLine(int x0, int y0, int x1, int y1, byte col) {
			var steep = false;

			if (Math.Abs(x1 - x0) < Math.Abs(y1 - y0)) {
				Util.Swap(ref x0, ref y0);
				Util.Swap(ref x1, ref y1);
				steep = true;
			}

			if (x0 > x1) {
				Util.Swap(ref x0, ref x1);
				Util.Swap(ref y0, ref y1);
			}

			var dx = x1 - x0;
			var dy = y1 - y0;
			var d_err = 2 * Math.Abs(dy);
			var err = 0;
			var y = y0;

			for (var x = x0; x <= x1; x++) {
				if (steep) {
					DrawPixel(y, x, col);
				}
				else {
					DrawPixel(x, y, col);
				}

				err += d_err;

				if (err > dx) {
					y += y1 > y0 ? 1 : -1;
					err -= dx * 2;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DrawPixel(int x, int y, byte c) {
			if (Emulator.Memory.drawState.GetFillPBit(x, y) == 0) {
				// Do not consider transparency bit for this operation.
				Emulator.Memory.WritePixel(x, y, (byte)(Emulator.Memory.drawState.GetDrawColor(c & 0x0f) & 0x0f));
			}
			else if (!Emulator.Memory.drawState.FillpTransparent) {
				// Do not consider transparency bit for this operation.
				Emulator.Memory.WritePixel(x, y, (byte)(Emulator.Memory.drawState.GetDrawColor(c >> 4) & 0x0f));
			}
		}
	}
}