using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace GeoLib
{
    public class Wavefront
    {
        public class Part
        {
            public string Name { get; set; }

            public string Material { get; set; }

            public List<Vertex> Vertices { get; } = new List<Vertex>();

            public List<int> Indices { get; } = new List<int>();

            public override string ToString() => Name;
        }

        public static Part CreatePart(IEnumerable<Vertex> vertices)
        {
            var part = new Part();
            part.Vertices.AddRange(vertices);
            for (int idx = 0; idx < part.Vertices.Count; ++idx)
            {
                part.Indices.Add(idx);
            }
            return part;
        }

        public static List<Part> Load(byte[] data)
        {
            return Load(Encoding.ASCII.GetString(data).Split('\n', '\r'));
        }

        public static List<Part> Load(string filename)
        {
            return Load(File.ReadAllLines(filename));
        }

        private static readonly CultureInfo CultureInfo = new CultureInfo("en-us");
        private static readonly char[] Separator = { ' ' };
        private static readonly char[] FaceSeparator = { '/' };

        public static List<Part> Load(string[] lines)
        {
            // The global geometry
            var positions = new List<Vector3>();
            var normals = new List<Vector3>();
            var texcoords = new List<Vector3>();

            var vertices = new List<Vertex>();
            var indices = new List<int>();

            var mesh = new List<Part>();

            var currentGroup = "";
            var currentMaterial = "unknown";

            // Parse all lines
            foreach (var rawLine in lines)
            {
                // Skip the line if empty or is a comment
                var line = rawLine.Trim();
                if (line == "" || line.StartsWith("#")) continue;

                // Cut the line in different parts
                var parts = line.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                var type = parts[0];

                // Vertex declaration
                if (type.StartsWith("v"))
                {
                    var vector = ParseVector3(parts);
                    if (type == "v")
                    {
                        positions.Add(vector);
                    }
                    else if (type == "vt")
                    {
                        texcoords.Add(vector);
                    }
                    else if (type == "vn")
                    {
                        normals.Add(vector);
                    }
                }
                else if (type == "f")
                {
                    // Face declaration
                    if (currentGroup != "")
                    {
                        // Add the vertices to the group
                        vertices.Add(GetVertexFromF(parts[1], positions, texcoords, normals));
                        var index1 = vertices.Count - 1;

                        vertices.Add(GetVertexFromF(parts[2], positions, texcoords, normals));
                        var index2 = vertices.Count - 1;

                        vertices.Add(GetVertexFromF(parts[3], positions, texcoords, normals));
                        var index3 = vertices.Count - 1;

                        // Add the indices
                        indices.Add(index1);
                        indices.Add(index2);
                        indices.Add(index3);

                        // Handle quads
                        if (parts.Length > 4)
                        {
                            vertices.Add(GetVertexFromF(parts[4], positions, texcoords, normals));
                            var index4 = vertices.Count - 1;

                            indices.Add(index1);
                            indices.Add(index3);
                            indices.Add(index4);
                        }
                    }
                }
                else if (type == "g" || type == "o")
                {
                    string name = string.Join("_", parts.Skip(1));
                    if (name != currentGroup)
                    {
                        if (currentGroup != "")
                        {
                            // Add the group to the mesh
                            var group = new Part() { Name = currentGroup, Material = currentMaterial };
                            group.Vertices.AddRange(vertices);
                            group.Indices.AddRange(indices);
                            mesh.Add(group);

                            vertices.Clear();
                            indices.Clear();
                        }
                        currentGroup = name;
                    }
                }
                else if (type.StartsWith("usemtl"))
                {
                    currentMaterial = (parts.Length > 1) ? parts[1] : "";
                }
            }

            // Add the last group to the mesh
            if (vertices.Any() && indices.Any())
            {
                var group = new Part() { Name = currentGroup, Material = currentMaterial };
                group.Vertices.AddRange(vertices);
                group.Indices.AddRange(indices);
                mesh.Add(group);
            }

            // Recalculate the normals if needed
            if (!normals.Any())
            {
                // Recreate the normals
                foreach (var part in mesh)
                {

                    // Default normal for every different vertex
                    var normalsPerVertex = new Dictionary<Vector3, Vector3>();
                    foreach (var vertex in part.Vertices)
                    {
                        normalsPerVertex[vertex.Position] = new Vector3();
                    }

                    for (int i = 0; i < (part.Indices.Count - 2); i += 3)
                    {
                        // Compute the normal for each triangle
                        var i1 = part.Indices[i + 0];
                        var i2 = part.Indices[i + 1];
                        var i3 = part.Indices[i + 2];

                        var pos1 = part.Vertices[i1].Position;
                        var pos2 = part.Vertices[i2].Position;
                        var pos3 = part.Vertices[i3].Position;

                        var normal = GeometryHelper.GetFaceNormal(pos1, pos2, pos3);

                        // Add the normals
                        normalsPerVertex[pos1] = Vector3.Normalize(normalsPerVertex[pos1] + normal);
                        normalsPerVertex[pos2] = Vector3.Normalize(normalsPerVertex[pos2] + normal);
                        normalsPerVertex[pos3] = Vector3.Normalize(normalsPerVertex[pos3] + normal);
                    }

                    // Apply the new normals
                    for (int i = 0; i < part.Vertices.Count; ++i)
                    {
                        var vertex = part.Vertices[i];
                        vertex.Normal = normalsPerVertex[vertex.Position];
                        part.Vertices[i] = vertex;
                    }

                }
            }

            // Return the mesh
            return mesh;
        }

        private static Vector3 ParseVector3(string[] parts)
        {
            if (parts.Length > 3)
            {
                return new Vector3(Convert.ToSingle(parts[1], CultureInfo), Convert.ToSingle(parts[2], CultureInfo), Convert.ToSingle(parts[3], CultureInfo));
            }
            else
            {
                return new Vector3(Convert.ToSingle(parts[1], CultureInfo), Convert.ToSingle(parts[2], CultureInfo), 0);
            }
        }

        private static Vertex GetVertexFromF(string f, IReadOnlyList<Vector3> positions, IReadOnlyList<Vector3> texcoords, IReadOnlyList<Vector3> normals)
        {
            var tex = new Vector3();
            var norm = new Vector3();

            var parts = f.Split(FaceSeparator);

            var vertexPositionIndex = Convert.ToInt32(parts[0]) - 1;
            var pos = positions[vertexPositionIndex];

            if (parts.Length > 1)
            {
                // Texture coordinates
                var vertexTexCoordIndex = Convert.ToInt32(parts[1]) - 1;
                tex = texcoords[vertexTexCoordIndex];

                // Normals
                if (parts.Length > 2)
                {
                    var vertexNormalIndex = Convert.ToInt32(parts[2]) - 1;
                    norm = normals[vertexNormalIndex];
                }
            }

            return new Vertex(pos, norm, new Vector2(tex.X, 1f - tex.Y));
        }

        public static string Write(IEnumerable<Part> parts)
        {
            // Write the OBJ file
            var sb = new StringBuilder();

            sb.AppendLine("# GeoLib");
            sb.AppendLine();

            int baseIdx = 0;
            foreach (var part in parts)
            {
                sb.Append("g ").AppendLine(part.Name);
                sb.Append("usemtl ").AppendLine(part.Material);
                foreach (var v in part.Vertices)
                {
                    sb.AppendLine("v " + v.Position.X.ToString("0.000000", CultureInfo) + " " + v.Position.Y.ToString("0.000000", CultureInfo) + " " + v.Position.Z.ToString("0.000000", CultureInfo));
                }

                sb.AppendLine();


                foreach (var v in part.Vertices)
                {
                    sb.AppendLine("vt " + v.UV.X.ToString("0.000000", CultureInfo) + " " + v.UV.Y.ToString("0.000000", CultureInfo));
                }

                sb.AppendLine();

                foreach (var v in part.Vertices)
                {
                    sb.AppendLine("vn " + v.Normal.X.ToString("0.000000", CultureInfo) + " " + v.Normal.Y.ToString("0.000000", CultureInfo) + " " + v.Normal.Z.ToString("0.000000", CultureInfo));
                }

                sb.AppendLine();

                int count = part.Vertices.Count();
                for (int i = 0; i < count - 2; i += 3)
                {
                    int v1 = baseIdx + i + 1;
                    int v2 = baseIdx + i + 2;
                    int v3 = baseIdx + i + 3;

                    sb.AppendLine("f " + v1 + "/" + v1 + "/" + v1 + " " + v2 + "/" + v2 + "/" + v2 + " " + v3 + "/" + v3 + "/" + v3);
                }

                sb.AppendLine();

                baseIdx += part.Vertices.Count;
            }

            return sb.ToString();
        }

        public static void Save(string filename, params Part[] parts)
        {
            File.WriteAllText(filename, Write(parts));
        }
        public static void Save(string filename, IEnumerable<Part> parts)
        {
            File.WriteAllText(filename, Write(parts));
        }
    }
}
