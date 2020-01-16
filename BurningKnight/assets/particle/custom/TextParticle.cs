using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.custom {
	public class TextParticle : Entity {
		private string text;
		
		public string Text {
			get => text;
			
			set {
				if (tweened) {
					return;
				}

				text = value;
				fullText = $"{(HasSign ? (Negative ? "-" : "+") : "")}{(count > 0 ? $"{count} " : "")} {text}";

				var size = Font.Medium.MeasureString(fullText);

				origin = size / 2;
				Width = size.Width;
				gamePosition = start;

				scale.X = 0;
				scale.Y = 3;
				t = 0;

				Tween.To(1, scale.X, x => scale.X = x, 0.3f);
				Tween.To(1, scale.Y, x => scale.Y = x, 0.3f);
			}
		}

		private float count;

		public float Count {
			get => count;

			set {
				count = value;
				// To update the text
				Text = text;
			}
		}
		
		public bool HasSign;
		public bool Negative;
		public bool Stacks = true;
		
		private string fullText;
		private Vector2 start;
		private Vector2 origin;
		private Vector2 scale;
		private float t;
		private bool tweened;
		private Vector2 gamePosition;
		private Vector2 offset;
		
		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			AlwaysVisible = true;

			scale.X = 0;
			scale.Y = 0;

			Tween.To(1, scale.X, x => scale.X = x, 0.2f);
			Tween.To(1, scale.Y, x => scale.Y = x, 0.2f);

			offset.Y = 8;
			Tween.To(0, offset.Y, x => offset.Y = x, 0.3f);
			
			AddTag(Tags.TextParticle);
		}

		public override void PostInit() {
			base.PostInit();
			
			start = Center;
			gamePosition = Center;
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;

			if (!tweened && t >= 2f) {
				tweened = true;
				
				Tween.To(0, scale.X, x => scale.X = x, 0.15f, Ease.QuadIn);
				Tween.To(3, scale.Y, x => scale.Y = x, 0.15f, Ease.QuadIn).OnEnd = () => Done = true;
				Tween.To(-18, offset.Y, x => offset.Y = x, 0.15f, Ease.QuadIn);
			}

			Center = Camera.Instance.CameraToUi(gamePosition) + offset;
		}

		public override void Render() {
			Graphics.Print(fullText, Font.Medium, Center, 0, origin, scale);
		}

		public static TextParticle Add(Entity owner, string text, float count = 0, bool hasSign = false, bool minus = false) {
			var where = owner.TopCenter - new Vector2(0, 4);
			var min = 72f;
			TextParticle prt = null;
			
			foreach (var p in Engine.Instance.State.Ui.Tagged[Tags.TextParticle]) {
				var pr = (TextParticle) p;

				if (!pr.Stacks) {
					continue;
				}
				
				if (pr.text == text && pr.Negative == minus && !pr.tweened) {
					var d = (pr.gamePosition - where).Length();
					
					if(d < min) {
						min = d;
						prt = pr; 
					}
				} 
			}

			if (prt != null) {
				prt.Count += count;
				return prt;
			}
			
			var part = new TextParticle();
			part.BottomCenter = where;
			Engine.Instance.State.Ui.Add(part);
			
			part.HasSign = hasSign;
			part.Count = count;
			part.Negative = minus;
			part.Text = text;

			return part;
		}
	}
}