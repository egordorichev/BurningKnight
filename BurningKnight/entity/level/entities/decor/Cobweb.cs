using BurningKnight.save;
using Lens.util.file;

namespace BurningKnight.entity.level.entities.decor {
	public class Cobweb : SaveableEntity {
		public static TextureRegion[] Textures = {Graphics.GetTexture("props-cobweb_top_left"), Graphics.GetTexture("props-cobweb_top_right"), Graphics.GetTexture("props-cobweb_bottom_left"), Graphics.GetTexture("props-cobweb_bottom_right")};
		public int Side;

		public Cobweb() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = 4;
			}
		}

		public override void Render() {
			Graphics.Render(Textures[Side], this.X, this.Y);
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteByte((byte) Side);
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Side = Reader.ReadByte();
		}
	}
}