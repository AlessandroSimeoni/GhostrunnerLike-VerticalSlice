using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerModel", menuName = "ScriptableObjects/Player/PlayerModel")]
    public class PlayerModel : ScriptableObject
    {
        [Min(1)] public int hp = 1;
        [Min(0.01f)] public float defaultCharacterHeight = 2.0f;
        [Header("Moving State")]
        [Min(0.01f)] public float movementSpeed = 5.0f;
        [Header("Standard Jumping State")]
        [Min(0.01f)] public float jumpHeight = 1.5f;
        [Min(1.0f)] public float jumpGravityMultiplier = 7.0f;
        [Min(0.01f)] public float midAirSpeed = 2.5f;
        [Header("Crouched State")]
        [Min(0.0f)] public float crouchedTransitionTime = 0.5f;
        [Min(0.05f)] public float crouchedMovementSpeed = 5.0f;
        [Min(0.1f)] public float crouchedCharacterHeight = 1.0f;
        [Header("Slide State")]
        [Header("Non boosted:")]
        [Min(0.1f)] public float slideTime = 1.0f;
        [Min(0.01f)] public float maxSlideSpeed = 20.0f;
        [Min(0.01f)] public float minSlideSpeed = 5.0f;
        [Min(0.0f)] public float slideFriction = 5.0f;
        [Header("Boosted:")]
        [Min(0.1f)] public float boostedSlideTime = 1.0f;
        [Min(0.01f)] public float boostedMaxSlideSpeed = 20.0f;
        [Min(0.01f)] public float boostedMinSlideSpeed = 5.0f;
        [Min(0.0f)] public float boostedSlideFriction = 5.0f;
        [Header("Slided Jump State")]
        [Header("Non boosted:")]
        [Min(0.0f)] public float slideJumpRange = 5.0f;
        [Min(0.0f)] public float maxSlideJumpHeight = 3.0f;
        [Min(1.0f)] public float slidedJumpGravityMultiplier = 4.0f;
        [Min(0.0f)] public float slidedMidAirSpeed = 2.5f;
        [Range(0.0f, 1.0f)] public float forwardMovementWeight = 0.5f;
        [Header("Boosted:")]
        [Min(0.0f)] public float boostedSlideJumpRange = 10.0f;
        [Min(0.0f)] public float boostedMaxSlideJumpHeight = 3.0f;
        [Min(1.0f)] public float boostedSlidedJumpGravityMultiplier = 4.0f;
        [Min(0.0f)] public float boostedSlidedMidAirSpeed = 2.5f;
        [Range(0.0f, 1.0f)] public float boostedforwardMovementWeight = 0.5f;
        [Header("Dash State")]
        [Min(0.01f)] public float dashSpeed = 30.0f;
        [Min(0.05f)] public float dashDistance = 4.0f;
        [Min(0.0f)] public float dashCooldown = 3.0f;
        [Header("Wall Run")]
        [Header("Wall Check")]
        public LayerMask wallCheckLayers = new LayerMask();
        public float wallRayLenght = 1.0f;
        public float wallRayHeightOffset = 1.0f;
    }
}
