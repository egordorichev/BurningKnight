using BurningKnight.entity.creature.player;
using BurningKnight.util;

namespace BurningKnight.ui {
	public class UiClass : UiButton {
		public const Player.Type Type;
		private static string[] Textures = {"ui-warrior", "ui-wizard", "ui-archer"};
		private TextureRegion Bottom = Graphics.GetTexture("ui-card_bottom");
		private TextureRegion BottomLeft = Graphics.GetTexture("ui-card_bottom_left");
		private TextureRegion BottomRight = Graphics.GetTexture("ui-card_bottom_right");
		private TextureRegion Center = Graphics.GetTexture("ui-card_center");
		private TextureRegion Left = Graphics.GetTexture("ui-card_left");
		private TextureRegion Mage = Graphics.GetTexture("ui-wizard");
		private float Mod = 1f;
		private string Name;
		private float Nw;
		private TextureRegion Ranger = Graphics.GetTexture("ui-archer");
		private TextureRegion Region;
		private TextureRegion Right = Graphics.GetTexture("ui-card_right");
		private TextureRegion Top = Graphics.GetTexture("ui-cart_top");
		private TextureRegion TopLeft = Graphics.GetTexture("ui-card_top_left");
		private TextureRegion TopRight = Graphics.GetTexture("ui-cart_top_right");
		private TextureRegion Warrior = Graphics.GetTexture("ui-warrior");

		public UiClass(Player.Type Type, int X, int Y) {
			base(null, X, Y);
			this.Type = Type;
			W = 96;
			H = 128;
			Log.Info(this.Type.ToString());
			Name = Locale.Get(this.Type.ToString().ToLowerCase());
			Graphics.Layout.SetText(Graphics.Medium, Name);
			Nw = Graphics.Layout.Width;
			Region = Graphics.GetTexture(Textures[Id]);
		}

		public override void Render() {
			var W = this.W * Scale;
			var H = this.H * Scale;
			var X = this.X - W / 2;
			var Y = this.Y - H / 2;
			var Sx = W - 10;
			var Sy = H - 11;
			Graphics.Render(Top, X + TopLeft.GetRegionWidth(), Y + H - TopLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(TopLeft, X, Y + H - TopLeft.GetRegionHeight());
			Graphics.Render(TopRight, X + W - TopRight.GetRegionWidth(), Y + H - TopRight.GetRegionHeight());
			Graphics.Render(Left, X, Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Right, X + W - Right.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Center, X + Left.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, Sy);
			Graphics.Render(Bottom, X + BottomLeft.GetRegionWidth(), Y, 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(BottomLeft, X, Y);
			Graphics.Render(BottomRight, X + W - TopRight.GetRegionWidth(), Y);
			var Yy = this.Y + H / 2 - (W - Warrior.GetRegionWidth() / 2) / 2 - 5;

			switch (this.Type) {
				case WARRIOR:
				default: {
					Graphics.Render(Warrior, this.X, Yy, 0, Warrior.GetRegionWidth() / 2, Warrior.GetRegionHeight() / 2, false, false);

					break;
				}

				case RANGER: {
					Graphics.Render(Ranger, this.X, Yy, 0, Warrior.GetRegionWidth() / 2, Warrior.GetRegionHeight() / 2, false, false);

					break;
				}

				case WIZARD: {
					Graphics.Render(Mage, this.X, Yy, 0, Warrior.GetRegionWidth() / 2, Warrior.GetRegionHeight() / 2, false, false);

					break;
				}
			}
		}
	}
}