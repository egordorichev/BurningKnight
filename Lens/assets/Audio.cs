using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Aseprite;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using VelcroPhysics;

namespace Lens.assets {
	public class Audio {
		public static float MasterVolume = 1;
		public static float SfxVolume = 1;
		public static float SfxVolumeBuffer = 1f;
		public static float SfxVolumeBufferResetTimer = 0;
		private const float CrossFadeTime = 0.25f;

		public static float Db3 = 0.1f;

		private static Song currentPlaying;
		private static string currentPlayingMusic;
		private static Dictionary<string, Song> musicInstances = new Dictionary<string, Song>();
		private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

		public static bool Repeat = true;
		public static DynamicSoundEffectInstance SoundEffectInstance;

		public static float Speed = 1;

		private static void LoadSfx(FileHandle file, string path, bool root = false) {
			if (file.Exists()) {
				path = $"{path}{file.Name}{(root ? "" : "/")}";

				foreach (var sfx in file.ListFileHandles()) {
					if (sfx.Extension == ".xnb") {
						LoadSfx(sfx.NameWithoutExtension, path);
					}
				}

				foreach (var dir in file.ListDirectoryHandles()) {
					LoadSfx(dir, path);
				}
			} else {
				Log.Error($"File {file.Name} is missing");
			}
		}

		public static void StartThread() {
			
		}
		
		internal static void Load() {
			LoadSfx(FileHandle.FromNearRoot("bin/Sfx/"), "", true);
		}

		private static void LoadSfx(string sfx, string path) {
			var s = Path.GetFileNameWithoutExtension(sfx);
			sounds[$"{path}{s}".Replace('/', '_')] = Assets.Content.Load<SoundEffect>($"bin/Sfx/{path}{s}");				
		}
		
		internal static void Destroy() {
			foreach (var sound in sounds.Values) {
				sound.Dispose();
				GC.SuppressFinalize(sound); // Lol what?
			}

			if (SoundEffectInstance != null) {
				Log.Info("Disposing audio mixer");

				SoundEffectInstance.Stop();
				SoundEffectInstance.Dispose();
				GC.SuppressFinalize(SoundEffectInstance);
				
				SoundEffectInstance = null;
			}
		}

		public static void PlaySfx(string id, float volume = 1, float pitch = 0, float pan = 0) {
			if (!Engine.Instance.Focused || !Assets.LoadSfx) {
				return;
			}
			
			PlaySfx(GetSfx(id), volume * (id.StartsWith("level_explosion") ? MasterVolume : SfxVolumeBuffer), pitch, pan);
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
			if (!Assets.LoadSfx) {
				return;
			}
			
			sfx?.Play(MathUtils.Clamp(0, 1, volume * SfxVolume * MasterVolume), pitch, pan);
		}
		
		public static void PlayMusic(string music, bool fromStart = false) {
			if (!Assets.LoadMusic) {
				return;
			}

			if (currentPlayingMusic == music && currentPlaying != null) {
				return;
			}

			if (!fromStart) {
				FadeOut(() => {
					LoadAndPlayMusic(music, fromStart);
				});
			} else {
				LoadAndPlayMusic(music, fromStart);
			}
		}

		private static bool loading;

		private static void LoadAndPlayMusic(string music, bool fromStart = false) {
			if (musicInstances.ContainsKey(music)) {
				ThreadLoad(music, fromStart);
			} else {
				new Thread(() => {
					ThreadLoad(music, fromStart);
				}).Start();
			}
		}

		public static void ThreadLoad(string music, bool fromStart = false) {
			try {
				loading = true;

				Log.Info($"Playing music {music} repeat = {Repeat}");
				currentPlayingMusic = music;

				if (!musicInstances.TryGetValue(music, out currentPlaying)) {
					currentPlaying = Assets.Content.Load<Song>($"bin/Music/{music}");
					musicInstances[music] = currentPlaying;
				}

				Repeat = true;

				MediaPlayer.Volume = 0;
				MediaPlayer.IsRepeating = Repeat;

				MediaPlayer.Play(currentPlaying);

				Tween.To(musicVolume, MediaPlayer.Volume, x => MediaPlayer.Volume = x, fromStart ? 0.05f : CrossFadeTime);

				loading = false;
			} catch (Exception e) {
				Log.Error($"Failed to load {music}");
				Log.Error(e);
				loading = false;
			}
		}

		public static void FadeOut(Action callback = null) {
			if (currentPlaying != null) {
				Tween.To(0, MediaPlayer.Volume, x => MediaPlayer.Volume = x, CrossFadeTime).OnEnd = () => {
					currentPlaying = null;
					currentPlayingMusic = null;

					MediaPlayer.Stop();
					callback?.Invoke();
				};
			} else {
				callback?.Invoke();
			}
		}
		
		public static void Stop() {
			MediaPlayer.Stop();

			currentPlaying = null;
			currentPlayingMusic = null;
		}

		private static float musicVolume = 1;

		public static void UpdateMusicVolume(float value) {
			MediaPlayer.Volume = value;
			musicVolume = value;
		}

		public static void Update(float dt) {
			if (SfxVolumeBufferResetTimer > 0) {
				SfxVolumeBufferResetTimer -= dt;

				if (SfxVolumeBufferResetTimer <= 0) {
					Tween.To(1, SfxVolumeBuffer, x => SfxVolumeBuffer = x, 0.3f);
				}
			}
		}
	}
}