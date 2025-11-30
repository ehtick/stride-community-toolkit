using Stride.CommunityToolkit.Bullet;
using Stride.CommunityToolkit.Engine;
using Stride.CommunityToolkit.Rendering.ProceduralModels;
using Stride.Core.Mathematics;
using Stride.Engine;

using var game = new Game();

game.Run(start: (Scene rootScene) =>
{
    game.SetupBase2DScene();
    game.AddProfiler();
    game.ShowColliders();

    //var entity = game.Create2DPrimitive(Primitive2DModelType.Capsule, new()
    //{
    //    Material = game.CreateFlatMaterial(Color.Orange),
    //    Size = new Vector2(0.2f, 0.4f),
    //    Depth = 1
    //});

    //entity.Transform.Position = new Vector3(0, 8, 0);

    //entity.Scene = rootScene;

    for (int i = 0; i <= 30; i++)
    {
        var primitive = game.Create2DPrimitive(Primitive2DModelType.Capsule, new()
        {
            Material = game.CreateFlatMaterial(Color.Orange),
            //Size = new Vector2(0.2f, 0.7f),
            //Size = new Vector2(0.1f, 1),
            Depth = 1
        });
        primitive.Transform.Position = new Vector3(0.001f * i, 10 + i * 2, 0);
        primitive.Scene = rootScene;
    }

    var entity2 = game.Create2DPrimitive(Primitive2DModelType.Rectangle, new()
    {
        Material = game.CreateFlatMaterial(Color.Red),
        Size = new Vector2(0.5f, 0.8f),
        Depth = 1
    });

    entity2.Transform.Position = new Vector3(0.2f, 20f, 0);
    //entity2.Add(new DebugRenderComponentScript());
    //entity2.Add(new CollidableGizmoScript()
    //{
    //    Color = new Color4(0.4f, 0.843f, 0, 0.9f),
    //    Visible = false
    //});

    entity2.Scene = rootScene;
});