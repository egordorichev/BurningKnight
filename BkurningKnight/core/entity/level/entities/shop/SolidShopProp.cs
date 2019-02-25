using BurningKnight.core.assets;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities.shop {
	public class SolidShopProp : SolidProp {
		private bool Shadow = false;

		public override Void RenderShadow() {
			if (Shadow) {
				base.RenderShadow();
			} 
		}

		public override Void Init() {
			base.Init();

			if (this.Sprite != null) {
				this.ParseName();
			} 
		}

		private Void ParseName() {
			if (this.Sprite.Equals("shop-table")) {
				this.Collider = new Rectangle(0, 10, 60, 25 - 15);
				this.CreateCollider();
			} else if (this.Sprite.Equals("shop-table_2")) {
				this.Collider = new Rectangle(0, 10, 42, 28 - 15);
				this.CreateCollider();
			} else if (this.Sprite.Equals("shop-cauldron")) {
				this.Collider = new Rectangle(6, 10, 37 - 12, 35 - 20);
				this.CreateCollider();
			} else if (this.Sprite.Equals("shop-shelf")) {
				this.Collider = new Rectangle(12, 10, 42 - 24, 10);
				this.CreateCollider();
			} 
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Sprite = Reader.ReadString();
			this.Region = Graphics.GetTexture(this.Sprite);
			ParseName();
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteString(this.Sprite);
		}
	}
}
