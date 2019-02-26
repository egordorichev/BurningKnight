using Lens.util.file;

namespace BurningKnight.entity.level.entities.shop {
	public class SolidShopProp : SolidProp {
		private bool Shadow = false;

		public override void RenderShadow() {
			if (Shadow) base.RenderShadow();
		}

		public override void Init() {
			base.Init();

			if (Sprite != null) ParseName();
		}

		private void ParseName() {
			if (Sprite.Equals("shop-table")) {
				Collider = new Rectangle(0, 10, 60, 25 - 15);
				CreateCollider();
			}
			else if (Sprite.Equals("shop-table_2")) {
				Collider = new Rectangle(0, 10, 42, 28 - 15);
				CreateCollider();
			}
			else if (Sprite.Equals("shop-cauldron")) {
				Collider = new Rectangle(6, 10, 37 - 12, 35 - 20);
				CreateCollider();
			}
			else if (Sprite.Equals("shop-shelf")) {
				Collider = new Rectangle(12, 10, 42 - 24, 10);
				CreateCollider();
			}
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Sprite = Reader.ReadString();
			Region = Graphics.GetTexture(Sprite);
			ParseName();
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteString(Sprite);
		}
	}
}