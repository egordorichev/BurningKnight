using BurningKnight.save;
using Lens.util.file;

namespace BurningKnight.entity.level {
	public class Room : SaveableEntity {
		public int MapX;
		public int MapY;
		public int MapW;
		public int MapH;
		
		protected override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.Room);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			MapX = stream.ReadInt16();
			MapY = stream.ReadInt16();
			MapW = stream.ReadInt16();
			MapH = stream.ReadInt16();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteInt16((short) MapX);
			stream.WriteInt16((short) MapY);
			stream.WriteInt16((short) MapW);
			stream.WriteInt16((short) MapH);
		}
	}
}