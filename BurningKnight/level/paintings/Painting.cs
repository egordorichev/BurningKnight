using BurningKnight.assets.particle;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.save;
using Lens;
using Lens.entity;
using Lens.util.camera;
using Lens.util.file;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.paintings {
	public class Painting : SaveableEntity {
		public string Id;
		private Entity from;

		public override void PostInit() {
			base.PostInit();
			
			AddComponent(new InteractableComponent(Interact));
			AddComponent(new InteractableSliceComponent("paintings", $"{Id}_small"));
			var region = GetComponent<InteractableSliceComponent>().Sprite;

			Width = region.Width;
			Height = region.Height;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));

			Depth = Layers.Door;
			
			AddComponent(new HealthComponent {
				InitMaxHealth = 1,
				RenderInvt = true
			});
			
			AddComponent(new ExplodableComponent());
		}

		protected virtual bool Interact(Entity entity) {
			// TODO: open up the painting

			return true;
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(Id);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Id = stream.ReadString();
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev) {
				var h = GetComponent<HealthComponent>();

				if (h.Health + ev.Amount == 0) {
					from = ev.From;
				}
			}

			return base.HandleEvent(e);
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			if (from != null && TryGetComponent<HealthComponent>(out var h) && h.InvincibilityTimer <= 0.45f) {
				Done = true;
				
				for (var i = 0; i < 4; i++) {
					var part = new ParticleEntity(Particles.Dust());
						
					part.Position = Center;
					part.Particle.Scale = Lens.util.math.Random.Float(0.4f, 0.8f);
					
					Area.Add(part);

					part.Depth = Depth;
				}

				Particles.BreakSprite(Area, GetComponent<InteractableSliceComponent>().Sprite, Position, Depth);
				Camera.Instance.Shake(2f);
				Engine.Instance.Freeze = 1f;
			}
		}
	}
}