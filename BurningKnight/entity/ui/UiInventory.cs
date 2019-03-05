using BurningKnight.entity.creature.player;
using Lens.assets;
using Lens.graphics;

namespace BurningKnight.entity.ui {
	public class UiInventory : UiEntity {
		private TextureRegion itemSlot;
		private TextureRegion bomb;
		private TextureRegion key;
		private TextureRegion coin;
		
		private TextureRegion heart;
		private TextureRegion halfHeart;
		private TextureRegion heartBackground;
		private TextureRegion changedHeartBackground;
		
		private Player player;
		
		public UiInventory(Player player) {
			this.player = player;

			var anim = Animations.Get("ui");

			itemSlot = anim.GetSlice("item_slot");
			bomb = anim.GetSlice("bomb");
			key = anim.GetSlice("key");
			coin = anim.GetSlice("coin");
			
			heart = anim.GetSlice("heart");
			halfHeart = anim.GetSlice("half_heart");
			heartBackground = anim.GetSlice("heart_bg");
			changedHeartBackground = anim.GetSlice("heart_hurt_bg");
		}

		public override void Render() {
			
		}
	}
}