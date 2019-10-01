using System;

namespace GeoLib
{
	public static class MathHelper
	{
		public static float Lerp(float min, float max, float t) => min + (t * (max - min));

		public static float Sine(float t) => (float)Math.Sin(t * (float)Math.PI);
	}
}
