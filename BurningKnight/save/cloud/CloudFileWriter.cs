using System.Collections.Generic;
using Lens.util.file;
using Steamworks;

namespace BurningKnight.save.cloud {
	public class CloudFileWriter : FileWriter {
		private List<byte> data = new List<byte>();

		protected override void OpenStream(string path, bool append) {
			
		}

		protected override void Write(byte value) {
			data.Add(value);
		}

		public CloudFileWriter(string path, bool append = false) : base(path, append) {
			SteamRemoteStorage.FileWrite(path, data.ToArray());
			data.Clear();
		}
	}
}