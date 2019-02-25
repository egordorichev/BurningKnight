using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.entities.fx;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.level.entities {
	public class Exit : SaveableEntity {
		private Body Body;
		private LadderFx Fx;
		private static TextureRegion Region;
		public static LadderFx ExitFx;
		public static float Al;
		public static Exit Instance;
		private byte Type;

		public Void SetType(byte Type) {
			this.Type = Type;

			if (Type == Entrance.NORMAL) {
				Instance = this;
			} 
		}

		public byte GetType() {
			return this.Type;
		}

		public override Void Init() {
			base.Init();
			this.AlwaysActive = true;
			this.Body = World.CreateSimpleBody(this, 0, 0, 16, 16, BodyDef.BodyType.DynamicBody, true);

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			} 

			if (Level.GENERATED) {
				this.AddSelf();
			} 
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		private Void AddSelf() {
			Log.Info("Checking for exit ladder");

			if (Dungeon.LoadType != Entrance.LoadType.GO_DOWN && (Dungeon.LadderId == this.Type || Player.Ladder == null)) {
				Player.Ladder = this;
				Log.Info("Set exit ladder!");
			} 

			if (this.Type == Entrance.NORMAL) {
				Instance = this;
			} 
		}

		public override Void Render() {

		}

		public override Void Load(FileReader Reader) {
			base.Load(Reader);
			this.Type = Reader.ReadByte();
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
			this.AddSelf();
		}

		public override Void Save(FileWriter Writer) {
			base.Save(Writer);
			Writer.WriteByte(this.Type);
		}

		public override Void OnCollision(Entity Entity) {
			if (Entity is Player && this.Fx == null) {
				this.Fx = new LadderFx(this, this.Type == Entrance.ENTRANCE_TUTORIAL ? "tutorial" : "descend");
				this.Area.Add(this.Fx);
				ExitFx = Fx;
			} 
		}

		public override Void OnCollisionEnd(Entity Entity) {
			if (Entity is Player && this.Fx != null) {
				this.Fx.Remove();
				this.Fx = null;
				ExitFx = null;
			} 
		}
	}
}
