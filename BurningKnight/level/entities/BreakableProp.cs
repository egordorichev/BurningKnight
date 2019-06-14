using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using Lens;
using Lens.entity;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities {
	public class BreakableProp : SolidProp {
		public static string[] Infos = {
			"pot_a",
			"pot_b",
			"pot_c",
			"pot_d",
			"pot_e",
			
			"cactus",
			"cup",
			
			"chair_a",
			"chair_b",
			"chair_c",
			
			"crate_a",
			"crate_b"
		};

		private Entity from;
		private bool hurts;
		
		public static BreakableProp Random() {
			return new BreakableProp {
				Sprite = Infos[Lens.util.math.Random.Int(Infos.Length)]
			};
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new HealthComponent {
				InitMaxHealth = 1,
				RenderInvt = true,
				AutoKill = false
			});

			AddComponent(new ExplodableComponent());
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		protected override Rectangle GetCollider() {
			var rect = GetComponent<SliceComponent>().Sprite.Source;

			if (Sprite.Contains("pot") || Sprite.Contains("crate")) {
				AddComponent(new PoolDropsComponent(ItemPool.Crate, 0.3f, 1, 3));
			}
			
			hurts = Sprite == "cactus";

			Width = rect.Width;
			Height = rect.Height;
			
			return new Rectangle(hurts ? 2 : 0, 0, (int) (hurts ? Width - 4 : Width), (int) (hurts ? Height - 6 : Height));
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev) {
				var h = GetComponent<HealthComponent>();
				
				if (h.Health + ev.Amount == 0) {
					from = ev.From;
				}
			} else if (e is CollisionStartedEvent c && hurts) {
				if (c.Entity is Player) {
					c.Entity.GetComponent<HealthComponent>().ModifyHealth(-1, this);
					c.Entity.GetAnyComponent<BodyComponent>().KnockbackFrom(this, 1);
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (from != null && TryGetComponent<HealthComponent>(out var h) && h.InvincibilityTimer <= 0.45f) {
				Done = true;

				if (!Camera.Instance.Overlaps(this)) {
					return;
				}

				if (TryGetComponent<PoolDropsComponent>(out var pool)) {
					pool.SpawnDrops();
				}
				
				for (var i = 0; i < 4; i++) {
					var part = new ParticleEntity(Particles.Dust());
						
					part.Position = Center;
					part.Particle.Scale = Lens.util.math.Random.Float(0.4f, 0.8f);
					
					Area.Add(part);
				}

				Particles.BreakSprite(Area, GetComponent<SliceComponent>().Sprite, Position);
				Camera.Instance.Shake(2f);
				Engine.Instance.Freeze = 1f;
			}
		}
	}
}