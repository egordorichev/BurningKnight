using System;
using System.Collections.Generic;
using System.Linq;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BurningKnight.entity.component {
	public class AudioEmitterComponent : Component {
		public static AudioListener Listener;
		public static Vector2 ListenerPosition;
		
		public static float PositionScale = 0.00000001f;
		public static float Distance = 200;
		
		public AudioEmitter Emitter = new AudioEmitter();
		public float PitchMod;
		public bool DestroySounds = false;

		public Dictionary<string, Sfx> Playing = new Dictionary<string, Sfx>();

		public class Sfx {
			public SoundEffectInstance Effect;
			public float BaseVolume = 1f;
			public bool KeepAround;
			public bool ApplyBuffer;
		}
		
		public override void Destroy() {
			base.Destroy();

			if (DestroySounds) {
				StopAll();
			}
		}

		public void StopAll() {
			foreach (var s in Playing.Values) {
				s.Effect.Stop();
			}

			Playing.Clear();
		}
		
		private void UpdatePosition() {
			Emitter.Position = new Vector3(Entity.CenterX * PositionScale, 0, Entity.CenterY * PositionScale);

			if (Listener != null) {
				var d = (ListenerPosition - Entity.Center).Length();

				foreach (var s in Playing.Values) {
					s.Effect.Volume = MathUtils.Clamp(0, 1, (1 - Math.Min(Distance, d) / Distance) * Settings.MasterVolume * Settings.SfxVolume * s.BaseVolume * (s.ApplyBuffer ? Audio.SfxVolumeBuffer : 1));
				}
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Playing.Count == 0) {
				return;
			}

			UpdatePosition();
			var keys = Playing.Keys.ToArray();
			
			foreach (var k in keys) {
				var s = Playing[k];

				if (!s.KeepAround && s.Effect.State != SoundState.Playing) {
					Playing.Remove(k);
				} else if (Listener != null) {
					s.Effect.Apply3D(Listener, Emitter);
				}
			}
		}

		public SoundEffectInstance EmitRandomizedPrefixed(string sfx, int prefixMax, float volume = 1f, bool insert = true, bool looped = false, bool tween = false, float sz = 0.4f) {
			if (sfx == null) {
				return null;
			}
		
			return Emit($"{sfx}_{Rnd.Int(1, prefixMax + 1)}", volume, PitchMod + Rnd.Float(-sz, sz), insert, looped, tween);
		}
		
		public SoundEffectInstance EmitRandomized(string sfx, float volume = 1f, bool insert = true, bool looped = false, bool tween = false, float sz = 0.4f) {
			if (sfx == null) {
				return null;
			}
			
			return Emit(sfx, volume, PitchMod + Rnd.Float(-sz, sz), insert, looped, tween);
    }

		public SoundEffectInstance Emit(string sfx, float volume = 1f, float pitch = 0f, bool insert = true, bool looped = false, bool tween = false) {
			if (!Assets.LoadSfx || sfx == null) {
				return null;
			}

			Sfx instance;
			var v = volume * 0.8f;

			if (!insert) {
				v *= Audio.MasterVolume * Audio.SfxVolume * Audio.SfxVolumeBuffer;
			}
			
			var applyBuffer = !sfx.StartsWith("level_explosion");

			/*if (applyBuffer) {
				v *= Audio.SfxVolumeBuffer;
			}*/

			if (!insert || !Playing.TryGetValue(sfx, out instance)) {
				var sound = Audio.GetSfx(sfx);

				if (sound == null) {
					return null;
				}

				instance = new Sfx {
					Effect = sound.CreateInstance(),
					KeepAround = tween,
					ApplyBuffer = applyBuffer
				};

				if (insert) {
					Playing[sfx] = instance;
				}

				instance.Effect.IsLooped = looped;
			}

			instance.BaseVolume = tween ? 0 : v;

			if (tween) {
				var t = Tween.To(v, 0, x => instance.BaseVolume = x, 0.5f);

				t.Delay = 1f;
				t.OnStart = () => {
					instance.Effect.Play();
					instance.KeepAround = false;
					instance.Effect.Apply3D(Listener, Emitter);
				};
			}

			UpdatePosition();
			
			instance.Effect.Stop();
			instance.Effect.Pitch = MathUtils.Clamp(-1f, 1f, pitch);

			if (!tween) {
				instance.Effect.Play();
			}

			if (Listener != null) {
				instance.Effect.Apply3D(Listener, Emitter);
			}
			
			return instance.Effect;
		}

		public static AudioEmitterComponent Dummy(Area area, Vector2 where) {
			var entity = new EmitterDummy();
			var component = new AudioEmitterComponent();

			area.Add(entity);
			entity.AddComponent(component);
			entity.Center = where;
			entity.AlwaysActive = true;

			return component;
		}

		private class EmitterDummy : Entity {
			public override void Update(float dt) {
				base.Update(dt);
				
				if (GetComponent<AudioEmitterComponent>().Playing.Count == 0) {
					Done = true;
				}
			}
		}
	}
}
