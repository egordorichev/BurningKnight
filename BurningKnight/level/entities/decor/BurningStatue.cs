using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.save;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.decor {
	public class BurningStatue : SolidProp {
		public bool Broken;
		private FireEmitter fea;
		private FireEmitter feb;
		
		public override void Init() {
			base.Init();

			Width = 14;
			Height = 28;
			Sprite = "burning_statue";
		}

		public override void AddComponents() {
			base.AddComponents();
			
			Subscribe<RoomChangedEvent>();
			AddComponent(new RoomComponent());
			
			AddComponent(new ShadowComponent());
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => !Broken
			});
		}

		public override void PostInit() {
			base.PostInit();

			UpdateSprite();

			if (!Broken) {
				Area.Add(fea = new FireEmitter {
					Depth = Depth + 1,
					Position = new Vector2(X + 11, Y + 15),
					Scale = 0.8f
				});
			
				Area.Add(feb = new FireEmitter {
					Depth = Depth + 1,
					Position = new Vector2(X + 4, Y + 15),
					Scale = 0.8f
				});
				
				AddComponent(new LightComponent(this, 32f, new Color(1f, 0.8f, 0.3f, 1f)));
			}
		}

		private bool Interact(Entity e) {
			Broken = true;
			GameSave.Put("statue_broken", true);
			
			Camera.Instance.Unfollow(this);
			
			AnimationUtil.Explosion(Center);
			Camera.Instance.Shake(10);
			Engine.Instance.Freeze = 1f;
			
			UpdateSprite();
			RemoveComponent<LightComponent>();
			
			return true;
		}

		private void UpdateSprite() {
			if (Broken) {
				var s = GetComponent<InteractableSliceComponent>();

				s.Sprite = CommonAse.Props.GetSlice("broken_statue");
				s.Offset.Y = Height - s.Sprite.Height;

				if (fea != null) {
					fea.Done = true;
					feb.Done = true;
				}
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Broken = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(Broken);
		}

		private float lastFlame;

		public override void Update(float dt) {
			base.Update(dt);

			if (Area.Tags[Tags.BurningKnight].Count > 0) {
				Done = true;
				return;
			}

			if (Broken) {
				return;
			}
			
			lastFlame += dt;

			if (lastFlame > 0.3f) {
				Area.Add(new FireParticle {
					X = X + 11,
					Y = Y + 15,
					XChange = 0.1f
				});
				
				Area.Add(new FireParticle {
					X = X + 4,
					Y = Y + 15,
					XChange = 0.1f
				});

				lastFlame = 0;
			}
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(2, 8, 10, 14);
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce) {
				var r = GetComponent<RoomComponent>().Room;

				if (rce.Who is LocalPlayer) {
					if (rce.New == r) {
						Camera.Instance.Follow(this, 0.3f);
					} else if (rce.Old == r) {
						Camera.Instance.Unfollow(this);
					}
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}