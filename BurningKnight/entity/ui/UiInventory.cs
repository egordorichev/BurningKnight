using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.input;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.ui {
	public class UiInventory : UiEntity {
		private TextureRegion itemSlot;
		private TextureRegion useSlot;
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

		private Vector activeScale = new Vector(1);
		private Vector itemScale = new Vector(1);
		
		public UiInventory(Player player) {
			this.player = player;

			var anim = Animations.Get("ui");

			itemSlot = anim.GetSlice("item_slot");
			useSlot = new TextureRegion();
			useSlot.Set(itemSlot);
			
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

		public override void Update(float dt) {
			base.Update(dt);

			if (Input.WasPressed(Controls.Active)) {
				Tween.To(activeScale, new {X = 0.6f, Y = 1.5f}, 0.1f).OnEnd = () => Tween.To(activeScale, new {X = 1.5f, Y = 0.6f}, 0.1f, Ease.QuadOut).OnEnd = () => Tween.To(activeScale, new {X = 1f, Y = 1f}, 0.2f, Ease.QuadOut);
				Tween.To(itemScale, new {X = 0.3f, Y = 2f}, 0.1f).OnEnd = () => Tween.To(itemScale, new {X = 1f, Y = 1f}, 0.2f);
			}
		}

		public override void Render() {
			RenderActiveItem();
			RenderHealthBar();
			RenderConsumables();
		}
		
		private void RenderActiveItem() {
			var component = player.GetComponent<ActiveItemComponent>();
			var item = component.Item;
			
			Graphics.Render(itemSlot, new Vector2(useSlot.Center.X + 2, useSlot.Center.Y + Display.UiHeight - itemSlot.Source.Height - 2), 0, itemSlot.Center, activeScale);

			if (item != null) {
				if (item.Delay > 0) {
					float progress = item.Delay / item.UseTime;

					useSlot.Source.Width = (int) Math.Ceiling(itemSlot.Source.Width * progress);

					Graphics.Color = new Color(0.5f, 0.5f, 0.5f, 1);
					Graphics.Render(useSlot, new Vector2(useSlot.Center.X + 2, useSlot.Center.Y + Display.UiHeight - itemSlot.Source.Height - 2), 0, useSlot.Center, activeScale);
					Graphics.Color = Color.White;
				}
				
				var region = item.GetComponent<SliceComponent>().Sprite;
				
				Graphics.Render(region, new Vector2(
					region.Center.X + 2 + (itemSlot.Source.Width - region.Source.Width) / 2f, 
					region.Center.Y + Display.UiHeight - itemSlot.Source.Height - 2 + (itemSlot.Source.Height - region.Source.Height) / 2f
				), 0, region.Center, itemScale);
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
			var bottomY = Display.UiHeight - itemSlot.Source.Height - 14;

			Graphics.Render(coin, new Vector2(4, bottomY + 1));
			Graphics.Print($"{component.Coins}", Font.Small, new Vector2(14, bottomY - 1));
			
			bottomY -= 12;
			Graphics.Render(key, new Vector2(2, bottomY));			
			Graphics.Print($"{component.Keys}", Font.Small, new Vector2(14, bottomY - 1));

			// Bomb sprite has bigger height
			Graphics.Render(bomb, new Vector2(2, bottomY - bomb.Source.Height - 2));
			Graphics.Print($"{component.Bombs}", Font.Small, new Vector2(14, bottomY - 10 - 3));
		}
	}
}