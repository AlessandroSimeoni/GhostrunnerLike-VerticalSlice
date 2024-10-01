using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MeshCut
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(Rigidbody))]
    public class Sliceable : MonoBehaviour
    {
        /*
         Add this script to any gameobject that can be cut
         */

        [Tooltip("The material for the part where the mesh will be cut")] 
        public Material fillMaterial = null;
        [SerializeField] private float cutReadyCooldown = 0.1f;
        [Min(1)] public int maxCuts = 5;

        private bool _cutReady = false;

        public int cutNumber { get; set; } = 0;
        public string originalName { get; set; } = "";

        public delegate void CutEvent();
        public event CutEvent OnCutEvent = null;

        public bool cutReady {
            get { return _cutReady; } 
            set {
                _cutReady = value;
                if (!_cutReady)
                    PrepareForCut().Forget();
            }
        }

        private MeshFilter meshFilter = null;

        public Mesh mesh { get { return meshFilter.mesh; } }

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            if (cutNumber >= maxCuts)
                return;

            if (cutNumber == 0)
                originalName = transform.name;

            PrepareForCut().Forget();
        }

        private void OnBecameInvisible()
        {
            if (cutNumber >= maxCuts)
                Destroy(gameObject);
        }

        private async UniTask PrepareForCut()
        {
            await UniTask.WaitForSeconds(cutReadyCooldown);
            cutReady = true;
        }

        public void OnCut() => OnCutEvent?.Invoke();
    }
}
