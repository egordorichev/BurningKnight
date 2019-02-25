using BurningKnight.core.entity.level.rooms.regular;
using BurningKnight.core.entity.level.save;

namespace BurningKnight.core.entity.pool.room {
	public class RegularRoomPool : Pool<RegularRoom>  {
		public static RegularRoomPool Instance = new RegularRoomPool();

		public RegularRoomPool() {
			Add(CircleRoom.GetType(), 3);
			Add(GardenRoom.GetType(), 1f);
			Add(MazeFloorRoom.GetType(), 0.3f);
			Add(CaveRoom.GetType(), 1f);
			Add(RectFloorRoom.GetType(), 0.3f);
			Add(CollumnRoom.GetType(), 1f);
			Add(CollumnsRoom.GetType(), 1);
			Add(LavaLakeRoom.GetType(), 1);
			Add(CavyChasmRoom.GetType(), 2f);
			Add(SideChasmsRoom.GetType(), 1f);
			Add(HalfRoomChasm.GetType(), 1f);
			Add(CrossRoom.GetType(), 2f);
			Add(CenterWallRoom.GetType(), 1f);
			Add(SmallAdditionRoom.GetType(), 1f);
			Add(PrisonRoom.GetType(), 1f);
			Add(PadRoom.GetType(), 2f);
			Add(RombRoom.GetType(), 2f);
			Add(FilledRombRoom.GetType(), 3f);
			Add(CornerRoom.GetType(), 1f);
			Add(MissingCornerRoom.GetType(), 3f);
			Add(DoubleCornerRoom.GetType(), 3f);
			Add(StatueRoom.GetType(), 2);

			if (GameSave.RunId != 0) {
				Add(RollingSpikeRoom.GetType(), 3);
				Add(TurretRoom.GetType(), 1);
				Add(FourSideTurretRoom.GetType(), 1);
				Add(RotatingTurretRoom.GetType(), 1);
				Add(CenterStructRoom.GetType(), 2);
				Add(BigHoleRoom.GetType(), 2f);
				Add(CRoom.GetType(), 2f);
			} 
		}
	}
}
