using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.physics;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.util.file;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.chest {
	public class Chest : Prop, CollisionFilterEntity {
		private bool open;
		protected internal float Scale = 1;

		public bool CanOpen = true;
		public bool Empty;
		
		protected virtual Rectangle GetCollider() {
			return new Rectangle(0, (int) (5 * Scale), (int) (Math.Max(1, 17 * Scale)), (int) (Math.Max(1, 8 * Scale)));
		}
		
		protected virtual BodyComponent CreateBody() {
			var collider = GetCollider();
			return new RectBodyComponent(collider.X, collider.Y, collider.Width, collider.Height);
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 16 * Scale;
			Height = 13 * Scale;

			AddGraphics();
			AddComponent(new SensorBodyComponent(-2, -2, Width + 4, Height + 4));
			AddComponent(new DropsComponent());
			AddComponent(new ShadowComponent());
			AddComponent(new RoomComponent());
			AddComponent(new AudioEmitterComponent());
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => CanOpen && !open
			});
			
			AddTag(Tags.Chest);
			AddTag(Tags.Item);
			
			DefineDrops();
		}

		protected virtual void AddGraphics() {
			AddComponent(new InteractableSliceComponent("props", GetSprite()));
		}

		protected virtual void DefineDrops() {
			
		}

		public override void PostInit() {
			base.PostInit();
			AddComponent(CreateBody());

			if (open) {
				UpdateSprite();
			}

			var body = GetComponent<RectBodyComponent>().Body;

			body.LinearDamping = 100;
			body.Mass = 1000000;

			GetComponent<RectBodyComponent>().KnockbackModifier = 0.1f;
			Animate();
		}
		
		protected virtual void Animate() {
			var a = GetComponent<InteractableSliceComponent>();

			a.Scale.X = 0.6f * Scale;
			a.Scale.Y = 1.7f * Scale;
					
			Tween.To(1.8f * Scale, a.Scale.X, x => a.Scale.X = x, 0.15f);
			Tween.To(0.2f * Scale, a.Scale.Y, x => a.Scale.Y = x, 0.15f).OnEnd = () => {
				Tween.To(Scale, a.Scale.X, x => a.Scale.X = x, 0.2f);
				Tween.To(Scale, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
			};
		}

		protected virtual string GetSprite() {
			return "chest";
		}

		protected virtual void UpdateSprite() {
			GetComponent<InteractableSliceComponent>().Sprite = CommonAse.Props.GetSlice($"{GetSprite()}_open");
		}

		public void Open() {
			if (open) {
				return;
			}

			open = true;
			AnimateOpening();

			HandleEvent(new OpenedEvent {
				Chest = this
			});
		}

		protected virtual void AnimateOpening() {
			var a = GetComponent<InteractableSliceComponent>();
					
			Tween.To(1.8f * Scale, a.Scale.X, x => a.Scale.X = x, 0.2f);
			Tween.To(0.2f * Scale, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
				DoOpening();
				
				Tween.To(0.6f * Scale, a.Scale.X, x => a.Scale.X = x, 0.1f);
				Tween.To(1.7f * Scale, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
					Tween.To(Scale, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(Scale, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
				};
			};
		}

		protected void DoOpening() {
			UpdateSprite();
			SpawnDrops();
			GetComponent<AudioEmitterComponent>().EmitRandomized("level_chest_open");
		}

		protected virtual void SpawnDrops() {
			if (!Empty) {
				Empty = true;
				GetComponent<DropsComponent>().SpawnDrops();
			}
		}

		protected virtual bool Interact(Entity entity) {
			if (open || !CanOpen) {
				return true;
			}

			if (TryOpen(entity)) {
				Open();
				return true;
			} else {
				AnimationUtil.ActionFailed();
			}
			
			return false;
		}

		protected virtual bool TryOpen(Entity entity) {
			return true;
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteBoolean(open);
			stream.WriteFloat(Scale);
			stream.WriteBoolean(Empty);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			open = stream.ReadBoolean();
			Scale = stream.ReadFloat();
			Empty = stream.ReadBoolean();
		}

		public override void RenderImDebug() {
			base.RenderImDebug();

			ImGui.Checkbox("Empty", ref Empty);
			ImGui.Checkbox("Can open", ref CanOpen);
		}

		public class OpenedEvent : Event {
			public Chest Chest;
		}

		public virtual bool ShouldCollide(Entity entity) {
			return !(entity is Creature c && c.InAir());
		}
	}
}