using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib
{
	class Cylinder : List<Circle>
	{
		public const int Precision = 24;

		public float Radius { get; private set; }

		public static List<Triangle> CreateCylinder(Vector3 start, Vector3 end, float startRadius, float endRadius)
		{
			var cylinder = new Cylinder();
			var length = (end - start).Length();
			for (int i = 0; i <= (Precision / 2); ++i)
			{
				float t = (i / (Precision / 2f));
				float radiusAmp = (float)Math.Pow((1 - ((t - 1) * (t - 1))), 0.5f);
				float posAmp = MathHelper.Lerp(startRadius, 0, t);
				cylinder.Add(Circle.Create(new Vector3(0, posAmp, 0), Vector3.UnitY, new Vector2(radiusAmp * startRadius), Precision));
			}
			for (int i = 0; i <= (Precision / 2); ++i)
			{
				float t = (i / (Precision / 2f));
				float radiusAmp = (float)Math.Pow((1 - (t * t)), 0.5f);
				float posAmp = MathHelper.Lerp(0, endRadius, t);
				cylinder.Add(Circle.Create(new Vector3(0, -length - posAmp, 0), Vector3.UnitY, new Vector2(radiusAmp * endRadius), Precision));
			}

			var lookVector = new Vector3(0f, -1f, 0f);
			var view = Vector3.Normalize(end - start);
			var rotationAxis = Vector3.Normalize(Vector3.Cross(lookVector, view));
			var rotationAngle = (float)Math.Acos(Vector3.Dot(lookVector, view) / lookVector.Length() / view.Length());
			var lookAt = Matrix4x4.CreateFromAxisAngle(rotationAxis, rotationAngle);

			return cylinder.ToMesh().Select(t => t.Transform(lookAt * Matrix4x4.CreateTranslation(start))).ToList();
		}

		public static Cylinder Create(Vector3 start, Vector3 end, float startRadius, float endRadius)
		{
			var dir = Vector3.Normalize(end - start);
			var cylinder = new Cylinder(start, startRadius);
			cylinder.Extrude(end - start, endRadius / startRadius);
			cylinder.Cap();
			return cylinder;
		}

		private Cylinder() { }

		public Cylinder(Vector3 start, float radius)
		{
			Radius = radius;
			for (int i = 0; i <= (Precision / 2); ++i)
			{
				float t = (i / (Precision / 2f));
				float radiusAmp = MathHelper.Sine(t / 2f);
				float posAmp = MathHelper.Lerp(radius, 0, t);
				Add(Circle.Create(start + new Vector3(0, posAmp, 0), Vector3.UnitY, new Vector2(radiusAmp * radius), Precision));
			}
		}

		public void Extrude(Vector3 offset, float scale)
		{
			Radius *= scale;
			var previous = this.Last();
			Add(Circle.Create(previous.Center + offset, Vector3.UnitY, new Vector2(Radius), Precision));
		}

		public void Cap()
		{
			var previous = this.Last().Center;
			for (int i = 0; i <= (Precision / 2); ++i)
			{
				float t = (i / (float)(Precision / 2));
				float radiusAmp = MathHelper.Sine(t / 2f);
				float posAmp = MathHelper.Lerp(Radius, 0, t);
				Add(Circle.Create(previous - new Vector3(0, posAmp, 0), Vector3.UnitY, new Vector2(radiusAmp * Radius), Precision));
			}
		}

		public List<Triangle> ToMesh()
		{
			var mesh = new List<Triangle>();
			for (int i = 0; i < Count - 1; ++i)
			{
				var a = this[i + 0];
				var b = this[i + 1];
				mesh.AddRange(Circle.CreateTube(a, b));
			}
			//mesh.Append(this.First().CreateCap());
			//mesh.Append(this.Last().CreateCap());
			return mesh;
		}
	}
}
