using System;
using System.Numerics;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.entity.editor {
	public class TileInfo {
		public Tile Tile;
		public Vector2 Uv0;
		public Vector2 Uv1;
		public IntPtr Texture;

		public TileInfo(Tile tile, Texture2D texture, IntPtr ptr, int x, int y) {
			Tile = tile;
			Texture = ptr;

			Uv0.X = (float) x / texture.Width;
			Uv0.Y = (float) y / texture.Height;
			Uv1.X = Uv0.X + 16f / texture.Width;
			Uv1.Y= Uv0.Y + 16f / texture.Height;
		}
	}
}