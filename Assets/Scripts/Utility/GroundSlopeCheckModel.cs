using UnityEngine;

namespace Utilities
{
    [CreateAssetMenu(fileName = "GroundSlopeCheckModel", menuName = "ScriptableObjects/Utility/Ground/GroundSlopeCheckModel")]
    public class GroundSlopeCheckModel : ScriptableObject
    {
        [Header("Slope check")]
        public float minSlopeAngle = 10.0f;
        public float maxSlopeAngle = 45.0f;
    }
}
