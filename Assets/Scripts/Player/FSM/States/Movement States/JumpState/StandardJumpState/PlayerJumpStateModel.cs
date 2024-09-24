using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerJumpStateModel", menuName = "ScriptableObjects/Player/PlayerStates/PlayerJumpStateModel")]
    public class PlayerJumpStateModel : ScriptableObject
    {
        [Min(0.01f)] public float jumpHeight = 1.5f;
        [Min(1.0f)] public float jumpGravityMultiplier = 7.0f;
        [Min(0.01f)] public float midAirSpeed = 2.5f;
    }
}
