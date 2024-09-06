using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

namespace Player
{
    public class BasePlayerJumpState : PlayerFallingState
    {
        [SerializeField] protected Gravity gravity = null;

        protected float verticalVelocity = 0.0f;

        public const string JUMP_ANIMATION = "Jump";

        public override async UniTask Enter()
        {
            gravity.enabled = false;
            player.playerAnimator.SetBool(JUMP_ANIMATION, true);
            await UniTask.NextFrame();
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(JUMP_ANIMATION, false);
            verticalVelocity = 0.0f;

            if (controller.nextTargetState == wallRunState)
                ((PlayerWallRunState)wallRunState).rightSide = (rightHit && IsVerticalWall(rightWallHitInfo));
            else
                gravity.enabled = true;

            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            CheckDashAction();

            if (verticalVelocity < 0.0f)
            {
                CheckGround();
                CheckWallRun();
            }
        }
    }
}
