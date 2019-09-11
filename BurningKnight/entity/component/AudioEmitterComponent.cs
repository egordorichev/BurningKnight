using System.Collections.Generic;
using System.Linq;
using Lens.assets;
using Lens.entity.component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BurningKnight.entity.component {
	public class AudioEmitterComponent : Component {
		public static AudioListener Listener;
		
		public AudioEmitter Emitter = new AudioEmitter();
		private Dictionary<string, SoundEffectInstance> Playing = new Dictionary<string, SoundEffectInstance>();

		public override void Destroy() {
			base.Destroy();

			foreach (var s in Playing.Values) {
				s.Stop();
			}

			Playing.Clear();
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			Emitter.Position = new Vector3(Entity.CenterX, 0, Entity.CenterY);

			if (Playing.Count == 0) {
				return;
			}

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

		public SoundEffectInstance Emit(string sfx, float volume = 1f) {
			SoundEffectInstance instance;

			if (!Playing.TryGetValue(sfx, out instance)) {
				var sound = Audio.GetSfx(sfx);

				if (sound == null) {
					return null;
				}
				
				instance = sound.CreateInstance();
				Playing[sfx] = instance;
			}
			
			instance.Stop();
			instance.Volume = volume * Settings.SfxVolume;

			if (Listener != null) {
				instance.Apply3D(Listener, Emitter);
			}
			
			instance.Play();

			return instance;
		}
	}
}