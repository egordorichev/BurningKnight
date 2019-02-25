namespace BurningKnight.core.util.file {
	public class FileReader {
		private DataInputStream Stream;

		public FileReader(string Path) {
			this.Stream = new DataInputStream(new BufferedInputStream(new FileInputStream(Path), 32768));
		}

		public byte ReadByte() {
			return this.Stream.ReadByte();
		}

		public bool ReadBoolean() {
			return this.Stream.ReadBoolean();
		}

		public short ReadInt16() {
			return this.Stream.ReadShort();
		}

		public int ReadInt32() {
			return this.Stream.ReadInt();
		}

		public string ReadString() {
			byte Length = this.Stream.ReadByte();

			if (Length == 0) {
				return null;
			} 

			StringBuilder Result = new StringBuilder();

			for (int I = 0; I < Length; I++) {
				Result.Append((char) this.Stream.ReadByte());
			}

			return Result.ToString();
		}

		public double ReadDouble() {
			return this.Stream.ReadDouble();
		}

		public float ReadFloat() {
			return this.Stream.ReadFloat();
		}

		public Void Close() {
			this.Stream.Close();
		}
	}
}
