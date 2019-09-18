using BurningKnight.assets.items;

namespace BurningKnight.entity.item.stand {
	public class ArtifactStand : EmeraldStand {
		protected override bool ApproveItem(ItemData item) {
			return item.Type == ItemType.Artifact;
		}
	}
}