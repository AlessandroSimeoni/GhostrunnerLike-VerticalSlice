using UnityEngine;

namespace MeshCut
{
    public class IntersectionPoint
    {
        public Vector3 vertex;
        public Vector3 normal;
        public Vector2 uv;
        public Vector3[] directlyConnectedVertices = new Vector3[2];     // contains the vertices used to calculate the intersection point
    }
}