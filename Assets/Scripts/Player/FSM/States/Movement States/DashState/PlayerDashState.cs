using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerDashState : PlayerState
    {
        [SerializeField] protected PlayerDashStateModel dashStateModel = null;
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState movingState = null;
        [SerializeField] private PlayerState boostedSlideState = null;
        [SerializeField] private PlayerState fallingState = null;

        private InputAction slideAction = null;
        private Vector3 dashDirection = Vector3.zero;
        private float currentTime = 0.0f;
        private float targetTime = 0.0f;

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            targetTime = dashStateModel.dashDistance / dashStateModel.dashSpeed;
            slideAction = player.controls.Player.Crouch;
        }

        public override async UniTask Enter()
        {
            dashDirection = (player.movementDirection == Vector3.zero) ? player.transform.forward : player.movementDirection;
            currentTime = 0.0f;
            player.stamina.UseStamina(dashStateModel.dashStaminaCost);
            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            if (currentTime >= targetTime)
                return;

            if (slideAction.triggered && player.groundCheck.isGrounded)
            {
                controller.ChangeState(boostedSlideState).Forget();
                return;
            }

            currentTime += Time.deltaTime;

            if (currentTime >= targetTime)
            {
                if (player.groundCheck.isGrounded)
                    controller.ChangeState((player.movementDirection == Vector3.zero) ? idleState : movingState).Forget();
                else
                    controller.ChangeState(fallingState).Forget();

                return;
            }

            player.characterController.Move(dashDirection * dashStateModel.dashSpeed * Time.deltaTime);
        }
    }
}
