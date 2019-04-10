using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiInventory : UiEntity {
		private TextureRegion itemSlot;
		private TextureRegion useSlot;
		private TextureRegion bomb;
		private TextureRegion key;
		private TextureRegion coin;

		public static TextureRegion Heart;
		public static TextureRegion HalfHeart;
		public static TextureRegion HeartBackground;
		private TextureRegion changedHeartBackground;
		
		private TextureRegion iron;
		private TextureRegion halfIron;
		
		private TextureRegion golden;
		private TextureRegion halfGolden;
		
		private Player player;

		private Vector2 activeScale = new Vector2(1);
		private Vector2 itemScale = new Vector2(1);

		private int coins;
		private int keys;
		private int bombs;
		
		private Vector2 coinScale = new Vector2(1);
		private Vector2 keyScale = new Vector2(1);
		private Vector2 bombScale = new Vector2(1);
		
		public UiInventory(Player player) {
			this.player = player;	
		}

		public override void Init() {
			base.Init();

			var anim = Animations.Get("ui");

			itemSlot = anim.GetSlice("item_slot");
			useSlot = new TextureRegion();
			useSlot.Set(itemSlot);
			
			bomb = anim.GetSlice("bomb");
			key = anim.GetSlice("key");
			coin = anim.GetSlice("coin");
			
			Heart = anim.GetSlice("heart");
			HalfHeart = anim.GetSlice("half_heart");
			HeartBackground = anim.GetSlice("heart_bg");
			changedHeartBackground = anim.GetSlice("heart_hurt_bg");
			
			iron = anim.GetSlice("iron_heart");
			halfIron = anim.GetSlice("half_iron_heart");
			
			golden = anim.GetSlice("gold_heart");
			halfGolden = anim.GetSlice("half_gold_heart");
		
			var component = player.GetComponent<ConsumablesComponent>();

			coins = component.Coins;
			keys = component.Keys;
			bombs = component.Bombs;

			var area = player.Area;
			
			Subscribe<ConsumableAddedEvent>(area);
			Subscribe<ConsumableRemovedEvent>(area);
			Subscribe<ItemUsedEvent>(area);
		}

		private void AnimateConsumableChange(int amount, int now, ItemType type) {
			if (type == ItemType.Bomb) {
				if (Math.Abs(amount) == 1) {
					bombs = now;
				} else {
					Tween.To(this, new {bombs = now}, 0.05f * Math.Abs(amount));					
				}
				
				Tween.To(0.3f, bombScale.X, x => bombScale.X = x, 0.1f).OnEnd = () =>
					Tween.To(1f, bombScale.X, x => bombScale.X = x, 0.2f);

				Tween.To(2f, bombScale.Y, x => bombScale.Y = x, 0.1f).OnEnd = () =>
					Tween.To(1f, bombScale.Y, x => bombScale.Y = x, 0.2f);
			} else if (type == ItemType.Key) {
				if (Math.Abs(amount) == 1) {
					keys = now;
				} else {
					Tween.To(this, new {keys = now}, 0.05f * Math.Abs(amount));					
				}
				
				Tween.To(0.3f, keyScale.X, x => keyScale.X = x, 0.1f).OnEnd = () =>
					Tween.To(1f, keyScale.X, x => keyScale.X = x, 0.2f);

				Tween.To(2f, keyScale.Y, x => keyScale.Y = x, 0.1f).OnEnd = () =>
					Tween.To(1f, keyScale.Y, x => keyScale.Y = x, 0.2f);
			} else if (type == ItemType.Coin) {
				if (Math.Abs(amount) == 1) {
					coins = now;
				} else {
					Tween.To(this, new {coins = now}, 0.05f * Math.Abs(amount));					
				}
				
				Tween.To(2f, coinScale.Y, x => coinScale.Y = x, 0.1f).OnEnd = () =>
					Tween.To(1f, coinScale.Y, x => coinScale.Y = x, 0.2f);
			}
		}

		public override bool HandleEvent(Event e) {
			switch (e) {
				case ConsumableAddedEvent add: {
					AnimateConsumableChange(add.Amount, add.TotalNow, add.Type);
					break;
				}

				case ConsumableRemovedEvent rem: {
					AnimateConsumableChange(rem.Amount, rem.TotalNow, rem.Type);
					break;
				}

				case ItemUsedEvent item: {
					if (player.GetComponent<ActiveItemComponent>().Item == item.Item) {
						Tween.To(0.6f, activeScale.X, x => activeScale.X = x, 0.1f).OnEnd = () =>
							Tween.To(1.5f, activeScale.X, x => activeScale.X = x, 0.1f).OnEnd = () =>
								Tween.To(1f, activeScale.X, x => activeScale.X = x, 0.2f);
						
						Tween.To(1.5f, activeScale.Y, x => activeScale.Y = x, 0.1f).OnEnd = () =>
							Tween.To(0.6f, activeScale.Y, x => activeScale.Y = x, 0.1f).OnEnd = () =>
								Tween.To(1f, activeScale.Y, x => activeScale.Y = x, 0.2f);
						
						Tween.To(0.3f, itemScale.X, x => itemScale.X = x, 0.1f).OnEnd = () =>
							Tween.To(1f, itemScale.X, x => itemScale.X = x, 0.2f);
						
						Tween.To(2f, itemScale.Y, x => itemScale.Y = x, 0.1f).OnEnd = () =>
							Tween.To(1f, itemScale.Y, x => itemScale.Y = x, 0.2f);
					}
					
					break;
				}
			}

			return base.HandleEvent(e);
		}

		public override void Render() {
			if (player.GetComponent<HealthComponent>().Dead) {
				return;
			}

			var show = Run.Depth > 0;

			if (show) {
				RenderActiveItem();
				RenderConsumables();
			}

			RenderHealthBar(show);
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
				
				var region = item.Region;

				if (region != null) {
					Graphics.Render(region, new Vector2(
						region.Center.X + 2 + (itemSlot.Source.Width - region.Source.Width) / 2f,
						region.Center.Y + Display.UiHeight - itemSlot.Source.Height - 2 + (itemSlot.Source.Height - region.Source.Height) / 2f
					), 0, region.Center, itemScale);
				}
			}
		}

		private Vector2 GetHeartPosition(bool pad, int i, bool bg = false) {
			// todo: make it wave only sometimes (like in mc)
			return new Vector2((bg ? 0 : 1) + (pad ? (4 + itemSlot.Source.Width) : 2) + (int) (i % HeartsComponent.PerRow * 5.5f),
				Display.UiHeight - (bg ? 11 : 10) - (i / HeartsComponent.PerRow) * 10 
				+ (float) Math.Cos(i / 8f * Math.PI + Engine.Time * 12) * 0.5f);
		}

		private void RenderHealthBar(bool pad) {
			var red = player.GetComponent<HealthComponent>();
			var totalRed = red.Health - 1; // -1 accounts for hidden "not lamp hp"
			var maxRed = red.MaxHealth - 1;
			
			var other = player.GetComponent<HeartsComponent>();
			var totalIron = other.IronHalfs;			
			var totalGolden = other.GoldenHalfs;
		
			var hurt = red.InvincibilityTimer > 0;

			int i = 0;
			
			for (; i < maxRed; i += 2) {
				Graphics.Render(hurt ? changedHeartBackground : HeartBackground, GetHeartPosition(pad, i, true));

				if (i < totalRed) {
					Graphics.Render(i == totalRed - 1 ? HalfHeart : Heart, GetHeartPosition(pad, i));					
				}
			}

			var ironI = totalIron + maxRed;
			var maxIron = ironI + totalIron % 2;
			
			for (; i < maxIron; i += 2) {
				Graphics.Render(hurt ? changedHeartBackground : HeartBackground, GetHeartPosition(pad, i, true));
				Graphics.Render(i == ironI - 1 ? halfIron : iron, GetHeartPosition(pad, i));					
			}

			var goldenI = totalGolden + maxIron;
			var maxGold = goldenI + totalGolden % 2;
			
			for (; i < maxGold; i += 2) {
				Graphics.Render(hurt ? changedHeartBackground : HeartBackground, GetHeartPosition(pad, i, true));
				Graphics.Render(i == goldenI - 1 ? halfGolden : golden, GetHeartPosition(pad, i));					
			}
		}

		private void RenderConsumables() {
			var bottomY = Display.UiHeight - itemSlot.Source.Height - 14;

			Graphics.Render(coin, new Vector2(4 + coin.Center.X, bottomY + 1 + coin.Center.Y), 0, coin.Center, coinScale);
			Graphics.Print($"{coins}", Font.Small, new Vector2(14, bottomY - 1));
			
			bottomY -= 12;
			Graphics.Render(key, new Vector2(2 + key.Center.X, bottomY + key.Center.Y), 0, key.Center, keyScale);			
			Graphics.Print($"{keys}", Font.Small, new Vector2(14, bottomY - 1));

			// Bomb sprite has bigger height
			Graphics.Render(bomb, new Vector2(2 + bomb.Center.X, bottomY - bomb.Source.Height - 2 + bomb.Center.Y), 0, bomb.Center, bombScale);
			Graphics.Print($"{bombs}", Font.Small, new Vector2(14, bottomY - 10 - 3));
		}
	}
}