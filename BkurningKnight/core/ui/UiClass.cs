using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.util;

namespace BurningKnight.core.ui {
	public class UiClass : UiButton {
		public const Player.Type Type;
		private static string[] Textures = { "ui-warrior", "ui-wizard", "ui-archer" };
		private TextureRegion Region;
		private string Name;
		private float Nw;
		private TextureRegion Top = Graphics.GetTexture("ui-cart_top");
		private TextureRegion TopLeft = Graphics.GetTexture("ui-card_top_left");
		private TextureRegion TopRight = Graphics.GetTexture("ui-cart_top_right");
		private TextureRegion Center = Graphics.GetTexture("ui-card_center");
		private TextureRegion Left = Graphics.GetTexture("ui-card_left");
		private TextureRegion Right = Graphics.GetTexture("ui-card_right");
		private TextureRegion Bottom = Graphics.GetTexture("ui-card_bottom");
		private TextureRegion BottomLeft = Graphics.GetTexture("ui-card_bottom_left");
		private TextureRegion BottomRight = Graphics.GetTexture("ui-card_bottom_right");
		private TextureRegion Warrior = Graphics.GetTexture("ui-warrior");
		private TextureRegion Ranger = Graphics.GetTexture("ui-archer");
		private TextureRegion Mage = Graphics.GetTexture("ui-wizard");
		private float Mod = 1f;

		public UiClass(Player.Type Type, int X, int Y) {
			base(null, X, Y);
			this.Type = Type;
			this.W = 96;
			this.H = 128;
			Log.Info(this.Type.ToString());
			this.Name = Locale.Get(this.Type.ToString().ToLowerCase());
			Graphics.Layout.SetText(Graphics.Medium, this.Name);
			this.Nw = Graphics.Layout.Width;
			this.Region = Graphics.GetTexture(Textures[this.Id]);
		}

		public override Void Render() {
			float W = this.W * Scale;
			float H = this.H * Scale;
			float X = this.X - W / 2;
			float Y = this.Y - H / 2;
			float Sx = (W - 10);
			float Sy = (H - 11);
			Graphics.Render(Top, X + TopLeft.GetRegionWidth(), Y + H - TopLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(TopLeft, X, Y + H - TopLeft.GetRegionHeight());
			Graphics.Render(TopRight, X + W - TopRight.GetRegionWidth(), Y + H - TopRight.GetRegionHeight());
			Graphics.Render(Left, X, Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Right, X + W - Right.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
			Graphics.Render(Center, X + Left.GetRegionWidth(), Y + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, Sy);
			Graphics.Render(Bottom, X + BottomLeft.GetRegionWidth(), Y, 0, 0, 0, false, false, Sx, 1);
			Graphics.Render(BottomLeft, X, Y);
			Graphics.Render(BottomRight, X + W - TopRight.GetRegionWidth(), Y);
			float Yy = this.Y + H / 2 - (W - Warrior.GetRegionWidth() / 2) / 2 - 5;

			switch (this.Type) {
				case WARRIOR: 
				default:{
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
