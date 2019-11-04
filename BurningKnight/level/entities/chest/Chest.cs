using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.level.entities.chest {
	public class Chest : SolidProp {
		private bool open;
		
		protected override Rectangle GetCollider() {
			return new Rectangle(2, 4, 12, 4);
		}

		public override void AddComponents() {
			base.AddComponents();

			AddComponent(new DropsComponent());
			AddComponent(new ShadowComponent());
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => !open
			});
		}

		public override void PostInit() {
			base.PostInit();

			if (open) {
				UpdateSprite();
			}
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
					
			Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.2f);
			Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
				UpdateSprite();
				SpawnDrops();
				
				Tween.To(0.6f, a.Scale.X, x => a.Scale.X = x, 0.1f);
				Tween.To(1.7f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
					Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
				};
			};
		}

		protected virtual void SpawnDrops() {
			GetComponent<DropsComponent>().SpawnDrops();
		}

		protected virtual bool Interact(Entity entity) {
			if (open) {
				return true;
			}

			if (TryOpen(entity)) {
				Open();
				return true;
			}
			
			return false;
		}

		protected bool TryOpen(Entity entity) {
			return true;
		}
	}
}