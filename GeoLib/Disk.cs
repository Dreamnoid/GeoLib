using System;
using System.Numerics;

namespace GeoLib
{
    public class Disk
    {
        public static Vector3[] CreatePoints(Vector3 center, Vector3 direction, float radius, int steps)
        {
            var quaternion = Quaternion.Normalize(MathHelper.FromVectors(Vector3.UnitY, direction));
            var side = Vector3.Transform(Vector3.UnitX, quaternion);
            var up = Vector3.Transform(Vector3.UnitZ, quaternion);

            var points = new Vector3[steps];
            for (int i = 0; i < steps; ++i)
            {
                var angle = MathHelper.Lerp(0, Pi.FullCircle, (i / (float)steps));
                var x = radius * (float)Math.Cos(angle);
                var y = radius * (float)Math.Sin(angle);
                points[i] = center + (x * side) + (y * up);
            }

            return points;
        }
    }
}
