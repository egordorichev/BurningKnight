using BurningKnight.assets;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	public class ShopStand : ItemStand {
		public bool Sells = true;
		private int price;
		private string priceString;
		private float priceX;

		protected override string GetSprite() {
			return "shop_stand";
		}

		protected override bool CanTake(Entity entity) {
			if (!base.CanTake(entity)) {
				return false;
			}

			if (!Sells) {
				return true;
			}

			if (!entity.TryGetComponent<ConsumablesComponent>(out var component)) {
				return false;
			}

			if (component.Coins < price) {
				AnimationUtil.ActionFailed();
				return false;
			}

			component.Coins -= price;
			Sells = false;
			
			return true;
		}

		protected override void OnTake(Item item, Entity who) {
			base.OnTake(item, who);
			
			HandleEvent(new ItemBoughtEvent {
				Item = item,
				Who = who,
				Stand = this
			});
		}

		public override void Render() {
			base.Render();

			if (Item != null && Sells) {
				Graphics.Print(priceString, Font.Small, Position + new Vector2(priceX, 14));
			}
		}

		public void Recalculate() {
			Sells = true;
			price = PriceCalculator.Calculate(Item);
			CalculatePriceSize();
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemPlacedEvent) {
				Recalculate();
			}
			
			return base.HandleEvent(e);
		}

		private void CalculatePriceSize() {
			priceString = $"{price}";
			priceX = (Width - Font.Small.MeasureString(priceString).Width) / 2f;
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			if (ImGui.InputInt("Price", ref price)) {
				CalculatePriceSize();
			}
		}
	}
}