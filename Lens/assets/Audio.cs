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
using VelcroPhysics;

namespace Lens.assets {
	public class Audio {
		public static float SfxVolume = 1;
		private const float CrossFadeTime = 0.5f;

		private static Music currentPlaying;
		private static string currentPlayingMusic;
		private static Dictionary<string, SoundEffectInstance> soundInstances = new Dictionary<string, SoundEffectInstance>();
		private static Dictionary<string, Music> musicInstances = new Dictionary<string, Music>();
		private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

		public static bool Repeat = true;
		public static DynamicSoundEffectInstance SoundEffectInstance;

		public static List<Music> Playing = new List<Music>();
		
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
			}
		}

		public static void StartThread() {
			new Thread(Update).Start();
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
			}

			quit = true;
		}

		public static void PlaySfx(string id, float volume = 1, float pitch = 0, float pan = 0) {
			if (!Engine.Instance.Focused || !Assets.LoadAudio) {
				return;
			}
			
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
			
			sfx?.Play(volume * SfxVolume, pitch, pan);
		}
		
		public static void PlayMusic(string music, bool fromStart = false) {
			if (!Assets.LoadAudio || loading) {
				return;
			}

			if (currentPlayingMusic == music && currentPlaying != null) {
				currentPlaying.Paused = false;
				currentPlaying.Volume = musicVolume;
				return;
			}
			
			FadeOut();
			LoadAndPlayMusic(music, fromStart);
		}

		private static bool loading;

		private static void LoadAndPlayMusic(string music, bool fromStart = false) {
			if (musicInstances.ContainsKey(music)) {
				ThreadLoad(music, true, fromStart);
			} else {
				new Thread(() => {
					ThreadLoad(music, true, fromStart);
				}).Start();
			}
		}

		public static void ThreadLoad(string music, bool play = true, bool fromStart = false) {		
			loading = true;

			if (!play) {
				musicInstances[music] = new Music($"Content/Music/{music}.ogg");
				loading = false;
				return;
			}

			currentPlayingMusic = music;

			if (!musicInstances.TryGetValue(music, out currentPlaying)) {
				currentPlaying = new Music($"Content/Music/{music}.ogg");
				musicInstances[music] = currentPlaying;
			}

			var mo = currentPlaying;

			currentPlaying.Volume = 0;
			currentPlaying.Repeat = Repeat;
			Repeat = true;			
			currentPlaying.Paused = false;

			if (fromStart) {
				position = 0;
			}

			Tween.To(musicVolume, mo.Volume, x => mo.Volume = x, fromStart ? 0.05f : CrossFadeTime).OnEnd = () => {
				if (currentPlaying == mo) {
					Playing.Clear();
					Playing.Add(currentPlaying);
				}
			};

			if (!Playing.Contains(currentPlaying)) {
				Playing.Add(currentPlaying);
			}
			
			loading = false;
		}

		public static void FadeOut() {
			if (currentPlaying != null) {
				var m = currentPlaying;
				
				Tween.To(0, m.Volume, x => m.Volume = x, CrossFadeTime).OnEnd = () => {
					m.Paused = true;
					Playing.Remove(m);
				};
				
				currentPlaying = null;
				currentPlayingMusic = null;
			}
		}
		
		public static void Stop() {
			position = 0;
			Playing.Clear();

			currentPlaying = null;
			currentPlayingMusic = null;
		}

		private static float musicVolume = 1;

		public static void UpdateMusicVolume(float value) {
			if (currentPlaying != null) {
				currentPlaying.Volume = value;
			}

			musicVolume = value;
		}

		private static bool loadedAll;
		private static List<string> toLoad = new List<string> {
			"Hub", "Shopkeeper", "Ma Precious", "Serendipity", "Nostalgia", "Gobbeon", "Fatiga", "Reckless", "Disk 1"
		};

		public static void Preload(string music) {
			if (!Assets.LoadAudio || toLoad.Contains(music) || musicInstances.ContainsKey(music)) {
				return;
			}
			
			Log.Info($"Added {music} to preloading");

			loadedAll = false;
			toLoad.Insert(0, music);
		}
		
		public static void UpdateAudio() {
			if (!Assets.LoadAudio || currentPlaying == null || loadedAll || loading) {
				return;
			}

			var name = toLoad[0];
			toLoad.RemoveAt(0);

			if (toLoad.Count == 0) {
				loadedAll = true;
			}

			var t = new Thread(() => { ThreadLoad(name, false); });

			t.Priority = ThreadPriority.BelowNormal;
			t.Start();
		}
		
		private const int BufferSize = 3000;
		private const int Channels = 2;
		private static byte[] byteBuffer = new byte[BufferSize * 2 * Channels];
		private static uint position;
		private static bool quit;

		private static void Update() {
			while (true) {
				if (quit) {
					return;
				}

				try {
					while (Playing.Count > 0 && SoundEffectInstance.PendingBufferCount < 3) {
						for (var i = 0; i < BufferSize; i++) {
							for (var c = 0; c < Channels; c++) {
								var floatSample = 0f;

								foreach (var p in Playing) {
									floatSample += p.GetSample(position, c);
								}

								floatSample = MathUtils.Clamp(-1f, 1f, floatSample);

								var shortSample =
									(short) (floatSample >= 0.0f ? floatSample * short.MaxValue : floatSample * short.MinValue * -1);

								var index = (i * Channels + c) * 2;

								if (!BitConverter.IsLittleEndian) {
									byteBuffer[index] = (byte) (shortSample >> 8);
									byteBuffer[index + 1] = (byte) shortSample;
								} else {
									byteBuffer[index] = (byte) shortSample;
									byteBuffer[index + 1] = (byte) (shortSample >> 8);
								}
							}

							position++;
						}

						SoundEffectInstance.SubmitBuffer(byteBuffer);
					}
				} catch (Exception e) {
					Log.Error(e);
				}

				Thread.Sleep(50);
			}
		}
	}
}