using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.game.state;

namespace BurningKnight.ui {
	public class StartingItem : UiButton {
		public static StartingItem Hovered;
		public Item Item;
		public string Name;
		public Player.Type Type;

		public StartingItem() {
			base("", 0, 0);
		}

		public override void Init() {
			base.Init();
			W = Item.GetSprite().GetRegionWidth() * 3;
			H = Item.GetSprite().GetRegionHeight() * 3;
		}

		protected override void OnHover() {
			base.OnHover();
			Hovered = this;
		}

		protected override void OnUnhover() {
			base.OnUnhover();

			if (Hovered == this) Hovered = null;
		}

		public override void OnClick() {
			base.OnClick();
			ItemSelectState.Pick(Item, Type);
		}

		public override void Render() {
			TextureRegion Region = Item.GetSprite();
			var A = Math.Cos(this.Y / 12 + Dungeon.Time * 6) * Math.Max(0, 0.5f - Rr * 0.5f) * 100;
			ScaleMod = 2;
			Graphics.Batch.SetColor(R * Ar, G * Ag, B * Ab, 1);
			Graphics.Render(Region, this.X, this.Y, A, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Scale * 2, Scale * 2);
			Graphics.Medium.SetColor(1, 1, 1, 1);
		}
	}
}