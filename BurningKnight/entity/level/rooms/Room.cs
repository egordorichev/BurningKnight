using System;
using BurningKnight.save;
using BurningKnight.util;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.level.rooms {
	public class Room : SaveableEntity {
		public int MapX;
		public int MapY;
		public int MapW;
		public int MapH;
		public TagLists Tagged = new TagLists();
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.Room);
		}

		public override void PostInit() {
			base.PostInit();

			X = MapX * 16 + 8;
			Y = MapY * 16;
			Width = MapW * 16 - 16;
			Height = MapH * 16 - 16;
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			MapX = stream.ReadInt16();
			MapY = stream.ReadInt16();
			MapW = stream.ReadInt16();
			MapH = stream.ReadInt16();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteInt16((short) MapX);
			stream.WriteInt16((short) MapY);
			stream.WriteInt16((short) MapW);
			stream.WriteInt16((short) MapH);
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
	}
}