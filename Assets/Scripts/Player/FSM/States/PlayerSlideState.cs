using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerSlideState : PlayerBaseCrouchedState
    {
        [SerializeField] private PlayerState crouchedState = null;
        [SerializeField] private PlayerState slideJumpState = null;

        private InputAction jumpAction = null;
        private float currentTime = 0.0f;
        private float slideSpeed = 0.0f;
        public Vector3 slideDirection { get; private set; } = Vector3.forward;

        private const string IDLE_ANIMATION = "Idle";                   //TODO: CREATE SLIDE ANIMATION

        public override void Init(PlayerCharacter player, PlayerStateController controller)
        {
            base.Init(player, controller);
            jumpAction = player.controls.Player.Jump;
        }

        public override async UniTask Enter()
        {
            Debug.Log("Entered SLIDING STATE");
            player.playerAnimator.SetBool(IDLE_ANIMATION, true);        // TODO: CREATE SLIDE ANIMATION
            currentTime = 0.0f;
            slideSpeed = player.playerModel.maxSlideSpeed;
            slideDirection = player.movementDirection;
            await base.Enter();
        }

        public override async UniTask Exit()
        {
            player.playerAnimator.SetBool(IDLE_ANIMATION, false);       // TODO: CREATE SLIDE ANIMATION
            
            if (controller.nextTargetState != crouchedState)
                await base.Exit();
        }

        public override void Tick()
        {
            if (currentTime > player.playerModel.slideTime)
                return;

            ((PlayerMovementStateController)controller).characterController.Move(slideDirection * slideSpeed * Time.deltaTime);
            slideSpeed = Mathf.Lerp(slideSpeed, player.playerModel.minSlideSpeed, player.playerModel.slideFriction * Time.deltaTime);

            if (jumpAction.triggered)
            {
                controller.ChangeState(slideJumpState).Forget();
                return;
            }

            currentTime += Time.deltaTime;

            if (currentTime > player.playerModel.slideTime)
            {
                controller.ChangeState(crouchedState).Forget();
                return;
            }
        }
    }
}
