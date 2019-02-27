using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.entities.fx;
using BurningKnight.physics;
using BurningKnight.save;
using BurningKnight.util;
using Lens.util.file;

namespace BurningKnight.entity.level.entities {
	public class Exit : SaveableEntity {
		private static TextureRegion Region;
		public static LadderFx ExitFx;
		public static float Al;
		public static Exit Instance;
		private Body Body;
		private LadderFx Fx;
		private byte Type;

		public void SetType(byte Type) {
			this.Type = Type;

			if (Type == Entrance.NORMAL) Instance = this;
		}

		public byte GetType() {
			return Type;
		}

		public override void Init() {
			base.Init();
			AlwaysActive = true;
			Body = World.CreateSimpleBody(this, 0, 0, 16, 16, BodyDef.BodyType.DynamicBody, true);

			if (Body != null) World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);

			if (Level.GENERATED) AddSelf();
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		private void AddSelf() {
			Log.Info("Checking for exit ladder");

			if (Dungeon.LoadType != Entrance.LoadType.GO_DOWN && (Dungeon.LadderId == Type || Player.Ladder == null)) {
				Player.Ladder = this;
				Log.Info("Set exit ladder!");
			}

			if (Type == Entrance.NORMAL) Instance = this;
		}

		public override void Render() {
		}

		public override void Load(FileReader Reader) {
			base.Load(Reader);
			Type = Reader.ReadByte();
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
			AddSelf();
		}

		public override void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteByte(Type);
		}

		public override void OnCollision(Entity Entity) {
			if (Entity is Player && Fx == null) {
				Fx = new LadderFx(this, Type == Entrance.ENTRANCE_TUTORIAL ? "tutorial" : "descend");
				Area.Add(Fx);
				ExitFx = Fx;
			}
		}

		public override void OnCollisionEnd(Entity Entity) {
			if (Entity is Player && Fx != null) {
				Fx.Remove();
				Fx = null;
				ExitFx = null;
			}
		}
	}
}