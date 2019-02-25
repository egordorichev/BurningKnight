using BurningKnight.util;

namespace BurningKnight.physics {
	public class World {
		public const float TIME_STEP = 1 / 100.0f;
		public static bool DRAW_DEBUG = false;
		public static Com.badlogic.gdx.physics.box2d.World World;
		private static Box2DDebugRenderer Debug = new Box2DDebugRenderer();
		private static float Accumulator;
		private static List<Body> ToDestroy = new List<>();
		public static RayHandler Lights;
		private static List<PointLight> LightPool = new List<>();

		public static PointLight NewLight(int Rays, Color Color, int Rad, float X, float Y) {
			return NewLight(Rays, Color, Rad, X, Y, false);
		}

		public static PointLight NewLight(int Rays, Color Color, int Rad, float X, float Y, bool Fast) {
			if (LightPool.Size() == 0) return new PointLight(Lights, Rays, Color, Rad, X, Y);

			for (var I = 0; I < LightPool.Size(); I++) {
				PointLight Light = LightPool.Get(I);

				if (Light.GetRayNum() == Rays) {
					LightPool.Remove(I);
					Light.SetColor(Color);

					if (Fast)
						Light.SetDistance(Rad);
					else
						Tween.To(new Tween.Task(Rad, 0.15f) {

		public override float GetValue() {
			return Light.GetDistance();
		}

		public override void SetValue(float Value) {
			Light.SetDistance(Value);
		}
	});
}


Light.SetPosition(X, Y);

Light.SetActive(true);
return Light;
}
}
return new PointLight(Lights, Rays, Color, Rad, X, Y);
}
public static void RemoveLight(PointLight Light) {
if (Light == null) {
return;
}
Tween.To(new Tween.Task(0, 0.2f) {
public override float GetValue() {
return Light.GetDistance();
}
public override void SetValue(float Value) {
Light.SetDistance(Value);
}
public override void OnEnd() {
LightPool.Add(Light);
Light.SetDistance(0);
Light.SetPosition(-1000, -1000);
Light.SetActive(false);
}
});
}
public static void Init() {
if (World != null) {
return;
}
Log.Physics("Creating new world");
World = new Com.Badlogic.Gdx.Physics.Box2d.World(new Vector2(0, 0), true);
RayHandler.IsDiffuse = true;
Lights = new RayHandler(World, Display.GAME_WIDTH, Display.GAME_HEIGHT);
Lights.SetLightMapRendering(false);
Lights.SetBlurNum(5);
Light.SetGlobalContactFilter((short) 1, (short) -1, (short) 0x0003);
}
private static void SetBits(FixtureDef Fixture, Entity Owner) {
if (!(Owner is SolidProp) && (Fixture.IsSensor || Owner is HeartFx || Owner is Upgrade || Owner is Exit || Owner is PoisonFx || Owner is ItemHolder || Owner is Item || Owner is Projectile || Owner is Shell || Owner is ManaFx || Owner is
BurningMan || Owner is Mage || Owner is IceElemental)) {
Fixture.Filter.CategoryBits = 0x0002;
Fixture.Filter.GroupIndex = -1;
Fixture.Filter.MaskBits = -1;
} else {
Fixture.Filter.CategoryBits = 0x0003;
Fixture.Filter.GroupIndex = 1;
Fixture.Filter.MaskBits = -1;
}
}
public static void Update(float Dt) {
float FrameTime = Math.Min(Dt, 0.25f);
Accumulator += FrameTime;
while (Accumulator >= TIME_STEP) {
World.Step(TIME_STEP, 6, 2);
Accumulator -= TIME_STEP;
}
if (!World.IsLocked() && ToDestroy.Size() > 0) {
Log.Physics("Removing " + ToDestroy.Size() + " bodies");
foreach (Body Body in ToDestroy) {
World.DestroyBody(Body);
}
ToDestroy.Clear();
}
}
public static void Render() {
if (DRAW_DEBUG) {
Graphics.Batch.End();
Debug.Render(World, Camera.Game.Combined);
Graphics.Batch.Begin();
}
}
public static void Destroy() {
if (World == null) {
return;
}
Log.Physics("Destroying the world");
World.Dispose();
World = null;
if (Lights == null) {
return;
}
foreach (Light Light in LightPool) {
try {
Light.Remove();
} catch (RuntimeException) {
}
}
Lights.Dispose();
Lights = null;
}
public static Body CreateSimpleBody(Entity Owner, float X, float Y, float W, float H, BodyDef.BodyType Type) {
return CreateSimpleBody(Owner, X, Y, W, H, Type, false);
}
public static Body CreateSimpleBody(Entity Owner, float X, float Y, float W, float H, BodyDef.BodyType Type, bool Sensor) {
return CreateSimpleBody(Owner, X, Y, W, H, Type, Sensor, 0f);
}
public static Body CreateSimpleBody(Entity Owner, float X, float Y, float W, float H, BodyDef.BodyType Type, bool Sensor, float Den) {
Log.Physics("Creating body for " + (Owner == null ? null : Owner.GetClass().GetSimpleName()) + " with params (" + X + ", " + Y + ", " + W + ", " + H + ") and sensor = " + Sensor);
if (World.IsLocked()) {
Log.Physics("World is locked! Failed to create body");
return null;
}
BodyDef Def = new BodyDef();
Def.Type = Type;
Body Body = World.CreateBody(Def);
PolygonShape Poly = new PolygonShape();
Poly.Set({ new Vector2(X, Y), new Vector2(X + W, Y), new Vector2(X, Y + H), new Vector2(X + W, Y + H) });
FixtureDef Fixture = new FixtureDef();
Fixture.Shape = Poly;
Fixture.Friction = 0;
Fixture.IsSensor = Sensor;
Fixture.Restitution = Den;
SetBits(Fixture, Owner);
Body.CreateFixture(Fixture);
Body.SetUserData(Owner);
Poly.Dispose();
return Body;
}
public static Body CheckLocked(Body Body) {
if (World.IsLocked()) {
throw new RuntimeException("World is locked!");
}
return Body;
}
public static Body CreateSimpleCentredBody(Entity Owner, float X, float Y, float W, float H, BodyDef.BodyType Type) {
return CreateSimpleCentredBody(Owner, X, Y, W, H, Type, false);
}
public static Body CreateSimpleCentredBody(Entity Owner, float X, float Y, float W, float H, BodyDef.BodyType Type, bool Sensor) {
return CreateSimpleCentredBody(Owner, X, Y, W, H, Type, Sensor, 0f);
}
public static Body CreateSimpleCentredBody(Entity Owner, float X, float Y, float W, float H, BodyDef.BodyType Type, bool Sensor, float Den) {
Log.Physics("Creating centred body for " + Owner.GetClass().GetSimpleName() + " with params (" + X + ", " + Y + ", " + W + ", " + H + ") and sensor = " + Sensor);
if (World.IsLocked()) {
Log.Physics("World is locked! Failed to create body");
return null;
}
BodyDef Def = new BodyDef();
Def.Type = Type;
Body Body = World.CreateBody(Def);
PolygonShape Poly = new PolygonShape();
Poly.Set({ new Vector2(X - W / 2, Y - H / 2), new Vector2(X + W / 2, Y - H / 2), new Vector2(X - W / 2, Y + H / 2), new Vector2(X + W / 2, Y + H / 2) });
FixtureDef Fixture = new FixtureDef();
Fixture.Shape = Poly;
Fixture.Friction = 0;
Fixture.Restitution = Den;
Fixture.IsSensor = Sensor;
SetBits(Fixture, Owner);
Body.CreateFixture(Fixture);
Body.SetUserData(Owner);
Poly.Dispose();
return Body;
}
public static Body CreateCircleCentredBody(Entity Owner, float X, float Y, float R, BodyDef.BodyType Type) {
return CreateCircleCentredBody(Owner, X, Y, R, Type, false);
}
public static Body CreateCircleBody(Entity Owner, float X, float Y, float R, BodyDef.BodyType Type, bool Sensor) {
return CreateCircleBody(Owner, X, Y, R, Type, Sensor, 0f);
}
public static Body CreateCircleBody(Entity Owner, float X, float Y, float R, BodyDef.BodyType Type, bool Sensor, float Restitution) {
Log.Physics("Creating circle centred body for " + Owner.GetClass().GetSimpleName() + " with params (" + X + ", " + Y + ", " + R + ") and sensor = " + Sensor);
if (World.IsLocked()) {
Log.Physics("World is locked! Failed to create body");
return null;
}
BodyDef Def = new BodyDef();
Def.Type = Type;
Body Body = World.CreateBody(Def);
CircleShape Poly = new CircleShape();
Poly.SetPosition(new Vector2(R + X, R + Y));
Poly.SetRadius(R);
FixtureDef Fixture = new FixtureDef();
Fixture.Shape = Poly;
Fixture.Friction = 0;
Fixture.IsSensor = Sensor;
Fixture.Restitution = Restitution;
SetBits(Fixture, Owner);
Body.CreateFixture(Fixture);
Body.SetUserData(Owner);
Poly.Dispose();
return Body;
}
public static Body CreateCircleCentredBody(Entity Owner, float X, float Y, float R, BodyDef.BodyType Type, bool Sensor) {
Log.Physics("Creating circle centred body for " + Owner.GetClass().GetSimpleName() + " with params (" + X + ", " + Y + ", " + R + ") and sensor = " + Sensor);
if (World.IsLocked()) {
Log.Physics("World is locked! Failed to create body");
return null;
}
BodyDef Def = new BodyDef();
Def.Type = Type;
Body Body = World.CreateBody(Def);
CircleShape Poly = new CircleShape();
Poly.SetPosition(new Vector2(X, Y));
Poly.SetRadius(R);
FixtureDef Fixture = new FixtureDef();
Fixture.Shape = Poly;
Fixture.Friction = 0;
Fixture.IsSensor = Sensor;
SetBits(Fixture, Owner);
Body.CreateFixture(Fixture);
Body.SetUserData(Owner);
Poly.Dispose();
return Body;
}
public static Body RemoveBody(Body Body) {
if (Body == null) {
return null;
}
ToDestroy.Add(Body);
return null;
}
}
}