using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerModel", menuName = "ScriptableObjects/Player/PlayerModel")]
    public class PlayerModel : ScriptableObject
    {
        public int hp = 1;
        public float defaultCharacterHeight = 2.0f;
        [Header("Moving State")]
        public float movementSpeed = 5.0f;
        [Header("Jumping State")]
        public float jumpHeight = 1.5f;
        public float gravityMultiplier = 2.0f;
        public float midAirSpeed = 2.5f;
        [Header("Crouched State")]
        public float crouchedTransitionTime = 0.5f;
        public float crouchedMovementSpeed = 5.0f;
        public float crouchedCharacterHeight = 1.0f;
    }
}
