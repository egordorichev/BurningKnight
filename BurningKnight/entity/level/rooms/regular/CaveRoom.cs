using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;

namespace BurningKnight.entity.level.rooms.regular {
	public class CaveRoom : PatchRoom {
		protected override float GetSizeChance() {
			return {
				1, 3, 6
			}
			;
		}

		public override void Paint(Level Level) {
			var F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			var Fill = 0.05f + GetWidth() * GetHeight() / 512f;
			SetupPatch(Level, Fill, 20, true);
			CleanDiagonalEdges();

			for (var I = Top + 1; I < Bottom; I++)
			for (var J = Left + 1; J < Right; J++) {
				var In = XyToPatchCoords(J, I);

				if (!Patch[In]) Level.Set(J, I, F);
			}

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}
	}
}