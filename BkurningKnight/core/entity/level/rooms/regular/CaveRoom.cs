using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class CaveRoom : PatchRoom {
		protected override float GetSizeChance() {
			return { 1, 3, 6 };
		}

		public override Void Paint(Level Level) {
			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			float Fill = 0.05f + (this.GetWidth() * this.GetHeight()) / 512f;
			SetupPatch(Level, Fill, 20, true);
			CleanDiagonalEdges();

			for (int I = Top + 1; I < Bottom; I++) {
				for (int J = Left + 1; J < Right; J++) {
					int In = XyToPatchCoords(J, I);

					if (!this.Patch[In]) {
						Level.Set(J, I, F);
					} 
				}
			}

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}
	}
}
