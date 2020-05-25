using BurningKnight.assets.items;
using BurningKnight.ui.editor;
using Lens.util.file;

namespace BurningKnight.entity.item {
	public class RandomItem : RoundItem {
		public bool Prevent;
	
		private static string GenerateId() {
			return Items.Generate(ItemPool.Treasure);
		}

		private void TryToOverrideId() {
			if (Prevent) {
				return;
			}
			
			ConvertTo(GenerateId());
		}

		public override void PostInit() {
			TryToOverrideId();
			base.PostInit();
		}
	}
}