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
		public static float MasterVolume = 1;
		public static float SfxVolume = 1;
		public static float SfxVolumeBuffer = 1f;
		public static float SfxVolumeBufferResetTimer = 0;
		private const float CrossFadeTime = 0.5f;

		public static float Db3 = 0.1f;

		private static Music currentPlaying;
		private static string currentPlayingMusic;
		private static Dictionary<string, Music> musicInstances = new Dictionary<string, Music>();
		private static Dictionary<string, SoundEffect> sounds = new Dictionary<string, SoundEffect>();

		public static bool Repeat = true;
		public static DynamicSoundEffectInstance SoundEffectInstance;

		public static List<Music> Playing = new List<Music>();
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
				GC.SuppressFinalize(sound);
			}

			if (SoundEffectInstance != null) {
				Log.Info("Disposing audio mixer");

				SoundEffectInstance.Stop();
				SoundEffectInstance.Dispose();
				GC.SuppressFinalize(SoundEffectInstance);
				
				SoundEffectInstance = null;
			}

			quit = true;
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
				currentPlaying.Paused = false;
				currentPlaying.Volume = musicVolume;
				return;
			}

			if (!fromStart) {
				FadeOut();
			}

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
			try {
				loading = true;

				if (!play) {
					musicInstances[music] = new Music($"Content/Music/{music}.ogg");
					loading = false;

					return;
				}

				Log.Info($"Playing music {music} repeat = {Repeat}");
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
			} catch (Exception e) {
				Log.Error($"Failed to load {music}");
				Log.Error(e);
				loading = false;
			}
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
			"Hub", "Shopkeeper", "Ma Precious", "Serendipity", "Nostalgia", "Gobbeon", "Fatiga", "Reckless", "piano"
		};

		public static void Preload(string music) {
			if (!Assets.LoadMusic || toLoad.Contains(music) || musicInstances.ContainsKey(music)) {
				return;
			}
			
			Log.Info($"Added {music} to preloading");

			loadedAll = false;
			toLoad.Insert(0, music);
		}
		
		public static void UpdateAudio() {
			if (!Assets.LoadMusic || currentPlaying == null || loadedAll || loading) {
				return;
			}

			var name = toLoad[0];
			toLoad.RemoveAt(0);

			if (toLoad.Count == 0) {
				loadedAll = true;
			}

			try {
				var t = new Thread(() => {
					try {
						ThreadLoad(name, false);
					} catch (Exception e) {
						Log.Error(e);
					}
				});

				t.Priority = ThreadPriority.BelowNormal;
				t.Start();
			} catch (Exception e) {
				Log.Error(e);
			}
		}
		
		private const int BufferSize = 3000;
		private const int Channels = 2;
		private static byte[] byteBuffer = new byte[BufferSize * 2 * Channels];
		private static float position;
		private static bool quit;
		
		public static void Update(float dt) {
			if (SfxVolumeBufferResetTimer > 0) {
				SfxVolumeBufferResetTimer -= dt;

				if (SfxVolumeBufferResetTimer <= 0) {
					Tween.To(1, SfxVolumeBuffer, x => SfxVolumeBuffer = x, 0.3f);
				}
			}
		}

		public static void SubmitBuffer(object o, EventArgs e) {
			if (SoundEffectInstance == null) {
				return;
			}
			
			try {
				while (SoundEffectInstance.PendingBufferCount < 3) {
					for (var i = 0; i < BufferSize; i++) {
						var ps = (uint) Math.Floor(position);

						for (var c = 0; c < Channels; c++) {
							var floatSample = 0f;

							for (var z = 0; z < Playing.Count; z++) {
								floatSample += Playing[z].GetSample(ps, c) * SfxVolumeBuffer;
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

						position += Speed;

						if (Playing.Count == 1 && Playing[0].Repeat) {
							var l = Playing[0].BufferLength;

							if (position >= l) {
								position -= l;
							}
						}

						if (position < 0) {
							position = 0;
						}
					}

					SoundEffectInstance.SubmitBuffer(byteBuffer);
				}
			} catch (Exception er) {
				Log.Error(er);
			}
		}
	}
}