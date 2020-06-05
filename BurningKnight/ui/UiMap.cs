using System;
using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.projectile;
using BurningKnight.entity.room;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiMap : Entity {
		private const int W = 64;
		private const int H = 64;

		private static Vector2 scale = new Vector2(2);
		private Player player;
		private TextureRegion slice;
		
		public UiMap(Player pl) {
			player = pl;

			Width = W;
			Height = H;

			X = Display.UiWidth - W - 8;
			Y = 8;

			slice = CommonAse.Particles.GetSlice("fire");

			AlwaysActive = true;
			AlwaysVisible = true;
		}

		public override void Render() {
			var fx = player.CenterX / 16;
			var fy = player.CenterY / 16;
			var x = (int) Math.Floor(fx);
			var y = (int) Math.Floor(fy);
			var level = Run.Level;
			
			var sx = MathUtils.Clamp(0, level.Width - 1, x - W / 4);
			var sy = MathUtils.Clamp(0, level.Height - 1, y - H / 4);
			var tx = MathUtils.Clamp(0, level.Width - 1, x + W / 4);
			var ty = MathUtils.Clamp(0, level.Height - 1, y + H / 4);
			var rect = new Rect(sx, sy, tx, ty);

			Graphics.Color.A = 100;
			Graphics.Render(slice, new Vector2(X, Y), 0, Vector2.Zero, new Vector2(W, H));
			Graphics.Color.A = 255;
			
			foreach (var rm in level.Area.Tagged[Tags.Room]) {
				var room = (Room) rm;
				var rc = new Rect().Setup(room.MapX, room.MapY, room.MapW, room.MapH);

				if (rect.Intersects(rc)) {
					for (var yy = room.MapY; yy < room.MapY + room.MapH; yy++) {
						for (var xx = room.MapX; xx < room.MapX + room.MapW; xx++) {
							var i = level.ToIndex(xx, yy);
							
							if (level.Light[i] > 0.5f && !level.Get(i).IsWall()) {
								if (xx >= sx && xx <= tx && yy >= sy && yy <= ty) {
									Graphics.Render(slice, new Vector2(X + W / 2 + (xx - fx) * 2, Y + H / 2 + (yy - fy) * 2), 0, Vector2.Zero, scale);
								}
							}
						}
					}
				}
			}
			
			
			Graphics.Color = ProjectileColor.Green;
			Graphics.Render(slice, new Vector2(X + W / 2f - 1f, Y + H / 2f - 1f));
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}