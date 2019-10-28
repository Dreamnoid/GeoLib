using System;
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

        public static void UVMap(Vertex[] vertices, Func<Vector3, Vector3, Vector2> mapper)
        {
            for (int i = 0; i < vertices.Length; ++i)
            {
                vertices[i] = new Vertex(
                    vertices[i].Position,
                    vertices[i].Normal,
                    mapper(vertices[i].Position, vertices[i].Normal));
            }
        }

        public static List<Triangle> ExtrudePoints(Vector3[] points, Vector3 vector)
        {
            var mesh = new List<Triangle>(points.Length * 2);
            for(int i = 0; i < points.Length; ++i)
            {
                int ip1 = (i + 1) % points.Length;

                mesh.Add(new Triangle(
                    points[i],
                    points[i] + vector,
                    points[ip1]));

                mesh.Add(new Triangle(
                    points[i] + vector,
                    points[ip1] + vector,
                    points[ip1]));
            }
            return mesh;
        }

        public static List<Triangle> ExtrudePoints(Vector3[] source, Vector3[] target)
        {
            if(source.Length != target.Length)
                throw new InvalidOperationException("ExtrudePoints: the two shapes must have the same number of points");

            var mesh = new List<Triangle>(source.Length * 2);
            for (int i = 0; i < source.Length; ++i)
            {
                int ip1 = (i + 1) % source.Length;

                mesh.Add(new Triangle(
                    source[i],
                    target[i] ,
                    source[ip1]));

                mesh.Add(new Triangle(
                    target[i],
                    target[ip1],
                    source[ip1]));
            }
            return mesh;
        }
    }
}
