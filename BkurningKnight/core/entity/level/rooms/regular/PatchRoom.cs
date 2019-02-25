using BurningKnight.core.entity.level.features;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class PatchRoom : RegularRoom {
		protected bool[] Patch;

		protected Void SetupPatch(Level Level, float Fill, int Clustering, bool EnsurePath) {
			if (EnsurePath) {
				PathFinder.SetMapSize(this.GetWidth() - 2, this.GetHeight() - 2);
				bool Valid = false;

				for (int J = 0; J < 100; J++) {
					Patch = Patch.Generate(this.GetWidth() - 2, this.GetHeight() - 2, Fill, Clustering);
					int StartPoint = 0;

					foreach (LDoor Door in Connected.Values()) {
						if (Door.X == Left) {
							StartPoint = XyToPatchCoords((int) Door.X + 1, (int) Door.Y);
							Patch[XyToPatchCoords((int) Door.X + 1, (int) Door.Y)] = false;
							Patch[XyToPatchCoords((int) Door.X + 2, (int) Door.Y)] = false;
						} else if (Door.X == Right) {
							StartPoint = XyToPatchCoords((int) Door.X - 1, (int) Door.Y);
							Patch[XyToPatchCoords((int) Door.X - 1, (int) Door.Y)] = false;
							Patch[XyToPatchCoords((int) Door.X - 2, (int) Door.Y)] = false;
						} else if (Door.Y == Top) {
							StartPoint = XyToPatchCoords((int) Door.X, (int) Door.Y + 1);
							Patch[XyToPatchCoords((int) Door.X, (int) Door.Y + 1)] = false;
							Patch[XyToPatchCoords((int) Door.X, (int) Door.Y + 2)] = false;
						} else if (Door.Y == Bottom) {
							StartPoint = XyToPatchCoords((int) Door.X, (int) Door.Y - 1);
							Patch[XyToPatchCoords((int) Door.X, (int) Door.Y - 1)] = false;
							Patch[XyToPatchCoords((int) Door.X, (int) Door.Y - 2)] = false;
						} 
					}

					PathFinder.BuildDistanceMap(StartPoint, BArray.Not(Patch, null));
					Valid = true;

					for (int I = 0; I < Patch.Length; I++) {
						if (!Patch[I] && PathFinder.Distance[I] == Integer.MAX_VALUE) {
							Valid = false;

							break;
						} 
					}

					if (Valid) {
						break;
					} 
				}

				if (!Valid) {
					SetupPatch(Level, Fill / 2, Clustering, EnsurePath);
				} 

				PathFinder.SetMapSize(Level.GetWidth(), Level.GetHeight());
			} else {
				Patch = Patch.Generate(this.GetWidth() - 2, this.GetHeight() - 2, Fill, Clustering);
			}

		}

		protected Void CleanDiagonalEdges() {
			if (Patch == null) return;


			int PWidth = this.GetWidth() - 2;

			for (int I = 0; I < Patch.Length - PWidth; I++) {
				if (!Patch[I]) continue;


				if (I % PWidth != 0) {
					if (Patch[I - 1 + PWidth] && !(Patch[I - 1] || Patch[I + PWidth])) {
						Patch[I - 1 + PWidth] = false;
					} 
				} 

				if ((I + 1) % PWidth != 0) {
					if (Patch[I + 1 + PWidth] && !(Patch[I + 1] || Patch[I + PWidth])) {
						Patch[I + 1 + PWidth] = false;
					} 
				} 
			}
		}

		protected int XyToPatchCoords(int X, int Y) {
			return (X - Left - 1) + ((Y - Top - 1) * (this.GetWidth() - 2));
		}
	}
}
