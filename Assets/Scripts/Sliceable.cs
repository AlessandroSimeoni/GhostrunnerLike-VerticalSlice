using UnityEngine;

namespace MeshCut
{
    [RequireComponent(typeof(MeshFilter))]
    public class Sliceable : MonoBehaviour
    {
        /*
         Add this script to any gameobject that can be cut
         */

        [Tooltip("The material for the part where the mesh will be cut")] 
        public Material fillMaterial = null;

        private MeshFilter meshFilter = null;
        public Mesh mesh { get { return meshFilter.mesh; } }

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }
    }
}
