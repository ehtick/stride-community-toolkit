using Example07_CubeClicker.Managers;
using Example07_CubeClicker.Scripts;
using NexVYaml;
using Stride.CommunityToolkit.Engine;
using Stride.Engine;
using Stride.Graphics;

// This example demonstrates how to load and save game data, specifically tracking left and right mouse clicks on dynamically generated cubes.
// Upon launch, the game automatically loads data from the previous session.
// In case of a corrupted Yaml file, navigate to the \bin\Debug\net8.0\data\ directory and delete the file manually.

using var game = new Game();

// Register all DataContracted Types
NexYamlSerializerRegistry.Init();

game.Run(start: Start);

void Start(Scene rootScene)
{
    game.SetupBase3DScene();
    game.AddGroundGizmo(showAxisName: true);

    CreateAndRegisterGameManagerUI(rootScene);
}

void CreateAndRegisterGameManagerUI(Scene rootScene)
{
    var font = game.Content.Load<SpriteFont>("StrideDefaultFont");
    var gameManager = new GameManager(font);
    game.Services.AddService(gameManager);

    var uiEntity = gameManager.CreateUI();
    uiEntity.Add(new ClickHandlerComponent());
    uiEntity.Scene = rootScene;
}