using UnityEngine;

namespace Player
{
    [RequireComponent (typeof(CharacterController))]
    public class PlayerMovementStateController : PlayerStateController
    {
        public CharacterController characterController { get; private set; } = null;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            transform.position += Vector3.up * characterController.skinWidth;
        }
    }
}
