using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Lens.Asset {
	public class Audio {
		private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
		private static Dictionary<string, Song> music = new Dictionary<string, Song>();
		
		public static void Load() {
			/*FileHandle sfxDir = FileHandle.FromRoot("Sfx/");

			if (sfxDir.Exists()) {
				foreach (var sfx in sfxDir.ListFiles()) {
					LoadSfx(sfx);
				} 	
			}
			
			FileHandle musicDir = FileHandle.FromRoot("Music/");
			
			if (musicDir.Exists()) {
				foreach (var name in musicDir.ListFiles()) {
					LoadMusic(name);
				} 	
			}*/

			// Assets.Content.Load<Song>("bin/Music/shop");
		}

		private static void LoadSfx(string sfx) {
			var fileStream = new FileStream(sfx, FileMode.Open);			
			sounds[Path.GetFileNameWithoutExtension(sfx)] = SoundEffect.FromStream(fileStream);
			fileStream.Dispose();
		}

		private static void LoadMusic(string name) {
			name = Path.GetFileNameWithoutExtension(name);
			music[name] = Assets.Content.Load<Song>("bin/Music/tech");
		}
		
		public static void Destroy() {
			
		}
		
		public static void PlaySfx(string id) {
			
		}
	}
}