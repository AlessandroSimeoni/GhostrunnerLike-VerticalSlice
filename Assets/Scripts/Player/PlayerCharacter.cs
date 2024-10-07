using GameCamera;
using InputControls;
using Projectiles;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using Utilities;

namespace Player
{
    [RequireComponent(typeof(GroundCheck))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Stamina))]
    public class PlayerCharacter : MonoBehaviour, IHitable
    {
        public FirstPersonCamera fpCamera = null;
        [SerializeField] private PlayerMovementStateController movementStateController = null;
        [SerializeField] private PlayerCombatStateController combatStateController = null;
        public Sword sword = null;
        public Animator playerAnimator = null;
        [Min(0.0f)] public float rigidbodyInteractionForce = 10.0f;

        public delegate void NoArgumentEvent();
        public event NoArgumentEvent OnCharacterControllerHit = null;
        public event NoArgumentEvent OnGrapplingHookUsed = null;
        public event NoArgumentEvent OnDeath = null;

        public delegate void HitEvent(Bullet bullet);
        public event HitEvent OnBulletHit = null;

        public Controls controls;
        public Vector3 inputMovementDirection { get; private set; } = Vector3.zero;     // the standard direction based on the input
        public Vector3 groundedMovementDirection { get; private set; } = Vector3.zero;  // the input direction adapted to handle slopes
        public GroundCheck groundCheck { get; private set; } = null;
        public CharacterController characterController { get; private set; } = null;
        public Stamina stamina { get; private set; } = null;
        public Transform hookTransform { get; set; } = null;

        private const string GROUND_TAG = "Ground";

        private bool alreadyDead = false;

        private void Awake()
        {
            controls = new Controls();
            groundCheck = GetComponent<GroundCheck>();
            characterController = GetComponent<CharacterController>();
            stamina = GetComponent<Stamina>();
        }

        private void Start()
        {
            Assert.IsNotNull(movementStateController, "Missing player movement state controller in PlayerCharacter");
            Assert.IsNotNull(combatStateController, "Missing player combat state controller in PlayerCharacter");
            Assert.IsNotNull(fpCamera, "Missing first person camera in PlayerCharacter");

            movementStateController.Init(this);
            combatStateController.Init(this);
            controls.Player.GrapplingHook.performed += GrabHook;

            transform.position += Vector3.up * characterController.skinWidth;
        }

        [ContextMenu("EnableControls")]
        public void EnableControls()
        {
            controls.Player.Enable();
            controls.Camera.Enable();
        }

        private void OnDisable()
        {
            DisableControls();
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

            Vector3 rightDirection = new Vector3(fpCamera.transform.right.x, 0.0f, fpCamera.transform.right.z);
            Vector3 forwardDirection = new Vector3(fpCamera.transform.forward.x, 0.0f, fpCamera.transform.forward.z).normalized;

            inputMovementDirection = rightDirection * inputValue.x + forwardDirection * inputValue.y;
            if(groundCheck.groundNormal != Vector3.zero)        // adapt movement direction for slopes
            {
                if (groundCheck.groundNormal != Vector3.up)     // normalize only if we are on slopes
                    groundedMovementDirection = Vector3.ProjectOnPlane(inputMovementDirection, groundCheck.groundNormal).normalized;
                else
                    groundedMovementDirection = Vector3.ProjectOnPlane(inputMovementDirection, groundCheck.groundNormal);
            }
            else
                groundedMovementDirection = inputMovementDirection;
        }

        private void ControlCamera() => fpCamera.ProcessMovement(controls.Camera.Rotation.ReadValue<Vector2>());

        private void GrabHook(InputAction.CallbackContext context)
        {
            if (hookTransform == null)
                return;

            movementStateController.HandleGrapplingHookRequest();
            OnGrapplingHookUsed?.Invoke();
        }

        public void Hit(Bullet bullet) => OnBulletHit?.Invoke(bullet);

        public void Death()
        {
            if (alreadyDead)
                return;

            alreadyDead = true;
            DisableControls();
            OnDeath?.Invoke();
        }

        public void DisableControls()
        {
            controls.Player.Disable();
            controls.Camera.Disable();
        }

        public void TrampolineJump(ParabolicJumpConfig jumpConfig, Vector3 trampolineDirection) => movementStateController.TrampolineJump(jumpConfig, trampolineDirection);
    }
}
