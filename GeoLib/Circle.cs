using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GeoLib
{
    public class Circle : List<Vector3>
    {
        public readonly Vector3 Center;

        private Circle(Vector3 center)
        {
            Center = center;
        }

        public Vector3 GetPoint(int i) =>this[i % Count];

        public static Circle Create(Vector3 center, Vector3 normal, Vector2 radius, int steps)
        {
            var points2D = new List<Vector3>(steps);
            float angleStep = (float) Math.PI * 2f / steps;
            float angle = 0;
            for (int i = 0; i < steps; ++i)
            {
                float x = center.X + radius.X * (float) Math.Cos(angle);
                float y = center.Y + radius.Y * (float) Math.Sin(angle);
                points2D.Add(new Vector3(x, y, 0));
                angle += angleStep;
            }

            var lookAt = Matrix4x4.CreateLookAt(center, center + normal, Vector3.UnitY);
            var circle = new Circle(center);
            circle.AddRange(points2D.Select(pt => Vector3.Transform(pt, lookAt)));
            return circle;
        }

        public List<Triangle> CreateCap()
        {
            var mesh = new List<Triangle>(Count);
            for (int i = 0; i < Count - 1; ++i)
            {
                mesh.Add(new Triangle(
                    Center,
                    GetPoint(i),
                    GetPoint(i + 1)));
            }

            return mesh;
        }

        public static List<Triangle> CreateTube(Circle a, Circle b)
        {
            if (a.Count != b.Count)
                throw new InvalidOperationException("The two circles must have the same resolution");

            var mesh = new List<Triangle>(a.Count);
            for (int i = 0; i < a.Count - 1; ++i)
            {
                mesh.Add(new Triangle(
                    a.GetPoint(i),
                    a.GetPoint(i + 1),
                    b.GetPoint(i)));

                mesh.Add(new Triangle(
                    b.GetPoint(i),
                    a.GetPoint(i + 1),
                    b.GetPoint(i + 1)));
            }

            return mesh;
        }
    }
}
