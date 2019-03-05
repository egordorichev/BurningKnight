using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens;
using Lens.assets;
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
			RenderActiveItem();
			RenderHealthBar();
			RenderConsumables();
		}

		private void RenderActiveItem() {
			var component = player.GetComponent<ActiveItemComponent>();
			Graphics.Render(itemSlot, new Vector2(2, Display.UiHeight - itemSlot.Source.Height - 2));
			
			if (component.Item != null) {
				// todo: render the item				
			}
		}

		private void RenderHealthBar() {
			var component = player.GetComponent<HealthComponent>();
			
			
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