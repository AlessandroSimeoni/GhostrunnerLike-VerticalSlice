using GameCamera;
using InputControls;
using UnityEngine;
using UnityEngine.Assertions;

namespace Player
{
    [RequireComponent(typeof(PlayerStateController))]
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private FirstPersonCamera fpCamera = null;

        private PlayerStateController playerStateController;
        private Controls controls;

        private void Awake()
        {
            playerStateController = GetComponent<PlayerStateController>();
            controls = new Controls();
        }

        private void Start()
        {
            Assert.IsNotNull(fpCamera, "Camera required for player controller");
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
            MovePlayer();
            RotateCamera();
        }

        private void MovePlayer()
        {
            Vector2 inputValue = controls.Player.Move.ReadValue<Vector2>();

            Vector3 rightDirection = new Vector3(fpCamera.transform.right.x, 0.0f, fpCamera.transform.right.z).normalized;
            Vector3 forwardDirection = new Vector3(fpCamera.transform.forward.x, 0.0f, fpCamera.transform.forward.z).normalized;

            playerStateController.AddMovementDirection(rightDirection*inputValue.x + forwardDirection*inputValue.y);
        }

        private void RotateCamera()
        {
            


        }
    }
}
