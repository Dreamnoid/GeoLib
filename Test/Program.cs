using System.Numerics;
using GeoLib;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var mesh = IcoSphere.Create(2);
            Wavefront.Save("sphere.obj", Wavefront.CreatePart(mesh));

            var capsule = Circle.CreateTube(
                Circle.Create(new Vector3(100, 100, 100), Vector3.UnitY, new Vector2(10, 10), 24),
                Circle.Create(new Vector3(-100, -100, -100), Vector3.UnitY, new Vector2(20, 20), 24));
            Wavefront.Save("capsule.obj", Wavefront.CreatePart(capsule.ToMesh()));
        }
    }
}
