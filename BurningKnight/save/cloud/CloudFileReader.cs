using Lens.util.file;
using Steamworks;

namespace BurningKnight.save.cloud {
	public class CloudFileReader : FileReader {
		public CloudFileReader(string path) : base(path) {
			
		}

		protected override void ReadData(string path) {
			read = SteamRemoteStorage.FileRead(path);
		}
	}
}