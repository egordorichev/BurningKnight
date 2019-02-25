using BurningKnight.core.assets;
using BurningKnight.core.physics;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities {
	public class Table : SolidProp {
		private bool S;

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.W = Reader.ReadInt16();
			this.H = Reader.ReadInt16();
		}

		private Void MakeBody() {
			this.Body = World.CreateSimpleBody(this, 2, 4, this.W - 4, Math.Max(1, this.H - 16 - 8), BodyDef.BodyType.StaticBody, false);

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			} 
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteInt16((short) this.W);
			Writer.WriteInt16((short) this.H);
		}

		public override Void Render() {
			if (!S) {
				S = true;

				for (int X = 0; X < this.W / 16; X++) {
					for (int Y = 0; Y < this.H / 16; Y++) {
						Dungeon.Level.SetPassable((int) (X + this.X / 16), (int) (Y + (this.Y + 8) / 16), false);
					}
				}
			} 

			if (this.Body == null) {
				this.MakeBody();
			} 

			for (int X = 0; X < this.W / 16; X++) {
				for (int Y = 0; Y < this.H / 16; Y++) {
					int Count = 0;

					if (Y < this.H / 16 - 1) {
						Count += 1;
					} 

					if (X < this.W / 16 - 1) {
						Count += 2;
					} 

					if (Y > 0) {
						Count += 4;
					} 

					if (X > 0) {
						Count += 8;
					} 

					TextureRegion Sprite = Terrain.TableVariants[Count];
					Graphics.Render(Sprite, this.X + X * 16 + (16 - Sprite.GetRegionWidth()) / 2, this.Y + Y * 16 - 8 + (16 - Sprite.GetRegionHeight()) / 2);
				}
			}
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, this.W, this.H);
		}
	}
}
