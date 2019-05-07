using System;
using System.IO;
using System.Text;

namespace Lens.util.file {
	public class FileReader {
		private byte[] read;
		private int position;
		
		public int Position {
			get => position;

			set {
				position = Math.Max(0, Math.Min(read.Length - 1, value));
			}
		}
		
		public FileReader(string path) {
			var file = File.Open(path, FileMode.Open);
			var stream = new BinaryReader(file);

			read = new byte[file.Length];

			for (var i = 0; i < file.Length; i++) {
				read[i] = (byte) file.ReadByte();
			}
			
			stream.Close();
		}
		
		public byte ReadByte() {
			if (read.Length == Position) {
				return 0;
			}
			
			return read[Position++];
		}
		
		public sbyte ReadSbyte() {
			return (sbyte) ReadByte();
		}

		public bool ReadBoolean() {
			return ReadByte() == 1;
		}

		public short ReadInt16() {
			return (short) ((ReadByte() << 8) | ReadByte());
		}

		public int ReadInt32() {
			return (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();
		}

		public string ReadString() {
			byte length = ReadByte();

			if (length == 0) {
				return null;
			}

			var result = new StringBuilder();

			for (int i = 0; i < length; i++) {
				result.Append((char) ReadByte());
			}

			return result.ToString();
		}

		public float ReadFloat() {
			return BitConverter.ToSingle(new[] { ReadByte(), ReadByte(), ReadByte(), ReadByte() }, 0);
		}
	}
}
