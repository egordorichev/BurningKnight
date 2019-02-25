using BurningKnight.entity.level.features;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class PatchRoom : RegularRoom {
		protected bool[] Patch;

		protected void SetupPatch(Level Level, float Fill, int Clustering, bool EnsurePath) {
			if (EnsurePath) {
				PathFinder.SetMapSize(GetWidth() - 2, GetHeight() - 2);
				var Valid = false;

				for (var J = 0; J < 100; J++) {
					Patch = Patch.Generate(GetWidth() - 2, GetHeight() - 2, Fill, Clustering);
					var StartPoint = 0;

					foreach (LDoor Door in Connected.Values())
						if (Door.X == Left) {
							StartPoint = XyToPatchCoords((int) Door.X + 1, (int) Door.Y);
							Patch[XyToPatchCoords((int) Door.X + 1, (int) Door.Y)] = false;
							Patch[XyToPatchCoords((int) Door.X + 2, (int) Door.Y)] = false;
						}
						else if (Door.X == Right) {
							StartPoint = XyToPatchCoords((int) Door.X - 1, (int) Door.Y);
							Patch[XyToPatchCoords((int) Door.X - 1, (int) Door.Y)] = false;
							Patch[XyToPatchCoords((int) Door.X - 2, (int) Door.Y)] = false;
						}
						else if (Door.Y == Top) {
							StartPoint = XyToPatchCoords((int) Door.X, (int) Door.Y + 1);
							Patch[XyToPatchCoords((int) Door.X, (int) Door.Y + 1)] = false;
							Patch[XyToPatchCoords((int) Door.X, (int) Door.Y + 2)] = false;
						}
						else if (Door.Y == Bottom) {
							StartPoint = XyToPatchCoords((int) Door.X, (int) Door.Y - 1);
							Patch[XyToPatchCoords((int) Door.X, (int) Door.Y - 1)] = false;
							Patch[XyToPatchCoords((int) Door.X, (int) Door.Y - 2)] = false;
						}

					PathFinder.BuildDistanceMap(StartPoint, BArray.Not(Patch, null));
					Valid = true;

					for (var I = 0; I < Patch.Length; I++)
						if (!Patch[I] && PathFinder.Distance[I] == Integer.MAX_VALUE) {
							Valid = false;

							break;
						}

					if (Valid) break;
				}

				if (!Valid) SetupPatch(Level, Fill / 2, Clustering, EnsurePath);

				PathFinder.SetMapSize(Level.GetWidth(), Level.GetHeight());
			}
			else {
				Patch = Patch.Generate(GetWidth() - 2, GetHeight() - 2, Fill, Clustering);
			}
		}

		protected void CleanDiagonalEdges() {
			if (Patch == null) return;


			var PWidth = GetWidth() - 2;

			for (var I = 0; I < Patch.Length - PWidth; I++) {
				if (!Patch[I]) continue;


				if (I % PWidth != 0)
					if (Patch[I - 1 + PWidth] && !(Patch[I - 1] || Patch[I + PWidth]))
						Patch[I - 1 + PWidth] = false;

				if ((I + 1) % PWidth != 0)
					if (Patch[I + 1 + PWidth] && !(Patch[I + 1] || Patch[I + PWidth]))
						Patch[I + 1 + PWidth] = false;
			}
		}

		protected int XyToPatchCoords(int X, int Y) {
			return X - Left - 1 + (Y - Top - 1) * (GetWidth() - 2);
		}
	}
}