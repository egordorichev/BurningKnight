using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item;
using BurningKnight.core.game.state;

namespace BurningKnight.core.ui {
	public class StartingItem : UiButton {
		public Item Item;
		public Player.Type Type;
		public string Name;
		public static StartingItem Hovered;

		public StartingItem() {
			base("", 0, 0);
		}

		public override Void Init() {
			base.Init();
			this.W = Item.GetSprite().GetRegionWidth() * 3;
			this.H = Item.GetSprite().GetRegionHeight() * 3;
		}

		protected override Void OnHover() {
			base.OnHover();
			Hovered = this;
		}

		protected override Void OnUnhover() {
			base.OnUnhover();

			if (Hovered == this) {
				Hovered = null;
			} 
		}

		public override Void OnClick() {
			base.OnClick();
			ItemSelectState.Pick(this.Item, this.Type);
		}

		public override Void Render() {
			TextureRegion Region = Item.GetSprite();
			float A = (float) (Math.Cos(this.Y / 12 + Dungeon.Time * 6) * Math.Max(0, 0.5f - Rr * 0.5f) * 100);
			ScaleMod = 2;
			Graphics.Batch.SetColor(this.R * this.Ar, this.G * this.Ag, this.B * this.Ab, 1);
			Graphics.Render(Region, this.X, this.Y, A, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, this.Scale * 2, this.Scale * 2);
			Graphics.Medium.SetColor(1, 1, 1, 1);
		}
	}
}
