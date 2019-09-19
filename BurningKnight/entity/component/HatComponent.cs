using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.save;
using Lens.util.file;

namespace BurningKnight.entity.component {
	public class HatComponent : ItemComponent {
		public bool DoNotRender;
		
		public override void PostInit() {
			base.PostInit();
			
			if (Item == null) {
				var hat = GlobalSave.GetString("hat");

				if (hat != null) {
					Set(Items.CreateAndAdd(hat, Entity.Area), false);
				} else {
					Set(Items.CreateAndAdd("bk:no_hat", Entity.Area), false);
				}
			}
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Hat;
		}
		
		public override void Set(Item item, bool animate = true) {
			base.Set(item, animate);
			GlobalSave.Put("hat", item?.Id);

			if (item != null) {
				DoNotRender = item.Id == "bk:no_hat";
			}
		}
	}
}