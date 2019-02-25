using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class LavaLakeRoom : PatchRoom {
		protected override float GetSizeChance() {
			return { 1, 3, 6 };
		}

		public override Void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			bool Chasm = Random.Chance(50);
			Painter.Fill(Level, this, 1, Chasm ? Terrain.CHASM : Terrain.LAVA);
			float Fill = 0.4f;
			SetupPatch(Level, Fill, 5, true);
			CleanDiagonalEdges();
			byte Floor = Chasm ? Terrain.RandomFloor() : Terrain.DIRT;

			for (int I = Top + 1; I < Bottom; I++) {
				for (int J = Left + 1; J < Right; J++) {
					int In = XyToPatchCoords(J, I);

					if (!this.Patch[In]) {
						Level.Set(J, I, Floor);
					} 
				}
			}

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}
	}
}
