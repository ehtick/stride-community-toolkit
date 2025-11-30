using Stride.CommunityToolkit.Bepu;
using Stride.CommunityToolkit.Engine;
using Stride.CommunityToolkit.Mathematics;
using Stride.CommunityToolkit.Rendering.ProceduralModels;
using Stride.Core.Mathematics;
using Stride.Engine;

var random = new Random(1);
List<Primitive2DModelType> primitives = [
    Primitive2DModelType.Circle,
    Primitive2DModelType.Capsule,
    Primitive2DModelType.Rectangle,
    Primitive2DModelType.Square,
    Primitive2DModelType.Triangle,
];

using var game = new Game();

game.Run(start: Start);

void Start(Scene rootScene)
{
    game.SetupBase2DScene();

    foreach (var (index, primitive) in primitives.Index())
    {
        var entity = game.Create2DPrimitive(primitive, new()
        {
            Material = game.CreateFlatMaterial(random.NextColor()),
        });

        entity.Transform.Position = new Vector3(0, 10 + index * 1.5f, 0);
        entity.Scene = rootScene;
    }
}