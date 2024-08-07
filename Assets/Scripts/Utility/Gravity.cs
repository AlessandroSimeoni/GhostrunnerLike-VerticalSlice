using UnityEngine;

namespace Utility
{
    [RequireComponent(typeof(GroundCheck))]
    [RequireComponent(typeof(CharacterController))]
    public class Gravity : MonoBehaviour
    {
        private GroundCheck groundCheck = null;
        private CharacterController characterController = null;

        public Vector3 currentAppliedGravity { private get; set; } = Vector3.zero;

        private void Awake()
        {
            groundCheck = GetComponent<GroundCheck>();
            characterController = GetComponent<CharacterController>();
        }

        private void FixedUpdate()
        {
            currentAppliedGravity = groundCheck.isGrounded ? Vector3.zero : currentAppliedGravity + Physics.gravity * Time.fixedDeltaTime;
            characterController.Move(currentAppliedGravity * Time.fixedDeltaTime);
        }
    }
}
