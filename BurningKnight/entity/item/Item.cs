using System;
using System.Text;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item.use;
using BurningKnight.entity.item.useCheck;
using BurningKnight.physics;
using BurningKnight.save;
using Lens.assets;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.util.file;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.item {
	public class Item : SaveableEntity, CollisionFilterEntity {
		private int count = 1;

		public int Count {
			get => count;
			set {
				count = Math.Max(0, value);

				if (count == 0) {
					Done = true;
				}
			}
		}

		public ItemType Type;
		public string Id;
		public string Name => Locale.Get(Id);
		public string Description => Locale.Get($"{Id}_desc");
		public float UseTime = 0.3f;
		public float Delay { get; protected set; }
		
		public ItemUse[] Uses;
		public ItemUseCheck UseCheck = ItemUseChecks.Default;
		
		public Item(params ItemUse[] uses) {
			Uses = uses;
		}

		public void Use(Entity entity) {
			if (!UseCheck.CanUse(entity, this)) {
				return;
			}

			foreach (var use in Uses) {
				use.Use(entity, this);
			}

			Delay = UseTime;

			HandleEvent(new ItemUsedEvent {
				Item = this,
				Who = entity
			});
		}

		public override void AddComponents() {
			base.AddComponents();
			SetGraphicsComponent(new ItemGraphicsComponent("items", Id));
		}

		private bool Interact(Entity entity) {
			if (entity.TryGetComponent<InventoryComponent>(out var inventory)) {
				inventory.Pickup(this);
				return true;
			}

			return false;
		}

		public virtual void AddDroppedComponents() {
			var slice = GetComponent<ItemGraphicsComponent>().Sprite;			
	
			AddComponent(new RectBodyComponent(0, 0, slice.Source.Width, slice.Source.Height, BodyType.Dynamic, true));
			AddComponent(new InteractableComponent(Interact));
		}

		public virtual void RemoveDroppedComponents() {
			RemoveComponent<InteractableComponent>();
			RemoveComponent<RectBodyComponent>();
		}

		public override void Save(FileWriter stream) {
			stream.WriteInt32(Count);
			stream.WriteString(Id);
		}

		public override void Load(FileReader stream) {
			Count = stream.ReadInt32();
			Id = stream.ReadString();
		}

		public StringBuilder BuildInfo() {
			var builder = new StringBuilder();
			builder.Append(Name).Append('\n').Append(Description);
			
			return builder;
		}

		public override void Update(float dt) {
			base.Update(dt);
			Delay = Math.Max(0, Delay - dt);
		}

		public bool ShouldCollide(Entity entity) {
			return !(entity is Player);
		}
	}
}