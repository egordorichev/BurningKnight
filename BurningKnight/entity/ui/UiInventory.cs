using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Microsoft.Xna.Framework;

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
		
		private TextureRegion iron;
		private TextureRegion halfIron;
		
		private TextureRegion golden;
		private TextureRegion halfGolden;
		
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
			
			iron = anim.GetSlice("iron_heart");
			halfIron = anim.GetSlice("half_iron_heart");
			
			golden = anim.GetSlice("gold_heart");
			halfGolden = anim.GetSlice("half_gold_heart");
		}

		public override void Render() {
			RenderActiveItem();
			RenderHealthBar();
			RenderConsumables();
		}

		private void RenderActiveItem() {
			var component = player.GetComponent<ActiveItemComponent>();
			Graphics.Render(itemSlot, new Vector2(2, Display.UiHeight - itemSlot.Source.Height - 2));
			
			if (component.Item != null) {
				var region = component.Item.GetComponent<SliceComponent>().Sprite;
				Graphics.Render(region, new Vector2(
					2 + (itemSlot.Source.Width - region.Source.Width) / 2, 
					Display.UiHeight - itemSlot.Source.Height - 2 + (itemSlot.Source.Height - region.Source.Height) / 2)
				);
			}
		}

		private void RenderHealthBar() {
			var red = player.GetComponent<HealthComponent>();
			var totalRed = red.Health;
			var maxRed = red.MaxHealth;
			
			var other = player.GetComponent<HeartsComponent>();
			var totalIron = other.IronHalfs;			
			var totalGolden = other.GoldenHalfs;
			
			var hurt = red.InvincibilityTimer > 0;

			int i = 0;
			
			for (; i < maxRed; i += 2) {
				Graphics.Render(hurt ? changedHeartBackground : heartBackground, new Vector2(4 + itemSlot.Source.Width + (int) (i * 5.5f), Display.UiHeight - 11));

				if (i < totalRed) {
					Graphics.Render(i == totalRed - 1 ? halfHeart : heart, new Vector2(5 + itemSlot.Source.Width + (int) (i * 5.5f), Display.UiHeight - 10));					
				}
			}

			var ironI = totalIron + maxRed;
			var maxIron = ironI + totalIron % 2;
			
			for (; i < maxIron; i += 2) {
				Graphics.Render(hurt ? changedHeartBackground : heartBackground, new Vector2(4 + itemSlot.Source.Width + (int) (i * 5.5f), Display.UiHeight - 11));
				Graphics.Render(i == ironI - 1 ? halfIron : iron, new Vector2(5 + itemSlot.Source.Width + (int) (i * 5.5f), Display.UiHeight - 10));					
			}

			var goldenI = totalGolden + maxIron + maxRed - 2;
			var maxGold = goldenI + totalGolden % 2;
			
			for (; i < maxGold; i += 2) {
				Graphics.Render(hurt ? changedHeartBackground : heartBackground, new Vector2(4 + itemSlot.Source.Width + (int) (i * 5.5f), Display.UiHeight - 11));
				Graphics.Render(i == goldenI - 1 ? halfGolden : golden, new Vector2(5 + itemSlot.Source.Width + (int) (i * 5.5f), Display.UiHeight - 10));					
			}
		}

		private void RenderConsumables() {
			var component = player.GetComponent<ConsumablesComponent>();
			var bottomY = Display.UiHeight - itemSlot.Source.Height - 4 - coin.Source.Height;

			Graphics.Render(coin, new Vector2(2, bottomY));
			Graphics.Print($"{component.Coins}", Font.Small, new Vector2(14, bottomY - 1));
			
			bottomY -= key.Source.Height + 2;
			Graphics.Render(key, new Vector2(2, bottomY));			
			Graphics.Print($"{component.Keys}", Font.Small, new Vector2(14, bottomY - 1));

			// Bomb sprite has bigger height
			Graphics.Render(bomb, new Vector2(2, bottomY - bomb.Source.Height - 2));
			Graphics.Print($"{component.Bombs}", Font.Small, new Vector2(14, bottomY - 10 - 3));
		}
	}
}