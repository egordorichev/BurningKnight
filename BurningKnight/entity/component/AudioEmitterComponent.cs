using System.Collections.Generic;
using System.Linq;
using Lens.assets;
using Lens.entity;
using Lens.entity.component;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BurningKnight.entity.component {
	public class AudioEmitterComponent : Component {
		public static AudioListener Listener;
		public static float PositionScale = 0.2f;
		public AudioEmitter Emitter = new AudioEmitter();
		public float PitchMod;
		public bool DestroySounds = false;

		private Dictionary<string, SoundEffectInstance> Playing = new Dictionary<string, SoundEffectInstance>();

		public override void Destroy() {
			base.Destroy();

			if (DestroySounds) {
				foreach (var s in Playing.Values) {
					s.Stop();
				}

				Playing.Clear();
			}
		}

		private void UpdatePosition() {
			Emitter.Position = new Vector3(Entity.CenterX * PositionScale, 0, Entity.CenterY * PositionScale);
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

				if (s.State != SoundState.Playing) {
					Playing.Remove(k);
				} else if (Listener != null) {
					s.Apply3D(Listener, Emitter);
				}
			}
		}

		public SoundEffectInstance EmitRandomizedPrefixed(string sfx, int prefixMax, float volume = 1f, bool insert = true) {
			if (sfx == null) {
				return null;
			}
		
			return Emit($"{sfx}_{Rnd.Int(1, prefixMax + 1)}", volume, PitchMod + Rnd.Float(-0.4f, 0.4f), insert);
		}
		
		public SoundEffectInstance EmitRandomized(string sfx, float volume = 1f, bool insert = true) {
			if (sfx == null) {
				return null;
			}
			
			return Emit(sfx, volume, PitchMod + Rnd.Float(-0.4f, 0.4f), insert);
    }

		public SoundEffectInstance Emit(string sfx, float volume = 1f, float pitch = 1f, bool insert = true) {
			if (!Assets.LoadAudio || sfx == null) {
				return null;
			}

			SoundEffectInstance instance;

			if (!insert || !Playing.TryGetValue(sfx, out instance)) {
				var sound = Audio.GetSfx(sfx);

				if (sound == null) {
					return null;
				}
				
				instance = sound.CreateInstance();

				if (insert) {
					Playing[sfx] = instance;
				}
			}

			UpdatePosition();
			instance.Stop();
			instance.Volume = volume * Settings.SfxVolume;
			instance.Pitch = MathUtils.Clamp(-1f, 1f, pitch);
			instance.Play();
			
			if (Listener != null) {
				instance.Apply3D(Listener, Emitter);
			}
			
			return instance;
		}

		public static AudioEmitterComponent Dummy(Area area, Vector2 where) {
			var entity = new Entity();
			var component = new AudioEmitterComponent();

			area.Add(entity);
			entity.AddComponent(component);
			entity.Center = where;

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