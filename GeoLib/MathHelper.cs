using System;
using System.Numerics;

namespace GeoLib
{
	public static class MathHelper
	{
		public static float Lerp(float min, float max, float t) => min + (t * (max - min));

		public static float Sine(float t) => (float)Math.Sin(t * (float)Math.PI);

        public static Quaternion FromVectors(Vector3 a, Vector3 b)
        {
            return new Quaternion(Vector3.Cross(a, b),
                (float) Math.Sqrt(a.LengthSquared() * b.LengthSquared()) + Vector3.Dot(a, b));
        }
	}
}
