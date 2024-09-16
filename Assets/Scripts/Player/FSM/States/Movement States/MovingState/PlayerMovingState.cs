using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovingState : PlayerState
    {
        [SerializeField] private PlayerMovingStateModel movingStateModel = null;
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState jumpState = null;
        [SerializeField] private PlayerState slideState = null;
        [SerializeField] private PlayerState dashState = null;
        [SerializeField] private PlayerState fallingState = null;

        private const string RUN_ANIMATION = "Run";

        private InputAction jumpAction = null;
        private InputAction slideAction = null;
        private InputAction dashAction = null;

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            jumpAction = player.controls.Player.Jump;
            slideAction = player.controls.Player.Crouch;
            dashAction = player.controls.Player.Dash;
        }

        public override async UniTask Enter()
        {
            player.playerAnimator.SetBool(RUN_ANIMATION, true);
            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(RUN_ANIMATION, false);
            await base.Exit();
        }

        public override void Tick()
        {
            if (player.movementDirection == Vector3.zero)
            {
                controller.ChangeState(idleState).Forget();
                return;
            }

            if (jumpAction.triggered && player.groundCheck.isGrounded)
            {
                controller.ChangeState(jumpState).Forget();
                return;
            }

            if (slideAction.triggered && player.groundCheck.isGrounded)
            {
                controller.ChangeState(slideState).Forget();
                return;
            }

            if (dashAction.triggered && player.stamina.currentStamina > 0)
            {
                controller.ChangeState(dashState).Forget();
                return;
            }

            if (!player.groundCheck.isGrounded)
            {
                controller.ChangeState(fallingState).Forget();
                return;
            }

            player.characterController.Move(player.movementDirection * movingStateModel.movementSpeed * Time.deltaTime);
        }
    }
}
