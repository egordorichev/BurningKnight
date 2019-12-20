using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.bk;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.entities.decor {
	public class BurningStatue : SolidProp {
		public bool Broken;
		private FireEmitter fea;
		private FireEmitter feb;
		private SpawnTrigger trigger;
		private float XSpread = 0.1f;
		private Vector2? target;
		
		public override void Init() {
			base.Init();

			Width = 14;
			Height = 28;
			Sprite = "burning_statue";
		}

		public override void AddComponents() {
			base.AddComponents();
			
			Subscribe<RoomChangedEvent>();
			Subscribe<SpawnTrigger.TriggeredEvent>();
				
			AddComponent(new RoomComponent());
			
			AddComponent(new ShadowComponent());
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => !Broken && !busy
			});

			var h = new HealthComponent {
				RenderInvt = true
			};
			
			AddComponent(h);

			h.InitMaxHealth = 3;
		}

		public override void Destroy() {
			base.Destroy();

			if (fea != null) {
				fea.Done = true;
				feb.Done = true;
			}
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
				
				AddComponent(new LightComponent(this, 32f, new Color(1f, 0.5f, 0f, 1f)));
			}
		}

		private bool Interact(Entity e) {
			if (Broken || busy) {
				return true;
			}

			Run.AddScourge(true);
			
			if (trigger != null) {
				trigger.Interrupted = true;
			}

			HandleEvent(new BrokenEvent {
				BurningStatue = this
			});

			XSpread = 1;
			
			Tween.To(1f, Camera.Instance.Zoom, xx => Camera.Instance.Zoom = xx, 0.2f);
			Tween.To(1.4f, Camera.Instance.TextureZoom, xx => Camera.Instance.TextureZoom = xx, 0.5f);
			Camera.Instance.GetComponent<ShakeComponent>().Amount = 0;
			GameSave.Put("statue_broken", true);

			var torches = GetComponent<RoomComponent>().Room.Tagged[Tags.Torch];

			foreach (var t in torches) { 
				var tr = (Torch) t;

				tr.XSpread = 0.1f;
				tr.On = true;
			}
			
			Timer.Add(() => {
				for (var i = 0; i < torches.Count; i++) {
					var i1 = i;
					
					Timer.Add(() => {
						var t = (Torch) torches[i1];

						t.Break();
						Camera.Instance.Shake(5);
					}, i);
				}
				
				Timer.Add(() => {
					Particles.BreakSprite(Area, GetComponent<InteractableSliceComponent>().Sprite, Position);
					Broken = true;
				
					Camera.Instance.Unfollow(this);
					Camera.Instance.Shake(10);

					Timer.Add(() => {
						Camera.Instance.GetComponent<ShakeComponent>().Amount = 0;
					}, 0.5f);

					Camera.Instance.Targets.Clear();
					Camera.Instance.Follow(this, 0.5f);
			
					UpdateSprite();
					RemoveComponent<LightComponent>();
			
					var exit = new Exit();
					Area.Add(exit);
				
					exit.To = Run.Depth + 1;

					var x = (int) Math.Floor(CenterX / 16);
					var y = (int) Math.Floor(Bottom / 16 + 0.6f);
					var p = new Vector2(x * 16 + 8, y * 16 + 8);
			
					exit.Center = p;

					Painter.Fill(Run.Level, x - 1, y - 1, 3, 3, Tiles.RandomFloor());
					Painter.Fill(Run.Level, x - 1, y - 3, 3, 3, Tiles.RandomFloor());

					Run.Level.TileUp();
					Run.Level.CreateBody();
			
					Timer.Add(() => {
						Tween.To(1f, Camera.Instance.TextureZoom, xx => Camera.Instance.TextureZoom = xx, 0.8f);
						((InGameState) BK.Instance.State).ResetFollowing();
					}, 1f);
				}, torches.Count + 1);
			}, 1f);
			
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
		private bool busy;

		public override void Update(float dt) {
			base.Update(dt);

			if (Area.Tagged[Tags.BurningKnight].Count > 0) {
				Done = true;
				return;
			}

			if (Broken) {
				return;
			}

			if (trigger != null && trigger.ReadyToSpawn) {
				trigger.ReadyToSpawn = false;
				SetupSpawn();
			}
			
			lastFlame += dt;

			if (lastFlame > 0.3f) {
				Area.Add(new FireParticle {
					X = X + 11,
					Y = Y + 15,
					XChange = XSpread,
					Target = target
				});
				
				Area.Add(new FireParticle {
					X = X + 4,
					Y = Y + 15,
					XChange = XSpread,
					Target = target
				});

				lastFlame = 0;
			}
		}

		private void SetupSpawn() {
			busy = true;
			
			var torches = GetComponent<RoomComponent>().Room.Tagged[Tags.Torch];
			target = new Vector2(CenterX, Y - 4);

			foreach (var t in torches) { 
				var tr = (Torch) t;

				tr.XSpread = 1;
				tr.On = true;
				tr.Target = target;
			}

			Timer.Add(() => {
				var bk = new entity.creature.bk.BurningKnight();
				Area.Add(bk);
				bk.Center = target.Value;
				
				Camera.Instance.Targets.Clear();
				Camera.Instance.Follow(bk, 0.1f);
					
				Timer.Add(() => {
					((InGameState) Engine.Instance.State).ResetFollowing();
				}, 2f);
					
				Camera.Instance.Shake(10);

				foreach (var t in torches) {
					t.Done = true;
				}

				Done = true;
			}, 2f);
		}

		protected override Rectangle GetCollider() {
			return new Rectangle(2, 8, 10, 14);
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomChangedEvent rce) {
				var r = GetComponent<RoomComponent>().Room;

				if (rce.Who is LocalPlayer) {
					if (rce.New == r) {
						Camera.Instance?.Follow(this, 0.3f);
					} else if (rce.Old == r) {
						Camera.Instance?.Unfollow(this);
					}
				}
			} else if (e is SpawnTrigger.TriggeredEvent stte) {
				trigger = stte.Trigger;
			} else if (e is DiedEvent de) {
				Interact(de.Who);
				return true;
			}
			
			return base.HandleEvent(e);
		}

		public class BrokenEvent : Event {
			public BurningStatue BurningStatue;
		}
	}
}