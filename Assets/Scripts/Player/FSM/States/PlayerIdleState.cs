using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerIdleState : PlayerState
    {
        [SerializeField] private PlayerState movingState = null;
        [SerializeField] private PlayerState jumpState = null;
        [SerializeField] private PlayerState crouchedState = null;
        [SerializeField] private PlayerState dashState = null;

        private const string IDLE_ANIMATION = "Idle";

        private InputAction jumpAction = null;
        private InputAction crouchAction = null;
        private InputAction dashAction = null;

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            jumpAction = player.controls.Player.Jump;
            crouchAction = player.controls.Player.Crouch;
            dashAction = player.controls.Player.Dash;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entered IDLE STATE");
            player.playerAnimator.SetBool(IDLE_ANIMATION, true);
            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(IDLE_ANIMATION, false);
            await base.Exit();
        }

        public override void Tick()
        {
            if (player.movementDirection != Vector3.zero)
            {
                controller.ChangeState(movingState).Forget();
                return;
            }

            if (jumpAction.triggered && player.groundCheck.isGrounded)
            {
                controller.ChangeState(jumpState).Forget();
                return;
            }

            if (crouchAction.triggered)
            {
                controller.ChangeState(crouchedState).Forget();
                return;
            }

            if (dashAction.triggered)
            {
                controller.ChangeState(dashState).Forget();
                return;
            }
        }
    }
}
