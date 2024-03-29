using System.Numerics;
using System.Xml;
using GeoLib;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var mesh = IcoSphere.Create(2);
            //Wavefront.Save("sphere.obj", Wavefront.CreatePart(mesh));

            //var capsule = Circle.CreateTube(
            //    Circle.Create(new Vector3(100, 100, 100), Vector3.UnitY, new Vector2(10, 10), 24),
            //    Circle.Create(new Vector3(-100, -100, -100), Vector3.UnitY, new Vector2(20, 20), 24));
            //Wavefront.Save("capsule.obj", Wavefront.CreatePart(capsule.ToMesh()));

            var capsule = Capsule.Create(new Vector3(0, 0, 0), 100, new Vector3(100, 100, 100), 120);

            Wavefront.Save("disk.obj", 
                Wavefront.CreatePart(capsule.ToMesh()));
        }
    }
}
