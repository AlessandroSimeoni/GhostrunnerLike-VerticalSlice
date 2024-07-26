using System.Collections.Generic;
using UnityEngine;

namespace MeshCut
{
    public class MeshTriangle
    {
        // each triangle has vertices, normals, uvs and it's associated to a subMesh

        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector2> uvs = new List<Vector2>();
        public int subMeshIndex;

        public MeshTriangle(List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs, int subMeshIndex)
        {
            this.vertices.AddRange(vertices);
            this.normals.AddRange(normals);
            this.uvs.AddRange(uvs);
            this.subMeshIndex = subMeshIndex;
        }
    }
}
