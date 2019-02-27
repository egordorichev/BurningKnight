using BurningKnight.save;
using BurningKnight.state;
using Lens.util.file;

namespace BurningKnight.entity.level {
	public abstract class Level : SaveableEntity {
		private int width;
		private int height;

		public new int Width {
			get => width;

			set {
				width = value;
				Size = width * height;
			}
		}
		
		public new int Height {
			get => height;

			set {
				height = value;
				Size = width * height;
			}
		}
		
		public int Size;
		public byte[] Tiles;
		public byte[] Liquid;
		public byte[] Variants;
		public byte[] Light;

		public Level() {
			Run.Level = this;
		}

		public void Set(int x, int y, Tile value, bool liquid = false) {
			if (liquid) {
				Liquid[ToIndex(x, y)] = (byte) value;
			} else {
				Tiles[ToIndex(x, y)] = (byte) value;
			}
		}
		
		public Tile Get(int x, int y, bool liquid = false) {
			return (Tile) (liquid ? Liquid[ToIndex(x, y)] : Tiles[ToIndex(x, y)]);
		}
		
		public void Set(int i, Tile value, bool liquid = false) {
			if (liquid) {
				Liquid[i] = (byte) value;
			} else {
				Tiles[i] = (byte) value;
			}
		}
		
		public Tile Get(int i, bool liquid = false) {
			return (Tile) (liquid ? Liquid[i] : Tiles[i]);
		}
		
		public int ToIndex(int x, int y) {
			return x + y * width;
		}

		public int FromIndexX(int index) {
			return index % width;
		}

		public int FromIndexY(int index) {
			return index / width;
		}

		public bool IsInside(int x, int y) {
			return x >= 0 && y >= 0 && x < width && y < height;
		}

		public bool IsInside(int i) {
			return i >= 0 && i < Size;
		}

		public bool CheckFor(int x, int y, int flag, bool liquid = false) {
			return Get(x, y, liquid).Matches(flag);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteInt32(width);
			stream.WriteInt32(height);

			for (int i = 0; i < Size; i++) {
				stream.WriteByte(Tiles[i]);
				stream.WriteByte(Liquid[i]);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			Width = stream.ReadInt32();
			Height = stream.ReadInt32();

			Setup();

			for (int i = 0; i < Size; i++) {
				Tiles[i] = stream.ReadByte();
				Liquid[i] = stream.ReadByte();
			}
		}

		public void Setup() {
			Tiles = new byte[Size];
			Liquid = new byte[Size];
			Variants = new byte[Size];
			Light = new byte[Size];
		}
	}
}