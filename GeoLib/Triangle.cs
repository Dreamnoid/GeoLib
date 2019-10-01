using System.Numerics;

namespace GeoLib
{
    public struct Triangle
    {
        public readonly Vector3 A, B, C;

        public Triangle(Vector3 a, Vector3 b, Vector3 c) : this()
        {
            A = a;
            B = b;
            C = c;
        }

        public Vector3 Center => (A + B + C) / 3f;

        public bool IsVertex(Vector3 p) => ((A == p) || (B == p) || (C == p));
    }
}
