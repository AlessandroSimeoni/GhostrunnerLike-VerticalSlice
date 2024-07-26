using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCut
{
    using UnityEngine;

    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TestPlane : MonoBehaviour
    {
        public Plane plane;

        void Start()
        {
            // Create the plane with a normal pointing to the left and passing through the origin
            Plane plane = new Plane(Vector3.left, 0.0f);

            // Create the mesh to visualize the plane
            CreatePlaneMesh(plane);
        }

        void CreatePlaneMesh(Plane plane)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;

            // Define the size of the plane
            float planeSize = 10f;

            // Calculate the right and up vectors based on the plane's normal
            Vector3 right = Vector3.Cross(Vector3.up, plane.normal).normalized * planeSize;
            Vector3 up = Vector3.Cross(plane.normal, right).normalized * planeSize;

            // Define vertices
            Vector3[] vertices = new Vector3[4]
            {
            plane.normal * plane.distance + (right + up) / 2,
            plane.normal * plane.distance + (right - up) / 2,
            plane.normal * plane.distance + (-right + up) / 2,
            plane.normal * plane.distance + (-right - up) / 2
            };

            // Define triangles
            int[] triangles = new int[6]
            {
            0, 2, 1,
            2, 3, 1
            };

            // Define UVs
            Vector2[] uv = new Vector2[4]
            {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0)
            };

            // Assign vertices, triangles, and UVs to the mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;

            // Recalculate normals for lighting
            mesh.RecalculateNormals();
        }
    }

}
