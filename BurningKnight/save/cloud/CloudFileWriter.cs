using System;
using System.Collections.Generic;
using Lens.util;
using Lens.util.file;
using Steamworks;

namespace BurningKnight.save.cloud {
	public class CloudFileWriter : FileWriter {
		private List<byte> data = new List<byte>();
		private string file;

		public CloudFileWriter(string path, bool append = false) : base(path, append) {
		}

		protected override void OpenStream(string path, bool append) {
			if (append) {
				try {
					var d = new CloudFileReader(path).Data;
					data.AddRange(d);
				} catch (Exception e) {
					Log.Error(e);
				}
			}
			
			file = path;
		}

		protected override void Write(byte value) {
			data.Add(value);
		}

		public override void Close() {
			SteamRemoteStorage.FileWrite(file, data.ToArray());
			data.Clear();
		}
	}
}