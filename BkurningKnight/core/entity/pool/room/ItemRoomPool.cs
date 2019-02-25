using BurningKnight.core.entity.level.rooms.item;

namespace BurningKnight.core.entity.pool.room {
	public class ItemRoomPool : Pool<ItemRoom>  {
		public static ItemRoomPool Instance = new ItemRoomPool();

		public ItemRoomPool() {
			Add(BrokeLineItemRoom.GetType(), 1f);
		}
	}
}
