using System;
using System.Collections.Generic;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.room;
using BurningKnight.level.floors;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.connection;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.rooms.secret;
using BurningKnight.level.rooms.shop;
using BurningKnight.level.rooms.special;
using BurningKnight.level.rooms.trap;
using BurningKnight.level.rooms.treasure;
using BurningKnight.level.tile;
using BurningKnight.level.walls;
using BurningKnight.state;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.util;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.rooms {
	public abstract class RoomDef : Rect {
		public enum Connection {
			All,
			Left,
			Right,
			Top,
			Bottom
		}

		public Dictionary<RoomDef, DoorPlaceholder> Connected = new Dictionary<RoomDef, DoorPlaceholder>();
		public int Id;
		public int Distance = -1;

		public List<RoomDef> Neighbours = new List<RoomDef>();
		private List<Dot> Busy = new List<Dot>();

		public virtual int GetMinWidth() {
			return 10;
		}

		public virtual int GetMinHeight() {
			return 10;
		}

		public virtual int GetMaxWidth() {
			return 16;
		}

		public virtual int GetMaxHeight() {
			return 16;
		}

		public abstract int GetMaxConnections(Connection Side);

		public abstract int GetMinConnections(Connection Side);

		public virtual void PaintFloor(Level level) {
			Painter.Fill(level, this, Tile.WallA);
			FloorRegistry.Paint(level, this);
		}
		
		public virtual void Paint(Level level) {
			WallRegistry.Paint(level, this);
		}

		public virtual void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Regular;
			}
		}

		public int GetCurrentConnections(Connection Direction) {
			if (Direction == Connection.All) {
				return Connected.Count;
			}

			var Total = 0;

			foreach (var R in Connected.Keys) {
				var I = Intersect(R);

				if (Direction == Connection.Left && I.GetWidth() == 0 && I.Left == Left) {
					Total++;
				} else if (Direction == Connection.Top && I.GetHeight() == 0 && I.Top == Top) {
					Total++;
				} else if (Direction == Connection.Right && I.GetWidth() == 0 && I.Right == Right) {
					Total++;
				} else if (Direction == Connection.Bottom && I.GetHeight() == 0 && I.Bottom == Bottom) {
					Total++;
				}
			}

			return Total;
		}

		public int GetLastConnections(Connection Direction) {
			if (GetCurrentConnections(Connection.All) >= GetMaxConnections(Connection.All)) {
				return 0;
			}

			return GetMaxConnections(Direction) - GetCurrentConnections(Direction);
		}

		public virtual bool CanConnect(RoomDef R, Dot P) {
			return ((int) P.X == Left || (int) P.X == Right) != ((int) P.Y == Top || (int) P.Y == Bottom);
		}

		public bool CanConnect(Connection Direction) {
			var Cnt = GetLastConnections(Direction);

			return Cnt > 0;
		}

		public virtual bool CanConnect(RoomDef R) {
			var I = Intersect(R);
			var FoundPoint = false;

			foreach (var P in I.GetPoints()) {
				if (CanConnect(R, P) && R.CanConnect(R, P)) {
					FoundPoint = true;

					break;
				}
			}

			if (!FoundPoint) {
				return false;
			}

			if (I.GetWidth() == 0 && I.Left == Left) {
				return CanConnect(Connection.Left) && R.CanConnect(Connection.Left);
			}

			if (I.GetHeight() == 0 && I.Top == Top) {
				return CanConnect(Connection.Top) && R.CanConnect(Connection.Top);
			}

			if (I.GetWidth() == 0 && I.Right == Right) {
				return CanConnect(Connection.Right) && R.CanConnect(Connection.Right);
			}

			if (I.GetHeight() == 0 && I.Bottom == Bottom) {
				return CanConnect(Connection.Bottom) && R.CanConnect(Connection.Bottom);
			}

			return false;
		}

		public bool ConnectTo(RoomDef Other) {
			if (Neighbours.Contains(Other)) {
				return true;
			}

			var I = Intersect(Other);
			var W = I.GetWidth();
			var H = I.GetHeight();

			if (W == 0 && H >= 2 || H == 0 && W >= 2) {
				Neighbours.Add(Other);
				Other.Neighbours.Add(this);

				return true;
			}

			return false;
		}

		public bool ConnectWithRoom(RoomDef roomDef) {
			if ((Neighbours.Contains(roomDef) || ConnectTo(roomDef)) && !Connected.ContainsKey(roomDef) && CanConnect(roomDef)) {
				Connected[roomDef] = null;
				roomDef.Connected[this] = null;

				return true;
			}

			return false;
		}

		public Dot GetRandomCell() {
			return new Dot(Random.Int(Left + 1, Right), Random.Int(Top + 1, Bottom));
		}
		
		public Dot GetRandomCellWithWalls() {
			return new Dot(Random.Int(Left, Right + 1), Random.Int(Top, Bottom + 1));
		}

		public Dot GetRandomFreeCell() {
			Dot dot;
			var At = 0;

			do {
				if (At++ > 200) {
					Log.Error($"To many attempts ({GetType().Name})");
					return null;
				}

				dot = GetRandomCell();
			} while (!Run.Level.CheckFor((int) dot.X, (int) dot.Y, TileFlags.Passable));

			return dot;
		}
		
		public Dot GetRandomCellNearWall() {
			Dot dot;
			var Att = 0;

			do {
				var At = 0;

				if (At++ > 200) {
					Log.Error($"To many attempts ({GetType().Name})");
					return null;
				}

				while (true) {
					if (At++ > 200) {
						Log.Error($"To many attempts ({GetType().Name})");
						return null;
					}

					var found = false;
					dot = GetRandomCellWithWalls();

					foreach (var b in Busy) {
						if ((int) b.X == (int) dot.X && (int) b.Y == (int) dot.Y) {
							found = true;
							break;
						}
					}

					if (found) {
						continue;
					}

					if (Connected.Count == 0) {
						return dot;
					}

					if (!Run.Level.Get((int) dot.X, (int) dot.Y).IsWall()) {
						continue;
					}

					foreach (var Door in Connected.Values) {
						var Dx = (int) (Door.X - dot.X);
						var Dy = (int) (Door.Y - dot.Y);
						var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

						if (D < 3) {
							found = true;
							break;
						}
					}

					if (!found) {
						break;
					}
				}

				if (dot.X + 1 < Right && !Run.Level.Get((int) dot.X + 1, (int) dot.Y).IsWall()) {
					Busy.Add(dot);
					return new Dot(dot.X + 1, dot.Y);
				}
				
				if (dot.X - 1 > Left && !Run.Level.Get((int) dot.X - 1, (int) dot.Y).IsWall()) {
					Busy.Add(dot);
					return new Dot(dot.X - 1, dot.Y);
				}
				
				if (dot.Y + 1 < Bottom && !Run.Level.Get((int) dot.X, (int) dot.Y + 1).IsWall()) {
					Busy.Add(dot);
					return new Dot(dot.X, dot.Y + 1);
				}
				
				if (dot.X - 1 > Top && !Run.Level.Get((int) dot.X, (int) dot.Y - 1).IsWall()) {
					Busy.Add(dot);
					return new Dot(dot.X, dot.Y - 1);
				}
			} while (true);
		}

		public Dot GetRandomDoorFreeCell() {
			Dot dot;
			var At = 0;

			while (true) {
				if (At++ > 200) {
					Log.Error($"To many attempts ({GetType().Name})");
					return null;
				}

				dot = GetRandomCell();

				if (Connected.Count == 0) {
					return dot;
				}

				if (!Run.Level.CheckFor((int) dot.X, (int) dot.Y, TileFlags.Passable)) {
					continue;
				}

				var found = false;

				foreach (var Door in Connected.Values) {
					var Dx = (int) (Door.X - dot.X);
					var Dy = (int) (Door.Y - dot.Y);
					var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

					if (D < 3) {
						found = true;
						break;
					}
				}

				if (!found) {
					return dot;
				}
			}
		}

		public RoomDef GetRandomNeighbour() {
			return Neighbours[Random.Int(Neighbours.Count)];
		}

		public bool SetSize() {
			return SetSize(GetMinWidth(), GetMaxWidth(), GetMinHeight(), GetMaxHeight());
		}

		protected virtual int ValidateWidth(int W) {
			return W;
		}

		protected virtual int ValidateHeight(int H) {
			return H;
		}

		protected bool SetSize(int MinW, int MaxW, int MinH, int MaxH) {
			if (MinW < GetMinWidth() || MaxW > GetMaxWidth() || MinH < GetMinHeight() || MaxH > GetMaxHeight() || MinW > MaxW || MinH > MaxH) {
				return false;
			}

			if (Quad()) {
				var V = Math.Min(ValidateWidth(Random.Int(MinW, MaxW) - 1), ValidateHeight(Random.Int(MinH, MaxH) - 1));
				Resize(V, V);
			} else {
				Resize(ValidateWidth(Random.Int(MinW, MaxW) - 1), ValidateHeight(Random.Int(MinH, MaxH) - 1));
			}


			return true;
		}

		protected bool Quad() {
			return false;
		}

		public bool SetSizeWithLimit(int W, int H) {
			if (W < GetMinWidth() || H < GetMinHeight()) {
				return false;
			}

			SetSize();

			if (GetWidth() > W || GetHeight() > H) {
				var Ww = ValidateWidth(Math.Min(GetWidth(), W) - 1);
				var Hh = ValidateHeight(Math.Min(GetHeight(), H) - 1);

				if (Ww >= W || Hh >= H) {
					return false;
				}

				Resize(Ww, Hh);
			}

			return true;
		}

		public void ClearConnections() {
			foreach (var R in Neighbours) {
				R.Neighbours.Remove(this);
			}

			Neighbours.Clear();

			foreach (var R in Connected.Keys) {
				R.Connected.Remove(this);
			}

			Connected.Clear();
		}

		public bool CanPlaceWater(Dot P) {
			return Inside(P);
		}

		public List<Dot> GetWaterPlaceablePoints() {
			var Points = new List<Dot>();

			for (var I = Left + 1; I <= Right - 1; I++) {
				for (var J = Top + 1; J <= Bottom - 1; J++) {
					var P = new Dot(I, J);

					if (CanPlaceWater(P)) {
						Points.Add(P);
					}
				}
			}

			return Points;
		}

		public bool CanPlaceGrass(Dot P) {
			return Inside(P);
		}

		public List<Dot> GetGrassPlaceablePoints() {
			var Points = new List<Dot>();

			for (var I = Left + 1; I <= Right - 1; I++) {
				for (var J = Top + 1; J <= Bottom - 1; J++) {
					var P = new Dot(I, J);

					if (CanPlaceGrass(P)) {
						Points.Add(P);
					}
				}
			}

			return Points;
		}
		
		public List<Dot> GetPassablePoints(Level level) {
			var Points = new List<Dot>();

			for (var I = Left + 1; I <= Right - 1; I++) {
				for (var J = Top + 1; J <= Bottom - 1; J++) {
					if (level.Get(I, J).Matches(TileFlags.Passable)) {
						Points.Add(new Dot(I, J));
					}
				}
			}

			return Points;
		}

		public override int GetWidth() {
			return base.GetWidth() + 1;
		}

		public override int GetHeight() {
			return base.GetHeight() + 1;
		}

		public Dot GetCenter() {
			return new Dot(Left + GetWidth() / 2, Top + GetHeight() / 2);
		}

		public Dot GetTileCenter() {
			return new Dot(Left + (int) (GetWidth() / 2f), Top + (int) (GetHeight() / 2f));
		}
		
		public Rect GetCenterRect() {
			var x = (int) (Left + GetWidth() / 2f); 
			var y = (int) (Top + GetHeight() / 2f);
			return new Rect(x, y, x + 1, y + 1);
		}

		public virtual Rect GetConnectionSpace() {
			var C = GetDoorCenter();
			return new Rect(C.X, C.Y, C.X, C.Y);
		}

		protected Dot GetDoorCenter() {
			var DoorCenter = new Dot(0, 0);

			foreach (var Door in Connected.Values) {
				DoorCenter.X += Door.X;
				DoorCenter.Y += Door.Y;
			}

			var N = Connected.Count;
			var C = new Dot(DoorCenter.X / N, DoorCenter.Y / N);

			if (Random.Float() < DoorCenter.X % 1) {
				C.X++;
			}

			if (Random.Float() < DoorCenter.Y % 1) {
				C.Y++;
			}

			C.X = (int) MathUtils.Clamp(Left + 1, Right - 1, C.X);
			C.Y = (int) MathUtils.Clamp(Top + 1, Bottom - 1, C.Y);

			return C;
		}

		public void PaintTunnel(Level Level, Tile Floor, Rect space = null, bool Bold = false, bool shift = true, bool randomRect = true) {
			if (Connected.Count == 0) {
				Log.Error("Invalid connection room");

				return;
			}

			var C = space ?? GetConnectionSpace();
			var minLeft = C.Left;
			var maxRight = C.Right;
			var minTop = C.Top;
			var maxBottom = C.Bottom;

			foreach (var Door in Connected.Values) {
				var Start = new Dot(Door.X, Door.Y);
				Dot Mid;
				Dot End;

				if (shift) {
					if ((int) Start.X == Left) {
						Start.X++;
					} else if ((int) Start.Y == Top) {
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

				if (Door.X == Left || Door.X == Right) {
					Mid = new Dot(MathUtils.Clamp(Left + 1, Right - 1, Start.X + RightShift), MathUtils.Clamp(Top + 1, Bottom - 1, Start.Y));
					End = new Dot(MathUtils.Clamp(Left + 1, Right - 1, Mid.X), MathUtils.Clamp(Top + 1, Bottom - 1, Mid.Y + DownShift));
				} else {
					Mid = new Dot(MathUtils.Clamp(Left + 1, Right - 1, Start.X), MathUtils.Clamp(Top + 1, Bottom - 1, Start.Y + DownShift));
					End = new Dot(MathUtils.Clamp(Left + 1, Right - 1, Mid.X + RightShift), MathUtils.Clamp(Top + 1, Bottom - 1, Mid.Y));
				}

				Painter.DrawLine(Level, Start, Mid, Floor, Bold);
				Painter.DrawLine(Level, Mid, End, Floor, Bold);

				if (Random.Chance(10)) {
					Painter.Set(Level, End, Tiles.RandomFloor());
				}

				minLeft = Math.Min(minLeft, End.X);
				minTop = Math.Min(minTop, End.Y);
				maxRight = Math.Max(maxRight, End.X);
				maxBottom = Math.Max(maxBottom, End.Y);
			}

			if (randomRect && Random.Chance(20)) {
				if (Random.Chance()) {
					minLeft--;
				}
				
				if (Random.Chance()) {
					minTop--;
				}
				
				if (Random.Chance()) {
					maxRight++;
				}
				
				if (Random.Chance()) {
					maxBottom++;
				}
			}

			minLeft = MathUtils.Clamp(Left + 1, Right - 1, minLeft);
			minTop = MathUtils.Clamp(Top + 1, Bottom - 1, minTop);
			maxRight = MathUtils.Clamp(Left + 1, Right - 1, maxRight);
			maxBottom = MathUtils.Clamp(Top + 1, Bottom - 1, maxBottom);

			if (Random.Chance()) {
				Painter.Fill(Level, minLeft, minTop, maxRight - minLeft + 1, maxBottom - minTop + 1, Random.Chance() ? Floor : Tiles.RandomFloorOrSpike());
			} else {
				Painter.Rect(Level, minLeft, minTop, maxRight - minLeft + 1, maxBottom - minTop + 1, Random.Chance() ? Floor : Tiles.RandomFloorOrSpike());
			}
		}

		public virtual float WeightMob(MobInfo info, SpawnChance chance) {
			return chance.Chance;
		}

		public virtual void ModifyMobList(List<MobInfo> infos) {
			
		}

		public virtual bool ShouldSpawnMobs() {
			return false;
		}

		public static RoomType DecideType(RoomDef r, Type room) {
			if (typeof(TrapRoom).IsAssignableFrom(room)) {
				return RoomType.Trap;
			}

			if (typeof(BossRoom).IsAssignableFrom(room)) {
				return RoomType.Boss;
			}
			
			if (typeof(EntranceRoom).IsAssignableFrom(room)) {
				return RoomType.Entrance;
			}
			
			if (typeof(ExitRoom).IsAssignableFrom(room)) {
				return RoomType.Exit;
			}
			
			if (typeof(SecretRoom).IsAssignableFrom(room)) {
				return RoomType.Secret;
			}
			
			if (typeof(TreasureRoom).IsAssignableFrom(room)) {
				return RoomType.Treasure;
			}
			
			if (typeof(ShopRoom).IsAssignableFrom(room)) {
				return RoomType.Shop;
			}
			
			if (typeof(SpecialRoom).IsAssignableFrom(room)) {
				return RoomType.Special;
			}
			
			if (typeof(ConnectionRoom).IsAssignableFrom(room)) {
				return RoomType.Connection;
			}
			
			return RoomType.Regular;
		}

		public virtual void ModifyRoom(Room room) {
			
		}

		public virtual bool ConvertToEntity() {
			return true;
		}
	}
}