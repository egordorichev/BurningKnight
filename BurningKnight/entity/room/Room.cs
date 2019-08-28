using System;
using System.Collections.Generic;
using BurningKnight.entity.item;
using BurningKnight.entity.room.controllable;
using BurningKnight.entity.room.controller;
using BurningKnight.entity.room.input;
using BurningKnight.level;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using ImGuiNET;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.room {
	public class Room : SaveableEntity {
		public int MapX;
		public int MapY;
		public int MapW = 4;
		public int MapH = 4;
		public TagLists Tagged = new TagLists();
		public RoomType Type;
		public bool Explored;
		public bool Cleared;
		
		public List<RoomControllable> Controllable = new List<RoomControllable>();
		public List<RoomInput> Inputs = new List<RoomInput>();
		public List<Piston> Pistons = new List<Piston>();
		public List<RoomController> Controllers = new List<RoomController>();
		
		public override void AddComponents() {
			base.AddComponents();
			AddTag(Tags.Room);
		}

		public ItemPool GetPool() {
			switch (Type) {
				case RoomType.Shop: return ItemPool.Shop;
				case RoomType.Secret: return ItemPool.Secret;
				case RoomType.Boss: return ItemPool.Boss;
				case RoomType.Treasure: return ItemPool.Chest;
			}

			return null;
		}

		public override void PostInit() {
			base.PostInit();

			X = MapX * 16 + 4;
			Y = MapY * 16 - 4;
			Width = MapW * 16 - 8;
			Height = MapH * 16 - 8;

			AlwaysActive = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!settedUp) {
				settedUp = true;
				Setup();
			}

			foreach (var c in Controllers) {
				c.Update(dt);
			}
		}

		private bool settedUp;
		
		private void Setup() {
			var level = Run.Level;
			Explored = level.Explored[level.ToIndex(MapX + 1, MapY + 1)];
			
			ApplyToEachTile((x, y) => {
				var tile = level.Get(x, y);

				if (tile.Matches(Tile.Piston, Tile.PistonDown)) {
					Pistons.Add(new Piston(x, y));
				}
			});
			
			foreach (var c in Controllers) {
				c.Init();
			}
		}

		public override void Destroy() {
			base.Destroy();
			
			foreach (var c in Controllers) {
				c.Destroy();
			}

			Pistons.Clear();
			Controllable.Clear();
			Inputs.Clear();
		}

		public void Discover() {
			Explored = true;

			ApplyToEachTile((x, y) => {
				Run.Level.Explored[Run.Level.ToIndex(x, y)] = true;
			});
		}

		public void ApplyToEachTile(Action<int, int> callback) {
			var level = Run.Level;
			
			for (int y = MapY; y < MapY + MapH - 1; y++) {
				for (int x = MapX; x < MapX + MapW; x++) {
					if (level.IsInside(x, y)) {
						callback(x, y);
					}
				}
			}
		}

		public void Generate() {
			foreach (var c in Controllers) {
				c.Generate();
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			MapX = stream.ReadInt16();
			MapY = stream.ReadInt16();
			MapW = stream.ReadInt16();
			MapH = stream.ReadInt16();
			
			Type = RoomRegistry.FromIndex(stream.ReadByte());

			if (Run.Depth < 1) {
				return;
			}
			
			var count = stream.ReadByte();

			for (var i = 0; i < count; i++) {
				var c = RoomControllerRegistery.Get(stream.ReadString());

				if (c != null) {
					Controllers.Add(c);
					c.Room = this;
					c.Load(stream);
				}
			}
		}
		
		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteInt16((short) MapX);
			stream.WriteInt16((short) MapY);
			stream.WriteInt16((short) MapW);
			stream.WriteInt16((short) MapH);

			stream.WriteByte((byte) RoomRegistry.FromType(Type));

			if (Run.SavingDepth < 1) {
				return;
			}
			
			stream.WriteByte((byte) Controllers.Count);

			foreach (var c in Controllers) {
				stream.WriteString(c.Id);
				c.Save(stream);
			}
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

		public List<Point> GetFreeTiles(Func<int, int, bool> filter = null) {
			var list = new List<Point>();

			for (var x = MapX; x < MapX + MapW; x++) {
				for (var y = MapY; y < MapY + MapH; y++) {
					if (Run.Level.CheckFor(x, y, TileFlags.Passable) && (filter == null || filter(x, y))) {
						list.Add(new Point(x, y));
					}
				}
			}
			
			return list;
		}

		public Vector2 GetRandomFreeTile(Func<int, int, bool> filter = null) {
			var tiles = GetFreeTiles(filter);

			if (tiles.Count == 0) {
				return Center;
			}

			var tile = tiles[Random.Int(tiles.Count)];
			return new Vector2(tile.X, tile.Y);
		}

		public Vector2 GetRandomFreeTileNearWall(Func<int, int, bool> filter = null) {
			return GetRandomFreeTile((x, y) => {
				if (Run.Level.CheckFor(x - 1, y, TileFlags.Passable)
				&& Run.Level.CheckFor(x + 1, y, TileFlags.Passable)
				&& Run.Level.CheckFor(x, y - 1, TileFlags.Passable)
				&& Run.Level.CheckFor(x, y + 1, TileFlags.Passable)) {
					// No wall here :/
					return false;
				}
				
				return filter == null || filter(x, y);
			});
		}

		public void AddController(string id) {
			var c = RoomControllerRegistery.Get(id);

			if (c != null) {
				Controllers.Add(c);
				c.Room = this;
				c.Init();
			}
		}

		public void HandleInputChange(RoomInput.ChangedEvent e) {
			foreach (var c in Controllers) {
				c.HandleInputChange(e);
			}
		}
	}
}