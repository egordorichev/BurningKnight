namespace BurningKnight.assets {
	public interface Mod {
		void Init();
		void Destroy();
		void Update(float dt);
		void Render();

		string GetPrefix();
	}
}