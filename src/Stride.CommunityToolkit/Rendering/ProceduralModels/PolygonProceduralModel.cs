using Stride.Graphics;
using Stride.Rendering.ProceduralModels;

namespace Stride.CommunityToolkit.Rendering.ProceduralModels;

/// <summary>
/// Generates a planar polygon mesh (convex fan triangulation) from an arbitrary set of 2D vertices.
/// </summary>
public class PolygonProceduralModel : PrimitiveProceduralModelBase
{
    /// <summary>
    /// Vertex positions (XY plane) defining the polygon outline. Must contain at least 3 vertices.
    /// </summary>
    public Vector2[] Vertices { get; set; } = [];

    /// <summary>
    /// Circumradius of the regular polygon. Used when <see cref="Vertices"/> is empty.
    /// </summary>
    public float Radius { get; set; } = 0.5f;

    /// <summary>
    /// Number of sides of the regular polygon (minimum 3). Used when <see cref="Vertices"/> is empty.
    /// </summary>
    public int Sides { get; set; } = 6;

    private static readonly Dictionary<string, GeometricMeshData<VertexPositionNormalTexture>> _meshCache = [];

    /// <inheritdoc />
    protected override GeometricMeshData<VertexPositionNormalTexture> CreatePrimitiveMeshData()
    {
        if (Vertices.Length >= 3)
            return New(Vertices, UvScale.X, UvScale.Y);

        return New(GenerateRegularPolygonVertices(Radius, Sides), UvScale.X, UvScale.Y);
    }

    /// <summary>
    /// Generates vertices for a regular polygon centered at the origin.
    /// </summary>
    public static Vector2[] GenerateRegularPolygonVertices(float radius, int sides)
    {
        if (sides < 3) throw new ArgumentOutOfRangeException(nameof(sides), "Sides must be >= 3");

        var vertices = new Vector2[sides];
        var angleStep = MathF.Tau / sides;

        for (var i = 0; i < sides; i++)
        {
            var angle = i * angleStep - MathF.PI / 2; // start at top
            vertices[i] = new Vector2(MathF.Cos(angle) * radius, MathF.Sin(angle) * radius);
        }

        return vertices;
    }

    /// <summary>
    /// Convenience factory for an isosceles triangle centered at the origin.
    /// </summary>
    public static PolygonProceduralModel CreateTriangle(Vector2 size)
    {
        return new PolygonProceduralModel
        {
            Vertices =
            [
                new(0, size.Y / 2),
                new(-size.X / 2, -size.Y / 2),
                new(size.X / 2, -size.Y / 2)
            ]
        };
    }

    /// <summary>
    /// Convenience factory for an axis-aligned rectangle centered at the origin.
    /// </summary>
    public static PolygonProceduralModel CreateRectangle(Vector2 size)
    {
        return new PolygonProceduralModel
        {
            Vertices =
            [
                new(-size.X / 2, -size.Y / 2),
                new(-size.X / 2, size.Y / 2),
                new(size.X / 2, size.Y / 2),
                new(size.X / 2, -size.Y / 2)
            ]
        };
    }

    /// <summary>
    /// Creates (or retrieves from cache) a mesh for the supplied polygon vertex list.
    /// </summary>
    public static GeometricMeshData<VertexPositionNormalTexture> New(Vector2[] vertices, float uScale = 1.0f, float vScale = 1.0f, bool toLeftHanded = false)
    {
        if (vertices.Length < 3)
            throw new ArgumentException("A polygon must have at least 3 vertices", nameof(vertices));

        var hash = string.Join(",", vertices.Select(v => $"{v.X},{v.Y}"));
        var cacheKey = $"{hash}_{uScale}_{vScale}_{toLeftHanded}";

        if (!_meshCache.TryGetValue(cacheKey, out var mesh))
        {
            mesh = CreateMesh(vertices, uScale, vScale, toLeftHanded);
            _meshCache[cacheKey] = mesh;
        }

        return mesh;
    }

    /// <summary>
    /// Builds a new mesh for the given points (no caching). Assumes convex ordering; uses fan triangulation.
    /// </summary>
    public static GeometricMeshData<VertexPositionNormalTexture> CreateMesh(Vector2[] points, float uScale = 1.0f, float vScale = 1.0f, bool toLeftHanded = false)
    {
        int vertexCount = points.Length;

        Span<VertexPositionNormalTexture> vertices = new VertexPositionNormalTexture[vertexCount];
        Span<int> indices = new int[(vertexCount - 2) * 3];

        Vector2 centroid = Vector2.Zero;
        foreach (var point in points)
            centroid += point;

        centroid /= vertexCount;

        for (int i = 0; i < vertexCount; i++)
        {
            Vector2 relativePos = points[i] - centroid;

            Vector2 uv = new(
                (relativePos.X / points.Max(p => Math.Abs(p.X - centroid.X)) + 1) * 0.5f * uScale,
                (relativePos.Y / points.Max(p => Math.Abs(p.Y - centroid.Y)) + 1) * 0.5f * vScale
            );

            vertices[i] = new VertexPositionNormalTexture(
                new Vector3(points[i].X, points[i].Y, 0),
                Vector3.UnitZ,
                uv
            );
        }

        // Fixed winding order
        for (var i = 0; i < vertexCount - 2; i++)
        {
            indices[i * 3] = 0;
            indices[i * 3 + 1] = i + 2;
            indices[i * 3 + 2] = i + 1;
        }

        return new GeometricMeshData<VertexPositionNormalTexture>(vertices.ToArray(), indices.ToArray(), toLeftHanded) { Name = "Polygon" };
    }
}