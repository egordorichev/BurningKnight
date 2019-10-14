using BurningKnight.assets;
using BurningKnight.entity.creature.mob;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;

namespace BurningKnight.entity.component {
	public class MobAnimationComponent : AnimationComponent {
		public MobAnimationComponent(string animationName, string layer = null, string tag = null) : base(animationName, layer, tag) {
			
		}

		public MobAnimationComponent(string animationName, ColorSet set) : base(animationName, set) {
			
		}

		public override void Render(bool shadow) {
			if (!shadow) {
				var m = (Mob) Entity;

				if (m.HasPrefix) {
					var p = m.Prefix;
					var pos = Entity.Position + Offset;
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					shader.Parameters["flash"].SetValue(1f);
					shader.Parameters["flashReplace"].SetValue(1f);
					shader.Parameters["flashColor"].SetValue(p.GetColor());

					foreach (var d in MathUtils.Directions) {
						CallRender(pos + d, false);
					}

					Shaders.End();
				}
			}

			base.Render(shadow);
		}
	}
}