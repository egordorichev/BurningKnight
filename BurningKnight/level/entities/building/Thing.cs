using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.physics;
using BurningKnight.state;
using BurningKnight.ui.editor;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities.building {
	public class Thing : Prop, CollisionFilterEntity {
		private TextureRegion shadow;
		private bool separateShadow;
		private bool hasShadow = true;
		private string file = "";
		private string sprite = "";
		private bool hasBody = true;
		private Rectangle collider;

		private bool displayCollider = true;
		
		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new ShadowComponent(RenderShadow));
		}

		public override void PostInit() {
			base.PostInit();

			if (file != null && sprite != null) {
				AddComponent(new SliceComponent(file, sprite));
			}

			if (!Engine.EditingLevel && hasBody) {
				AddComponent(new RectBodyComponent(collider.X, collider.Y, collider.Width, collider.Height, BodyType.Static));
			}
		}

		private void UpdateSprite() {
			var f = Animations.Get(file);

			if (f == null) {
				return;
			}
			
			var sp = f.GetSlice(sprite);

			if (!HasComponent<SliceComponent>()) {
				AddComponent(new SliceComponent(file, sprite));
			} else {
				GetComponent<SliceComponent>().Sprite = sp;
			}

			if (sp != null) {
				Width = sp.Width;
				Height = sp.Height;
			}
			
			if (separateShadow) {
				shadow = f.GetSlice($"{sprite}_shadow");
			}
		}

		private void RenderShadow() {
			if (!hasShadow) {
				return;
			}

			if (separateShadow) {
				Graphics.Render(shadow, Position + new Vector2(0,  Height + shadow.Height), 0, Vector2.Zero, MathUtils.InvertY);
			} else {
				GraphicsComponent?.Render(true);
			}
		}

		public virtual bool ShouldCollide(Entity entity) {
			return !(entity is Creature c && c.InAir());
		}

		public override void Render() {
			base.Render();

			if (Engine.EditingLevel && displayCollider && hasBody) {
				Graphics.Batch.DrawRectangle(new RectangleF(X + collider.X, Y + collider.Y, collider.Width, collider.Height), Color.Red);
			}
		}

		public override void RenderImDebug() {
			if (ImGui.InputText("File", ref file, 128)) {
				UpdateSprite();
			}
		
			if (ImGui.InputText("Slice", ref sprite, 128)) {
				UpdateSprite();
			}
			
			ImGui.Checkbox("Has shadow", ref hasShadow);
			ImGui.Checkbox("Custom shadow sprite", ref separateShadow);
			ImGui.Separator();
			ImGui.Checkbox("Has body", ref hasBody);
			
			if (hasBody) {
				var x = collider.X;
				var y = collider.Y;
				var w = collider.Width;
				var h = collider.Height;

				if (ImGui.InputInt("X", ref x)) {
					collider.X = x;
				}
				
				if (ImGui.InputInt("Y", ref y)) {
					collider.Y = y;
				}
				
				if (ImGui.InputInt("W", ref w)) {
					collider.Width = w;
				}
				
				if (ImGui.InputInt("H", ref h)) {
					collider.Height = h;
				}
			
				ImGui.Checkbox("Display collider", ref displayCollider);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			hasShadow = stream.ReadBoolean();
			hasBody = stream.ReadBoolean();

			if (hasBody) {
				collider.X = stream.ReadInt16();
				collider.Y = stream.ReadInt16();
				collider.Width = stream.ReadInt16();
				collider.Height = stream.ReadInt16();
			}

			file = stream.ReadString();
			sprite = stream.ReadString();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteBoolean(hasShadow);
			stream.WriteBoolean(hasBody);

			if (hasBody) {
				stream.WriteInt16((short) collider.X);
				stream.WriteInt16((short) collider.Y);
				stream.WriteInt16((short) collider.Width);
				stream.WriteInt16((short) collider.Height);
			}
			
			stream.WriteString(file);
			stream.WriteString(sprite);
		}
	}
}