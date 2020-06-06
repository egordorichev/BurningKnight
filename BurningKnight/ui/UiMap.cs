using System;
using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.projectile;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiMap : Entity {
		private const int W = 64;
		private const int H = 64;

		private static Vector2 scale = new Vector2(1f);
		private static Vector2 bigScale = new Vector2(2f);

		private static Vector2 centerOffset = new Vector2(W / 2, H / 2);
		private static Vector2 size = new Vector2(W, H);
		private Player player;
		private TextureRegion slice;

		private TextureRegion shopIcon;
		private TextureRegion exitIcon;
		private TextureRegion treasureIcon;
		private TextureRegion playerIcon;
		private TextureRegion frame;

		public UiMap(Player pl) {
			player = pl;

			Width = W;
			Height = H;

			X = Display.UiWidth - W - 11;
			Y = 11;

			slice = CommonAse.Particles.GetSlice("fire");

			shopIcon = CommonAse.Ui.GetSlice("shop");
			exitIcon = CommonAse.Ui.GetSlice("exit");
			treasureIcon = CommonAse.Ui.GetSlice("treasure");
			playerIcon = CommonAse.Ui.GetSlice("gps");
			frame = CommonAse.Ui.GetSlice("map_frame");

			AlwaysVisible = true;
		}

		public override void Render() {
			if (!Settings.Minimap || Engine.Instance.State.Paused || Run.Type == RunType.Twitch) {
				return;
			}
			
			var fx = player.CenterX / 16;
			var fy = player.CenterY / 16;
			var x = (int) Math.Floor(fx);
			var y = (int) Math.Floor(fy);
			var level = Run.Level;
			
			var sx = MathUtils.Clamp(0, level.Width - 1, x - W / 2);
			var sy = MathUtils.Clamp(0, level.Height - 1, y - H / 2);
			var tx = MathUtils.Clamp(0, level.Width - 1, x + W / 2);
			var ty = MathUtils.Clamp(0, level.Height - 1, y + H / 2);
			var rect = new Rect(sx, sy, tx, ty);

			var r = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;

			r.End();

			var rc = Engine.GraphicsDevice.ScissorRectangle;
			var en = r.EnableClip;

			r.EnableClip = true;
			Engine.GraphicsDevice.ScissorRectangle = new Rectangle((int) (X * Engine.Instance.UiUpscale), (int) (Y * Engine.Instance.UiUpscale), (int) (W * Engine.Instance.UiUpscale), (int) (H * Engine.Instance.UiUpscale));

			r.Begin();

			Graphics.Color = ColorUtils.BlackColor;
			Graphics.Color.A = 150;
			Graphics.Render(slice, new Vector2(X, Y), 0, Vector2.Zero, new Vector2(W, H));
			Graphics.Color.A = 255;
			
			foreach (var rm in level.Area.Tagged[Tags.Room]) {
				var room = (Room) rm;

				if (rect.Intersects(room.Rect)) {
					for (var yy = room.MapY - 1; yy <= room.MapY + room.MapH; yy++) {
						for (var xx = room.MapX; xx < room.MapX + room.MapW; xx++) {
							var i = level.ToIndex(xx, yy);

							if (level.Explored[i] && !level.Get(i).IsWall() && xx >= sx && xx <= tx && yy >= sy && yy <= ty) {
								Graphics.Render(slice, new Vector2((int) Math.Floor(X + W / 2 + (xx - fx)) - 1, (int) Math.Floor(Y + H / 2 + (yy - fy)) - 1), 0, Vector2.Zero, bigScale);
							}
						}
					}
				}
			}
			
			Graphics.Color = ColorUtils.WhiteColor;
			Graphics.Color = Run.Level.Biome.GetMapColor();

			foreach (var rm in level.Area.Tagged[Tags.Room]) {
				var room = (Room) rm;

				if (rect.Intersects(room.Rect)) {
					for (var yy = room.MapY; yy < room.MapY + room.MapH; yy++) {
						for (var xx = room.MapX; xx < room.MapX + room.MapW; xx++) {
							var i = level.ToIndex(xx, yy);

							if (level.Explored[i] && !level.Get(i).IsWall() && xx >= sx && xx <= tx && yy >= sy && yy <= ty) {
								Graphics.Render(slice, new Vector2((int) Math.Floor(X + W / 2 + (xx - fx)), (int) Math.Floor(Y + H / 2 + (yy - fy))), 0, Vector2.Zero, scale);
							}
						}
					}
				}
			}
			
			Graphics.Color = ColorUtils.WhiteColor;

			foreach (var rm in level.Area.Tagged[Tags.Room]) {
				var room = (Room) rm;

				if (!room.Explored) {
					continue;
				}
				
				var tp = room.Type;

				if (tp == RoomType.Shop || tp == RoomType.Treasure || tp == RoomType.Exit || tp == RoomType.Boss) {
					if (rect.Intersects(room.Rect)) {
						var icon = shopIcon;

						switch (tp) {
							case RoomType.Treasure: {
								icon = treasureIcon;
								break;
							}
							
							case RoomType.Boss:
							case RoomType.Exit: {
								icon = exitIcon;
								break;
							}
						}
						
						Graphics.Render(icon, new Vector2((int) Math.Floor(X + W * 0.5f + (room.MapX + room.MapW * 0.5f - fx)), (int) Math.Floor(Y + H * 0.5f + (room.MapY + room.MapH * 0.5f - fy))), 0, icon.Center);
					}
				}
			}

			Graphics.Render(playerIcon, new Vector2(X + W / 2f, Y + H / 2f), 0, playerIcon.Center);

			r.End();
			
			Engine.GraphicsDevice.ScissorRectangle = rc;
			r.EnableClip = en;
			
			r.Begin();

			Graphics.Render(frame, Position - new Vector2(3));
		}
	}
}