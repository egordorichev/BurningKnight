using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.stand {
	public class DarkMageStand : ShopStand {
		private TextureRegion heart;
		private float priceWidth;
		
		protected override string GetSprite() {
			return "dm_stand";
		}

		public override void Init() {
			base.Init();
			
			OnSale = false;
			heart = CommonAse.Ui.GetSlice("deal_heart");
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
				if (p.GetComponent<HealthComponent>().MaxHealth < Price) {
					Graphics.Color *= 0.6f;
					break;
				}					
			}
				
			Graphics.Print(PriceString, Font.Small, Position + new Vector2(PriceX, 14));
			Graphics.Render(heart, Position + new Vector2(PriceX + priceWidth + 2, 17));
			
			Graphics.Color = ColorUtils.WhiteColor;
		}

		protected override bool TryPay(Entity entity) {
			if (!entity.TryGetComponent<HealthComponent>(out var component)) {
				return false;
			}

			if (component.MaxHealth < Price) {
				return false;
			}

			component.MaxHealth -= Price;
			component.ModifyHealth(-Price, this);
			return true;
		}
	}
}