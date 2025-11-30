using Stride.CommunityToolkit.Bepu;
using Stride.CommunityToolkit.Engine;
using Stride.CommunityToolkit.Mathematics;
using Stride.CommunityToolkit.Rendering.ProceduralModels;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Rendering.Lights;

var random = new Random(1);
var count = 10;
List<Primitive2DModelType> primitives = [
    Primitive2DModelType.Circle,
    Primitive2DModelType.Capsule,
    Primitive2DModelType.Rectangle,
    Primitive2DModelType.Square,
    Primitive2DModelType.Triangle,
    Primitive2DModelType.Circle,
    Primitive2DModelType.Capsule,
];

using var game = new Game();

game.Run(start: Start);

void Start(Scene rootScene)
{
    game.Window.AllowUserResizing = true;
    game.Window.Title = "Bepu 2D Physics Primitives";

    game.SetupBase2D();
    game.Add2DCameraController();

    var ground = game.Add2DGround();
    ground.Transform.Position = new Vector3(0, -4, 0);

    AddSpotLight(rootScene);

    for (int i = -count / 2; i < count / 2; i++)
    {
        foreach (var (index, primitive2) in primitives.Index())
        {
            var entity = game.Create2DPrimitive(primitive2, new()
            {
                Material = game.CreateFlatMaterial(random.NextColor()),
            });

            entity.Transform.Position = new Vector3(i, 10 + index * 1.5f, 0);
            entity.Scene = rootScene;
        }
    }

    var debugGizmoEntity = new Entity("DebugGizmo")
    {
        new DebugRenderComponentScript(),
        new CollidableGizmoScript()
        {
            Color = new Color4(0.4f, 0.843f, 0, 0.9f),
            Visible = false
        }
    };

    debugGizmoEntity.Scene = rootScene;
}

static void AddSpotLight(Scene rootScene)
{
    var spotLight = new Entity("SpotLight")
    {
        new LightComponent
        {
            Type = new LightSpot
            {
                Range = 20f,
                AngleInner = 20f,
                AngleOuter = 35f
            },
            Intensity = 1000f,
        }
    };

    spotLight.Transform.Position = new Vector3(0, -4, 2);
    spotLight.Scene = rootScene;
}