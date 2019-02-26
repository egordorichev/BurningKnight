using Lens.util.file;

namespace BurningKnight.entity.level.entities.shop {
	public class ShopProp : Prop {
		private bool Parsed;
		private bool Shadow;

		public override void Update(float Dt) {
			base.Update(Dt);

			if (!Parsed) {
				Parsed = true;
				ParseName();
			}
		}

		public override void RenderShadow() {
			if (Shadow) base.RenderShadow();
		}

		private void ParseName() {
			if (Sprite.Equals("shop-carpet") || Sprite.Equals("shop-blood")) {
				Depth = -9;
				Shadow = false;
			}
			else if (Sprite.Equals("shop-bat") || Sprite.Equals("shop-frog") || Sprite.Equals("shop-bone") || Sprite.Equals("shop-skull")) {
				Depth = 1;
			}
			else if (Sprite.Equals("shop-frame_a") || Sprite.Equals("shop-frame_b") || Sprite.Equals("shop-target") || Sprite.Equals("shop-shields") || Sprite.Equals("shop-maniken")) {
				Depth = 6;
			}

			if (Sprite.StartsWith("shop-frame")) Shadow = false;

			W = Region.GetRegionWidth();
			H = Region.GetRegionHeight();
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Sprite = Reader.ReadString();
			Region = Graphics.GetTexture(Sprite);
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteString(Sprite);
		}
	}
}