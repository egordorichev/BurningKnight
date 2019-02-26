using BurningKnight.entity.level.rooms.item;

namespace BurningKnight.entity.pool.room {
	public class ItemRoomPool : Pool<ItemRoom> {
		public static ItemRoomPool Instance = new ItemRoomPool();

		public ItemRoomPool() {
			Add(BrokeLineItemRoom.GetType(), 1f);
		}
	}
}