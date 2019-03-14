using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.save;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.entity.component.logic;
using Lens.graphics.animation;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.chest {
	public class Chest : SaveableEntity {
		public bool IsOpen { get; private set; }
		protected List<Item> items = new List<Item>();
		
		public void Open() {
			if (IsOpen) {
				return;
			}

			IsOpen = true;
			GetComponent<StateComponent>().Become<OpeningState>();
		}

		public virtual void GenerateLoot() {
			items.Add(ItemRegistry.Create("health_potion"));
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
					items.Add(ItemRegistry.Create(stream.ReadString()));
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
			Open();
			return true;
		}

		protected virtual bool CanInteract() {
			return !IsOpen;
		}
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 20;
			Height = 14;
			
			SetGraphicsComponent(new AnimationComponent("chest", GetPalette()) {
				Offset = new Vector2(-1, -5)
			});
			
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Static, true));
			AddComponent(new StateComponent());

			AddComponent(new InteractableComponent(Interact) {
				CanInteract = CanInteract
			});
		}

		protected virtual ColorSet GetPalette() {
			return null;
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