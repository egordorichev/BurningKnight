using System.Collections.Generic;
using System.IO;
using Lens.Util.File;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Lens.Asset {
	public class Audio {
		private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
		private static Dictionary<string, Song> music = new Dictionary<string, Song>();
		
		internal static void Load() {
			var sfxDir = FileHandle.FromRoot("Sfx/");

			if (sfxDir.Exists()) {
				foreach (var sfx in sfxDir.ListFiles()) {
					LoadSfx(Path.GetFileNameWithoutExtension(sfx));
				} 	
			}
			
			var musicDir = FileHandle.FromRoot("Music/");
			
			if (musicDir.Exists()) {
				foreach (var name in musicDir.ListFiles()) {
					LoadMusic(Path.GetFileNameWithoutExtension(name));
				} 	
			}
		}

		private static void LoadSfx(string sfx) {
			sounds[sfx] = Assets.Content.Load<SoundEffect>($"bin/Music/{sfx}");
		}

		private static void LoadMusic(string name) {
			music[name] = Assets.Content.Load<Song>($"bin/Music/{name}");
		}
		
		internal static void Destroy() {
			foreach (var sound in sounds.Values) {
				sound.Dispose();
			}
			
			foreach (var m in music.Values) {
				m.Dispose();
			}
		}
		
		public static void PlaySfx(string id) {
			
		}
	}
}