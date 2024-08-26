using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Player
{
    public class BasePlayerJumpState : PlayerState
    {
        [SerializeField] protected PlayerState idleState = null;
        [SerializeField] protected PlayerState movingState = null;
        [SerializeField] protected PlayerState dashState = null;
        [SerializeField] protected Gravity gravity = null;

        protected float velocity = 0.0f;
        protected InputAction dashAction = null;

        public const string JUMP_ANIMATION = "Jump";

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            dashAction = player.controls.Player.Dash;
        }

        public override async UniTask Enter()
        {
            await UniTask.NextFrame();
            gravity.enabled = false;
            player.playerAnimator.SetBool(JUMP_ANIMATION, true);
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(JUMP_ANIMATION, false);
            velocity = 0.0f;
            gravity.enabled = true;
            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            if (dashAction.triggered && ((PlayerDashState)dashState).dashReady)
            {
                controller.ChangeState(dashState).Forget();
                return;
            }

            if (player.groundCheck.isGrounded && velocity < 0)
                controller.ChangeState(player.movementDirection == Vector3.zero ? idleState : movingState).Forget();
        }
    }
}
