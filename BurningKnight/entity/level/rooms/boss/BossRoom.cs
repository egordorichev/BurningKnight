using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.rooms.entrance;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.boss {
	public class BossRoomDef : EntranceRoomDef {
		protected bool AlwaysElipse;

		public override void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			var M = Random.NewInt(2, 4) + 1;

			if (Random.Chance(30)) {
				if (AlwaysElipse || Random.Chance(50)) {
					Painter.FillEllipse(Level, this, 1, Random.Chance(50) ? Terrain.LAVA : Terrain.CHASM);
					Painter.FillEllipse(Level, this, M, Terrain.RandomFloor());
				}
				else {
					Painter.Fill(Level, this, 1, Random.Chance(50) ? Terrain.LAVA : Terrain.CHASM);
					Painter.Fill(Level, this, M, Terrain.RandomFloor());
				}


				M += Random.NewInt(1, 3);
				PaintTunnels(Level, true);
			}
			else {
				if (AlwaysElipse || Random.Chance(50)) {
					Painter.FillEllipse(Level, this, 1, Terrain.RandomFloor());
					PaintTunnels(Level, true);
				}
				else {
					Painter.Fill(Level, this, 1, Terrain.RandomFloor());
				}
			}


			if (Random.Chance(90)) {
				if (!AlwaysElipse && Random.Chance(50))
					Painter.Fill(Level, this, M, Terrain.RandomFloor());
				else
					Painter.FillEllipse(Level, this, M, Terrain.RandomFloor());


				PaintTunnels(Level, false);

				if (Random.Chance(90)) {
					M += Random.NewInt(1, 3);

					if (!AlwaysElipse && Random.Chance(50))
						Painter.Fill(Level, this, M, Terrain.RandomFloor());
					else
						Painter.FillEllipse(Level, this, M, Terrain.RandomFloor());
				}
			}
			else if (Random.Chance(50)) {
				PaintTunnels(Level, false);
			}

			PaintTunnels(Level, true);

			if (Random.Chance(50)) {
				var N = Math.Min(GetWidth() / 2, GetHeight() / 2) - Random.NewInt(2, 6);

				if (Random.Chance(50))
					Painter.Fill(Level, this, N, Random.Chance(50) ? Terrain.FLOOR_D : Terrain.RandomFloorNotLast());
				else
					Painter.FillEllipse(Level, this, N, Random.Chance(50) ? Terrain.FLOOR_D : Terrain.RandomFloorNotLast());


				if (Random.Chance(50)) {
					N += 1;

					if (Random.Chance(50))
						Painter.Fill(Level, this, N, Terrain.RandomFloorNotLast());
					else
						Painter.FillEllipse(Level, this, N, Terrain.RandomFloorNotLast());
				}
			}

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.BOSS);
		}

		private void PaintTunnels(Level Level, bool Force) {
			if (Random.Chance(50) || Force) {
				if (Random.Chance(50)) {
					PaintTunnel(Level, Terrain.RandomFloor(), true);
					PaintTunnel(Level, Terrain.RandomFloorNotLast());
				}
				else {
					PaintTunnel(Level, Terrain.RandomFloor());
				}
			}
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}

		public override int GetMinWidth() {
			return 18 + 5;
		}

		public override int GetMinHeight() {
			return 18 + 5;
		}

		public override int GetMaxWidth() {
			return 36;
		}

		public override int GetMaxHeight() {
			return 36;
		}

		public override int GetMaxConnections(Connection Side) {
			return 1;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;

			return 0;
		}
	}
}