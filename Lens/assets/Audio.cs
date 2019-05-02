using System.Collections.Generic;
using System.IO;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Lens.assets {
	public class Audio {
		private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
		private static Dictionary<string, Song> music = new Dictionary<string, Song>();

		private static void LoadSfx(FileHandle file) {
			if (file.Exists()) {
				foreach (var sfx in file.ListFileHandles()) {
					if (sfx.Extension == ".xnb") {
						LoadSfx(sfx.NameWithoutExtension);
					}
				}

				foreach (var dir in file.ListDirectoryHandles()) {
					LoadSfx(dir);
				}
			}
		}
		
		internal static void Load() {
			LoadSfx(FileHandle.FromNearRoot("Sfx/"));
			
			var musicDir = FileHandle.FromNearRoot("Music/");
			
			if (musicDir.Exists()) {
				foreach (var h in musicDir.ListFileHandles()) {
					if (h.Extension == ".xnb") {
						LoadMusic(h.NameWithoutExtension);
					}
				}
			}
		}

		private static void LoadSfx(string sfx) {
			if (false && Assets.LoadOriginalFiles) {
				Log.Debug($"Content/Sfx/{Path.GetFileName(sfx)} {sfx}");
				var fileStream = new FileStream(sfx, FileMode.Open);
				sounds[Path.GetFileNameWithoutExtension(sfx)] = SoundEffect.FromStream(fileStream);
				fileStream.Dispose();
			} else {
				sfx = Path.GetFileNameWithoutExtension(sfx);
				sounds[sfx] = Assets.Content.Load<SoundEffect>($"bin/Sfx/{sfx}");				
			}
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
		
		public static void PlaySfx(string id, float volume = 1, float pitch = 0, float pan = 0) {
			PlaySfx(GetSfx(id), volume, pitch, pan);
		}

		public static SoundEffect GetSfx(string id) {
			SoundEffect effect;

			if (sounds.TryGetValue(id, out effect)) {
				return effect;
			}

			Log.Error($"Sound effect {id} was not found!");
			return null;
		}

		public static void PlaySfx(SoundEffect sfx, float volume = 1, float pitch = 0, float pan = 0) {
			sfx.Play(volume, pitch, pan);
		}

		public static void PlayMusic(string music, float volume = 1) {
			PlayMusic(GetMusic(music), volume);
		}
		
		public static Song GetMusic(string id) {
			Song m;

			if (music.TryGetValue(id, out m)) {
				return m;
			}

			Log.Error($"Music {id} was not found!");
			return null;
		}

		public static bool Repeat {
			get => MediaPlayer.IsRepeating;
			set => MediaPlayer.IsRepeating = value;
		}

		private static Song currentPlaying;

		public static void PlayMusic(Song music, float volume = 1) {
			if (currentPlaying == music && MediaPlayer.State != MediaState.Stopped) {
				return;
			}

			if (MediaPlayer.State != MediaState.Stopped) {
				// TODO: tween music volumes some how
				// Tweening currently doesnt support static classes
			}
			
			MediaPlayer.Play(music);
			MediaPlayer.Volume = volume;
			currentPlaying = music;
		}
	}
}