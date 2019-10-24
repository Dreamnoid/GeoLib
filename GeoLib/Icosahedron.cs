using System;
using System.Numerics;

namespace GeoLib
{
    public class Icosahedron
    {
        public static Vertex[] Create() => CreateTriangles().ToMesh();

        private static readonly float T = (1.0f + (float)Math.Sqrt(5.0f)) / 2.0f;

        private static readonly Vector3[] Vertices =
        {
            Vector3.Normalize(new Vector3(-1, T, 0)),
            Vector3.Normalize(new Vector3(1, T, 0)),
            Vector3.Normalize(new Vector3(-1, -T, 0)),
            Vector3.Normalize(new Vector3(1, -T, 0)),

            Vector3.Normalize(new Vector3(0, -1, T)),
            Vector3.Normalize(new Vector3(0, 1, T)),
            Vector3.Normalize(new Vector3(0, -1, -T)),
            Vector3.Normalize(new Vector3(0, 1, -T)),

            Vector3.Normalize(new Vector3(T, 0, -1)),
            Vector3.Normalize(new Vector3(T, 0, 1)),
            Vector3.Normalize(new Vector3(-T, 0, -1)),
            Vector3.Normalize(new Vector3(-T, 0, 1))
        };

        internal static Triangle[] CreateTriangles()
        {
            return new[]
            {
                new Triangle(Vertices[0], Vertices[11], Vertices[5]),
                new Triangle(Vertices[0], Vertices[5], Vertices[1]),
                new Triangle(Vertices[0], Vertices[1], Vertices[7]),
                new Triangle(Vertices[0], Vertices[7], Vertices[10]),
                new Triangle(Vertices[0], Vertices[10], Vertices[11]),

                new Triangle(Vertices[1], Vertices[5], Vertices[9]),
                new Triangle(Vertices[5], Vertices[11], Vertices[4]),
                new Triangle(Vertices[11], Vertices[10], Vertices[2]),
                new Triangle(Vertices[10], Vertices[7], Vertices[6]),
                new Triangle(Vertices[7], Vertices[1], Vertices[8]),

                new Triangle(Vertices[3], Vertices[9], Vertices[4]),
                new Triangle(Vertices[3], Vertices[4], Vertices[2]),
                new Triangle(Vertices[3], Vertices[2], Vertices[6]),
                new Triangle(Vertices[3], Vertices[6], Vertices[8]),
                new Triangle(Vertices[3], Vertices[8], Vertices[9]),

                new Triangle(Vertices[4], Vertices[9], Vertices[5]),
                new Triangle(Vertices[2], Vertices[4], Vertices[11]),
                new Triangle(Vertices[6], Vertices[2], Vertices[10]),
                new Triangle(Vertices[8], Vertices[6], Vertices[7]),
                new Triangle(Vertices[9], Vertices[8], Vertices[1])
            };
        }
    }
}
