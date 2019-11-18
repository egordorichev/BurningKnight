using System;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.level;
using BurningKnight.level.entities.decor;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.bk {
	public class SpawnTrigger : SaveableEntity {
		public byte[] Tiles;
		public byte[] Liquid;
		public ushort RoomX;
		public ushort RoomY;
		public byte RoomWidth;
		public byte RoomHeight;

		public bool Triggered;
		public bool Interrupted;
		public bool ReadyToSpawn;
		
		private float t;
		private bool did;

		public override void Update(float dt) {
			base.Update(dt);

			if (!did && Triggered) {
				Camera.Instance.Shake(0.5f);
				t += dt;

				if (t >= 1f) {
					did = true;
					var r = (int) Math.Ceiling(Math.Sqrt((RoomWidth + 1) * (RoomWidth + 1) + (RoomHeight + 1) * (RoomHeight + 1)));

					Camera.Instance.Targets.Clear();
					Camera.Instance.Follow(this, 3f);
					Tween.To(0.5f, Camera.Instance.Zoom, x => Camera.Instance.Zoom = x, 0.3f);

					for (var j = 1; j < r; j++) {
						var level = Run.Level;
						var j1 = j;

						Timer.Add(() => {
							if (Interrupted) {
								level.CreateBody();
								return;
							}
								
							for (var y = 0; y < RoomHeight; y++) {
								for (var x = 0; x < RoomWidth; x++) {
									var dx = x - RoomWidth / 2f;
									var dy = y - RoomHeight / 2f;

									if (Math.Sqrt(dx * dx + dy * dy) > j1) {
										continue;	
									}

									var i = x + y * RoomWidth;

									if (Tiles[i] == 255) {
										continue;
									}
									
									var li = level.ToIndex(RoomX + x, RoomY + y);
									
									if (level.Get(li).IsWall()) {
										level.Variants[li] = 0;
									}
									
									// tmp
									Area.Add(new TileFx {
										X = (RoomX + x) * 16,
										Y = (RoomY + y) * 16 - 8
									});
									
									level.ReCreateBodyChunk(RoomX + x, RoomY + y);

									level.Tiles[li] = Tiles[i];
									level.Liquid[li] = Liquid[i];

									Tiles[i] = 255; // Mark as already checked;
								}
							}
							
							for (var y = -1; y < RoomHeight + 1; y++) {
								for (var x = -1; x < RoomWidth + 1; x++) {
									LevelTiler.TileUp(level, level.ToIndex(RoomX + x, RoomY + y));
								}
							}
				
							Camera.Instance.Shake(2);
						}, j * 0.05f);
					}

					Timer.Add(() => {
						if (Interrupted) {
							return;
						}
							
						Tween.To(1f, Camera.Instance.Zoom, x => Camera.Instance.Zoom = x, 0.3f);

						Timer.Add(() => {
							if (Interrupted) {
								return;
							}

							Done = true;
							ReadyToSpawn = true;
						}, 1f);
					}, r * 0.05f + 1f);
				}
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteByte((byte) Width);
			stream.WriteByte((byte) Height);
			
			stream.WriteUInt16(RoomX);
			stream.WriteUInt16(RoomY);
			
			stream.WriteByte(RoomWidth);
			stream.WriteByte(RoomHeight);

			for (var i = 0; i < RoomWidth * RoomHeight; i++) {
				stream.WriteByte(Tiles[i]);
				stream.WriteByte(Liquid[i]);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			Width = stream.ReadByte();
			Height = stream.ReadByte();

			RoomX = stream.ReadUInt16();
			RoomY = stream.ReadUInt16();

			RoomWidth = stream.ReadByte();
			RoomHeight = stream.ReadByte();

			var s = RoomWidth * RoomHeight;

			Tiles = new byte[s];
			Liquid = new byte[s];
			
			for (var i = 0; i < s; i++) {
				Tiles[i] = stream.ReadByte();
				Liquid[i] = stream.ReadByte();
			}
		}

		public override void PostInit() {
			base.PostInit();
			
			Subscribe<BurningStatue.BrokenEvent>();
			
			AddComponent(new RoomComponent());
			AddComponent(new RectBodyComponent(8, 8, Width - 16, Height - 16, BodyType.Static, true));
		}

		public override bool HandleEvent(Event e) {
			if (e is BurningStatue.BrokenEvent) {
				Interrupted = true;
				Done = true;

				return base.HandleEvent(e);
			}
			
			if (Triggered) {
				return base.HandleEvent(e);
			}
			
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Player p) {
					Timer.Add(() => {
						Triggered = true;
					}, 1f);
					
					var xx = (int) Math.Floor(CenterX / 16);
					var xy = (int) Math.Floor(CenterY / 16);

					Painter.Rect(Run.Level, xx - 3, xy - 3, 6, 6, Tile.Chasm);

					var torches = GetComponent<RoomComponent>().Room.Tagged[Tags.Torch];

					foreach (var t in torches) {
						((Torch) t).On = false;
					}
					
					Timer.Add(() => {
						if (Interrupted) {
							return;
						}
						
						foreach (var t in torches) {
							var tr = (Torch) t;
							tr.On = true;
							tr.XSpread = 0.1f;
						}
					}, 3f);
					
					HandleEvent(new TriggeredEvent {
						Trigger = this,
						Who = p
					});

					for (var x = X - 16; x < X + Width + 16; x += 16) {
						for (var i = 0; i < Rnd.Int(3, 9); i++) {
							Area.Add(new FireParticle {
								Position = new Vector2(x + Rnd.Float(-2, 18), Y - 16 + Rnd.Float(-2, 18)),
								Delay = Rnd.Float(0.5f),
								XChange = 0.1f,
								Scale = 0.3f,
								Vy = 8,
								T = 0.5f,
								B = 0
							});
							
							Area.Add(new FireParticle {
								Position = new Vector2(x + Rnd.Float(-2, 18), Y + Height + Rnd.Float(-2, 18)),
								Delay = Rnd.Float(0.5f),
								XChange = 0.1f,
								Scale = 0.3f,
								Vy = 8,
								T = 0.5f,
								B = 0
							});
							
							Area.Add(new TileFx {
								Position = new Vector2(x, Y - 16)
							});
							
							Area.Add(new TileFx {
								Position = new Vector2(x, Y + Height)
							});
						}
					}

					for (var y = Y; y < Y + Height; y += 16) {
						for (var i = 0; i < Rnd.Int(3, 9); i++) {
							Area.Add(new FireParticle {
								Position = new Vector2(X + Rnd.Float(-2, 18) - 16, y + Rnd.Float(-2, 18)),
								Delay = Rnd.Float(0.5f),
								XChange = 0.1f,
								Scale = 0.3f,
								Vy = 8,
								T = 0.5f,
								B = 0
							});
							
							Area.Add(new FireParticle {
								Position = new Vector2(X + Width + Rnd.Float(-2, 18), y + Rnd.Float(-2, 18)),
								Delay = Rnd.Float(0.5f),
								XChange = 0.1f,
								Scale = 0.3f,
								Vy = 8,
								T = 0.5f,
								B = 0
							});
							
							Area.Add(new TileFx {
								Position = new Vector2(X - 16, y)
							});
							
							Area.Add(new TileFx {
								Position = new Vector2(X + Width, y)
							});
						}
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		public class TriggeredEvent : Event {
			public SpawnTrigger Trigger;
			public Player Who;
		}
	}
}