using UnityEngine;

namespace Utility
{
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private float sphereCastOriginHeightOffset = 0.5f;
        [SerializeField] private float sphereCastRadius = 0.15f;
        [SerializeField] private float sphereCastGroundOffset = 0.001f;

        public bool isGrounded { get; private set; } = true;

        private float sphereCastDistance = 0.0f;
        private Ray ray;

        private void Start()
        {
            sphereCastDistance = sphereCastOriginHeightOffset - sphereCastRadius + sphereCastGroundOffset;
        }

        private void Update()
        {
            ray = new Ray(transform.position + Vector3.up * sphereCastOriginHeightOffset, Vector3.down);
            isGrounded = Physics.SphereCast(ray, sphereCastRadius, sphereCastDistance);
        }
    }
}
