using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item.renderer;
using BurningKnight.entity.item.use;
using BurningKnight.entity.item.useCheck;
using BurningKnight.physics;
using BurningKnight.save;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using Random = Lens.util.math.Random;

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
		public string Animation;
		public bool AutoPickup;
		
		public ItemUse[] Uses;
		public ItemUseCheck UseCheck = ItemUseChecks.Default;
		public ItemRenderer Renderer;

		public TextureRegion Region => Animation != null ? GetComponent<AnimatedItemGraphicsComponent>().Animation.GetCurrentTexture() : GetComponent<ItemGraphicsComponent>().Sprite;
		public Entity Owner => GetComponent<OwnerComponent>().Owner;
		
		public Item(ItemRenderer renderer, params ItemUse[] uses) {
			Uses = uses;
			Renderer = renderer;
			Renderer.Item = this;
		}
		
		public Item(params ItemUse[] uses) {
			Uses = uses;
		}

		public Item() {
			
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

		public override void PostInit() {
			base.PostInit();

			if (Animation != null) {
				AddComponent(new AnimatedItemGraphicsComponent(Animation));
			} else {
				AddComponent(new ItemGraphicsComponent(Id));
			}
		}

		private bool Interact(Entity entity) {
			if (entity.TryGetComponent<InventoryComponent>(out var inventory)) {
				inventory.Pickup(this);
				return true;
			}

			return false;
		}

		public void OnInteractionStart(Entity entity) {
			if (AutoPickup && entity.TryGetComponent<InventoryComponent>(out var inventory)) {
				if (Type != ItemType.Heart || !(entity is Creature c) || !c.GetComponent<HealthComponent>().IsFull()) {
					inventory.Pickup(this);
					entity.GetComponent<InteractorComponent>().EndInteraction();	
				}
			} else {
				Area.Add(new ItemPickupFx(this));
			}			
		}

		public virtual void AddDroppedComponents() {
			var slice = Region;
			var body = new RectBodyComponent(0, 0, slice.Source.Width, slice.Source.Height);
			
			AddComponent(body);

			body.Body.LinearDamping = 4;
			
			AddComponent(new InteractableComponent(Interact) {
				OnStart = OnInteractionStart
			});
			
			AddComponent(new ShadowComponent(RenderShadow));
		}

		public void RandomizeVelocity(float force) {
			if (!TryGetComponent<RectBodyComponent>(out var component)) {
				return;
			}

			force *= 60f;
			var angle = Random.AnglePI();
			
			component.Velocity += new Vector2((float) Math.Cos(angle) * force, (float) Math.Sin(angle) * force);
		}

		public virtual void RemoveDroppedComponents() {
			RemoveComponent<InteractableComponent>();
			RemoveComponent<RectBodyComponent>();
			RemoveComponent<ShadowComponent>();
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		public override void Save(FileWriter stream) {
			stream.WriteInt32(Count);
			stream.WriteString(Id);
		}

		public override void Load(FileReader stream) {
			Count = stream.ReadInt32();
			Id = stream.ReadString();

			var item = Items.Create(Id);

			if (item == null) {
				Log.Error($"Failed to load item {Id}, such id does not exist!");
				return;
			}
			
			Uses = item.Uses;
			Renderer = item.Renderer;
			Animation = item.Animation;
			AutoPickup = item.AutoPickup;

			if (Renderer != null) {
				Renderer.Item = this;
			}
		}
		
		public override void Update(float dt) {
			base.Update(dt);
			Delay = Math.Max(0, Delay - dt);
		}

		public bool ShouldCollide(Entity entity) {
			return !(entity is Player);
		}

		public override bool HandleEvent(Event e) {
			if (e is LostSupportEvent) {
				Done = true;
				return true;
			}
			
			return base.HandleEvent(e);
		}
	}
}