using System.Linq;
using System.Numerics;

namespace GeoLib
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        public Vertex(Vector3 pos, Vector3 normal, Vector2 uv) : this()
        {
            Position = pos;
            Normal = normal;
            UV = uv;
        }

        public static Vertex[] FromPositions(Vector3[] positions) =>
            positions.Select(p => new Vertex(p, Vector3.Zero, Vector2.Zero)).ToArray();
    }

}
