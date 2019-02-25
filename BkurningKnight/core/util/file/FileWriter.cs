namespace BurningKnight.core.util.file {
	public class FileWriter {
		private DataOutputStream Stream;

		public FileWriter(string Path) {
			File File = new File(Path);

			if (!File.Exists()) {
				File.GetParentFile().Mkdirs();
				File.CreateNewFile();
			} 

			this.Stream = new DataOutputStream(new BufferedOutputStream(new FileOutputStream(File), 32768));
		}

		public Void WriteByte(byte Value) {
			this.Stream.WriteByte(Value);
		}

		public Void WriteBoolean(bool Value) {
			this.Stream.WriteBoolean(Value);
		}

		public Void WriteInt16(short Value) {
			this.Stream.WriteShort(Value);
		}

		public Void WriteInt32(int Value) {
			this.Stream.WriteInt(Value);
		}

		public Void WriteString(string String) {
			if (String == null) {
				this.Stream.WriteByte((byte) 0);
			} else {
				this.Stream.WriteByte(String.Length());

				for (int I = 0; I < String.Length(); I++) {
					this.Stream.WriteByte(String.CharAt(I));
				}
			}

		}

		public Void WriteDouble(double Value) {
			this.Stream.WriteDouble(Value);
		}

		public Void WriteFloat(float Value) {
			this.Stream.WriteFloat(Value);
		}

		public Void Close() {
			this.Stream.Close();
		}
	}
}
