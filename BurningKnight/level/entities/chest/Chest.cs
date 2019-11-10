using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.util.file;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.chest {
	public class Chest : SolidProp {
		private bool open;
		protected internal float Scale = 1;

		public bool CanOpen = true;
		public bool Empty;
		
		protected override Rectangle GetCollider() {
			return new Rectangle(0, (int) (5 * Scale), (int) (Math.Max(1, 17 * Scale)), (int) (Math.Max(1, 8 * Scale)));
		}
		
		protected override BodyComponent CreateBody() {
			var collider = GetCollider();
			return new RectBodyComponent(collider.X, collider.Y, collider.Width, collider.Height);
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 16 * Scale;
			Height = 13 * Scale;

			AddComponent(new SensorBodyComponent(-2, -2, Width + 4, Height + 4));
			AddComponent(new DropsComponent());
			AddComponent(new ShadowComponent());
			AddComponent(new RoomComponent());
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => CanOpen && !open
			});
			
			AddTag(Tags.Chest);
			AddTag(Tags.Item);
			
			DefineDrops();
		}

		protected virtual void DefineDrops() {
			
		}

		public override void PostInit() {
			base.PostInit();

			if (open) {
				UpdateSprite();
			}

			var body = GetComponent<RectBodyComponent>().Body;

			body.LinearDamping = 100;
			body.Mass = 1000000;

			GetComponent<RectBodyComponent>().KnockbackModifier = 0.1f;

			var a = GetComponent<InteractableSliceComponent>();

			a.Scale.X = 0.6f * Scale;
			a.Scale.Y = 1.7f * Scale;
					
			Tween.To(1.8f * Scale, a.Scale.X, x => a.Scale.X = x, 0.15f);
			Tween.To(0.2f * Scale, a.Scale.Y, x => a.Scale.Y = x, 0.15f).OnEnd = () => {
				Tween.To(Scale, a.Scale.X, x => a.Scale.X = x, 0.2f);
				Tween.To(Scale, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
			};
		}

		private void UpdateSprite() {
			GetComponent<InteractableSliceComponent>().Sprite = CommonAse.Props.GetSlice($"{Sprite}_open");
		}

		public void Open() {
			if (open) {
				return;
			}

			open = true;
			
			var a = GetComponent<InteractableSliceComponent>();
					
			Tween.To(1.8f * Scale, a.Scale.X, x => a.Scale.X = x, 0.2f);
			Tween.To(0.2f * Scale, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
				UpdateSprite();
				SpawnDrops();
				
				Tween.To(0.6f * Scale, a.Scale.X, x => a.Scale.X = x, 0.1f);
				Tween.To(1.7f * Scale, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
					Tween.To(Scale, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(Scale, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
				};
			};

			HandleEvent(new OpenedEvent {
				Chest = this
			});
		}

		protected virtual void SpawnDrops() {
			if (!Empty) {
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
	}
}