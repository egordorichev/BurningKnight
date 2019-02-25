using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.game.state;
using BurningKnight.core.util;
using BurningKnight.core.util.file;

namespace BurningKnight.core.entity.item {
	public class Item : Entity {
		public static TextureRegion Missing = Graphics.GetTexture("item-missing");
		public bool UseOnPickup;
		public float A = 1;
		public bool Shop;
		protected string Sprite;
		protected string Name;
		protected string Description;
		protected bool Stackable = false;
		protected int Count = 1;
		protected bool AutoPickup = false;
		protected bool Useable = true;
		protected float Delay = 0;
		protected float UseTime = 0.5f;
		protected Creature Owner;
		protected TextureRegion Region;
		protected bool Auto = false;
		protected string UseSpeedStr;
		public int Price = 15;
		public bool Sale;

		public Item() {
			InitStats();
		}

		public Void DisableAutoPickup() {
			this.AutoPickup = false;
		}

		public int GetPrice() {
			return 5;
		}

		protected Item(string Name, string Description, string Sprite) {
			this.Name = Name;
			this.Description = Description;
			this.Sprite = Sprite;
			InitStats();
		}

		private Void InitStats() {
			string UnlocalizedName = Utils.PascalCaseToSnakeCase(GetClass().GetSimpleName());

			if (this.Sprite == null) {
				this.Sprite = "item-" + UnlocalizedName;
			} 

			if (this.Name == null) {
				this.Name = Locale.Get(UnlocalizedName);
			} 

			if (this.Description == null) {
				this.Description = Locale.Get(UnlocalizedName + "_desc");
			} 
		}

		public Void UpdateInHands(float Dt) {

		}

		public Void Generate() {

		}

		public Void OnPickup() {
			if (Dungeon.Depth != -1 && Dungeon.DarkR == Dungeon.MAX_R && Dungeon.Game.GetState() is InGameState) {
				Audio.PlaySfx("pickup_item");
			} 

			if (UseOnPickup) {
				this.Owner = Player.Instance;
				this.Use();
			} 
		}

		public bool IsAuto() {
			return this.Auto && !this.Shop;
		}

		public int GetValue() {
			return this.GetCount();
		}

		public Void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {

		}

		public override Void Update(float Dt) {
			this.Delay = Math.Max(0, this.Delay - Dt);
		}

		public Creature GetOwner() {
			return this.Owner;
		}

		public Void SetOwner(Creature Owner) {
			this.Owner = Owner;
		}

		public Void Use() {
			this.Delay = this.UseTime;
		}

		public Void SecondUse() {
			this.Delay = this.UseTime;
		}

		public Void EndUse() {

		}

		public float GetDelay() {
			return this.Delay;
		}

		public Void Save(FileWriter Writer) {
			Writer.WriteInt32(this.Count);
			Writer.WriteBoolean(this.Shop);
			Writer.WriteByte((byte) this.Price);
			Writer.WriteBoolean(this.Sale);
		}

		public Void Load(FileReader Reader) {
			this.Count = Reader.ReadInt32();
			this.Shop = Reader.ReadBoolean();
			this.Price = Reader.ReadByte();
			this.Sale = Reader.ReadBoolean();
		}

		public TextureRegion GetSprite() {
			if (this.Region == null) {
				this.Region = Graphics.GetTexture(this.Sprite);

				if (this.Region == null) {
					Log.Error("Invalid item sprite " + this.GetClass().GetSimpleName());
					this.Region = Missing;
				} 
			} 

			return this.Region;
		}

		public bool IsUseable() {
			return this.Useable;
		}

		public bool CanBeUsed() {
			return Delay == 0;
		}

		public string GetName() {
			return this.Count == 0 || this.Count >= 1 ? this.Name : this.Name + " (" + this.Count + ")";
		}

		public bool IsStackable() {
			return this.Stackable;
		}

		public int GetCount() {
			return this.Count;
		}

		public Item SetCount(int Count) {
			this.Count = Count;

			return this;
		}

		public bool HasAutoPickup() {
			return this.AutoPickup;
		}

		public string GetDescription() {
			return this.Description;
		}

		public float GetUseTime() {
			return this.UseTime;
		}

		public StringBuilder BuildInfo() {
			StringBuilder Builder = new StringBuilder();
			Builder.Append(this.GetName());
			Builder.Append("[gray]");

			if (!this.Description.IsEmpty()) {
				Builder.Append('\n');
				Builder.Append(this.GetDescription());
			} 

			return Builder;
		}
	}
}
