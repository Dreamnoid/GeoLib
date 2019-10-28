using System.Collections.Generic;
using System.Numerics;

namespace GeoLib
{
	public class Cylinder : List<Circle>
	{
        public static List<Triangle> Create(Vector3 start, float startRadius, Vector3 end, float endRadius, int steps = 24)
        {
            var dir = Vector3.Normalize(end - start);
            return GeometryHelper.ExtrudePoints(
                Disk.CreatePoints(start, dir, startRadius, steps),
                end - start);
        }
	}
}
