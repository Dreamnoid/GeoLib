using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GeoLib
{
    public static class Subdivision
    {
        public static Vector3[] Subdivide(Vector3[] vertices)
        {
            var faces = BuildFacesFromTriangles(vertices);
            var mesh = new List<Vector3>();
            foreach (var face in faces)
            {
                var a = GetNewPosition(faces, face.A);
                var b = GetNewPosition(faces, face.B);
                var c = GetNewPosition(faces, face.C);

                var facePoint = face.Center;
                mesh.AddFace(a, GetEdgePoint(faces, face.A, face.B), facePoint, GetEdgePoint(faces, face.C, face.A));
                mesh.AddFace(b, GetEdgePoint(faces, face.B, face.C), facePoint, GetEdgePoint(faces, face.A, face.B));
                mesh.AddFace(c, GetEdgePoint(faces, face.C, face.A), facePoint, GetEdgePoint(faces, face.B, face.C));
            }

            return mesh.ToArray();
        }

        private static Vector3 GetNewPosition(IReadOnlyList<Triangle> allFaces, Vector3 vertex)
        {
            var faces = GetFacesForPoint(allFaces, vertex);
            var edges = GetEdgesForPoint(allFaces, vertex);
            if (faces.Count == edges.Count)
            {
                return Barycentre(vertex, faces.Count,
                    GeometryHelper.Average(faces.Select(f => f.Center)),
                    GeometryHelper.Average(edges.Select(p => GeometryHelper.GetEdgeMiddle(vertex, p))));
            }
            else
            {
                var avgMidEdges = GeometryHelper.Average(edges.Where(p => GetFacesForEdge(allFaces, vertex, p).Count == 1).Select(p => GeometryHelper.GetEdgeMiddle(vertex, p)));
                return (vertex + avgMidEdges) / 2f;
            }
        }

        private static void AddFace(this List<Vector3> mesh, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            mesh.Add(a);
            mesh.Add(b);
            mesh.Add(c);

            mesh.Add(a);
            mesh.Add(c);
            mesh.Add(d);
        }

        private static Vector3 Barycentre(Vector3 oldCoords, int nbFaces, Vector3 avgFacePoints, Vector3 avgMidEdges)
        {
            float m1 = (nbFaces - 3) / (float)nbFaces;
            float m2 = 1 / (float)nbFaces;
            float m3 = 2 / (float)nbFaces;
            return (oldCoords * m1) + (avgFacePoints * m2) + (avgMidEdges * m3);
        }

        private static List<Triangle> BuildFacesFromTriangles(Vector3[] vertices)
        {
            var faces = new List<Triangle>();
            for (int i = 0; i < vertices.Length - 2; i += 3)
            {
                faces.Add(new Triangle(
                    vertices[i + 0], 
                    vertices[i + 1], 
                    vertices[i + 2]));
            }
            return faces;
        }

        private static Vector3 GetEdgePoint(IEnumerable<Triangle> inFaces, Vector3 a, Vector3 b)
        {
            var faces = GetFacesForEdge(inFaces, a, b);
            if (faces.Count == 1)
            {
                return (a + b) / 2f;
            }
            else
            {
                return (a + b + faces[0].Center + faces[1].Center) / 4f;
            }
        }

        private static List<Triangle> GetFacesForEdge(IEnumerable<Triangle> faces, Vector3 a, Vector3 b)
        {
            return faces.Where(f => f.IsVertex(a) && f.IsVertex(b)).ToList();
        }

        private static List<Vector3> GetEdgesForPoint(IEnumerable<Triangle> faces, Vector3 p)
        {
            var otherPoints = new HashSet<Vector3>();
            foreach (var face in faces)
            {
                if (face.IsVertex(p))
                {
                    otherPoints.Add(face.A);
                    otherPoints.Add(face.B);
                    otherPoints.Add(face.C);
                }
            }
            return otherPoints.Where(op => op != p).ToList();
        }

        private static List<Triangle> GetFacesForPoint(IEnumerable<Triangle> faces, Vector3 p)
        {
            return faces.Where(f => f.IsVertex(p)).ToList();
        }
    }
}
