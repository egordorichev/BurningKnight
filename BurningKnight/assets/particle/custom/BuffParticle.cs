using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.custom {
	public class BuffParticle : Entity {
		public Buff Buff;
		public Entity Entity;
		public int Id;

		private TextureRegion region;
		private Vector2 scale = new Vector2(0, Display.UiScale * 3);
		private bool removing;
		
		public BuffParticle(Buff buff, Entity entity) {
			Buff = buff;
			Entity = entity;
			Id = entity.GetComponent<BuffsComponent>().Particles.Count;

			region = CommonAse.Ui.GetSlice(buff.GetIcon());
			Width = 8 * Display.UiScale;
			Height = region.Height * Display.UiScale;
			
			Tween.To(Display.UiScale, scale.X, x => scale.X = x, 0.3f);
			Tween.To(Display.UiScale, scale.Y, x => scale.Y = x, 0.3f);
		}
		
		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Entity.Done) {
				Remove();
			}
		}
		
		public void Remove() {
			if (removing) {
				return;
			}

			removing = true;
			
			Tween.To(Display.UiScale * 4, scale.X, x => scale.X = x, 0.3f);
			Tween.To(0, scale.Y, x => scale.Y = x, 0.3f).OnEnd = () => {
				Done = true;

				foreach (var p in Entity.GetComponent<BuffsComponent>().Particles) {
					if (p.Id > Id) {
						p.Id--;
						p.lastX -= (Width + 4) * 0.5f;
					}
				}
			};
		}

		private float lastX;

		public override void Render() {
			var origin = region.Center;
			var tar = (Entity.GetComponent<BuffsComponent>().Particles.Count - 1) * (Width + 4) * 0.5f;

			lastX += (tar - lastX) * Engine.Delta * 5;
			
			var x = Id * (Width + 4) - lastX;
			var pos = Entity.TopCenter;

			if (Entity.TryGetComponent<ZComponent>(out var z)) {
				pos += new Vector2(0, z.Z);
			}
			
			Center = Camera.Instance.CameraToUi(pos) + new Vector2(x, scale.X - Display.UiScale - 8);

			Graphics.Render(region, Position + origin, 0, origin, scale);
		}
	}
}