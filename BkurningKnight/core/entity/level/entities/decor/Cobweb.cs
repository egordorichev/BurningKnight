using BurningKnight.core.assets;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities.decor {
	public class Cobweb : SaveableEntity {
		protected void _Init() {
			{
				Depth = 4;
			}
		}

		public static TextureRegion[] Textures = { Graphics.GetTexture("props-cobweb_top_left"), Graphics.GetTexture("props-cobweb_top_right"), Graphics.GetTexture("props-cobweb_bottom_left"), Graphics.GetTexture("props-cobweb_bottom_right") };
		public int Side;

		public override Void Render() {
			Graphics.Render(Textures[this.Side], this.X, this.Y);
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteByte((byte) Side);
		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			Side = Reader.ReadByte();
		}

		public Cobweb() {
			_Init();
		}
	}
}
