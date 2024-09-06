using Architecture;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerSlideState : BasePlayerCrouchedState
    {
        [SerializeField] private PlayerSlideStateModel slideStateModel = null;
        [SerializeField] private PlayerState crouchedState = null;
        [SerializeField] private PlayerState parabolicJumpState = null;
        [SerializeField] private PlayerState dashState = null;

        private InputAction jumpAction = null;
        private InputAction dashAction = null;
        private float currentTime = 0.0f;
        private float currentSlideSpeed = 0.0f;

        public Vector3 slideDirection { get; private set; } = Vector3.forward;

        private const string IDLE_ANIMATION = "Idle";                   //TODO: CREATE SLIDE ANIMATION

        public override void Init<T>(T entity, AStateController controller)
        {
            base.Init(entity, controller);
            jumpAction = player.controls.Player.Jump;
            dashAction = player.controls.Player.Dash;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entered SLIDING STATE");
            player.playerAnimator.SetBool(IDLE_ANIMATION, true);        // TODO: CREATE SLIDE ANIMATION
            currentTime = 0.0f;
            currentSlideSpeed = slideStateModel.maxSlideSpeed;
            slideDirection = (player.movementDirection == Vector3.zero) ? player.transform.forward : player.movementDirection.normalized;
            player.OnCharacterControllerHit += CrouchedStateTransition;
            await base.Enter();
        }


        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(IDLE_ANIMATION, false);       // TODO: CREATE SLIDE ANIMATION

            player.OnCharacterControllerHit -= CrouchedStateTransition;

            if (controller.nextTargetState != crouchedState)
                await base.Exit();
        }

        public override void Tick()
        {
            if (currentTime > slideStateModel.slideTime)
                return;

            player.characterController.Move(slideDirection * currentSlideSpeed * Time.deltaTime);
            currentSlideSpeed = Mathf.Lerp(currentSlideSpeed, slideStateModel.minSlideSpeed, slideStateModel.slideFriction * Time.deltaTime);

            if (jumpAction.triggered)
            {
                controller.ChangeState(parabolicJumpState).Forget();
                return;
            }

            if (dashAction.triggered)
            {
                controller.ChangeState(dashState).Forget();
                return;
            }

            currentTime += Time.deltaTime;

            if (currentTime > slideStateModel.slideTime)
            {
                CrouchedStateTransition();
                return;
            }
        }

        private void CrouchedStateTransition() => controller.ChangeState(crouchedState).Forget();
    }
}
