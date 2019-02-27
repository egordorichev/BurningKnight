using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class LavaLakeRoomDef : PatchRoomDef {
		protected override float GetSizeChance() {
			return {
				1, 3, 6
			}
			;
		}

		public override void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			var Chasm = Random.Chance(50);
			Painter.Fill(Level, this, 1, Chasm ? Terrain.CHASM : Terrain.LAVA);
			var Fill = 0.4f;
			SetupPatch(Level, Fill, 5, true);
			CleanDiagonalEdges();
			var Floor = Chasm ? Terrain.RandomFloor() : Terrain.DIRT;

			for (var I = Top + 1; I < Bottom; I++)
			for (var J = Left + 1; J < Right; J++) {
				var In = XyToPatchCoords(J, I);

				if (!Patch[In]) Level.Set(J, I, Floor);
			}

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}
	}
}