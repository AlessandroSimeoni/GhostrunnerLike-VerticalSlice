using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Player
{
    public class PlayerSlopeSlideState : PlayerSlideState
    {
        public override async UniTask Enter()
        {
            player.playerAnimator.SetBool(PlayerSlideState.IDLE_ANIMATION, true);        // TODO: CREATE SLIDE ANIMATION
            slideDirection = -Vector3.ProjectOnPlane(Vector3.up, player.groundCheck.groundNormal).normalized;
            currentSlideSpeed = slideStateModel.maxSlideSpeed;
            player.OnCharacterControllerHit += CrouchedStateTransition;
            await UniTask.NextFrame();
        }

        public override void Tick()
        {
            player.characterController.Move(slideDirection * currentSlideSpeed * Time.deltaTime);

            if (!player.groundCheck.IsSlope())
            {
                currentSlideSpeed = Mathf.MoveTowards(currentSlideSpeed, slideStateModel.minSlideSpeed, slideStateModel.slideFriction * Time.deltaTime);
                if (currentSlideSpeed <= slideStateModel.minSlideSpeed)
                    CrouchedStateTransition();
            }

            CheckActions();
        }
    }
}
