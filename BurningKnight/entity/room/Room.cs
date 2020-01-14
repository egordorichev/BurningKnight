using System;
using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.projectile;
using BurningKnight.entity.room.controllable;
using BurningKnight.entity.room.controller;
using BurningKnight.entity.room.input;
using BurningKnight.level;
using BurningKnight.level.entities.chest;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.granny;
using BurningKnight.level.rooms.oldman;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.editor;
using BurningKnight.util;
using BurningKnight.util.geometry;
using ImGuiNET;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.room {
	public class Room : SaveableEntity, PlaceableEntity {
		public int MapX;
		public int MapY;
		public int MapW = 4;
		public int MapH = 4;
		public TagLists Tagged = new TagLists();
		public RoomType Type;
		public bool Explored;
		public bool Cleared;
		public RoomDef Parent;
		
		public List<RoomControllable> Controllable = new List<RoomControllable>();
		public List<RoomInput> Inputs = new List<RoomInput>();
		public List<Piston> Pistons = new List<Piston>();
		public List<RoomController> Controllers = new List<RoomController>();
		public List<Door> Doors = new List<Door>();

		private bool checkCleared;
		private Entity cleared;
		private float t;

		public void CheckCleared(Entity entity) {
			if (!Cleared) {
				checkCleared = true;
				cleared = entity;
			}
		}
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.Room);
		}

		public ItemPool GetPool() {
			switch (Type) {
				case RoomType.Shop: return ItemPool.Shop;
				case RoomType.Secret: return ItemPool.Secret;
				case RoomType.Boss: return ItemPool.Boss;
				case RoomType.Treasure: return ItemPool.Treasure;
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
			
			if (Type == RoomType.Shop || Type == RoomType.Treasure || Type == RoomType.Boss) {
				AddComponent(new LightComponent(this, 128f, new Color(1f, 0.9f, 0.5f, 0.8f)));
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;
			
			if (!settedUp && t >= 0.1f) {
				settedUp = true;
				Setup();
			}

			foreach (var c in Controllers) {
				c.Update(dt);
			}

			if (checkCleared) {
				var found = false;

				foreach (var m in Tagged[Tags.MustBeKilled]) {
					if (m.GetComponent<HealthComponent>().Health > 0) {
						found = true;
						break;
					}
				}

				if (!found) {
					if (!Cleared) {
						SpawnReward();

						Cleared = true;
					
						cleared.HandleEvent(new RoomClearedEvent {
							Room = this
						});
					}
				}

				checkCleared = false;
			}
		}

		private static string[] rewards = {
			// "bk:copper_coin",
			"bk:key",
			"bk:key",
			"bk:bomb",
			"bk:heart",
			"bk:pouch"
		};

		private Entity CreateReward() {
			if (Rnd.Chance(20)) {
				var chest = (Chest) Activator.CreateInstance(ChestRegistry.Instance.Generate());
				Area.Add(chest);

				return chest;
			}
			
			return Items.CreateAndAdd(rewards[Rnd.Int(rewards.Length)], Area);
		}

		private void SpawnReward() {
			if (Run.Depth < 1 || Type != RoomType.Regular || Rnd.Chance(70 - Run.Luck * 10)) {
				return;
			}
			
			var where = new Dot(MapX + MapW / 2, MapY + MapH / 2);
			
			for (var x = -1; x < 2; x++) {
				for (var y = -1; y < 2; y++) {
					var x1 = x;
					var y1 = y;

					Timer.Add(() => {
						var part = new TileParticle();

						part.Top = Run.Level.Tileset.FloorD[0];
						part.TopTarget = Run.Level.Tileset.WallTopADecor;
						part.Side = Run.Level.Tileset.FloorSidesD[0];
						part.Sides = Run.Level.Tileset.WallSidesA[2];
						part.Tile = Tile.FloorD;

						part.X = (where.X + x1) * 16;
						part.Y = (where.Y + y1) * 16 + 8;
						part.Target.X = (where.X + x1) * 16;
						part.Target.Y = (where.Y + y1) * 16 + 8;
						part.TargetZ = -8f;

						Area.Add(part);
					}, Rnd.Float(1f));
				}
			}
			
			Timer.Add(() => {
				var reward = CreateReward();
				reward.BottomCenter = where * 16 + new Vector2(8, 24);
				AnimationUtil.Poof(reward.Center);
			}, 1.5f);
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

		public void Hide() {
			Explored = false;
			
			ApplyToEachTile((x, y) => {
				var i = Run.Level.ToIndex(x, y);

				if (!Run.Level.Get(i).IsWall() || !Run.Level.Get(i + Run.Level.Width).IsWall()) {
					Run.Level.Explored[i] = false;
					Tween.To(0, 1f, xx => Run.Level.Light[i] = xx, 0.5f);
				}
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
			
			ImGui.Text($"Doors: {Doors.Count}");
			
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

			for (var x = MapX + 1; x < MapX + MapW - 1; x++) {
				for (var y = MapY + 1; y < MapY + MapH - 1; y++) {
					if (Run.Level.IsPassable(x, y) && (filter == null || filter(x, y))) {
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

			var tile = tiles[Rnd.Int(tiles.Count)];
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

		public Entity FindClosest(Vector2 to, int tag, Func<Entity, bool> filter = null) {
			var min = float.MaxValue;
			Entity en = null;
			
			foreach (var e in Tagged[tag]) {
				if (filter?.Invoke(e) ?? true) {
					var d = e.DistanceTo(to);

					if (d < min) {
						min = d;
						en = e;
					}
				}
			}

			return en;
		}

		public void OpenHiddenDoors() {
			var level = Run.Level;
			
			foreach (var door in Doors) {
				var x = (int) Math.Floor(door.CenterX / 16);
				var y = (int) Math.Floor(door.CenterY / 16);
				var t = level.Get(x, y);

				if (t == Tile.WallA) {
					var index = level.ToIndex(x, y);
			
					level.Set(index, Type == RoomType.OldMan ? Tile.EvilFloor : Tile.GrannyFloor);
					level.UpdateTile(x, y);
					level.ReCreateBodyChunk(x, y);
					level.LoadPassable();

					ExplosionMaker.LightUp(x * 16 + 8, y * 16 + 8);

					Level.Animate(Area, x, y);
				}
			}
		}

		public void CloseHiddenDoors() {
			var level = Run.Level;
			
			foreach (var door in Doors) {
				var x = (int) Math.Floor(door.CenterX / 16);
				var y = (int) Math.Floor(door.CenterY / 16);
				var t = level.Get(x, y);

				if (level.Get(x, y).Matches(TileFlags.Passable)) {
					var index = level.ToIndex(x, y);
			
					level.Set(index, Tile.WallA);
					level.UpdateTile(x, y);
					level.ReCreateBodyChunk(x, y);
					level.LoadPassable();

					Hide();

					Camera.Instance.Shake(10);
				}
			}
		}

		public void PaintTunnel(List<Door> Doors, Tile Floor, Rect space = null, bool Bold = false, bool shift = true, bool randomRect = true) {
			if (Doors.Count == 0) {
				return;
			}

			var Level = Run.Level;
			var C = space;

			if (C == null) {
				var c = new Dot(MapX + MapW /2, MapY + MapH / 2);
				C = new Rect(c.X, c.Y, c.X, c.Y);
			}

			var minLeft = C.Left;
			var maxRight = C.Right;
			var minTop = C.Top;
			var maxBottom = C.Bottom;
			var Right = MapX + MapW - 1;
			var Bottom = MapY + MapH - 1;

			Painter.Clip = new Rect(MapX, MapY, MapX + MapW - 1, MapY + MapH - 1);

			foreach (var Door in Doors) {
				var dx = (int) Math.Floor(Door.CenterX / 16f);
				var dy = (int) Math.Floor(Door.CenterY / 16f);
				var Start = new Dot(dx, dy);
				Dot Mid;
				Dot End;

				if (shift) {
					if ((int) Start.X == MapX) {
						Start.X++;
					} else if ((int) Start.Y == MapY) {
						Start.Y++;
					} else if ((int) Start.X == Right) {
						Start.X--;
					} else if ((int) Start.Y == Bottom) {
						Start.Y--;
					}
				}

				int RightShift;
				int DownShift;

				if (Start.X < C.Left) {
					RightShift = (int) (C.Left - Start.X);
				} else if (Start.X > C.Right) {
					RightShift = (int) (C.Right - Start.X);
				} else {
					RightShift = 0;
				}

				if (Start.Y < C.Top) {
					DownShift = (int) (C.Top - Start.Y);
				} else if (Start.Y > C.Bottom) {
					DownShift = (int) (C.Bottom - Start.Y);
				} else {
					DownShift = 0;
				}

				if (dx == MapX || dx == Right) {
					Mid = new Dot(MathUtils.Clamp(MapX + 1, Right - 1, Start.X + RightShift), MathUtils.Clamp(MapY + 1, Bottom - 1, Start.Y));
					End = new Dot(MathUtils.Clamp(MapX + 1, Right - 1, Mid.X), MathUtils.Clamp(MapY + 1, Bottom - 1, Mid.Y + DownShift));
				} else {
					Mid = new Dot(MathUtils.Clamp(MapX + 1, Right - 1, Start.X), MathUtils.Clamp(MapY + 1, Bottom - 1, Start.Y + DownShift));
					End = new Dot(MathUtils.Clamp(MapX + 1, Right - 1, Mid.X + RightShift), MathUtils.Clamp(MapY + 1, Bottom - 1, Mid.Y));
				}

				Painter.DrawLine(Level, Start, Mid, Floor, Bold);
				Painter.DrawLine(Level, Mid, End, Floor, Bold);

				if (Rnd.Chance(10)) {
					Painter.Set(Level, End, Tiles.RandomFloor());
				}

				minLeft = Math.Min(minLeft, End.X);
				minTop = Math.Min(minTop, End.Y);
				maxRight = Math.Max(maxRight, End.X);
				maxBottom = Math.Max(maxBottom, End.Y);
			}

			if (randomRect && Rnd.Chance(20)) {
				if (Rnd.Chance()) {
					minLeft--;
				}
				
				if (Rnd.Chance()) {
					minTop--;
				}
				
				if (Rnd.Chance()) {
					maxRight++;
				}
				
				if (Rnd.Chance()) {
					maxBottom++;
				}
			}

			minLeft = MathUtils.Clamp(MapX + 1, Right - 1, minLeft);
			minTop = MathUtils.Clamp(MapY + 1, Bottom - 1, minTop);
			maxRight = MathUtils.Clamp(MapX + 1, Right - 1, maxRight);
			maxBottom = MathUtils.Clamp(MapY + 1, Bottom - 1, maxBottom);

			if (Rnd.Chance()) {
				Painter.Fill(Level, minLeft, minTop, maxRight - minLeft + 1, maxBottom - minTop + 1, Rnd.Chance() ? Floor : Tiles.RandomFloor());
			} else {
				Painter.Rect(Level, minLeft, minTop, maxRight - minLeft + 1, maxBottom - minTop + 1, Rnd.Chance() ? Floor : Tiles.RandomFloor());
			}
			
			Painter.Clip = null;
		}
	}
}