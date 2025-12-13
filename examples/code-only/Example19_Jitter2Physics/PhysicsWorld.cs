using Jitter2;

namespace Example19_Jitter2Physics
{
    public class PhysicsWorld
    {
        public static World world;

        public static void Init()
        {
            if (world != null) return;
            world = new World();
            world.SubstepCount = 1;
        }

        public static void Update()
        {
            world.Step(1.0f / 100.0f, true);
        }
    }
}
