using UnityEngine;

namespace Player
{
    [RequireComponent (typeof(CharacterController))]
    public class PlayerMovementStateController : PlayerStateController
    {
        private float rigidbodyInteractionForce = 0.0f;

        public CharacterController characterController { get; private set; } = null;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            transform.position += Vector3.up * characterController.skinWidth;
        }

        public override void Init(PlayerCharacter playerCharacter)
        {
            base.Init(playerCharacter);
            rigidbodyInteractionForce = playerCharacter.rigidbodyInteractionForce;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
                hit.rigidbody.AddForceAtPosition(hit.moveDirection * rigidbodyInteractionForce, hit.point);
        }
    }
}
