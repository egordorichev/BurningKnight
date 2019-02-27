using BurningKnight.entity.level.rooms.regular;
using BurningKnight.state;

namespace BurningKnight.entity.pool.room {
	public class RegularRoomPool : Pool<RegularRoomDef> {
		public static RegularRoomPool Instance = new RegularRoomPool();

		public RegularRoomPool() {
			Add(typeof(CircleRoomDef), 3);
			Add(typeof(GardenRoomDef), 1f);
			Add(typeof(MazeFloorRoomDef), 0.3f);
			Add(typeof(CaveRoomDef), 1f);
			Add(typeof(RectFloorRoomDef), 0.3f);
			Add(typeof(CollumnRoomDef), 1f);
			Add(typeof(CollumnsRoomDef), 1);
			Add(typeof(LavaLakeRoomDef), 1);
			Add(typeof(CavyChasmRoomDef), 2f);
			Add(typeof(SideChasmsRoomDef), 1f);
			Add(typeof(HalfRoomDefChasm), 1f);
			Add(typeof(CrossRoomDef), 2f);
			Add(typeof(CenterWallRoomDef), 1f);
			Add(typeof(SmallAdditionRoomDef), 1f);
			Add(typeof(PrisonRoomDef), 1f);
			Add(typeof(PadRoomDef), 2f);
			Add(typeof(RombRoomDef), 2f);
			Add(typeof(FilledRombRoomDef), 3f);
			Add(typeof(CornerRoomDef), 1f);
			Add(typeof(MissingCornerRoomDef), 3f);
			Add(typeof(DoubleCornerRoomDef), 3f);
			Add(typeof(StatueRoomDef), 2);

			if (Run.Id != 0) {
				Add(typeof(RollingSpikeRoomDef), 3);
				Add(typeof(TurretRoomDef), 1);
				Add(typeof(FourSideTurretRoomDef), 1);
				Add(typeof(RotatingTurretRoomDef), 1);
				Add(typeof(CenterStructRoomDef), 2);
				Add(typeof(BigHoleRoomDef), 2f);
				Add(typeof(CRoomDef), 2f);
			}
		}
	}
}