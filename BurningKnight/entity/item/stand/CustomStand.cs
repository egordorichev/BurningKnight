using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item.stand {
	public class CustomStand : ShopStand {
		private TextureRegion icon;
		private float priceWidth;
		private float iconY;

		protected virtual string GetIcon() {
			return "deal_heart";
		}

		protected override void CalculatePriceSize() {
			base.CalculatePriceSize();
			
			PriceString = $"{Price}";
			priceWidth = Font.Small.MeasureString(PriceString).Width;
			PriceX = (Width - priceWidth - icon.Width - 2) / 2f;
			iconY = (7 - icon.Height) / 2;
		}

		public override void Init() {
			base.Init();
			icon = CommonAse.Ui.GetSlice(GetIcon());
		}

		protected override void RenderPrice() {
			if (HasSale) {
				Graphics.Color = Palette.Default[35];
			}
				
			var r = GetComponent<RoomComponent>().Room;

			foreach (var p in r.Tagged[Tags.Player]) {
				if (p.GetComponent<ConsumablesComponent>().Bombs < Price) {
					Graphics.Color *= 0.6f;
					break;
				}					
			}
				
			Graphics.Print(PriceString, Font.Small, Position + new Vector2(PriceX, 14));
			Graphics.Color = ColorUtils.WhiteColor;
			Graphics.Render(icon, Position + new Vector2(PriceX + priceWidth + 2, 17 + iconY));
		}
	}
}