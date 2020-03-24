using System;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.file;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.stand {
	public class DarkMageStand : ShopStand {
		private TextureRegion heart;
		private float priceWidth;

		private Entity payer;
		private Item takenItem;
		private int lastPrice;

		protected override int CalculatePrice() {
			return (int) Math.Max(1, PriceCalculator.GetModifier(Item));
		}

		protected override string GetSprite() {
			return "dm_stand";
		}

		public override void Init() {
			base.Init();
			
			OnSale = false;
			heart = CommonAse.Ui.GetSlice("deal_heart");
			
			Subscribe<ItemUsedEvent>();
			Subscribe<ItemAddedEvent>();
		}

		protected override void CalculatePriceSize() {
			PriceString = $"{Price}";
			priceWidth = Font.Small.MeasureString(PriceString).Width;
			PriceX = (Width - priceWidth - heart.Width - 2) / 2f;
		}

		protected override void RenderPrice() {
			if (HasSale) {
				Graphics.Color = Palette.Default[35];
			}
				
			var r = GetComponent<RoomComponent>().Room;

			foreach (var p in r.Tagged[Tags.Player]) {
				if (p.GetComponent<HealthComponent>().MaxHealth < Price * 2) {
					Graphics.Color *= 0.6f;
					break;
				}					
			}
				
			Graphics.Print(PriceString, Font.Small, Position + new Vector2(PriceX, 14));
			Graphics.Color = ColorUtils.WhiteColor;
			Graphics.Render(heart, Position + new Vector2(PriceX + priceWidth + 2, 17));
		}

		protected override bool TryPay(Entity entity) {
			if (!entity.TryGetComponent<HealthComponent>(out var component)) {
				return false;
			}

			if (component.MaxHealth < Price * 2) {
				return false;
			}

			payer = entity;
			lastPrice = Price * 2;
			takenItem = Item;
			
			var stats = entity.GetComponent<StatsComponent>();
			
			stats.TookDeal = true;
			stats.HeartsPayed += Price;
			
			Achievements.Unlock("bk:deal");
			
			return true;
		}

		public override bool HandleEvent(Event e) {
			if ((e is ItemUsedEvent ite && ite.Who == payer && ite.Item == takenItem) || (e is ItemAddedEvent iae && iae.Who == payer && iae.Item == takenItem)) {
				var component = payer.GetComponent<HealthComponent>();
				 
				component.ModifyHealth(-lastPrice, this, DamageType.Custom);
				component.MaxHealth -= lastPrice;		
				
				TextParticle.Add(payer, Locale.Get("max_hp"), lastPrice, true, true);
				
				payer = null;
				takenItem = null;
			}
			
			return base.HandleEvent(e);
		}

		public override ItemPool GetPool() {
			return ItemPool.OldMan;
		}
	}
}