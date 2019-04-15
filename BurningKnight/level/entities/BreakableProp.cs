using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using Lens.util;
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
		
		public static BreakableProp Random() {
			return new BreakableProp {
				Sprite = Infos[Lens.util.math.Random.Int(Infos.Length)]
			};
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new HealthComponent {
				InitMaxHealth = Lens.util.math.Random.Int(1, 3),
				RenderInvt = true
			});

			AddComponent(new ExplodableComponent());
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		protected override Rectangle GetCollider() {
			var rect = GetComponent<SliceComponent>().Sprite.Source;

			Width = rect.Width;
			Height = rect.Height;
			
			return new Rectangle(0, 0, (int) Width, (int) Height);
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev) {
				var h = GetComponent<HealthComponent>();
				
				if (h.Health == 1) {
					h.Kill(ev.From);
				}
			} else if (e is DiedEvent) {
				Done = true;
				
				for (var i = 0; i < 4; i++) {
					var part = new ParticleEntity(Particles.Dust());
						
					part.Position = Center;
					part.Particle.Scale = Lens.util.math.Random.Float(0.4f, 0.8f);
					
					Area.Add(part);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}