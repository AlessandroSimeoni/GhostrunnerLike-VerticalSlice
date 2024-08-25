using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerDashState : PlayerState
    {
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState movingState = null;

        private Vector3 dashDirection = Vector3.zero;
        private float currentTime = 0.0f;
        private float targetTime = 0.0f;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            targetTime = player.playerModel.dashDistance / player.playerModel.dashSpeed;
        }

        public override async UniTask Enter()
        {
            dashDirection = (player.movementDirection == Vector3.zero) ? player.transform.forward : player.movementDirection;
            currentTime = 0.0f;
            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            if (currentTime >= targetTime)
                return;

            currentTime += Time.deltaTime;

            if (currentTime >= targetTime)
            {
                controller.ChangeState((player.movementDirection == Vector3.zero) ? idleState : movingState).Forget();
                return;
            }

            ((PlayerMovementStateController)controller).characterController.Move(dashDirection * player.playerModel.dashSpeed * Time.deltaTime);
        }
    }
}
