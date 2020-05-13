using BurningKnight.assets.items;
using BurningKnight.entity.creature.npc;
using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.secret {
	public class SecretItemRoom : SecretRoom {
		public override void Paint(Level level) {
			var c = GetTileCenter();
			var stand = new ItemStand();
			
			level.Area.Add(stand);
			stand.Center = c * 16 + new Vector2(8, 16);
			stand.SetItem(Items.CreateAndAdd(Items.Generate(ItemPool.Secret), level.Area), null);

			var npc = new OldMan {
				RickRoll = true
			};
			
			level.Area.Add(npc);
			npc.BottomCenter = c * 16 + new Vector2(8, -8);
		}
	}
}