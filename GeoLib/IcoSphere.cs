using System;
using System.Collections.Generic;
using System.Numerics;

namespace GeoLib
{
    public static class IcoSphere
    {
        public static Vertex[] Create(int smoothLevel = 0)
        {
            IReadOnlyList<Triangle> triangles = Icosahedron.CreateTriangles();

            for (var i = 0; i < smoothLevel; ++i)
            {
                var smoothedTriangles = new List<Triangle>(triangles.Count * 4);
                foreach (var tri in triangles)
                {
                    var a = Vector3.Normalize(GeometryHelper.GetEdgeMiddle(tri.A, tri.B));
                    var b = Vector3.Normalize(GeometryHelper.GetEdgeMiddle(tri.B, tri.C));
                    var c = Vector3.Normalize(GeometryHelper.GetEdgeMiddle(tri.C, tri.A));

                    smoothedTriangles.Add(new Triangle(tri.A, a, c));
                    smoothedTriangles.Add(new Triangle(tri.B, b, a));
                    smoothedTriangles.Add(new Triangle(tri.C, c, b));
                    smoothedTriangles.Add(new Triangle(a, b, c));
                }

                triangles = smoothedTriangles;
            }

            var mesh = triangles.ToMesh();
            GeometryHelper.UVMap(mesh, UVMapping);
            return mesh;
        }

        private static Vector2 UVMapping(Vector3 position, Vector3 normal)
        {
            return new Vector2(
                (float) Math.Asin(normal.X) / (float) Math.PI + 0.5f,
                (float) Math.Asin(normal.Y) / (float) Math.PI + 0.5f);
        }
    }
}
