using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.controller;
using BurningKnight.assets.particle.renderer;
using BurningKnight.entity;
using BurningKnight.entity.creature.player;
using BurningKnight.save;
using ImGuiNET;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.entities {
	public class Gramophone : Prop {
		private TextureRegion top;
		private TextureRegion bottom;
		private TextureRegion tdisk;
		private float t;
		private float tillNext;
		private bool broken;
		private int disk = 10;
		
		public override void Init() {
			base.Init();

			disk = GlobalSave.GetInt("disk");
			
			Width = 16;
			Height = 23;

			top = CommonAse.Props.GetSlice("player_top");
			bottom = CommonAse.Props.GetSlice("player");
			tdisk = CommonAse.Props.GetSlice("disk");
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			disk = stream.ReadByte();
			broken = GetComponent<HealthComponent>().Health == 0;
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteByte((byte) disk);
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddTag(Tags.Gramophone);
			
			AddComponent(new RoomComponent());
			AddComponent(new ExplodableComponent());
			AddComponent(new ShadowComponent(RenderWithShadow));
			AddComponent(new RectBodyComponent(0, 5, 16, 6, BodyType.Static, false));
			AddComponent(new SensorBodyComponent(2, 2, Width - 4, Height - 4, BodyType.Static));
			AddComponent(new InteractableComponent(Interact));
			
			AddComponent(new HealthComponent {
				InitMaxHealth = 5
			});
		}

		private bool Interact(Entity entity) {
			var h = entity.TryGetComponent<ActiveWeaponComponent>(out var c);
			var l = h && c.Item != null && c.Item.Id.StartsWith("bk:disk_");
			var hd = false;

			if (disk > 0) {
				if (h) {
					hd = true;
				} else {
					DropDisk();
				}
			}

			if (l) {
				try {
					var id = byte.Parse(c.Item.Id.Replace("bk:disk_", ""));
					var old = c.Item;

					if (hd) {
						entity.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd($"bk:disk_{disk}", Area));
					} else {
						c.Set(null, false);
					}

					disk = id;
					
					old.Done = true;
				} catch (Exception e) {
					Log.Error(e);
				}
			} else if (hd) {
				entity.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd($"bk:disk_{disk}", Area));
				disk = 0;
			}
			
			GlobalSave.Put("disk", disk);
			
			SendEvent();
			return false;
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			t += dt;

			if (GetComponent<HealthComponent>().Health == 0 || disk == 0) {
				return;
			}
			
			tillNext -= dt;

			if (tillNext <= 0) {
				tillNext = Random.Float(1, 3f);
				
				var part = new ParticleEntity(new Particle(Controllers.Float, new TexturedParticleRenderer(CommonAse.Particles.GetSlice($"note_{Random.Int(1, 3)}"))));
				part.Position = Center;
				Area.Add(part);
				
				part.Particle.Velocity = new Vector2(Random.Float(8, 16) * (Random.Chance() ? -1 : 1), -Random.Float(40, 66));
				part.Particle.Angle = 0;
				part.Depth = Layers.InGameUi;
			}
		}

		public override void Render() {
			RealRender();
		}

		private void RealRender(bool shadow = false) {
			if (shadow) {
				Graphics.Render(bottom, Position + new Vector2(0, 34), 0, Vector2.Zero, MathUtils.InvertY);

				if (broken) {
					return;
				}
				
				Graphics.Render(top, Position + new Vector2(9, 28), (float) Math.Cos(t) * -0.1f, new Vector2(9, 14),
					new Vector2((float) Math.Cos(t * 2f) * 0.05f + 1f, (float) Math.Sin(t * 2f) * -0.05f - 1f));

				return;
			}

			var c = GetComponent<InteractableComponent>();
			
			if (c.OutlineAlpha > 0.05f) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(c.OutlineAlpha);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					Graphics.Render(bottom, Position + new Vector2(0, 12) + d);

					if (!broken) {
						Graphics.Render(top, Position + new Vector2(9, 14) + d, (float) Math.Cos(t) * 0.1f, new Vector2(9, 14),
							new Vector2((float) Math.Cos(t * 2f) * 0.05f + 1f, (float) Math.Sin(t * 2f) * 0.05f + 1f));
					}
				}

				Shaders.End();
			}

			var stopShader = false;
			var h = GetComponent<HealthComponent>();
			
			if (h.RenderInvt) {
				var i = h.InvincibilityTimer;

				if (i > h.InvincibilityTimerMax / 2f || i % 0.1f > 0.05f) {
					var shader = Shaders.Entity;
					Shaders.Begin(shader);

					shader.Parameters["flash"].SetValue(1f);
					shader.Parameters["flashReplace"].SetValue(1f);
					shader.Parameters["flashColor"].SetValue(ColorUtils.White);
					
					stopShader = true;
				}
			}

			Graphics.Render(bottom, Position + new Vector2(0, 12));

			if (disk > 0) {
				Graphics.Render(tdisk, Position + new Vector2(2, 13));
			}

			if (!broken) {
				Graphics.Render(top, Position + new Vector2(9, 14), (float) Math.Cos(t) * 0.1f, new Vector2(9, 14),
					new Vector2((float) Math.Cos(t * 2f) * 0.05f + 1f, (float) Math.Sin(t * 2f) * 0.05f + 1f));
			}
			
			if (stopShader) {
				Shaders.End();
			}
		}

		private void RenderWithShadow() {
			RealRender(true);
		}

		public override bool HandleEvent(Event e) {
			if (e is PostHealthModifiedEvent) {
				if (GetComponent<HealthComponent>().Health == 0) {
					if (!broken) {
						HandleEvent(new GramophoneBrokenEvent {
							Gramophone = this
						});

						DropDisk();
						SendEvent();
						broken = true;
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		private void DropDisk() {
			if (disk == 0) {
				return;
			}

			var item = Items.CreateAndAdd($"bk:disk_{disk}", Area);
			item.CenterX = CenterX;
			item.Y = Bottom + 4;

			disk = 0;
			GlobalSave.Put("disk", disk);
		}

		private void SendEvent() {
			HandleEvent(new DiskChangedEvent {
				Gramophone = this,
				Disk = disk
			});
		}

		public string GetTune() {
			if (disk == 0) {
				return null;
			} else if (disk == 10) {
				return "Shopkeeper";
			}

			return $"Disk {disk}";
		}
		
		public class DiskChangedEvent : Event {
			public Gramophone Gramophone;
			public int Disk;
		}

		public override void RenderImDebug() {
			base.RenderImDebug();
			ImGui.InputInt("Disk", ref disk);
		}
	}
}