using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GeoLib
{
    public static class Extensions
    {
        public static Vertex[] ToMesh(this IEnumerable<Triangle> triangles)
        {
            return triangles.SelectMany(t => new[]
            {
                new Vertex(t.A, t.A, Vector2.Zero),
                new Vertex(t.B, t.B, Vector2.Zero),
                new Vertex(t.C, t.C, Vector2.Zero)
            }).ToArray();
        }
    }
}
