using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerSlideState : BasePlayerCrouchedState
    {
        [SerializeField] private PlayerState crouchedState = null;
        [SerializeField] private PlayerState slideJumpState = null;
        [SerializeField] private PlayerState boostedSlideJumpState = null;
        [SerializeField] private PlayerState dashState = null;
        [SerializeField] private bool boostedState = false;

        private InputAction jumpAction = null;
        private InputAction dashAction = null;
        private float currentTime = 0.0f;
        private float currentSlideSpeed = 0.0f;
        private float maxSlideSpeed = 0.0f;
        private float minSlideSpeed = 0.0f;
        private float slideTime = 0.0f;
        private float slideFriction = 0.0f;

        public Vector3 slideDirection { get; private set; } = Vector3.forward;

        private const string IDLE_ANIMATION = "Idle";                   //TODO: CREATE SLIDE ANIMATION

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            jumpAction = player.controls.Player.Jump;
            dashAction = player.controls.Player.Dash;

            slideTime = boostedState ? player.playerModel.boostedSlideTime : player.playerModel.slideTime;
            maxSlideSpeed = boostedState ? player.playerModel.boostedMaxSlideSpeed : player.playerModel.maxSlideSpeed;
            minSlideSpeed = boostedState ? player.playerModel.boostedMinSlideSpeed : player.playerModel.minSlideSpeed;
            slideFriction = boostedState ? player.playerModel.boostedSlideFriction : player.playerModel.slideFriction;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entered SLIDING STATE");
            player.playerAnimator.SetBool(IDLE_ANIMATION, true);        // TODO: CREATE SLIDE ANIMATION
            currentTime = 0.0f;
            currentSlideSpeed = maxSlideSpeed;
            slideDirection = (player.movementDirection == Vector3.zero) ? player.transform.forward : player.movementDirection.normalized;
            ((PlayerMovementStateController)controller).OnCharacterControllerHit += CrouchedStateTransition;
            await base.Enter();
        }


        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(IDLE_ANIMATION, false);       // TODO: CREATE SLIDE ANIMATION

            ((PlayerMovementStateController)controller).OnCharacterControllerHit -= CrouchedStateTransition;

            if (controller.nextTargetState != crouchedState)
                await base.Exit();
        }

        public override void Tick()
        {
            if (currentTime > slideTime)
                return;

            ((PlayerMovementStateController)controller).characterController.Move(slideDirection * currentSlideSpeed * Time.deltaTime);
            currentSlideSpeed = Mathf.Lerp(currentSlideSpeed, minSlideSpeed, slideFriction * Time.deltaTime);

            if (jumpAction.triggered)
            {
                controller.ChangeState(boostedState ? boostedSlideJumpState : slideJumpState).Forget();
                return;
            }

            if (dashAction.triggered)
            {
                controller.ChangeState(dashState).Forget();
                return;
            }

            currentTime += Time.deltaTime;

            if (currentTime > slideTime)
            {
                CrouchedStateTransition();
                return;
            }
        }

        private void CrouchedStateTransition() => controller.ChangeState(crouchedState).Forget();
    }
}
