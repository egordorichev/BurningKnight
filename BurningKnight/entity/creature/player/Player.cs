using BurningKnight.entity.component;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.player {
	public class Player : Creature, CollisionFilterEntity {
		public override void AddComponents() {
			base.AddComponents();
			
			SetGraphicsComponent(new PlayerGraphicsComponent {
				Offset = new Vector2(0, -4)
			});
			
			// Inventory
			AddComponent(new InventoryComponent());
			AddComponent(new ActiveItemComponent());
			AddComponent(new WeaponComponent());
			AddComponent(new ActiveWeaponComponent());
			AddComponent(new ConsumablesComponent());
			
			// Stats
			AddComponent(new HeartsComponent());
			
			// Collisions
			AddComponent(new RectBodyComponent(2, 0, 12, 12));
			AddComponent(new InteractorComponent());
			
			GetComponent<StateComponent>().State = typeof(IdleState);
			
			AddTag(Tags.Player);
			AddTag(Tags.PlayerSave);
			RemoveTag(Tags.LevelSave);

			AlwaysActive = true;
		}

		public override void PostInit() {
			base.PostInit();

			// todo: pick the exit one
			var room = Area.Tags[Tags.Room][0];
			
			CenterX = room.CenterX;
			CenterY = room.CenterY;
		}

		#region Player States
		public class IdleState : EntityState {
			
		}
		
		public class RunState : EntityState {
			
		}
		
		public class RollState : EntityState {
			
		}
		#endregion

		public bool ShouldCollide(Entity entity) {
			return !(entity is Player);
		}

		protected override bool HasNoHealth() {
			return base.HasNoHealth() && GetComponent<HeartsComponent>().Total == 0;
		}
	}
}