using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities;

namespace Player
{
    public class PlayerSlideState : BasePlayerCrouchedState
    {
        [SerializeField] protected PlayerSlideStateModel slideStateModel = null;
        [SerializeField] private PlayerState crouchedState = null;
        [SerializeField] private PlayerState parabolicJumpState = null;
        [SerializeField] private PlayerState dashState = null;
        [SerializeField] private PlayerState slopeSlideState = null;

        private InputAction jumpAction = null;
        private InputAction dashAction = null;
        private float currentTime = 0.0f;
        protected float currentSlideSpeed = 0.0f;

        public Vector3 slideDirection { get; protected set; } = Vector3.forward;

        protected const string IDLE_ANIMATION = "Idle";                   //TODO: CREATE SLIDE ANIMATION

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            jumpAction = player.controls.Player.Jump;
            dashAction = player.controls.Player.Dash;
        }

        public override async UniTask Enter()
        {
            player.playerAnimator.SetBool(IDLE_ANIMATION, true);        // TODO: CREATE SLIDE ANIMATION
            currentTime = 0.0f;
            currentSlideSpeed = slideStateModel.maxSlideSpeed;
            slideDirection = (player.groundedMovementDirection == Vector3.zero) ? player.transform.forward : player.groundedMovementDirection;
            player.OnCharacterControllerHit += CrouchedStateTransition;
            await base.Enter();
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(IDLE_ANIMATION, false);       // TODO: CREATE SLIDE ANIMATION

            player.OnCharacterControllerHit -= CrouchedStateTransition;

            if (controller.nextTargetState == parabolicJumpState)
                ((PlayerParabolicJumpState)parabolicJumpState).jumpDirection = slideDirection;

            if (controller.nextTargetState != crouchedState && controller.nextTargetState != slopeSlideState)
                await base.Exit();
        }

        public override void Tick()
        {
            if (player.groundCheck.IsSlope() && MyUtility.SameDirection(player.groundCheck.groundNormal, player.transform.forward))
            {
                controller.ChangeState(slopeSlideState).Forget();
                return;
            }
            else
            {
                if (currentTime > slideStateModel.slideTime)
                    return;

                player.characterController.Move(slideDirection * currentSlideSpeed * Time.deltaTime);
                currentSlideSpeed = Mathf.MoveTowards(currentSlideSpeed, slideStateModel.minSlideSpeed, slideStateModel.slideFriction * Time.deltaTime);

                CheckActions();

                currentTime += Time.deltaTime;

                if (currentTime > slideStateModel.slideTime)
                {
                    CrouchedStateTransition();
                    return;
                }
            }
        }

        protected void CheckActions()
        {
            if (jumpAction.triggered)
            {
                controller.ChangeState(parabolicJumpState).Forget();
                return;
            }

            if (dashAction.triggered && player.stamina.currentStamina > 0)
            {
                controller.ChangeState(dashState).Forget();
                return;
            }
        }

        protected void CrouchedStateTransition() => controller.ChangeState(crouchedState).Forget();
    }
}
