using BurningKnight.core.assets;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities.shop {
	public class ShopProp : Prop {
		private bool Shadow = false;
		private bool Parsed;

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!Parsed) {
				Parsed = true;
				this.ParseName();
			} 
		}

		public override Void RenderShadow() {
			if (Shadow) {
				base.RenderShadow();
			} 
		}

		private Void ParseName() {
			if (this.Sprite.Equals("shop-carpet") || this.Sprite.Equals("shop-blood")) {
				this.Depth = -9;
				this.Shadow = false;
			} else if (this.Sprite.Equals("shop-bat") || this.Sprite.Equals("shop-frog") || this.Sprite.Equals("shop-bone") || this.Sprite.Equals("shop-skull")) {
				this.Depth = 1;
			} else if (Sprite.Equals("shop-frame_a") || Sprite.Equals("shop-frame_b") || Sprite.Equals("shop-target") || Sprite.Equals("shop-shields") || Sprite.Equals("shop-maniken")) {
				this.Depth = 6;
			} 

			if (Sprite.StartsWith("shop-frame")) {
				this.Shadow = false;
			} 

			this.W = this.Region.GetRegionWidth();
			this.H = this.Region.GetRegionHeight();
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Sprite = Reader.ReadString();
			this.Region = Graphics.GetTexture(this.Sprite);
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteString(this.Sprite);
		}
	}
}
