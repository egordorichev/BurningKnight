using System;
using BurningKnight.entity;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.editor;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace BurningKnight.level.rooms {
	public class Room : SaveableEntity {
		public int MapX;
		public int MapY;
		public int MapW = 4;
		public int MapH = 4;
		public TagLists Tagged = new TagLists();
		public RoomType Type;
		public bool Explored;
		
		public override void AddComponents() {
			base.AddComponents();
			AddTag(Tags.Room);
		}

		public override void PostInit() {
			base.PostInit();

			X = MapX * 16 + 4;
			Y = MapY * 16 - 4;
			Width = MapW * 16 - 8;
			Height = MapH * 16 - 8;

			var level = Run.Level;
			Explored = level.Explored[level.ToIndex(MapX + 1, MapY + 1)];
		}

		public void Discover() {
			if (Explored) {
				return;
			}

			Explored = true;
			var level = Run.Level;

			for (int y = MapY; y < MapY + MapH - 1; y++) {
				for (int x = MapX; x < MapX + MapW; x++) {
					if (level.IsInside(x, y)) {
						level.Explored[level.ToIndex(x, y)] = true;
					}
				}
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			MapX = stream.ReadInt16();
			MapY = stream.ReadInt16();
			MapW = stream.ReadInt16();
			MapH = stream.ReadInt16();
			
			Type = RoomRegistry.FromIndex(stream.ReadByte());
		}
		
		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteInt16((short) MapX);
			stream.WriteInt16((short) MapY);
			stream.WriteInt16((short) MapW);
			stream.WriteInt16((short) MapH);
			
			stream.WriteByte((byte) RoomRegistry.FromType(Type));
		}
		
		protected int GetRenderLeft(Camera camera, Level level) {
			return (int) MathUtils.Clamp(0, level.Width - 1, Math.Max((int) Math.Floor(camera.X / 16 - 1f), MapX));
		}

		protected int GetRenderTop(Camera camera, Level level) {
			return (int) MathUtils.Clamp(0, level.Height - 1, Math.Max((int) Math.Floor(camera.Y / 16 - 1f), MapY));
		}

		protected int GetRenderRight(Camera camera, Level level) {
			return (int) MathUtils.Clamp(0, level.Width - 1, Math.Min((int) Math.Ceiling(camera.Right / 16 + 1f), MapX + MapW));
		}

		protected int GetRenderBottom(Camera camera, Level level) {
			return (int) MathUtils.Clamp(0, level.Height - 1, Math.Min((int) Math.Ceiling(camera.Bottom / 16 + 1f), MapY + MapH));
		}

		public override void RenderDebug() {
			Graphics.Batch.DrawRectangle(new RectangleF(X, Y, Width, Height), Color.Red);
		}

		public override void RenderImDebug() {
			ImGui.InputInt("Map X", ref MapX);
			ImGui.InputInt("Map Y", ref MapY);
			ImGui.InputInt("Map W", ref MapW);
			ImGui.InputInt("Map H", ref MapH);
			
			X = MapX * 16 + 4;
			Y = MapY * 16 - 4;
			Width = MapW * 16 - 8;
			Height = MapH * 16 - 8;

			/*if (ImGui.Button("Sync")) {
				MapX = (int) Math.Floor(X / 16);
				MapY = (int) Math.Floor(Y / 16);
				MapW = (int) Math.Floor(Width / 16);
				MapH = (int) Math.Floor(Height / 16);
			}*/
		}
	}
}