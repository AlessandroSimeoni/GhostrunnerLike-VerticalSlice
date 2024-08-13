using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

namespace Player
{
    public class PlayerJumpState : PlayerState
    {
        [SerializeField] private PlayerState idleState = null;
        [SerializeField] private PlayerState movingState = null;
        [SerializeField] private Gravity gravity = null;

        private float verticalVelocity = 0.0f;

        public const string JUMP_ANIMATION = "Jump";

        public override async UniTask Enter()
        {
            Debug.Log("ENTERED JUMP STATE");
            await UniTask.NextFrame();
            verticalVelocity += Mathf.Sqrt(player.playerModel.jumpHeight * -2.0f * Physics.gravity.y * player.playerModel.gravityMultiplier);
            gravity.enabled = false;
            player.playerAnimator.SetBool(JUMP_ANIMATION, true);
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(JUMP_ANIMATION, false);
            await base.Exit();
        }

        public override void Tick()
        {
            base.Tick();

            if (player.groundCheck.isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = 0.0f;
                gravity.enabled = true;
                controller.ChangeState(player.movementDirection == Vector3.zero ? idleState : movingState).Forget();
            }

            ((PlayerMovementStateController)controller).characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
            verticalVelocity += Physics.gravity.y * player.playerModel.gravityMultiplier * Time.deltaTime;

            ((PlayerMovementStateController)controller).characterController.Move(player.movementDirection * player.playerModel.midAirSpeed * Time.deltaTime);
        }
    }
}
