using System;
using System.Collections.Generic;
using System.IO;
using BinaryWriter = System.IO.BinaryWriter;

namespace Lens.util.file {
	public class FileWriter {
		private BinaryWriter stream;
		private List<byte> cache = new List<byte>();

		public int CacheSize => cache.Count;
		public bool Cache;
		
		public FileWriter(string path) {
			stream = new BinaryWriter(File.Open(path, FileMode.Create));
		}

		public void Flush() {
			foreach (var b in cache) {
				WriteByte(b);
			}

			cache.Clear();
		}
		
		public void WriteByte(byte value) {
			if (Cache) {
				cache.Add(value);
			} else {
				stream.Write(value);
			}
		}

		public unsafe void WriteSbyte(sbyte value) {
			WriteByte(*((byte*) &value));
		}

		public void WriteBoolean(bool value) {
			WriteByte((byte) (value ? 1 : 0));
		}

		public void WriteInt16(short value) {
			WriteByte((byte) (value >> 8));
			WriteByte((byte) value);
		}

		public void WriteInt32(int value) {
			WriteByte((byte) ((value >> 24) & 0xFF));
			WriteByte((byte) ((value >> 16) & 0xFF));
			WriteByte((byte) ((value >> 8) & 0xFF));
			WriteByte((byte) (value & 0xFF));
		}

		public void WriteUInt16(ushort value) {
			WriteByte((byte) (value >> 8));
			WriteByte((byte) value);
		}

		public void WriteUInt32(uint value) {
			WriteByte((byte) ((value >> 24) & 0xFF));
			WriteByte((byte) ((value >> 16) & 0xFF));
			WriteByte((byte) ((value >> 8) & 0xFF));
			WriteByte((byte) (value & 0xFF));
		}

		public void WriteString(string str) {
			if (str == null) {
				WriteByte(0);
			} else {
				if (str.Length > 255) {
					Log.Error("String ${str} is longer than 255 chars! It will be trimmed to fit!");
				}
				
				WriteByte((byte) str.Length);

				for (var i = 0; i < Math.Min(255, str.Length); i++) {
					WriteByte((byte) str[i]);
				}
			}
		}

		public unsafe void WriteFloat(float value) {
			uint val = *((uint*) &value);

			WriteByte((byte) (val & 0xFF));
			WriteByte((byte) ((val >> 8) & 0xFF));
			WriteByte((byte) ((val >> 16) & 0xFF));
			WriteByte((byte) ((val >> 24) & 0xFF));
		}

		public void Close() {
			stream.Close();
		}
	}
}
