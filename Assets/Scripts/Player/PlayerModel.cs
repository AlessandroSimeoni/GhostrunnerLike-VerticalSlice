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
        [Header("Standard Jumping State")]
        public float jumpHeight = 1.5f;
        public float jumpGravityMultiplier = 7.0f;
        public float midAirSpeed = 2.5f;
        [Header("Crouched State")]
        public float crouchedTransitionTime = 0.5f;
        public float crouchedMovementSpeed = 5.0f;
        public float crouchedCharacterHeight = 1.0f;
        [Header("Slide State")]
        public float slideTime = 1.0f;
        public float maxSlideSpeed = 20.0f;
        public float minSlideSpeed = 5.0f;
        public float slideFriction = 5.0f;
        [Header("Slided Jump State")]
        [Range(0.0f, 90.0f)] public float slideJumpDegree = 60.0f;
        public float maxSlideJumpHeight = 3.0f;
        public float slidedJumpGravityMultiplier = 4.0f;
        public float slidedMidAirSpeed = 2.5f;
    }
}
