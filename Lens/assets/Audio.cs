using System;
using System.Collections.Generic;
using System.IO;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Lens.assets {
	public class Audio {
		private const float CrossFadeTime = 0.5f;
		
		private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();
		private static Dictionary<string, SoundEffect> music = new Dictionary<string, SoundEffect>();

		private static void LoadSfx(FileHandle file) {
			if (file.Exists()) {
				foreach (var sfx in file.ListFileHandles()) {
					if (sfx.Extension == ".xnb") {
						LoadSfx(sfx.NameWithoutExtension);
					}
				}

				/*foreach (var dir in file.ListDirectoryHandles()) {
					LoadSfx(dir);
				}*/
			}
		}
		
		internal static void Load() {
			LoadSfx(FileHandle.FromNearRoot("bin/Sfx/"));
			
			/*var musicDir = FileHandle.FromNearRoot("bin/Music/");
			
			if (musicDir.Exists()) {
				foreach (var h in musicDir.ListFileHandles()) {
					if (h.Extension == ".xnb") {
						LoadMusic(h.NameWithoutExtension);
					}
				}
			}*/
		}

		private static void LoadSfx(string sfx) {
			sfx = Path.GetFileNameWithoutExtension(sfx);
			sounds[sfx] = Assets.Content.Load<SoundEffect>($"bin/Sfx/{sfx}");				
		}

		private static void LoadMusic(string name) {
			music[name] = Assets.Content.Load<SoundEffect>($"bin/Music/{name}");
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
			if (!Assets.LoadAudio) {
				return;
			}
			
			sfx.Play(volume, pitch, pan);
		}

		public static SoundEffect GetMusic(string id) {
			SoundEffect m;

			if (music.TryGetValue(id, out m)) {
				return m;
			}

			Log.Error($"Music {id} was not found!");
			return null;
		}

		private static bool repeat;
		
		public static bool Repeat {
			get => repeat;
			set {
				if (currentPlaying != null) {
					currentPlaying.IsLooped = value;
				}

				repeat = value;
			}
		}

		private static SoundEffectInstance currentPlaying;
		private static string currentPlayingMusic;
		private static Dictionary<string, SoundEffectInstance> instances = new Dictionary<string, SoundEffectInstance>();
		
		public static void PlayMusic(string music, AudioListener listener = null, AudioEmitter emitter = null) {
			if (!Assets.LoadAudio) {
				return;
			}
			
			if (currentPlayingMusic == music) {
				return;
			}

			FadeOut();

			Repeat = true;
			
			if (!instances.TryGetValue(music, out currentPlaying)) {
				var ms = GetMusic(music);

				if (ms == null) {
					return;
				}
				
				currentPlaying = ms.CreateInstance();
				instances[music] = currentPlaying;
				currentPlaying.Apply3D(l, e);
				currentPlaying.Play();
			} else {
				currentPlaying.Apply3D(l, e);
				currentPlaying.Resume();
			}

			currentPlayingMusic = music;
			currentPlaying.Volume = 0;
			currentPlaying.IsLooped = repeat;

			var m = currentPlaying;
			Tween.To(musicVolume, m.Volume, x => m.Volume = x, CrossFadeTime);

			// e = emitter;
			// l = listener;
		}

		private static AudioListener l = new AudioListener();
		private static AudioEmitter e = new AudioEmitter();

		public static void FadeOut() {
			if (currentPlaying != null) {
				var m = currentPlaying;
				
				Tween.To(0, m.Volume, x => m.Volume = x, CrossFadeTime).OnEnd = () => {
					m.Pause();
				};
				
				currentPlaying = null;
				currentPlayingMusic = null;
			}
		}
		
		public static void Stop() {
			if (currentPlaying != null) {
				currentPlaying.Stop();
				currentPlaying = null;
				currentPlayingMusic = null;
			}
		}

		private static float musicVolume = 1;

		public static void UpdateMusicVolume(float value) {
			if (currentPlaying != null) {
				currentPlaying.Volume = value;
			}

			musicVolume = value;
		}
		
		public static void Update() {
			if (currentPlaying != null && currentPlaying.State == SoundState.Stopped && currentPlaying.IsLooped) {
				currentPlaying.Play();
			}
			
			/*if (e != null && l != null && currentPlaying != null) {
				l.Position = Vector3.Zero;

				var t = Engine.Time;
				var d = 5;

				e.Position = new Vector3((float) Math.Cos(t) * d, 0, (float) Math.Sin(t) * d);
				currentPlaying.Apply3D(l, e);
			}*/
		}
	}
}