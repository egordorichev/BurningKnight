using Lens.entity.component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BurningKnight.entity.component {
	public class AudioEmitterComponent : Component {
		public AudioEmitter Emitter = new AudioEmitter();

		public override void Update(float dt) {
			base.Update(dt);
			Emitter.Position = new Vector3(Entity.CenterX, 0, Entity.CenterY);
		}
	}
}