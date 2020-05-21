using BurningKnight.assets.items;
using BurningKnight.ui.editor;

namespace BurningKnight.entity.item {
	public class RandomItem : RoundItem {
		private static string GenerateId() {
			return Items.Generate(ItemPool.Treasure);
		}

		private void TryToOverrideId() {
			Id = GenerateId();
		}

		public override void PostInit() {
			TryToOverrideId();
			base.PostInit();
		}
	}
}