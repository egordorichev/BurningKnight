using BurningKnight.entity.level.rooms.regular;
using BurningKnight.state;

namespace BurningKnight.entity.pool.room {
	public class RegularRoomPool : Pool<RegularRoom> {
		public static RegularRoomPool Instance = new RegularRoomPool();

		public RegularRoomPool() {
			Add(typeof(CircleRoom), 3);
			Add(typeof(GardenRoom), 1f);
			Add(typeof(MazeFloorRoom), 0.3f);
			Add(typeof(CaveRoom), 1f);
			Add(typeof(RectFloorRoom), 0.3f);
			Add(typeof(CollumnRoom), 1f);
			Add(typeof(CollumnsRoom), 1);
			Add(typeof(LavaLakeRoom), 1);
			Add(typeof(CavyChasmRoom), 2f);
			Add(typeof(SideChasmsRoom), 1f);
			Add(typeof(HalfRoomChasm), 1f);
			Add(typeof(CrossRoom), 2f);
			Add(typeof(CenterWallRoom), 1f);
			Add(typeof(SmallAdditionRoom), 1f);
			Add(typeof(PrisonRoom), 1f);
			Add(typeof(PadRoom), 2f);
			Add(typeof(RombRoom), 2f);
			Add(typeof(FilledRombRoom), 3f);
			Add(typeof(CornerRoom), 1f);
			Add(typeof(MissingCornerRoom), 3f);
			Add(typeof(DoubleCornerRoom), 3f);
			Add(typeof(StatueRoom), 2);

			if (Run.Id != 0) {
				Add(typeof(RollingSpikeRoom), 3);
				Add(typeof(TurretRoom), 1);
				Add(typeof(FourSideTurretRoom), 1);
				Add(typeof(RotatingTurretRoom), 1);
				Add(typeof(CenterStructRoom), 2);
				Add(typeof(BigHoleRoom), 2f);
				Add(typeof(CRoom), 2f);
			}
		}
	}
}