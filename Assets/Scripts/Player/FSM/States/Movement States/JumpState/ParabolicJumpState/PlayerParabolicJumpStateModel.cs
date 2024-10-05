using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerParabolicJumpStateModel", menuName = "ScriptableObjects/Player/PlayerStates/PlayerParabolicJumpStateModel")]
    public class PlayerParabolicJumpStateModel : ScriptableObject
    {
        public ParabolicJumpConfig jumpModel;
    }

    [System.Serializable]
    public class ParabolicJumpConfig
    {
        [Min(0.0f)] public float jumpRange = 5.0f;
        [Min(0.0f)] public float maxJumpHeight = 3.0f;
        [Min(1.0f)] public float jumpGravityMultiplier = 4.0f;
        [Min(0.0f)] public float midAirSpeed = 2.5f;
        [Range(0.0f, 1.0f)] public float forwardMovementWeight = 0.5f;
    }
}
