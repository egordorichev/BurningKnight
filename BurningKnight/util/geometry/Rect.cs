using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BurningKnight.util.geometry {
	public class Rect {
		public int Bottom;
		public int Left;
		public int Right;
		public int Top;

		public Rect() {
			
		}

		public Rect(Rect Rect) {
			Left = Rect.Left;
			this.Top = Rect.Top;
			this.Right = Rect.Right;
			this.Bottom = Rect.Bottom;
		}

		public Rect(int Left, int Top, int Right, int Bottom) {
			this.Left = Left;
			this.Top = Top;
			this.Right = Right;
			this.Bottom = Bottom;
		}

		public static Rect Make(int Left, int Top, int W, int H) {
			var Rect = new Rect();
			Rect.Left = Left;
			Rect.Top = Top;
			Rect.Resize(W, H);

			return Rect;
		}

		public virtual int GetWidth() {
			return Right - Left;
		}

		public virtual int GetHeight() {
			return Bottom - Top;
		}

		public Rect Set(int Left, int Top, int Right, int Bottom) {
			this.Left = Left;
			this.Top = Top;
			this.Right = Right;
			this.Bottom = Bottom;

			return this;
		}

		public Rect Set(Rect Rect) {
			return Set(Rect.Left, Rect.Top, Rect.Right, Rect.Bottom);
		}

		public Rect SetPos(int X, int Y) {
			return Set(X, Y, X + (Right - Left), Y + (Bottom - Top));
		}

		public Rect Shift(int X, int Y) {
			return Set(Left + X, Top + Y, Right + X, Bottom + Y);
		}

		public Rect Resize(int W, int H) {
			return Set(Left, Top, Left + W, Top + H);
		}

		public bool IsEmpty() {
			return Right <= Left || Bottom <= Top;
		}

		public Rect SetEmpty() {
			Left = Right = Top = Bottom = 0;

			return this;
		}

		public Rect Intersect(Rect Other) {
			var Result = new Rect();
			Result.Left = Math.Max(Left, Other.Left);
			Result.Right = Math.Min(Right, Other.Right);
			Result.Top = Math.Max(Top, Other.Top);
			Result.Bottom = Math.Min(Bottom, Other.Bottom);

			return Result;
		}

		public Rect Union(Rect Other) {
			var Result = new Rect();
			Result.Left = Math.Min(Left, Other.Left);
			Result.Right = Math.Max(Right, Other.Right);
			Result.Top = Math.Min(Top, Other.Top);
			Result.Bottom = Math.Max(Bottom, Other.Bottom);

			return Result;
		}

		public Rect Union(int X, int Y) {
			if (IsEmpty()) {
				return Set(X, Y, X + 1, Y + 1);
			}

			if (X < Left)
				Left = X;
			else if (X >= Right) Right = X + 1;

			if (Y < Top)
				Top = Y;
			else if (Y >= Bottom) Bottom = Y + 1;

			return this;
		}

		public Rect Union(Vector2 P) {
			return Union((int) P.X, (int) P.Y);
		}

		public bool Inside(Vector2 P) {
			return P.X >= Left && P.X < Right && P.Y >= Top && P.Y < Bottom;
		}

		public Rect Shrink(int D = 1) {
			return new Rect(Left + D, Top + D, Right - D, Bottom - D);
		}

		public Rect Shrink(int D, int H) {
			return new Rect(Left + D, Top + H, Right - D, Bottom - H);
		}

		public List<Vector2> GetPoints() {
			var Points = new List<Vector2>();

			for (int I = Math.Min(Right, Left); I <= Math.Max(Right, Left); I++)
			for (int J = Math.Min(Top, Bottom); J <= Math.Max(Top, Bottom); J++)
				Points.Add(new Vector2(I, J));

			return Points;
		}
	}
}