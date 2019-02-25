using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class Stone : SolidProp {
		private bool Flip;
		private string Old;

		public override void Init() {
			if (Sprite == null) return;

			Old = Sprite;
			Flip = Random.Chance(50);
			CreateSprite(Old);
		}

		private void CreateSprite(string Sprite) {
			switch (Sprite) {
				case "prop_stone": {
					this.Sprite = "props-rock_a";
					Collider = new Rectangle(4, 6, 14 - 4 * 2, 12 - 6 - 2);
					W = 14;
					H = 12;

					break;
				}

				case "prop_high_stone": {
					this.Sprite = "props-rock_b";
					Collider = new Rectangle(4, 8, 14 - 4 * 2, 21 - 8 * 2);
					W = 14;
					H = 21;

					break;
				}

				case "prop_big_stone": {
					this.Sprite = "props-rock_c";
					Collider = new Rectangle(4, 8, 28 - 4 * 2, 23 - 8 * 2);
					W = 28;
					H = 23;

					break;
				}
			}

			base.Init();
			CreateCollider();
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteString(Old);
			Writer.WriteBoolean(Flip);
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Old = Reader.ReadString();
			Flip = Reader.ReadBoolean();
			CreateSprite(Old);
		}

		public override void Render() {
			Graphics.Render(Region, this.X + Region.GetRegionWidth() / 2, this.Y + Region.GetRegionHeight() / 2, 0, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Flip ? -1 : 1, 1);
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X, this.Y + 2, W, H);
		}
	}
}