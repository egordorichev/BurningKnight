using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.save;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util.file;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.chest {
	public class Chest : SaveableEntity {
		public bool IsOpen { get; private set; }
		protected List<Item> items = new List<Item>();

		public void Open(Entity entity) {
			if (IsOpen) {
				return;
			}

			if (!HandleEvent(new ChestOpenedEvent {
				Chest = this,
				Who = entity
			})) {
				IsOpen = true;
				GetComponent<StateComponent>().Become<OpeningState>();
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is StateChangedEvent ev && ev.NewState == typeof(OpenState)) {
				foreach (var i in items) {
					Area.Add(i);
					i.AddDroppedComponents();

					i.CenterX = CenterX;
					i.Y = Bottom;
				}
				
				items.Clear();
			}
			
			return base.HandleEvent(e);
		}

		public virtual void GenerateLoot() {
			items.Add(Items.Generate(ItemPool.Chest));
		}

		public override void PostInit() {
			base.PostInit();

			if (IsOpen) {
				GetComponent<StateComponent>().Become<OpenState>();				
			} else {
				GetComponent<StateComponent>().Become<ClosedState>();
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			IsOpen = stream.ReadBoolean();

			if (!IsOpen) {
				var count = stream.ReadByte();

				for (int i = 0; i < count; i++) {
					items.Add(Items.Create(stream.ReadString()));
				}
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(IsOpen);

			if (!IsOpen) {
				stream.WriteByte((byte) items.Count);

				foreach (var item in items) {
					stream.WriteString(item.Id);
				}
			}
		}

		protected bool Interact(Entity entity) {
			Open(entity);
			return true;
		}

		protected virtual bool CanInteract() {
			return !IsOpen;
		}
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 20;
			Height = 12;
			
			AddComponent(new AnimationComponent("chest", GetPalette()) {
				Offset = new Vector2(-1, -6)
			});
			
			AddComponent(new RectBodyComponent(1, 6, Width - 2, Height - 14, BodyType.Static));
			AddComponent(new SensorBodyComponent(0, 0, Width, Height, BodyType.Static));
			AddComponent(new StateComponent());
			AddComponent(new ShadowComponent(RenderShadow));

			AddComponent(new InteractableComponent(Interact) {
				CanInteract = CanInteract,
				AlterInteraction = AlterInteraction
			});
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		protected virtual Entity AlterInteraction() {
			return this;
		}

		protected virtual ColorSet GetPalette() {
			return null;
		}
		
		public override void RenderDebug() {
			Graphics.Batch.DrawRectangle(new RectangleF(X, Y, Width, Height), Color.Blue);
		}
		
		#region Chest States
		public class ClosedState : EntityState {
			
		}
		
		public class OpeningState : EntityState {
			public override void Init() {
				base.Init();
				Self.GetComponent<AnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<AnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<AnimationComponent>().Animation.Paused) {
					Self.GetComponent<StateComponent>().Become<OpenState>();
				}
			}
		}
		
		public class OpenState : EntityState {
			
		}
		#endregion
	}
}