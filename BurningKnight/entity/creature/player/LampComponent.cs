using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.pet;
using BurningKnight.entity.item;
using BurningKnight.util;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature.player {
	public class LampComponent : ItemComponent {
		private FollowerPet pet;
		private bool loaded;
		private bool hadLamp;
		
		public override void Set(Item item, bool animate = true) {
			base.Set(item, (item == null || item.Id != "bk:no_pet") && animate);
		}

		public override void PostInit() {
			base.PostInit();
			loaded = true;

			if (Item == null) {
				Set(Items.CreateAndAdd("bk:no_pet", Entity.Area), false);
			}
		}

		protected override void OnItemSet(Item previous) {
			base.OnItemSet(previous);
			Log.Debug(Item?.Id);

			if (pet != null) {
				pet.GetComponent<FollowerComponent>().Remove();
				pet.Done = true;
				pet = null;
			}

			if (hadLamp) {
				Entity.RemoveComponent<HealthComponent>();
				Entity.RemoveComponent<HeartsComponent>();
				Entity.RemoveComponent<InventoryComponent>();
				
				Entity.AddComponent(new HealthComponent());
				Entity.AddComponent(new HeartsComponent());
				Entity.AddComponent(new InventoryComponent());
				
				if (Entity.TryGetComponent<WeaponComponent>(out var w)) {
					w.Disabled = false;
				}
			
				if (Entity.TryGetComponent<ActiveWeaponComponent>(out var aw)) {
					aw.Disabled = false;
				}
			}

			if (loaded) {
				Log.Info("using");
				Item?.Use(Entity);
				hadLamp = true;
			}

			if (Item == null || Item.Id == "bk:no_pet") {
				return;
			}
			
			pet = new LampPet(Item.Id) {
				Owner = Entity
			};

			Entity.Area.Add(pet);
			pet.Center = Entity.Center + MathUtils.CreateVector(Rnd.AnglePI(), Rnd.Float(16f, 48f));
			AnimationUtil.Poof(pet.Center);
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Lamp;
		}
	}
}