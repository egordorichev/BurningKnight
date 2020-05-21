using Pico8Emulator.lua;
using Pico8Emulator.unit.mem;
using System;
using System.Collections.Generic;
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

			script.AddFunction("spr", (Action<byte, int?, int?, int?, int?, bool, bool>)Spr);
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

		public void Cls(byte? color = null) {
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

					int width = digit.GetLength(1);
					int height = digit.GetLength(0);

					int iStart = 0, jStart = 0;

					//
					// Clip x and y values to the screen.
					//

					if (x + width < Emulator.Memory.drawState.ClipLeft ||
						x > Emulator.Memory.drawState.ClipRight ||
						y + height < Emulator.Memory.drawState.ClipTop ||
						y > Emulator.Memory.drawState.ClipBottom)
					{
						x += digit.GetLength(1) + 1;
						continue;
					}

					if (x < Emulator.Memory.drawState.ClipLeft)
					{
						jStart = Emulator.Memory.drawState.ClipLeft - x.Value;
					}

					if (x + width > Emulator.Memory.drawState.ClipRight)
					{
						width = width - (x.Value + width - 1 - Emulator.Memory.drawState.ClipRight);
					}

					if (y < Emulator.Memory.drawState.ClipTop)
					{
						iStart = Emulator.Memory.drawState.ClipTop - y.Value;
					}

					if (y + height > Emulator.Memory.drawState.ClipBottom)
					{
						height = height - (y.Value + height - 1 - Emulator.Memory.drawState.ClipBottom);
					}

					//
					// Write pixels.
					//

					for (int i = iStart; i < height; i += 1) {
						for (int j = jStart; j < width; j += 1) {
							if (digit[i, j] == 1) {
								int xx = x.Value + j;
								int yy = y.Value + i;

								byte color;
								var k = ((yy & 0b11) << 2) + (xx & 0b11);

								if ((Emulator.Memory.drawState.FillPattern & (1 << 15) >> k) >> (15 - k) == 0)
								{
									//Do not consider transparency bit for this operation.
									color = (byte)(Emulator.Memory.ram[RamAddress.Palette0 + (c & 0x0f)] & 0x0f);
								}
								else if (!Emulator.Memory.drawState.FillpTransparent)
								{
									//Do not consider transparency bit for this operation.
									color = (byte)(Emulator.Memory.drawState.GetDrawColor(c >> 4) & 0x0f);
								}
								else
								{
									continue;
								}

								int index = (((yy << 7) + xx) >> 1) + 0x6000;

								if ((xx & 1) == 0)
								{
									Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0xf0) | (color & 0x0f));
								}
								else
								{
									Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0x0f) | ((color & 0x0f) << 4));
								}
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

		public void Spr(byte n, int? xx = null, int? yy = null, int? w = null, int? h = null, bool flipX = false, bool flipY = false) {
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

			int pWidth = 8 * width;
			int pHeight = 8 * height;
			int iStart = 0, jStart = 0;

			if (x + pWidth < Emulator.Memory.drawState.ClipLeft ||
				x > Emulator.Memory.drawState.ClipRight ||
				y + pHeight < Emulator.Memory.drawState.ClipTop ||
				y > Emulator.Memory.drawState.ClipBottom)
			{
				return;
			}

			if (x < Emulator.Memory.drawState.ClipLeft)
			{
				iStart = Emulator.Memory.drawState.ClipLeft - x;
			}

			if (x + pWidth > Emulator.Memory.drawState.ClipRight)
			{
				pWidth = pWidth - (x + pWidth - 1 - Emulator.Memory.drawState.ClipRight);
			}

			if (y < Emulator.Memory.drawState.ClipTop)
			{
				jStart = Emulator.Memory.drawState.ClipTop - y;
			}

			if (y + pHeight > Emulator.Memory.drawState.ClipBottom)
			{
				pHeight = pHeight - (y + pHeight - 1 - Emulator.Memory.drawState.ClipBottom);
			}

			for (int i = iStart; i < pWidth; i++)
			{
				for (int j = jStart; j < pHeight; j++)
				{
					Spset(
						x + i, 
						y + j, 
						Sget(sprX + (flipX ? 8 * width - 1 - i : i), sprY + (flipY ? 8 * height - 1 - j : j)));
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

			float endScreenX = dx + dw.Value;
			float endScreenY = dy + dh.Value;
			float startScreenX = dx;
			float startScreenY = dy;
			float startX = sx;
			float startY = sy;

			if (endScreenX < Emulator.Memory.drawState.ClipLeft ||
				startScreenX > Emulator.Memory.drawState.ClipRight ||
				endScreenY < Emulator.Memory.drawState.ClipTop ||
				startScreenY > Emulator.Memory.drawState.ClipBottom)
			{
				return;
			}

			if (startScreenX < Emulator.Memory.drawState.ClipLeft)
			{
				startX += (Emulator.Memory.drawState.ClipLeft - startScreenX) * ratioX;
				startScreenX = Emulator.Memory.drawState.ClipLeft;
			}

			if (endScreenX > Emulator.Memory.drawState.ClipRight)
			{
				endScreenX = Emulator.Memory.drawState.ClipRight + 1;
			}

			if (startScreenY < Emulator.Memory.drawState.ClipTop)
			{
				startY += (Emulator.Memory.drawState.ClipTop - startScreenY) * ratioY;
				startScreenY = Emulator.Memory.drawState.ClipTop;
			}

			if (endScreenY > Emulator.Memory.drawState.ClipBottom)
			{
				endScreenY = Emulator.Memory.drawState.ClipBottom + 1;
			}

			screenX = startScreenX;
			x = startX;
			while (x < sx + sw && screenX < endScreenX) {
				y = startY;
				screenY = startScreenY;

				while (y < sy + sh && screenY < endScreenY) {
					Spset(
						(int)screenX,
						(int)screenY,
						Sget(
							(int)(flipX ? sx + sw - 1 - ((int)x - sx) : x), 
							(int)(flipY ? sy + sh - 1 - ((int)y - sy) : y)));

					y += ratioY;
					screenY += 1;
				}

				x += ratioX;
				screenX += 1;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte Sget(int x, int y) {
			int index = (y * 128 + x) / 2;

			if (index < 0 || index > 64 * 128 - 1)
			{
				return 0x10;
			}

			return Util.GetHalf(Emulator.Memory.ram[index + RamAddress.Gfx], (x & 1) == 0);
		}

		public void Sset(int x, int y, byte? col = null) {
			if (col.HasValue) {
				Emulator.Memory.drawState.DrawColor = col.Value;
			}

			if (x < Emulator.Memory.drawState.ClipLeft || 
				y < Emulator.Memory.drawState.ClipTop || 
				x > Emulator.Memory.drawState.ClipRight || 
				y > Emulator.Memory.drawState.ClipBottom)
			{
				return;
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

			Emulator.Memory.drawState.DrawColor = (byte)(col.Value & 0x0f);

			if (x < Emulator.Memory.drawState.ClipLeft ||
				x > Emulator.Memory.drawState.ClipRight ||
				y < Emulator.Memory.drawState.ClipTop ||
				y > Emulator.Memory.drawState.ClipBottom)
			{
				return;
			}

			int c = col.Value;

			byte color;
			var k = ((y & 0b11) << 2) + (x & 0b11);

			if ((Emulator.Memory.drawState.FillPattern & (1 << 15) >> k) >> (15 - k) == 0)
			{
				//Do not consider transparency bit for this operation.
				color = (byte)(Emulator.Memory.ram[RamAddress.Palette0 + (c & 0x0f)] & 0x0f);
			}
			else if (!Emulator.Memory.drawState.FillpTransparent)
			{
				//Do not consider transparency bit for this operation.
				color = (byte)(Emulator.Memory.drawState.GetDrawColor(c >> 4) & 0x0f);
			}
			else
			{
				return;
			}

			int index = (((y << 7) + x) >> 1) + 0x6000;

			if ((x & 1) == 0)
			{
				Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0xf0) | (color & 0x0f));
			}
			else
			{
				Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0x0f) | ((color & 0x0f) << 4));
			}
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
			if ((Emulator.Memory.ram[RamAddress.Palette0 + col] & 0x10) == 0) {
				col = Emulator.Memory.ram[RamAddress.Palette0 + (col & 0x0f)];

				int index = (((y << 7) + x) >> 1) + 0x6000;

				if ((x & 1) == 0)
				{
					Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0xf0) | (col & 0x0f));
				}
				else
				{
					Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0x0f) | ((col & 0x0f) << 4));
				}
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
				List<int> drawList = new List<int> { x - offX, y + offY, x + offX, y + offY};
				if (offY != 0)
				{
					drawList.AddRange(new List<int> { x - offX, y - offY, x + offX, y - offY });
				}

				//
				// Draw pixels.
				//
				
				for (int i = 0; i < drawList.Count; i += 2)
				{
					int xx = drawList[i];
					int yy = drawList[i + 1];

					if (xx < Emulator.Memory.drawState.ClipLeft ||
						xx > Emulator.Memory.drawState.ClipRight ||
						yy < Emulator.Memory.drawState.ClipTop ||
						yy > Emulator.Memory.drawState.ClipBottom)
					{
						continue;
					}

					byte color;
					var k = ((yy & 0b11) << 2) + (xx & 0b11);

					if ((Emulator.Memory.drawState.FillPattern & (1 << 15) >> k) >> (15 - k) == 0)
					{
						//Do not consider transparency bit for this operation.
						color = (byte)(Emulator.Memory.ram[RamAddress.Palette0 + (c & 0x0f)] & 0x0f);
					}
					else if (!Emulator.Memory.drawState.FillpTransparent)
					{
						//Do not consider transparency bit for this operation.
						color = (byte)(Emulator.Memory.drawState.GetDrawColor(c >> 4) & 0x0f);
					}
					else
					{
						continue;
					}

					int index = (((yy << 7) + xx) >> 1) + 0x6000;

					if ((xx & 1) == 0)
					{
						Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0xf0) | (color & 0x0f));
					}
					else
					{
						Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0x0f) | ((color & 0x0f) << 4));
					}
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

			if (x1 < Emulator.Memory.drawState.ClipLeft ||
				x0 > Emulator.Memory.drawState.ClipRight ||
				y1 > Emulator.Memory.drawState.ClipBottom ||
				y0 < Emulator.Memory.drawState.ClipTop)
			{
				return;
			}

			if (x0 < Emulator.Memory.drawState.ClipLeft)
			{
				x0 = Emulator.Memory.drawState.ClipLeft;
			}

			if (x1 > Emulator.Memory.drawState.ClipRight)
			{
				x1 = Emulator.Memory.drawState.ClipRight;
			}

			if (y0 < Emulator.Memory.drawState.ClipTop)
			{
				y0 = Emulator.Memory.drawState.ClipTop;
			}

			if (y1 > Emulator.Memory.drawState.ClipBottom)
			{
				y1 = Emulator.Memory.drawState.ClipBottom;
			}


			var dx = x1 - x0;
			var dy = y1 - y0;
			var d_err = 2 * Math.Abs(dy);
			var err = 0;
			var y = y0;

			for (var x = x0; x <= x1; x++) {
				int xx, yy;
				if (steep) {
					xx = y; yy = x;
				}
				else {
					xx = x; yy = y;
				}

				err += d_err;

				if (err > dx)
				{
					y += y1 > y0 ? 1 : -1;
					err -= dx * 2;
				}

				//
				// Draw pixel
				//

				byte color;
				var i = ((yy & 0b11) << 2) + (xx & 0b11);

				if ((Emulator.Memory.drawState.FillPattern & (1 << 15) >> i) >> (15 - i) == 0)
				{
					//Do not consider transparency bit for this operation.
					color = (byte)(Emulator.Memory.ram[RamAddress.Palette0 + (col & 0x0f)] & 0x0f);
				}
				else if (!Emulator.Memory.drawState.FillpTransparent)
				{
					//Do not consider transparency bit for this operation.
				   color = (byte)(Emulator.Memory.drawState.GetDrawColor(col >> 4) & 0x0f);
				}
				else
				{
					continue;
				}

				int index = (((yy << 7) + xx) >> 1) + 0x6000;

				if ((xx & 1) == 0)
				{
					Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0xf0) | (color & 0x0f));
				}
				else
				{
					Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0x0f) | ((color & 0x0f) << 4));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DrawPixel(int x, int y, byte c) {
			byte color;
			var i = ((y & 0b11) << 2) + (x & 0b11);
			color = (byte)(Emulator.Memory.ram[RamAddress.Palette0 + (c & 0x0f)] & 0x0f);
			//if ((Emulator.Memory.drawState.FillPattern & (1 << 15) >> i) >> (15 - i) == 0) {
			//	 Do not consider transparency bit for this operation.
			//}
			//else if (!Emulator.Memory.drawState.FillpTransparent) {
			//	 Do not consider transparency bit for this operation.
			//	color = (byte)(Emulator.Memory.drawState.GetDrawColor(c >> 4) & 0x0f);
			//}
			//else
			//{
			//	return;
			//}

			//if (x < Emulator.Memory.drawState.ClipLeft || 
			//	y < Emulator.Memory.drawState.ClipTop || 
			//	x > Emulator.Memory.drawState.ClipRight || 
			//	y > Emulator.Memory.drawState.ClipBottom)
			//{
			//	return;
			//}

			int index = (((y << 7) + x) >> 1) + 0x6000;

			if ((x & 1) == 0)
			{
				Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0xf0) | (color & 0x0f));
			}
			else
			{
				Emulator.Memory.ram[index] = (byte)((byte)(Emulator.Memory.ram[index] & 0x0f) | ((color & 0x0f) << 4));
			}
		}
	}
}