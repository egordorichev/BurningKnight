using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.save;
using Lens.entity;

namespace BurningKnight.entity.item.stand {
	public class HatStand : EmeraldStand {
		protected override bool ApproveItem(ItemData item) {
			return item.Type == ItemType.Hat;
		}

		protected override void OnTake(Item item, Entity who) {
			base.OnTake(item, who);

			CheckHats();

			foreach (var i in Area.Tagged[Tags.Item].ToArray()) {
				if (i is GarderobeStand gs) {
					gs.UpdateItem();
				}
			}
		}

		public static void CheckHats() {
			var total = 0;
			var progress = 0;

			foreach (var i in Items.Datas.Values) {
				if (i.Type == ItemType.Hat) {
					total++;

					if (GlobalSave.IsTrue(i.Id)) {
						progress++;
					}
				}
			}

			Achievements.SetProgress("bk:fashion_matters", progress, total);
		}
	}
}