using UnityEngine;

namespace Utilities
{
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] private float sphereCastOriginHeightOffset = 0.5f;
        [SerializeField] private float sphereCastRadius = 0.15f;
        [SerializeField] private float sphereCastGroundOffset = 0.001f;
        [SerializeField] private LayerMask layerMask;
        [Header("Slope check")]
        [SerializeField] private GroundSlopeCheckModel slopeCheckModel = null;

        public bool isGrounded { get; private set; } = true;

        private float sphereCastDistance = 0.0f;
        private float raycastDistance = 0.0f;
        private Ray ray;
        private RaycastHit hit;
        private float minSlopeCos = 0.0f;
        private float maxSlopeCos = 0.0f;

        public Vector3 groundNormal {  get; private set; } = Vector3.zero;

        private const float DOT_CORRECTION_OFFSET = 0.000001f;

        private void Start()
        {
            sphereCastDistance = sphereCastOriginHeightOffset - sphereCastRadius + sphereCastGroundOffset;
            raycastDistance = sphereCastOriginHeightOffset + 2*sphereCastGroundOffset;
            minSlopeCos = Mathf.Cos(Mathf.Deg2Rad * slopeCheckModel.maxSlopeAngle) - DOT_CORRECTION_OFFSET;     // here goes the maxSlopeAngle because we are using the cosine
            maxSlopeCos = Mathf.Cos(Mathf.Deg2Rad * slopeCheckModel.minSlopeAngle) + DOT_CORRECTION_OFFSET;     // here goes the minSlopeAngle because we are using the cosine
        }

        private void Update()
        {
            ray = new Ray(transform.position + Vector3.up * sphereCastOriginHeightOffset, Vector3.down);
            isGrounded = Physics.SphereCast(ray, sphereCastRadius, sphereCastDistance, layerMask);
            Physics.Raycast(ray, out hit, raycastDistance, layerMask);        // raycast is better than spherecast to check ground normal, this avoids problems when passing from ground with different angles
            groundNormal = hit.normal;      // this is not very accurate on planes, make sure to use cubes for the ground
        }

        /// <summary>
        /// Check if the ground is a slope based on the constraints in the slopeCheckModel
        /// </summary>
        /// <returns>true if it's considered a slope, false otherwise</returns>
        public bool IsSlope()
        {
            float groundNormalDot = Vector3.Dot(groundNormal, Vector3.up);
            return groundNormalDot >= minSlopeCos && groundNormalDot <= maxSlopeCos;
        }

#if UNITY_EDITOR
        [ContextMenu("TestGrounded")]
        private void TestGrounded()
        {
            Debug.Log("Grounded: "+isGrounded);
        }
#endif
    }
}
