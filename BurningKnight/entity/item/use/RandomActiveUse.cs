using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.util;
using Lens.util.timer;

namespace BurningKnight.entity.item.use {
	public class RandomActiveUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			var id = Items.Generate(ItemType.Active);

			if (id == null) {
				Log.Error("Failed to chose an active item");
				return;
			}

			Log.Info($"Using active item {id} with d8");

			var it = Items.Create(id);
			item.GetComponent<ItemGraphicsComponent>().Sprite = CommonAse.Items.GetSlice(id);

			foreach (var u in it.Uses) {
				u.Item = it;
				u.Use(entity, it);
			}
			
			Timer.Add(() => {
				item.GetComponent<ItemGraphicsComponent>().Sprite = CommonAse.Items.GetSlice(item.Id);
			}, 2);
		}
	}
}