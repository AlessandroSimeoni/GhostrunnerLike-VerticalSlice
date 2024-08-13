using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerCrouchedState : PlayerState
    {
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState movingState = null;
        [SerializeField] private PlayerState jumpState = null;

        private InputAction jumpAction = null;
        private InputAction crouchAction = null;
        private float originalCameraHeightOffset = 0.0f;

        private const string IDLE_ANIMATION = "Idle";

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            jumpAction = player.controls.Player.Jump;
            crouchAction = player.controls.Player.Crouch;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entered CROUCHED STATE");
            player.playerAnimator.SetBool(IDLE_ANIMATION, true);        // TODO: CREATE CROUCHED ANIMATION
            originalCameraHeightOffset = player.fpCamera.heightOffset;

            float targetCameraHeightOffset = originalCameraHeightOffset - (player.playerModel.defaultCharacterHeight - player.playerModel.crouchedCharacterHeight);
            await SmoothCameraOffset(originalCameraHeightOffset, targetCameraHeightOffset);

            ((PlayerMovementStateController)controller).characterController.height = player.playerModel.crouchedCharacterHeight;
            ((PlayerMovementStateController)controller).characterController.center = Vector3.up * player.playerModel.crouchedCharacterHeight / 2;
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(IDLE_ANIMATION, false);

            await SmoothCameraOffset(player.fpCamera.heightOffset, originalCameraHeightOffset);
            
            ((PlayerMovementStateController)controller).characterController.height = player.playerModel.defaultCharacterHeight;
            ((PlayerMovementStateController)controller).characterController.center = Vector3.up * player.playerModel.defaultCharacterHeight / 2;
        }

        public override void Tick()
        {
            if (crouchAction.triggered && player.movementDirection == Vector3.zero)
            {
                controller.ChangeState(idleState).Forget();
                return;
            }

            if (crouchAction.triggered && player.movementDirection != Vector3.zero)
            {
                controller.ChangeState(movingState).Forget();
                return;
            }

            if (jumpAction.triggered)
            {
                controller.ChangeState(jumpState).Forget();
                return;
            }

            ((PlayerMovementStateController)controller).characterController.Move(player.movementDirection * player.playerModel.crouchedMovementSpeed * Time.deltaTime);
        }

        private async UniTask SmoothCameraOffset(float startValue, float targetValue)
        {
            float currentTime = 0.0f;
            float interpolation = 0.0f;
            while (true)
            {
                currentTime += Time.deltaTime;
                interpolation = currentTime / player.playerModel.crouchedTransitionTime;
                player.fpCamera.heightOffset = Mathf.Lerp(startValue, targetValue, interpolation);

                if (player.fpCamera.heightOffset == targetValue)
                    break;

                await UniTask.NextFrame();
            }
        }
    }
}
