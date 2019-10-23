using System;
using System.Collections.Generic;
using System.Numerics;

namespace GeoLib
{
	public static class CatmullRomSpline
	{
		public static IReadOnlyList<Vector2> Generate(Vector2[] points, int steps)
		{
			if (points.Length < 4)
			{
				throw new ArgumentException("A spline requires at least four points");
			}

			var result = new List<Vector2>();

			for (int i = 0; i < points.Length - 3; ++i)
			{
				for (int step = 0; step < steps; ++step)
				{
					var t = step * (1f / steps);
					result.Add(PointOnCurve(
						points[i + 0],
						points[i + 1],
						points[i + 2],
						points[i + 3],
						t));
				}
			}

			result.Add(points[points.Length - 2]);

			return result;
		}

		public static Vector2 PointOnCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
		{
			var t2 = t * t;
			var t3 = t2 * t;
            return new Vector2()
            {
                X = 0.5f * ((2.0f * p1.X) +
                            (-p0.X + p2.X) * t +
                            (2.0f * p0.X - 5.0f * p1.X + 4 * p2.X - p3.X) * t2 +
                            (-p0.X + 3.0f * p1.X - 3.0f * p2.X + p3.X) * t3),
                Y = 0.5f * ((2.0f * p1.Y) +
                            (-p0.Y + p2.Y) * t +
                            (2.0f * p0.Y - 5.0f * p1.Y + 4 * p2.Y - p3.Y) * t2 +
                            (-p0.Y + 3.0f * p1.Y - 3.0f * p2.Y + p3.Y) * t3)
            };
        }

		public static Vector3 GetCatmullRomPosition(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			// The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
			var a = 2f * p1;
			var b = p2 - p0;
			var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
			var d = -p0 + 3f * p1 - 3f * p2 + p3;

			// The cubic polynomial: a + b * t + c * t^2 + d * t^3
			return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
		}
	}
}
