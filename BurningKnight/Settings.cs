using BurningKnight.level;
using BurningKnight.save;
using Lens;
using Lens.assets;
using Lens.entity.component.logic;
using Lens.graphics.gamerenderer;
using Microsoft.Xna.Framework;

namespace BurningKnight {
	public class Settings {
		// Audio
		public static float MasterVolume {
			get => Audio.MasterVolume;

			set {
				Audio.MasterVolume = value;
				Audio.UpdateMusicVolume(Audio.MasterVolume * musicVolume);
			}
		}
		
		private static float musicVolume;
		public static float MusicVolume {
			get => musicVolume;

			set {
				musicVolume = value;
				Audio.UpdateMusicVolume(Audio.MasterVolume * musicVolume);
			}
		}

		public static float SfxVolume {
			get => Audio.SfxVolume;
			set => Audio.SfxVolume = value;
		}
		
		// Graphics
		public static bool Fullscreen;
		public static bool Vsync;
		public static bool Blood;
		public static bool UiSfx;
		public static bool Minimap;
		public static int Cursor;
		public static bool RotateCursor;
		public static float FreezeFrames;
		public static float FlashFrames;
		public static bool ShowFps;
		public static bool Flashes;
		public static bool LowQuality;

		public static bool PixelPerfect {
			get => Engine.PixelPerfect;
			set => Engine.PixelPerfect = value;
		}

		private static float floorDarkness;

		public static float FloorDarkness {
			get => floorDarkness;

			set {
				floorDarkness = value;
				Level.FloorColor = new Color(floorDarkness, floorDarkness, floorDarkness, 1f);
			}
		}

		public static float GameScale {
			get => PixelPerfectGameRenderer.GameScale;
			set => PixelPerfectGameRenderer.GameScale = value;
		}

		// Game
		public static bool SpeedrunMode;
		public static bool SpeedrunTimer;
		public static float Screenshake;
		public static bool Vegan;
		public static bool Autopause;
		public static bool Autosave;
		public static string Gamepad;
		public static bool Vibrate;
		public static float Sensivity;
		public static float CursorRadius;
		public static string Language;
	
		// Not saved
		public static bool HideUi;
		public static bool HideCursor;

		static Settings() {
			Setup();
		}

		public static void Setup() {
			Fullscreen = true;
			ShowFps = false;
			Blood = true;
			UiSfx = true;
			Vsync = true;
			Screenshake = 1f;
			SpeedrunMode = false;
			SpeedrunTimer = false;
			FreezeFrames = 0.5f;
			FlashFrames = 0.5f;
			SfxVolume = 0.8f;
			MusicVolume = 0.2f;
			Audio.MasterVolume = 1f;
			Cursor = 0;
			RotateCursor = false;
			Vegan = false;
			Autopause = false;
			Autosave = true;
			Gamepad = null;
			Vibrate = true;
			Sensivity = 1.5f;
			GameScale = 1f;
			FloorDarkness = 1f;
			PixelPerfect = false;
			CursorRadius = 1f;
			Minimap = true;
			Flashes = true;
			LowQuality = false;
			Language = Locale.PrefferedClientLanguage;
			
			ShakeComponent.Modifier = Screenshake;
			Engine.FreezeModifier = FreezeFrames;
			Engine.FlashModifier = FlashFrames;
			Engine.Flashes = Flashes;
		}
		
		public static void Load() {
			Fullscreen = GlobalSave.IsTrue("s_fullscreen");
			ShowFps = GlobalSave.IsTrue("s_fps");
			Blood = GlobalSave.IsTrue("s_blood");
			UiSfx = GlobalSave.IsTrue("s_uisfx");
			Vsync = GlobalSave.IsTrue("s_vsync");
			SpeedrunMode = GlobalSave.IsTrue("s_sm");
			SpeedrunTimer = GlobalSave.IsTrue("s_stmr");
			Screenshake = GlobalSave.GetFloat("s_screenshake");
			SfxVolume = GlobalSave.GetFloat("s_sfx");
			FreezeFrames = GlobalSave.GetFloat("s_frf");
			FlashFrames = GlobalSave.GetFloat("s_ff");
			MusicVolume = GlobalSave.GetFloat("s_music");
			MasterVolume = GlobalSave.GetFloat("s_master");
			Cursor = GlobalSave.GetInt("s_cursor");
			RotateCursor = GlobalSave.IsTrue("s_rotate_cursor", true);
			Vegan = GlobalSave.IsTrue("s_v", false);
			Autosave = GlobalSave.IsTrue("s_as");
			Autopause = GlobalSave.IsTrue("s_ap");
			Gamepad = GlobalSave.GetString("s_gp");
			Vibrate = GlobalSave.IsTrue("s_vb", true);
			Minimap = GlobalSave.IsTrue("s_mm", true);
			Flashes = GlobalSave.IsTrue("s_fl", true);
			LowQuality = GlobalSave.IsTrue("s_lq", false);
			Sensivity = GlobalSave.GetFloat("s_ss", 1);
			GameScale = GlobalSave.GetFloat("s_gs", 1);
			FloorDarkness = GlobalSave.GetFloat("s_fd", 1);
			PixelPerfect = GlobalSave.IsTrue("s_pp", false);
			CursorRadius = GlobalSave.GetFloat("s_cr", 1);
			Language = GlobalSave.GetString("s_ln", Locale.PrefferedClientLanguage);
			Locale.Load(Language);

			ShakeComponent.Modifier = Screenshake;
			Engine.FreezeModifier = FreezeFrames;
			Engine.FlashModifier = FlashFrames;
			Engine.Flashes = Flashes;
		}

		public static void Save() {
			GlobalSave.Put("s_fullscreen", Fullscreen);
			GlobalSave.Put("s_fps", ShowFps);
			GlobalSave.Put("s_blood", Blood);
			GlobalSave.Put("s_uisfx", UiSfx);
			GlobalSave.Put("s_vsync", Vsync);
			GlobalSave.Put("s_sm", SpeedrunMode);
			GlobalSave.Put("s_stmr", SpeedrunTimer);
			GlobalSave.Put("s_frf", FreezeFrames);
			GlobalSave.Put("s_ff", FlashFrames);
			GlobalSave.Put("s_screenshake", Screenshake);
			GlobalSave.Put("s_sfx", SfxVolume);
			GlobalSave.Put("s_music", MusicVolume);
			GlobalSave.Put("s_master", MasterVolume);
			GlobalSave.Put("s_cursor", Cursor);
			GlobalSave.Put("s_rotate_cursor", RotateCursor);
			GlobalSave.Put("s_v", Vegan);
			GlobalSave.Put("s_as", Autosave);
			GlobalSave.Put("s_ap", Autopause);
			GlobalSave.Put("s_gp", Gamepad);
			GlobalSave.Put("s_vb", Vibrate);
			GlobalSave.Put("s_ss", Sensivity);
			GlobalSave.Put("s_gs", GameScale);
			GlobalSave.Put("s_fd", FloorDarkness);
			GlobalSave.Put("s_pp", PixelPerfect);
			GlobalSave.Put("s_cr", CursorRadius);
			GlobalSave.Put("s_ln", Language);
			GlobalSave.Put("s_mm", Minimap);
			GlobalSave.Put("s_fl", Flashes);
			GlobalSave.Put("s_lq", LowQuality);
		}

		public static void Generate() {
			Setup();
			Save();
		}
	}
}