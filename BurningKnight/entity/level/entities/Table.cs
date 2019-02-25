using BurningKnight.physics;
using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class Table : SolidProp {
		private bool S;

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			W = Reader.ReadInt16();
			H = Reader.ReadInt16();
		}

		private void MakeBody() {
			Body = World.CreateSimpleBody(this, 2, 4, W - 4, Math.Max(1, H - 16 - 8), BodyDef.BodyType.StaticBody, false);

			if (Body != null) World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteInt16((short) W);
			Writer.WriteInt16((short) H);
		}

		public override void Render() {
			if (!S) {
				S = true;

				for (var X = 0; X < W / 16; X++)
				for (var Y = 0; Y < H / 16; Y++)
					Dungeon.Level.SetPassable(X + this.X / 16, Y + (this.Y + 8) / 16, false);
			}

			if (Body == null) MakeBody();

			for (var X = 0; X < W / 16; X++)
			for (var Y = 0; Y < H / 16; Y++) {
				var Count = 0;

				if (Y < H / 16 - 1) Count += 1;

				if (X < W / 16 - 1) Count += 2;

				if (Y > 0) Count += 4;

				if (X > 0) Count += 8;

				TextureRegion Sprite = Terrain.TableVariants[Count];
				Graphics.Render(Sprite, this.X + X * 16 + (16 - Sprite.GetRegionWidth()) / 2, this.Y + Y * 16 - 8 + (16 - Sprite.GetRegionHeight()) / 2);
			}
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, W, H);
		}
	}
}