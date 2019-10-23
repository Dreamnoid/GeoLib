using System.Collections.Generic;
using System.Numerics;

namespace GeoLib
{
    public static class GeometryHelper
    {
        public static Vector3 GetFaceNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            var v1 = a - b;
            var v2 = b - c;
            return Vector3.Normalize(new Vector3()
            {
                X = (v1.Y * v2.Z) - (v1.Z * v2.Y),
                Y = (v1.Z * v2.X) - (v1.X * v2.Z),
                Z = (v1.X * v2.Y) - (v1.Y * v2.X)
            });
        }

        public static Vector3 GetEdgeMiddle(Vector3 a, Vector3 b) => (a + b) / 2f;

        public static Vector3 Average(IEnumerable<Vector3> points)
        {
            var result = Vector3.Zero;
            var cnt = 0;
            foreach (var pt in points)
            {
                cnt++;
                result += pt;
            }
            return result / cnt;
        }

        public static void Transform(Vertex[] vertices, Matrix4x4 matrix)
        {
            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] = new Vertex(
                    Vector3.Transform(vertices[i].Position, matrix),
                    Vector3.TransformNormal(vertices[i].Normal, matrix),
                    vertices[i].UV);
            }
        }
    }
}
