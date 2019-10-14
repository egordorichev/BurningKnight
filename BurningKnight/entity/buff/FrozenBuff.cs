using BurningKnight.entity.component;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.buff {
	public class FrozenBuff : Buff {
		public const string Id = "bk:frozen";
		public static Vector4 Color = new Vector4(0.5f, 0.5f, 1f, 1f);
		
		public FrozenBuff() : base(Id) {
			Duration = 3;
		}

		public override void Init() {
			base.Init();
			Entity.GetComponent<BuffsComponent>().Remove<BurningBuff>();

			if (Entity.TryGetComponent<StateComponent>(out var s)) {
				s.Pause++;
			}

			var a = Entity.GetAnyComponent<AnimationComponent>();

			if (a != null) {
				a.Animation.Paused = true;
			}
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			var body = Entity.GetAnyComponent<BodyComponent>();

			if (body != null) {
				body.Velocity -= body.Velocity * (dt * 20);
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (Entity.TryGetComponent<StateComponent>(out var s)) {
				s.Pause--;
			}

			var a = Entity.GetAnyComponent<AnimationComponent>();

			if (a != null) {
				a.Animation.Paused = false;
			}
		}
	}
}