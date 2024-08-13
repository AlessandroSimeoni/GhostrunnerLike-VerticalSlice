using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovingState : PlayerState
    {
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState jumpState = null;

        private const string RUN_ANIMATION = "Run";

        private InputAction jumpAction = null;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            jumpAction = player.controls.Player.Jump;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entered MOVING STATE");
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

            ((PlayerMovementStateController)controller).characterController.Move(player.movementDirection * player.playerModel.movementSpeed * Time.deltaTime);
        }
    }
}
