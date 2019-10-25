using System.Collections.Generic;
using System.Numerics;

namespace GeoLib
{
    public class Capsule
    {
        public static List<Triangle> Create(Vector3 start, float startRadius, Vector3 end, float endRadius, int steps = 24)
        {
            var dir = end - start;
            var startPoints = Disk.CreatePoints(start, Vector3.Normalize(dir), startRadius, steps);
            return GeometryHelper.ExtrudePoints(startPoints, dir);
        }
    }
}
