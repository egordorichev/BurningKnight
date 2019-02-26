using System.IO;
using System.Text;

namespace Lens.util.file {
	public class FileReader {
		private BinaryReader stream;

		public FileReader(string path) {
			stream = new BinaryReader(File.Open(path, FileMode.Create));
		}

		public byte ReadByte() {
			return stream.ReadByte();
		}

		public bool ReadBoolean() {
			return stream.ReadBoolean();
		}

		public short ReadInt16() {
			return stream.ReadInt16();
		}

		public int ReadInt32() {
			return stream.ReadInt32();
		}

		public string ReadString() {
			byte Length = stream.ReadByte();

			if (Length == 0) {
				return null;
			}

			var Result = new StringBuilder();

			for (int I = 0; I < Length; I++) {
				Result.Append((char) stream.ReadByte());
			}

			return Result.ToString();
		}

		public double ReadDouble() {
			return stream.ReadDouble();
		}

		public float ReadFloat() {
			return stream.ReadSingle();
		}

		public void Close() {
			stream.Close();
		}
	}
}
