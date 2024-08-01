using System.Collections.Generic;
using UnityEngine;

namespace MeshCut
{
    public class MeshTriangle
    {
        // each mesh triangle has vertices, normals, uvs and it's associated to a subMesh

        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector2> uvs = new List<Vector2>();
        public int subMeshIndex;

        public MeshTriangle(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int subMeshIndex)
        {
            this.vertices.AddRange(vertices);
            this.normals.AddRange(normals);
            this.uvs.AddRange(uvs);
            this.subMeshIndex = subMeshIndex;
        }
    }
}
