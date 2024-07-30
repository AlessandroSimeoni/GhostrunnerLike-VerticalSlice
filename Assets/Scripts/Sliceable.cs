using UnityEngine;

namespace MeshCut
{
    [RequireComponent(typeof(MeshFilter))]
    public class Sliceable : MonoBehaviour
    {
        public Material fillMaterial = null;

        private MeshFilter meshFilter = null;
        public Mesh mesh { get { return meshFilter.mesh; } }

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }
    }
}
