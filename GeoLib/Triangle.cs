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

        public Vector3 Normal
        {
            get
            {
                var v1 = A - B;
                var v2 = B - C;
                return Vector3.Normalize(new Vector3(
                    (v1.Y * v2.Z) - (v1.Z * v2.Y),
                    (v1.Z * v2.X) - (v1.X * v2.Z),
                    (v1.X * v2.Y) - (v1.Y * v2.X)
                ));
            }
        }

        public Triangle Reverse() => new Triangle(C, B, A);
    }
}
