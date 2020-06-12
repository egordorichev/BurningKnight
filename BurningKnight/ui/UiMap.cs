using System;
using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.projectile;
using BurningKnight.entity.room;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util.geometry;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Lens.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BurningKnight.ui {
	public class UiMap : Entity {
		private const int W = 64;
		private const int H = 64;

		private static Vector2 scale = new Vector2(1f);
		private static Vector2 bigScale = new Vector2(3f);
		private static Color doorColor = new Color(93, 44, 40);
		
		private Player player;
		private TextureRegion slice;

		private TextureRegion playerIcon;
		private TextureRegion frame;

		private RenderTarget2D target;

		public UiMap(Player pl) {
			player = pl;
			target = new RenderTarget2D(Engine.GraphicsDevice, W, H, false, Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24);
			
			Width = W;
			Height = H;

			X = Display.UiWidth - W - 11;
			Y = 11;

			slice = CommonAse.Particles.GetSlice("fire");

			playerIcon = CommonAse.Ui.GetSlice("gps");
			frame = CommonAse.Ui.GetSlice("map_frame");

			AlwaysVisible = true;
		}

		public override void Destroy() {
			base.Destroy();
			target.Dispose();
		}

		public override void Render() {
			if (!Settings.Minimap || Engine.Instance.State.Paused || Run.Type == RunType.Twitch) {
				return;
			}

			if (player.TryGetComponent<PlayerInputComponent>(out var inp) && inp.InDialog) {
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
			Engine.GraphicsDevice.SetRenderTarget(target);
			Graphics.Clear(Color.Transparent);
			r.BeginUi(true);

			Graphics.Color = ColorUtils.BlackColor;
			Graphics.Color.A = 150;
			Graphics.Render(slice, Vector2.Zero, 0, Vector2.Zero, new Vector2(W, H));
			Graphics.Color.A = 255;
			
			foreach (var rm in level.Area.Tagged[Tags.Room]) {
				var room = (Room) rm;

				if (room.Explored && rect.Intersects(room.Rect)) {
					for (var yy = room.MapY - 1; yy < room.MapY + room.MapH; yy++) {
						for (var xx = room.MapX; xx < room.MapX + room.MapW; xx++) {
							var i = level.ToIndex(xx, yy);

							if (level.Explored[i] && !level.Get(i).IsWall() && xx >= sx && xx <= tx && yy >= sy && yy <= ty) {
								Graphics.Render(slice, new Vector2((int) Math.Floor(W / 2 + (xx - fx)) - 1, (int) Math.Floor(H / 2 + (yy - fy)) - 1), 0, Vector2.Zero, bigScale);
							}
						}
					}
				}
			}
			
			var cl = Run.Level.Biome.GetMapColor();
			Graphics.Color = cl;

			foreach (var rm in level.Area.Tagged[Tags.Room]) {
				var room = (Room) rm;

				if (room.Explored && rect.Intersects(room.Rect)) {
					for (var yy = room.MapY; yy < room.MapY + room.MapH; yy++) {
						for (var xx = room.MapX; xx < room.MapX + room.MapW; xx++) {
							var i = level.ToIndex(xx, yy);

							if (level.Explored[i] && !level.Get(i).IsWall() && xx >= sx && xx <= tx && yy >= sy && yy <= ty) {
								Graphics.Render(slice, new Vector2((int) Math.Floor(W / 2 + (xx - fx)), (int) Math.Floor(H / 2 + (yy - fy))), 0, Vector2.Zero, scale);
							}
							
							
							if (room.Explored && !(room.Type == RoomType.Granny || room.Type == RoomType.OldMan || room.Type == RoomType.Boss)) {
								Graphics.Color = doorColor;
								
								foreach (var d in room.Doors) {
									Graphics.Render(slice, new Vector2((int) Math.Floor(W / 2 - fx + (int) Math.Floor(d.CenterX / 16)), (int) Math.Floor(H / 2 - fy + (int) Math.Floor(d.Bottom / 16))));
								}
								
								Graphics.Color = cl;
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

				if (tp == RoomType.Exit ? Run.Depth % 2 == 1 : RoomTypeHelper.ShouldBeDisplayOnMap(tp)) {
					if (rect.Intersects(room.Rect)) {
						var icon = RoomTypeHelper.Icons[(int) tp];
						Graphics.Render(icon, new Vector2((int) Math.Floor(W * 0.5f + (int) Math.Floor(room.MapX + room.MapW * 0.5f) - fx), (int) Math.Floor(H * 0.5f + (int) Math.Floor(room.MapY + room.MapH * 0.5f) - fy)), 0, icon.Center);
					}
				}
			}

			Graphics.Render(playerIcon, new Vector2(W / 2f, H / 2f), 0, playerIcon.Center);

			r.End();
			r.Begin();

			Graphics.Render(target, Position);
			Graphics.Render(frame, Position - new Vector2(3));
		}
	}
}