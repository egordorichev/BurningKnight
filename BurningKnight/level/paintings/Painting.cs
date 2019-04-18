using System;
using BurningKnight.assets;
using BurningKnight.assets.particle;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.save;
using BurningKnight.state;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.paintings {
	public class Painting : SaveableEntity {
		public string Id;
		public string Author;
		
		private Entity from;
		private float scale;
		private float uiY;

		private string name;
		private string author;
		
		private float nameWidth;
		private float authorWidth;

		private TextureRegion big;
		
		protected virtual TextureRegion GetRegion() {
			return big;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new HealthComponent {
				InitMaxHealth = 1,
				RenderInvt = true
			});

			AddComponent(new ExplodableComponent());
			AddComponent(new InteractableComponent(Interact));
		}

		public override void PostInit() {
			base.PostInit();

			if (!HasComponent<AnimationComponent>()) {
				big = Animations.Get("paintings").GetSlice(Id);
			}
			
			AddComponent(new InteractableSliceComponent("paintings", $"{Id}_small"));
			var region = GetComponent<InteractableSliceComponent>().Sprite;

			Width = region.Width;
			Height = region.Height;
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));

			Depth = Layers.Door;
			
			name = Locale.Get($"painting_{Id}");
			author = $"{Locale.Get("by")} {Author}";

			nameWidth = Font.Medium.MeasureString(name).Width;
			authorWidth = Font.Small.MeasureString(author).Width;
		}

		protected virtual bool Interact(Entity entity) {
			((InGameState) Engine.Instance.State).CurrentPainting = this;

			uiY = Display.UiHeight;
			
			Tween.To(1, 0, x => scale = x, 1.1f, Ease.BackOut);
			Tween.To(0, uiY, x => uiY = x, 0.8f, Ease.BackOut);
			
			return false;
		}

		public void Remove() {
			if (uiY > 1f) {
				return;
			}
			
			Tween.To(0, 1, x => scale = x, 0.2f);
			Tween.To(Display.Height, uiY, x => uiY = x, 0.2f).OnEnd = () => {
				((InGameState) Engine.Instance.State).CurrentPainting = null;
			};
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteString(Id);
			stream.WriteString(Author);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Id = stream.ReadString();
			Author = stream.ReadString();
		}

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent ev) {
				var h = GetComponent<HealthComponent>();

				if (h.Health + ev.Amount == 0 && Id != "egor") {
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

		public virtual void RenderUi() {
			var region = GetRegion();
			var sc = (float) Math.Max(1, Math.Floor(Math.Min((Display.Width - 96f) / region.Width, (Display.Height - 96f) / region.Height)));
			
			Graphics.Render(region, new Vector2(Display.UiWidth / 2f, Display.UiHeight / 2f + uiY), 
				(float) (Math.Cos(Engine.Time) * 0.1f),
				region.Center, new Vector2(scale * sc));
			
			Graphics.Print(name, Font.Medium, 
				new Vector2(Display.UiWidth / 2f, Display.UiHeight / 2f + uiY + region.Height * 0.5f * sc + 16f), 0,
				new Vector2(nameWidth / 2, 8), new Vector2(scale));
			
			Graphics.Print(author, Font.Small, 
				new Vector2(Display.UiWidth / 2f, Display.UiHeight / 2f + uiY + region.Height * 0.5f * sc + 28f), 0,
				new Vector2(authorWidth / 2, 8), new Vector2(scale));
		}
	}
}