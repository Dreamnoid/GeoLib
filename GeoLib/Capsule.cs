using System;
using System.Collections.Generic;
using System.Numerics;

namespace GeoLib
{
    public class Capsule
    {
        public static List<Triangle> Create(Vector3 start, float startRadius, Vector3 end, float endRadius, int steps = 24, int precision = 12)
        {
            var dir = Vector3.Normalize(end - start);
            var startPoints = Disk.CreatePoints(start, dir, startRadius, steps);
            var endPoints = Disk.CreatePoints(end, dir, endRadius, steps);
            var triangles = GeometryHelper.ExtrudePoints(startPoints, endPoints);

            triangles.AddRange(CreateCap(start, startRadius, Vector3.Normalize(start - end), steps, precision));
            triangles.AddRange(CreateCap(end, endRadius, Vector3.Normalize(end - start), steps, precision));

            return triangles;
        }

        private static List<Triangle> CreateCap(Vector3 start, float radius, Vector3 normal, int steps, int precision)
        {
            var triangles = new List<Triangle>();
            var end = start + normal * radius;
            for (int i = 0; i < precision; ++i)
            {
                float t1 = ((i + 0) / (float) precision);
                float t2 = ((i + 1) / (float) precision);

                float radius1 = MathHelper.Lerp(0, radius, (float) Math.Sqrt(1f - (t1 * t1)));
                float radius2 = MathHelper.Lerp(0, radius, (float) Math.Sqrt(1f - (t2 * t2)));

                var pos1 = Vector3.Lerp(start, end, t1);
                var pos2 = Vector3.Lerp(start, end, t2);

                var startPoints = Disk.CreatePoints(pos1, normal, radius1, steps);
                var endPoints = Disk.CreatePoints(pos2, normal, radius2, steps);

                triangles.AddRange(GeometryHelper.ExtrudePoints(startPoints, endPoints));
            }

            return triangles;
        }
    }
}
