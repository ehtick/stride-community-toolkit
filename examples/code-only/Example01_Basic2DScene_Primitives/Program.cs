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

/*
---example-metadata
title:
  en: Basic2D Scene (Multiple Primitives)
  cs: Základní 2D scéna (Více primitiv)
level: Getting Started
category: Shapes
complexity: 1
description:
  en: |
    Create a minimal 2D scene using toolkit helpers and place multiple different primitive shapes.
    Demonstrates entity creation, basic positioning, and attaching the entities to the scene.
  cs: |
    Vytvoření minimální 2D scény pomocí nástrojů sady a umístění více různých primitivních tvarů.
    Ukazuje vytvoření entity, základní umístění a připojení entit k scéně.
concepts:
    - Creating multiple 2D primitives (Circle, Capsule, Rectangle, Square, Triangle)
    - Positioning entities with Transform.Position
    - Adding entities to a Scene (rootScene)
    - "Using helpers: SetupBase2DScene"
related:
    - Example01_Basic2DScene_DebugRender
    - Example01_Basic2DScene_BulletPhysics
    - Example01_Basic2DScene
tags:
    - 2D
    - Bepu
    - Shapes
    - Primitives
    - Scene Setup
    - Transform
    - Position
    - Getting Started
Order: 2
enabled: true
created: 2025-11-30
---
*/