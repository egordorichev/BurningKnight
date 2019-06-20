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
using VelcroPhysics.Utilities;
using MathUtils = Lens.util.MathUtils;

namespace BurningKnight.ui {
	public class UiInventory : UiEntity {
		private TextureRegion itemSlot;
		private TextureRegion useSlot;
		private TextureRegion bomb;
		private TextureRegion key;
		private TextureRegion coin;

		
		private TextureRegion activeSide;
		private TextureRegion activeBorder;
		private TextureRegion activeEmpty;
		private TextureRegion activeFull;
		private TextureRegion activeDouble;
		
		public static TextureRegion Heart;
		public static TextureRegion HalfHeart;
		public static TextureRegion HeartBackground;
		private TextureRegion changedHeartBackground;
		private static TextureRegion halfHeartBackground;
		private TextureRegion changedHalfHeartBackground;
		
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

			activeSide = anim.GetSlice("active_side");
			activeBorder = anim.GetSlice("active_border");
			activeEmpty = anim.GetSlice("active_empty");
			activeFull = anim.GetSlice("active_full");
			activeDouble = anim.GetSlice("active_double");
			
			Heart = anim.GetSlice("heart");
			HalfHeart = anim.GetSlice("half_heart");
			HeartBackground = anim.GetSlice("heart_bg");
			changedHeartBackground = anim.GetSlice("heart_hurt_bg");
			halfHeartBackground = anim.GetSlice("half_heart_bg");
			changedHalfHeartBackground = anim.GetSlice("half_heart_hurt");
			
			iron = anim.GetSlice("iron_heart");
			halfIron = anim.GetSlice("half_iron_heart");
			
			golden = anim.GetSlice("gold_heart");
			halfGolden = anim.GetSlice("half_gold_heart");

			if (player != null) {
				var component = player.GetComponent<ConsumablesComponent>();

				coins = component.Coins;
				keys = component.Keys;
				bombs = component.Bombs;

				var area = player.Area;

				Subscribe<ConsumableAddedEvent>(area);
				Subscribe<ConsumableRemovedEvent>(area);
				Subscribe<ItemUsedEvent>(area);
				Subscribe<ItemAddedEvent>(area);
			}
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
						TweenActive();
					}
					
					break;
				}

				case ItemAddedEvent iae: {
					if (iae.Who == player) {
						if (iae.Item.Type == ItemType.Lamp) {
							hpZero = 0;
							Tween.To(this, new {hpZero = 1}, 0.6f, Ease.QuadInOut).Delay = 1f;
						} else if (iae.Item.Type == ItemType.Active) {
							if (activePosition <= 0f) {
								Tween.To(0, -1, x => activePosition = x, 0.6f, Ease.BackOut);
							} else {
								TweenActive();
							}
						}
					}
					
					break;
				}
			}

			return base.HandleEvent(e);
		}

		private float activePosition = -1f;
		
		private void TweenActive() {
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
		
		public override void Render() {
			if (player == null || player.Done) {
				Done = true;
				return;
			}

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
			if (activePosition <= -0.99f) {
				return;
			}
			
			var component = player.GetComponent<ActiveItemComponent>();
			var item = component.Item;

			var v = activePosition * (itemSlot.Width + 10);
			var pos = new Vector2(useSlot.Center.X + 2 + v, useSlot.Center.Y + Display.UiHeight - itemSlot.Source.Height - 2);
			
			Graphics.Render(itemSlot, pos, 0, itemSlot.Center, activeScale);

			if (item != null) {
				var region = item.Region;
				var timed = item.UseTime < 0;
				var chargeMax = Math.Abs(item.UseTime);
				var charge = timed ? chargeMax - item.Delay : (float) Math.Floor(chargeMax - item.Delay);

				if (region != null) {
					var p = new Vector2(
						region.Center.X + 2 + (itemSlot.Source.Width - region.Source.Width) / 2f + v,
						region.Center.Y + Display.UiHeight - itemSlot.Source.Height - 2 +
						(itemSlot.Source.Height - region.Source.Height) / 2f);
					
					if (Math.Abs(charge - chargeMax) < 0.01f) {
						var shader = Shaders.Entity;
						Shaders.Begin(shader);

						shader.Parameters["flash"].SetValue(1f);
						shader.Parameters["flashReplace"].SetValue(1f);
						shader.Parameters["flashColor"].SetValue(ColorUtils.White);

						foreach (var d in MathUtils.Directions){
							Graphics.Render(region, p + d, 0, region.Center, itemScale);
						}
						
						Shaders.End();
					}
			
					Graphics.Render(region, p, 0, region.Center, itemScale);
				}
				
				pos.X += itemSlot.Width / 2f;
				pos.Y -= itemSlot.Height / 2f;

				var h = itemSlot.Height;
				var cellH = timed ? h : (float) Math.Floor(h / chargeMax);
				var barH = timed ? cellH : cellH * chargeMax;
			
				Graphics.Render(activeSide, pos);
				Graphics.Render(activeSide, pos + new Vector2(0, itemSlot.Height - 1));
			
				Graphics.Render(activeBorder, pos + new Vector2(3, 1), 0, Vector2.Zero, new Vector2(1, barH - 2));

				if (charge < chargeMax) {
					Graphics.Render(activeEmpty, pos + new Vector2(0, 1), 0, Vector2.Zero, new Vector2(1, barH - 2));
				}

				if (charge > 0) {
					if (timed) {
						var hh = charge / chargeMax * (cellH - 2);
						Graphics.Render(activeFull, pos + new Vector2(0, barH - hh - 1), 0, Vector2.Zero,
							new Vector2(1, hh));
					} else {
						for (var i = 0; i < charge; i++) {
							Graphics.Render(activeFull, pos + new Vector2(0, barH - (i + 1) * cellH), 0, Vector2.Zero,
								new Vector2(1, cellH - 1));
						}
					}
				}

				if (!timed) {
					for (var i = 0; i < chargeMax - 1; i++) {
						Graphics.Render(activeSide, pos + new Vector2(0, barH - 1 - (i + 1) * cellH));
					}
				}
			}
		}

		private float hpZero = 1f;

		private Vector2 GetHeartPosition(bool pad, int i, bool bg = false) {
			var component = player.GetComponent<HealthComponent>();
			var red = component.Health - 1;
			
			var from = Camera.Instance.CameraToUi(player.Center);

			var angle = (i - Math.Floor(red / 2f) + 1f) * Math.PI / Math.Max(component.MaxHealth / 2, 8) - Math.PI / 2;
			const float distance = 24f;
		
			var x = from.X + (float) Math.Cos(angle) * distance - Heart.Width / 2f;
			var y = from.Y + (float) Math.Sin(angle) * distance - Heart.Height / 2f;

			var d = 0;
			var it = player.GetComponent<ActiveItemComponent>().Item;

			if (pad && it != null && Math.Abs(it.UseTime) > 0.01f) {
				d = 4;
			}
			
			var a = 0;
			
			if (it == null && (coins != 0 || bombs != 0 || keys != 0)) {
				a += 24;
			}
			
			return new Vector2((bg ? 0 : 1) + (pad ? (2 + (2 + itemSlot.Source.Width + d) * (activePosition + 1) + a) : 6) + (int) (i % HeartsComponent.PerRow * 5.5f),
				Display.UiHeight - (bg ? 11 : 10) - (i / HeartsComponent.PerRow) * 10 - (pad ? 0 : 4)
				+ (float) Math.Cos(i / 8f * Math.PI + Engine.Time * 12) * 0.5f * Math.Max(0, (float) (Math.Cos(Engine.Time * 0.25f) - 0.9f) * 10f)) * hpZero 
			       + new Vector2((bg ? -1 : 0) + x, (bg ? -1 : 0) + y) * (1 - hpZero);
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
				var region = hurt ? changedHeartBackground : HeartBackground;

				if (i == maxRed - 1) {
					region = hurt ? changedHalfHeartBackground : halfHeartBackground;
				}
				
				Graphics.Render(region, GetHeartPosition(pad, i, true));

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
			var bottomY = Display.UiHeight - itemSlot.Source.Height * (activePosition + 1) - 14;

			if (coins > 0) {
				Graphics.Render(coin, new Vector2(4 + coin.Center.X, bottomY + 1 + coin.Center.Y), 0, coin.Center, coinScale);
				Graphics.Print($"{coins}", Font.Small, new Vector2(14, bottomY - 1));
				bottomY -= 12;
			}

			if (keys > 0) {
				Graphics.Render(key, new Vector2(2 + key.Center.X, bottomY + key.Center.Y), 0, key.Center, keyScale);
				Graphics.Print($"{keys}", Font.Small, new Vector2(14, bottomY - 1));
				bottomY -= bomb.Source.Height + 2;
			}

			if (bombs > 0) {
				// Bomb sprite has bigger height
				Graphics.Render(bomb, new Vector2(2 + bomb.Center.X, bottomY + bomb.Center.Y), 0,
					bomb.Center, bombScale);

				Graphics.Print($"{bombs}", Font.Small, new Vector2(14, bottomY - 1));
			}
		}
	}
}