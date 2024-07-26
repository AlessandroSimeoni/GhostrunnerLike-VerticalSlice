using System.Collections.Generic;
using UnityEngine;

namespace MeshCut
{
    public class CutGeneratedMesh
    {
        /*
         Each mesh has vertices, normals, uvs and a list of triangles (subMeshTriangles) for each submesh.
         The subMeshTriangles is a list of list of integers because for each submesh index it contains 
         the corresponding list of triangles (each mesh can have more than one submesh)
        */

        public List<Vector3> vertices = new List<Vector3>();
        public List<Vector3> normals = new List<Vector3>();
        public List<Vector2> uvs = new List<Vector2>();
        public List<List<int>> subMeshTriangles = new List<List<int>>();
        
        /// <summary>
        /// Add a triangle to this mesh.
        /// </summary>
        /// <param name="triangle">the triangle to add</param>
        public void AddTriangle(MeshTriangle triangle)
        {
            // if the subMeshTriangles list doesn't have enough entries to handle the triangle's subMeshIndex, then add them to the list
            for (int i = subMeshTriangles.Count; i <= triangle.subMeshIndex; i++)
                subMeshTriangles.Add(new List<int>());

            // each entry in the subMeshTriangles list contains indexes referencing the vertices in the vertices list
            for (int i = 0; i < 3; i++)
                subMeshTriangles[triangle.subMeshIndex].Add(vertices.Count + i);

            vertices.AddRange(triangle.vertices);
            normals.AddRange(triangle.normals);
            uvs.AddRange(triangle.uvs);
        }
    }
}
