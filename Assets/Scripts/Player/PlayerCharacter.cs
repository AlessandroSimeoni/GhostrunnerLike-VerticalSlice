using GameCamera;
using InputControls;
using UnityEngine;
using UnityEngine.Assertions;
using Utility;

namespace Player
{
    [RequireComponent(typeof(GroundCheck))]
    public class PlayerCharacter : MonoBehaviour
    {
        public FirstPersonCamera fpCamera = null;
        [SerializeField] private PlayerMovementStateController movementStateController = null;
        [SerializeField] private PlayerAttackStateController attackStateController = null;
        public PlayerModel playerModel = null;
        public Animator playerAnimator = null;
        [Min(0.0f)] public float rigidbodyInteractionForce = 10.0f;


        public Controls controls;
        public Vector3 movementDirection { get; private set; } = Vector3.zero;
        public GroundCheck groundCheck { get; private set; } = null;

        private void Awake()
        {
            controls = new Controls();
            groundCheck = GetComponent<GroundCheck>();
        }

        private void Start()
        {
            Assert.IsNotNull(movementStateController, "Missing player movement state controller in PlayerCharacter");
            Assert.IsNotNull(attackStateController, "Missing player attack state controller in PlayerCharacter");
            Assert.IsNotNull(fpCamera, "Missing first person camera in PlayerCharacter");

            movementStateController.Init(this);
            attackStateController.Init(this);
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
