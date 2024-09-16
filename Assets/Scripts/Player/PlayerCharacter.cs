using GameCamera;
using InputControls;
using UnityEngine;
using UnityEngine.Assertions;
using Utility;

namespace Player
{
    [RequireComponent(typeof(GroundCheck))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacter : MonoBehaviour
    {
        public FirstPersonCamera fpCamera = null;
        [SerializeField] private PlayerStateController movementStateController = null;
        [SerializeField] private PlayerStateController combatStateController = null;
        public Sword sword = null;
        public Animator playerAnimator = null;
        [Min(0.0f)] public float rigidbodyInteractionForce = 10.0f;
        public Stamina stamina = null;

        public delegate void CharacterControllerEvent();
        public event CharacterControllerEvent OnCharacterControllerHit = null;

        public Controls controls;
        public Vector3 movementDirection { get; private set; } = Vector3.zero;
        public GroundCheck groundCheck { get; private set; } = null;
        public CharacterController characterController { get; private set; } = null;

        private const string GROUND_TAG = "Ground";

        private void Awake()
        {
            controls = new Controls();
            groundCheck = GetComponent<GroundCheck>();
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            Assert.IsNotNull(movementStateController, "Missing player movement state controller in PlayerCharacter");
            Assert.IsNotNull(combatStateController, "Missing player combat state controller in PlayerCharacter");
            Assert.IsNotNull(fpCamera, "Missing first person camera in PlayerCharacter");

            movementStateController.Init(this);
            combatStateController.Init(this);

            transform.position += Vector3.up * characterController.skinWidth;
        }

        private void OnEnable()
        {
            controls.Player.Enable();
            controls.Camera.Enable();
        }
        private void OnDisable()
        {
            controls.Player.Disable();
            controls.Camera.Disable();
        }

        private void Update()
        {
            ReadMovementDirection();
            ControlCamera();
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
                hit.rigidbody.AddForceAtPosition(hit.moveDirection * rigidbodyInteractionForce, hit.point);

            if (!hit.transform.CompareTag(GROUND_TAG))
                OnCharacterControllerHit?.Invoke();
        }

        private void ReadMovementDirection()
        {
            Vector2 inputValue = controls.Player.Move.ReadValue<Vector2>();

            Vector3 rightDirection = new Vector3(fpCamera.transform.right.x, 0.0f, fpCamera.transform.right.z).normalized;
            Vector3 forwardDirection = new Vector3(fpCamera.transform.forward.x, 0.0f, fpCamera.transform.forward.z).normalized;

            movementDirection = Vector3.ClampMagnitude(rightDirection * inputValue.x + forwardDirection * inputValue.y, 1.0f);
        }

        private void ControlCamera() => fpCamera.ProcessMovement(controls.Camera.Rotation.ReadValue<Vector2>());
    }
}
