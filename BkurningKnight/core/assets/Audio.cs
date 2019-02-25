using BurningKnight.core.util;

namespace BurningKnight.core.assets {
	public class Audio {
		public static Dictionary<string, Integer> Bpm = new Dictionary<>();
		public static Dictionary<string, Float> Volumes = new Dictionary<>();
		public static Music Current;
		public static bool Important;
		private static string Last = "";

		public static Void TargetAssets() {
			JsonReader Reader = new JsonReader();
			JsonValue Root = Reader.Parse(Gdx.Files.Internal("sfx/sfx.json"));

			foreach (JsonValue Name in Root) {
				Assets.Manager.Load("sfx/" + Name.ToString() + ".mp3", Sound.GetType());
				Volumes.Put(Name.ToString(), 1f);
			}

			Root = Reader.Parse(Gdx.Files.Internal("music/music.json"));

			foreach (JsonValue Name in Root) {
				Bpm.Put(Name.Name, Name.AsInt());
				Assets.Manager.Load("music/" + Name.Name + ".mp3", Music.GetType());
			}

			FileHandle File = Gdx.Files.External("sfx.json");

			if (File.Exists()) {
				Root = Reader.Parse(File);

				foreach (JsonValue Name in Root) {
					Log.Info("Set " + Name.Name + " to " + Name.AsFloat());
					Volumes.Put(Name.Name, Name.AsFloat());
				}
			} 
		}

		public static Void LoadAssets() {

		}

		public static Long PlaySfx(string Name, float Volume, float Pitch) {
			if (Name.StartsWith("menu") && !Settings.Uisfx) {
				return 0;
			} 

			Sound Sound = GetSound(Name);

			try {
				Long Id = Sound.Play(Volume * Volumes.Get(Name) * Settings.Sfx);
				Sound.SetPitch(Id, Pitch);

				return Id;
			} catch (GdxRuntimeException) {

			}

			return -1;
		}

		public static Sound GetSound(string Sfx) {
			Sound Sound = Assets.Manager.Get("sfx/" + Sfx + ".mp3", Sound.GetType());

			if (Sound == null) {
				Log.Error("Sfx '" + Sfx + "' is not found!");
			} 

			return Sound;
		}

		public static Music GetMusic(string Name) {
			Music Music = Assets.Manager.Get("music/" + Name + ".mp3", Music.GetType());

			if (Music == null) {
				Log.Error("Music '" + Name + "' is not found!");
			} 

			return Music;
		}

		public static Void Destroy() {

		}

		public static Void HighPriority(string Name) {
			if (Last.Equals(Name)) {
				return;
			} 

			Music Music = GetMusic(Name);

			if (Music == null) {
				Log.Error("Music '" + Name + "' is not found");

				return;
			} 

			FadeOut();

			try {
				Music.SetLooping(false);
				Music.Stop();
				Music.SetVolume(Settings.Music);
				Music.Play();
			} catch (GdxRuntimeException) {

			}

			Current = Music;
			Last = Name;
			Important = true;
		}

		public static Void Reset() {
			if (Current != null) {
				Current.Stop();
				Current.SetVolume(Settings.Music);
				Current.Play();
			} 
		}

		public static Void Play(string Name) {
			if (Name == null || Last.Equals(Name)) {
				return;
			} 

			if (Important) {
				if (!Current.IsPlaying()) {
					Important = false;
				} else {
					return;
				}

			} 

			Music Music = GetMusic(Name);

			if (Music == null) {
				Log.Error("Music '" + Name + "' is not found");

				return;
			} 

			Music.SetLooping(true);
			Music.SetVolume(0);
			Music.Play();
			Tween.To(new Tween.Task(Settings.Music, 0.5f) {
				public override float GetValue() {
					return Music.GetVolume();
				}

				public override Void SetValue(float Value) {
					Music.SetVolume(Value);
				}
			});
			FadeOut();
			Current = Music;
			Last = Name;
		}

		public static Void Stop() {
			if (Current == null) {
				return;
			} 

			Music M = Current;
			Last = "";
			Tween.To(new Tween.Task(0, 0.2f) {
				public override float GetValue() {
					return M.GetVolume();
				}

				public override Void SetValue(float Value) {
					M.SetVolume(Value);
				}
			});
		}

		private static Void FadeOut() {
			if (Current != null) {
				Music Last = Current;
				Tween.To(new Tween.Task(0, 0.5f) {
					public override float GetValue() {
						return Last.GetVolume();
					}

					public override Void SetValue(float Value) {
						Last.SetVolume(Value);
					}
				});
			} 
		}

		public static Void Update() {
			if (Current != null) {
				Current.SetVolume(Settings.Music);
			} 
		}

		public static Long PlaySfx(string Name) {
			return PlaySfx(Name, 1f, Name.StartsWith("menu") ? 1f : 0.95f + Random.NewFloat(0.1f));
		}

		public static Long PlaySfx(string Name, float Volume) {
			return PlaySfx(Name, Volume, Name.StartsWith("menu") ? 1f : 0.95f + Random.NewFloat(0.1f));
		}
	}
}
