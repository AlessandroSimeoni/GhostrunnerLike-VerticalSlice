using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerParabolicJumpStateModel", menuName = "ScriptableObjects/Player/PlayerStates/PlayerParabolicJumpStateModel")]
    public class PlayerParabolicJumpStateModel : ScriptableObject
    {
        [Min(0.0f)] public float slideJumpRange = 5.0f;
        [Min(0.0f)] public float maxSlideJumpHeight = 3.0f;
        [Min(1.0f)] public float slidedJumpGravityMultiplier = 4.0f;
        [Min(0.0f)] public float slidedMidAirSpeed = 2.5f;
        [Range(0.0f, 1.0f)] public float forwardMovementWeight = 0.5f;
    }
}
