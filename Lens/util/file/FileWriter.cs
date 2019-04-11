using System.IO;
using BinaryWriter = System.IO.BinaryWriter;

namespace Lens.util.file {
	public class FileWriter {
		private BinaryWriter stream;

		public FileWriter(string path) {
			stream = new BinaryWriter(File.Open(path, FileMode.Create));
		}

		public void WriteByte(byte Value) {
			stream.Write(Value);
		}

		public void WriteSbyte(sbyte Value) {
			stream.Write(Value);
		}

		public void WriteBoolean(bool Value) {
			stream.Write(Value);
		}

		public void WriteInt16(short Value) {
			stream.Write(Value);
		}

		public void WriteInt32(int Value) {
			stream.Write(Value);
		}

		public void WriteString(string String) {
			if (String == null) {
				stream.Write((byte) 0);
			} else {
				stream.Write((byte) String.Length);

				foreach (var t in String) {
					stream.Write((byte) t);
				}
			}
		}

		public void WriteDouble(double Value) {
			stream.Write(Value);
		}

		public void WriteFloat(float Value) {
			stream.Write(Value);
		}

		public void Close() {
			stream.Close();
		}
	}
}
