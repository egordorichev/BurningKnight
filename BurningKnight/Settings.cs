using BurningKnight.save;
using Lens;
using Lens.assets;

namespace BurningKnight {
	public class Settings {
		// Audio
		private static float musicVolume;
		public static float MusicVolume {
			get => musicVolume;

			set {
				musicVolume = value;
				Audio.UpdateMusicVolume(musicVolume);
			}
		}
		
		public static float SfxVolume;
		
		// Graphics
		public static bool Fullscreen;
		public static bool Vsync;
		public static bool Blood;
		public static bool Gore;
		public static bool UiSfx;
		public static string Cursor;
		public static bool RotateCursor;
		public static float FreezeFrames;
		public static float FlashFrames;
		public static bool ShowFps;

		// Game
		public static bool SpeedrunMode;
		public static bool SpeedrunTimer;
		public static float Screenshake;
		public static bool Vegan;
		public static bool Autosave;
	
		// Not saved
		public static bool HideUi;
		public static bool HideCursor;

		static Settings() {
			Setup();
		}

		private static void Setup() {
			Fullscreen = false;
			ShowFps = false;
			Blood = true;
			Gore = true;
			UiSfx = true;
			Vsync = true;
			Screenshake = 0.5f;
			SpeedrunMode = false;
			SpeedrunTimer = false;
			FreezeFrames = 0.5f;
			FlashFrames = 0.5f;
			SfxVolume = 0.75f;
			MusicVolume = 0.5f;
			Cursor = "default";
			RotateCursor = false;
			Vegan = false;
			Autosave = true;
		}
		
		public static void Load() {
			Fullscreen = GlobalSave.IsTrue("s_fullscreen");
			ShowFps = GlobalSave.IsTrue("s_fps");
			Blood = GlobalSave.IsTrue("s_blood");
			UiSfx = GlobalSave.IsTrue("s_uisfx");
			Gore = GlobalSave.IsTrue("s_gore");
			Vsync = GlobalSave.IsTrue("s_vsync");
			SpeedrunMode = GlobalSave.IsTrue("s_sm");
			SpeedrunTimer = GlobalSave.IsTrue("s_st");
			Screenshake = GlobalSave.GetFloat("s_screenshake");
			SfxVolume = GlobalSave.GetFloat("s_sfx");
			FreezeFrames = GlobalSave.GetFloat("s_frf");
			FlashFrames = GlobalSave.GetFloat("s_ff");
			MusicVolume = GlobalSave.GetFloat("s_music");
			Cursor = GlobalSave.GetString("s_cursor", "cursor-standart");
			RotateCursor = GlobalSave.IsTrue("s_rotate_cursor", true);
			Vegan = GlobalSave.IsTrue("s_v", false);
			Autosave = GlobalSave.IsTrue("s_as");

			if (Fullscreen) {
				Engine.Instance.SetFullscreen();
			} else {
				Engine.Instance.SetWindowed(Display.Width * 3, Display.Height * 3);
			}

			Engine.Graphics.SynchronizeWithVerticalRetrace = Vsync;
		}

		public static void Save() {
			GlobalSave.Put("s_fullscreen", Fullscreen);
			GlobalSave.Put("s_fps", ShowFps);
			GlobalSave.Put("s_blood", Blood);
			GlobalSave.Put("s_uisfx", UiSfx);
			GlobalSave.Put("s_gore", Gore);
			GlobalSave.Put("s_vsync", Vsync);
			GlobalSave.Put("s_sm", SpeedrunMode);
			GlobalSave.Put("s_st", SpeedrunTimer);
			GlobalSave.Put("s_frf", FreezeFrames);
			GlobalSave.Put("s_ff", FlashFrames);
			GlobalSave.Put("s_screenshake", Screenshake);
			GlobalSave.Put("s_sfx", SfxVolume);
			GlobalSave.Put("s_music", MusicVolume);
			GlobalSave.Put("s_cursor", Cursor);
			GlobalSave.Put("s_rotate_cursor", RotateCursor);
			GlobalSave.Put("s_v", Vegan);
			GlobalSave.Put("s_as", Autosave);
		}

		public static void Generate() {
			Setup();
			Save();
		}
	}
}