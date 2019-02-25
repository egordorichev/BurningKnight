namespace BurningKnight.entity.level.builders {
	public class SingleRoomBuilder : Builder {
		public override List Build<Room>(List Init) {
			Room Room = Init.Get(0);
			Room.SetSize();
			Room.SetPos(0, 0);

			return Init;
		}
	}
}