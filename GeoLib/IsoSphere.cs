using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GeoLib
{
    public static class IsoSphere
    {
        public static Vertex[] Create(int smoothLevel = 0)
        {
            IReadOnlyList<Triangle> triangles = CreateIsocahedronTriangles(CreateIsocahedronVertices());

            for (var i = 0; i < smoothLevel; ++i)
            {
                var smoothedTriangles = new List<Triangle>(triangles.Count * 4);
                foreach (var tri in triangles)
                {
                    var a = GeometryHelper.GetEdgeMiddle(tri.A, tri.B);
                    var b = GeometryHelper.GetEdgeMiddle(tri.B, tri.C);
                    var c = GeometryHelper.GetEdgeMiddle(tri.C, tri.A);

                    smoothedTriangles.Add(new Triangle(tri.A, a, c));
                    smoothedTriangles.Add(new Triangle(tri.B, b, a));
                    smoothedTriangles.Add(new Triangle(tri.C, c, b));
                    smoothedTriangles.Add(new Triangle(a, b, c));
                }

                triangles = smoothedTriangles;
            }

            return triangles.SelectMany(t => new []
            {
                new Vertex(t.A, t.A, UVMapping(t.A)),
                new Vertex(t.B, t.B, UVMapping(t.B)),
                new Vertex(t.C, t.C, UVMapping(t.C))
            }).ToArray();
        }

        private static Vector2 UVMapping(Vector3 normal)
        {
            return new Vector2(
                (float) Math.Asin(normal.X) / (float) Math.PI + 0.5f,
                (float) Math.Asin(normal.Y) / (float) Math.PI + 0.5f);
        }

        private static Vector3[] CreateIsocahedronVertices()
        {
            var t = (1.0f + (float)Math.Sqrt(5.0f)) / 2.0f;
            return new []
            {
                Vector3.Normalize(new Vector3(-1, t, 0)),
                Vector3.Normalize(new Vector3(1, t, 0)),
                Vector3.Normalize(new Vector3(-1, -t, 0)),
                Vector3.Normalize(new Vector3(1, -t, 0)),

                Vector3.Normalize(new Vector3(0, -1, t)),
                Vector3.Normalize(new Vector3(0, 1, t)),
                Vector3.Normalize(new Vector3(0, -1, -t)),
                Vector3.Normalize(new Vector3(0, 1, -t)),

                Vector3.Normalize(new Vector3(t, 0, -1)),
                Vector3.Normalize(new Vector3(t, 0, 1)),
                Vector3.Normalize(new Vector3(-t, 0, -1)),
                Vector3.Normalize(new Vector3(-t, 0, 1))
            };
        }

        private static Triangle[] CreateIsocahedronTriangles(Vector3[] vertices)
        {
            return new[]
            {
                new Triangle(vertices[0], vertices[11], vertices[5]),
                new Triangle(vertices[0], vertices[5], vertices[1]),
                new Triangle(vertices[0], vertices[1], vertices[7]),
                new Triangle(vertices[0], vertices[7], vertices[10]),
                new Triangle(vertices[0], vertices[10], vertices[11]),

                new Triangle(vertices[1], vertices[5], vertices[9]),
                new Triangle(vertices[5], vertices[11], vertices[4]),
                new Triangle(vertices[11], vertices[10], vertices[2]),
                new Triangle(vertices[10], vertices[7], vertices[6]),
                new Triangle(vertices[7], vertices[1], vertices[8]),

                new Triangle(vertices[3], vertices[9], vertices[4]),
                new Triangle(vertices[3], vertices[4], vertices[2]),
                new Triangle(vertices[3], vertices[2], vertices[6]),
                new Triangle(vertices[3], vertices[6], vertices[8]),
                new Triangle(vertices[3], vertices[8], vertices[9]),

                new Triangle(vertices[4], vertices[9], vertices[5]),
                new Triangle(vertices[2], vertices[4], vertices[11]),
                new Triangle(vertices[6], vertices[2], vertices[10]),
                new Triangle(vertices[8], vertices[6], vertices[7]),
                new Triangle(vertices[9], vertices[8], vertices[1])
            };
        }
    }
}
