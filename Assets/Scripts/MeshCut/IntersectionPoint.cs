using UnityEngine;

namespace MeshCut
{
    public class IntersectionPoint
    {
        /* 
         This class represents an intersection point between the mesh and the plane which is cutting it.
         This points are used to process new triangles for left/right mesh and the filling for the hole generated in the meshes after the cut.
        */

        public Vector3 vertex;
        public Vector3 normal;
        public Vector2 uv;
        public Vector3[] directlyConnectedVertices = new Vector3[2];     // contains the vertices used to calculate the intersection point
    }
}