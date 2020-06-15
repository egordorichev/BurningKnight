using BurningKnight.assets.achievements;
using BurningKnight.assets.items;
using BurningKnight.save;
using Lens.entity;
using Lens.util;

namespace BurningKnight.entity.item.stand {
	public class HatStand : EmeraldStand {
		protected override bool ApproveItem(ItemData item) {
			return item.Type == ItemType.Hat;
		}

		protected override void DoStuff() {
			base.DoStuff();
			
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
				if (i.Type == ItemType.Hat && i.Id != "bk:no_hat") {
					total++;

					if (GlobalSave.IsTrue(i.Id)) {
						progress++;
					}
				}
			}

			if (progress > 0) {
				Achievements.Unlock("bk:fancy_hat");
			}

			Log.Info($"Fashion matters progress: {progress}/{total}");
			Achievements.SetProgress("bk:fashion_matters2", progress, total);
		}
	}
}