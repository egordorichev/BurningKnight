using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.entity.item.use;
using BurningKnight.level.rooms;
using BurningKnight.state;
using BurningKnight.ui.str;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui.inventory {
	public class UiInventory : UiEntity {
		public TextureRegion ItemSlot;
		public TextureRegion UseSlot;
		
		private TextureRegion question;		
		private TextureRegion bomb;
		private TextureRegion key;
		private TextureRegion coin;
		private TextureRegion pointer;
		
		private UiString description;
		private UiItem lastItem;

		public static TextureRegion Heart;
		public static TextureRegion HalfHeart;
		public static TextureRegion HeartBackground;
		
		public static TextureRegion Mana;
		public static TextureRegion HalfMana;
		public static TextureRegion ManaBackground;
		public static TextureRegion ChangedManaBackground;

		private TextureRegion changedHeartBackground;
		private static TextureRegion halfHeartBackground;
		private TextureRegion changedHalfHeartBackground;
		
		public static TextureRegion ShieldBackground;
		private TextureRegion changedShieldBackground;
		private static TextureRegion halfShieldBackground;
		private TextureRegion changedHalfShieldBackground;
		
		public Player Player;

		private int coins;
		private int keys;
		private int bombs;
		
		private Vector2 coinScale = new Vector2(1);
		private Vector2 keyScale = new Vector2(1);
		private Vector2 bombScale = new Vector2(1);

		private List<UiItem> items = new List<UiItem>();
		private UiActiveItemSlot activeSlot;
		private UiWeaponSlot weaponSlot;
		private UiWeaponSlot activeWeaponSlot;

		public UiInventory(Player player) {
			Player = player;	
			activeSlot = new UiActiveItemSlot(this);

			if (player.HasComponent<WeaponComponent>()) {
				weaponSlot = new UiWeaponSlot(this);
			}

			activeWeaponSlot = new UiWeaponSlot(this) {
				Active = true
			};
		}

		public override void Init() {
			base.Init();

			description = new UiString(Font.Small);
			Area.Add(description);
			description.DisableRender = true;
			
			Area.Add(activeSlot);

			if (weaponSlot != null) {
				Area.Add(weaponSlot);
			}

			Area.Add(activeWeaponSlot);

			var anim = Animations.Get("ui");

			ItemSlot = anim.GetSlice("item_slot");
			UseSlot = new TextureRegion();
			UseSlot.Set(ItemSlot);
			
			question = anim.GetSlice("question");
			bomb = anim.GetSlice("bomb");
			key = anim.GetSlice("key");
			coin = anim.GetSlice("coin");
			pointer = anim.GetSlice("pointer");

			Heart = anim.GetSlice("heart");
			HalfHeart = anim.GetSlice("half_heart");
			HeartBackground = anim.GetSlice("heart_bg");
			changedHeartBackground = anim.GetSlice("heart_hurt_bg");
			halfHeartBackground = anim.GetSlice("half_heart_bg");
			changedHalfHeartBackground = anim.GetSlice("half_heart_hurt");
			
			Mana = anim.GetSlice("mana");
			HalfMana = anim.GetSlice("half_mana");
			ManaBackground = anim.GetSlice("mana_bg");
			ChangedManaBackground = anim.GetSlice("mana_hurt_bg");
			
			ShieldBackground = anim.GetSlice("shield_bg");
			changedShieldBackground = anim.GetSlice("shield_hurt");
			halfShieldBackground = anim.GetSlice("half_shield_bg");
			changedHalfShieldBackground = anim.GetSlice("half_shield_hurt");
			
			if (Player != null) {
				var component = Player.GetComponent<ConsumablesComponent>();

				coins = component.Coins;
				keys = component.Keys;
				bombs = component.Bombs;

				var area = Player.Area;

				Subscribe<ConsumableAddedEvent>(area);
				Subscribe<ConsumableRemovedEvent>(area);
				Subscribe<ItemUsedEvent>(area);
				Subscribe<ItemAddedEvent>(area);
				Subscribe<ItemRemovedEvent>(area);
				Subscribe<RerollItemsOnPlayerUse.RerolledEvent>(area);

				var inventory = Player.GetComponent<InventoryComponent>();

				foreach (var item in inventory.Items) {
					AddArtifact(item);
				}
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

				case RerollItemsOnPlayerUse.RerolledEvent re: {
					if (re.Entity == Player) {
						foreach (var i in items) {
							i.Done = true;
						}						
						
						items.Clear();
						var inventory = Player.GetComponent<InventoryComponent>();

						foreach (var item in inventory.Items) {
							AddArtifact(item);
						}
					}

					break;
				}
				
				case ItemAddedEvent iae: {
					if (iae.Who == Player) {
						var item = iae.Item;
								
						if (item.Type == ItemType.Artifact || item.Type == ItemType.Scourge) {
							AddArtifact(item);
						}
					}
					
					break;
				}

				case ItemRemovedEvent ire: {
					if (ire.Owner == Player) {
						RemoveArtifact(ire.Item);
					}

					break;
				}
			}

			return base.HandleEvent(e);
		}
		
		private void AddArtifact(Item item) {
			UiItem old = null;

			foreach (var i in items) {
				if (i.Id == item.Id) {
					old = i;
					break;
				}
			}

			if (old == null) {
				var x = Display.UiWidth - 8f;

				if (items.Count > 0) {
					x = items[items.Count - 1].X - 8;
				}
								
				old = new UiItem();
				old.Id = item.Id;
				old.Scourged = item.Scourged;
				Area.Add(old);
				items.Add(old);

				old.Right = x;
				old.Bottom = Display.UiHeight - 8f;
			} else {
				old.Count++;
			}
		}

		private void RemoveArtifact(Item item) {
			UiItem old = null;
			var j = 0;

			foreach (var i in items) {
				if (i.Id == item.Id) {
					old = i;
					break;
				}		

				j++;
			}

			if (old == null) {
				return;
			}

			if (old.Count > 1) {
				old.Count--;
				return;
			}

			items.Remove(old);
			old.Done = true;

			for (var i = j; i < items.Count; i++) {
				items[i].Right = items[i - 1].X - 8;
			}
		}
		
		public override void Render() {
			if (Player == null || Player.Done/* || (Run.Depth < 1 && Run.Depth != -2)*/) {
				Done = true;
				return;
			}
			
			/*if (Player.GetComponent<HealthComponent>().Dead) {
				return;
			}*/

			Entity target = null;
			var r = Player.GetComponent<RoomComponent>().Room;
			
			if (r != null && r.Type != RoomType.Connection && r.Tagged[Tags.MustBeKilled].Count == 1 && !(r.Tagged[Tags.MustBeKilled][0] is Boss)) {
				target = r.Tagged[Tags.MustBeKilled][0];
			}

			if (target != null && target is Creature c && c.GetComponent<HealthComponent>().Health >= 1f) {
				var d = Player.DistanceTo(target);

				if (d > 64) {
					var dd = d * 0.7f;
					var a = Player.AngleTo(target);
					var m = (float) Math.Cos(Engine.Time * 4f) * 6f + 6f;
					var v = (float) Math.Cos(Engine.Time * 5f) * 0.3f + 0.7f;

					var point = Player.Center + new Vector2((float) Math.Cos(a) * dd, (float) Math.Sin(a) * dd);
					var center = new Vector2(Display.UiWidth, Display.UiHeight) * 0.5f;
					
					point = Camera.Instance.CameraToUi(point);
					point.X = MathUtils.Clamp((float) -Math.Cos(a) * (center.X - 16) + center.X, (float) Math.Cos(a) * (center.X - 16) + center.X, point.X);
					point.Y = MathUtils.Clamp((float) -Math.Sin(a) * (center.Y - 16) + center.Y,  (float) Math.Sin(a) * (center.Y - 16) + center.Y, point.Y);
					point -= MathUtils.CreateVector(a, m);
					
					Graphics.Color = new Color(v, v, v, 1f - MathUtils.Clamp(0, 1, (80 - d) / 16f));
					Graphics.Render(pointer, point, a, pointer.Center);
					Graphics.Color = ColorUtils.WhiteColor;
				}
			}

			var show = Run.Depth > 0;
			var hasMana = Player.GetComponent<WeaponComponent>().Item?.Data?.WeaponType == WeaponType.Magic || Player.GetComponent<ActiveWeaponComponent>().Item?.Data?.WeaponType == WeaponType.Magic;

			RenderHealthBar(true);

			if (show && hasMana) {
				RenderMana();
			}

			if ((show || Run.Depth == -2) && Player != null) {
				RenderConsumables(hasMana);
			}

			if (show && Player != null) {
				if (UiItem.Hovered != null) {
					var item = UiItem.Hovered;

					if (lastItem != UiItem.Hovered) {
						lastItem = UiItem.Hovered;
						description.Label = item.Description;
						description.FinishTyping();
					}

					var x = MathUtils.Clamp(item.OnTop ? 40 : 4, Display.UiWidth - 6 - Math.Max(item.DescriptionSize.X, item.NameSize.X), item.Position.X);
					var y = item.OnTop ? MathUtils.Clamp(8 + item.NameSize.Y, Display.UiHeight - 6 - item.DescriptionSize.Y, item.Y) : 
						MathUtils.Clamp(4, Display.UiHeight - 6 - item.DescriptionSize.Y - item.NameSize.Y - 4, item.Y);

					Graphics.Color = new Color(1f, 1f, 1f, item.TextA);
					Graphics.Print(item.Name, Font.Small,  new Vector2(x, y - item.DescriptionSize.Y + 2));
					// Graphics.Print(item.Description, Font.Small, new Vector2(x, y));

					description.X = x;
					description.Y = y - 2;
					Graphics.Color = ColorUtils.WhiteColor;

					description.Tint.A = (byte) (item.TextA * 255f);
					description.RenderString();
				}
			}
		}

		private Vector2 GetHeartPosition(bool pad, int i, bool bg = false) {
			var d = 0;
			var it = Player.GetComponent<ActiveItemComponent>().Item;

			if (pad && it != null && Math.Abs(it.UseTime) > 0.01f) {
				d = 4;
			}
			
			return new Vector2(
				(bg ? 0 : 1) + (pad ? (4 + (4 + ItemSlot.Source.Width + d) * (activeSlot.ActivePosition + 1)) : 6) + 4 + (int) (i % HeartsComponent.PerRow * 5.5f),
				(bg ? 0 : 1) + (i / HeartsComponent.PerRow) * 10 + 11
				+ (float) Math.Cos(i / 8f * Math.PI + Engine.Time * 12) * 0.5f * Math.Max(0, (float) (Math.Cos(Engine.Time * 0.25f) - 0.9f) * 10f)
			);
		}

		private float lastRed;
		
		private void RenderHealthBar(bool pad) {
			var red = Player.GetComponent<HealthComponent>();
			var phases = red.Phases;

			if (Scourge.IsEnabled(Scourge.OfRisk)) {
				Graphics.Render(question, new Vector2(8, 11));

				if (phases > 0) {
					Graphics.Print($"x{phases}", Font.Small, new Vector2(8 + question.Width + 4, 12));
				}
				
				return;
			}
			
			var hearts = Player.GetComponent<HeartsComponent>();
			var totalRed = red.Health;

			if (lastRed > totalRed) {
				lastRed = totalRed;
			} else if (lastRed < totalRed) { 
				lastRed = Math.Min(totalRed, lastRed + Engine.Delta * 30);
			}

			var r = (int) lastRed;
			var maxRed = red.MaxHealth;
			var hurt = red.InvincibilityTimer > 0;
			var shields = hearts.ShieldHalfs;

			var n = r;
			var jn = maxRed;
			
			if (jn % 2 == 1) {
				jn++;
			}
			
			for (var i = 0; i < maxRed; i += 2) {
				var region = hurt ? changedHeartBackground : HeartBackground;

				if (i == maxRed - 1) {
					region = hurt ? changedHalfHeartBackground : halfHeartBackground;
				}
				
				Graphics.Render(region, GetHeartPosition(pad, i, true));
			}
			
			for (var i = jn; i < maxRed + shields; i += 2) {
				var region = hurt ? changedShieldBackground : ShieldBackground;

				if (i == maxRed + shields - 1) {
					region = hurt ? changedHalfShieldBackground : halfShieldBackground;
				}
				
				Graphics.Render(region, GetHeartPosition(pad, i, true));
			}

			for (var j = 0; j < n; j++) {
				var h = j % 2 == 0;
				Graphics.Render(h ? HalfHeart : Heart, GetHeartPosition(pad, j) + (h ? Vector2.Zero : new Vector2(-1, 0)));
			}
			
			if (phases > 0) {
				Graphics.Print($"x{phases}", Font.Small, GetHeartPosition(pad, Math.Min(8, maxRed + shields)) + new Vector2(4, -2));
			}
		}
		
		private float lastMana;
		private float changedTime;

		private Vector2 GetStarPosition(bool pad, int i, bool bg = false) {
			var d = 0;
			var it = Player.GetComponent<ActiveItemComponent>().Item;

			if (pad && it != null && Math.Abs(it.UseTime) > 0.01f) {
				d = 4;
			}
			
			return new Vector2(
				(bg ? 0 : 1) + (pad ? (4 + (4 + ItemSlot.Source.Width + d) * (activeSlot.ActivePosition + 1)) : 6) + 4 + (int) (i % HeartsComponent.PerRow * 11f) - 2,
				(bg ? 0 : 1) + (i / HeartsComponent.PerRow) * 10 + 11 + (Player.GetComponent<HealthComponent>().MaxHealth + Player.GetComponent<HeartsComponent>().ShieldHalfs > HeartsComponent.PerRow ? 10 : 0) + 10
				+ (float) Math.Cos(i / 8f * Math.PI + Engine.Time * 12 - 1) * 0.5f * Math.Max(0, (float) (Math.Cos(Engine.Time * 0.25f - 1) - 0.9f) * 10f)
			);
		}
		
		private void RenderMana() {
			var manaComponent = Player.GetComponent<ManaComponent>();
			
			var totalMana = manaComponent.Mana;
			
			if (changedTime > 0) {
				changedTime -= Engine.Delta;
			}

			if (lastMana > totalMana) {
				lastMana = totalMana;
				changedTime = 0.5f;
			} else if (lastMana < totalMana) { 
				lastMana = Math.Min(totalMana, lastMana + Engine.Delta * 30);
				changedTime = 0.5f;
			}


			var r = (int) lastMana;
			var maxMana = manaComponent.ManaMax;
			var hurt = changedTime > 0;

			var n = r;
			var jn = maxMana;
			
			for (var i = 0; i < maxMana / 2; i++) {
				Graphics.Render(hurt ? ChangedManaBackground : ManaBackground, GetStarPosition(false, i, true) + (hurt ? new Vector2(-1) : Vector2.Zero));
			}

			for (var j = 0; j < n; j += 2) {
				Graphics.Render(j == r - 1 ? HalfMana : Mana, GetStarPosition(false, j / 2));
			}
		}

		private void RenderConsumables(bool hasMana) {
			var bottomY = 8 + 9 + 8 + (hasMana ? 10 : 0) + (Player.GetComponent<HealthComponent>().MaxHealth + Player.GetComponent<HeartsComponent>().ShieldHalfs > HeartsComponent.PerRow ? 10 : 0) + (int) (12 * (activeSlot.ActivePosition + 1));

			if (Scourge.IsEnabled(Scourge.OfKeys)) {
				Graphics.Render(question, new Vector2(8, bottomY + 1));
				return;
			}
			
			//if (coins > 0) {
				Graphics.Render(coin, new Vector2(8 + coin.Center.X, bottomY + 1 + coin.Center.Y), 0, coin.Center, coinScale);
				Graphics.Print($"{coins}", Font.Small, new Vector2(18, bottomY - 1));
				bottomY += 12;
			//}

			//if (keys > 0) {
				Graphics.Render(key, new Vector2(7 + key.Center.X, bottomY + key.Center.Y + 2), 0, key.Center, keyScale);
				Graphics.Print($"{keys}", Font.Small, new Vector2(18, bottomY - 1));
				bottomY += bomb.Source.Height + 2;
			//}

			//if (bombs > 0) {
				// Bomb sprite has bigger height
				Graphics.Render(bomb, new Vector2(8 + bomb.Center.X, bottomY + bomb.Center.Y), 0,
					bomb.Center, bombScale);

				Graphics.Print($"{bombs}", Font.Small, new Vector2(18, bottomY - 1));
			//}
		}
	}
}